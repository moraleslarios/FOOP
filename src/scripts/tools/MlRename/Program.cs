using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MlRename;

// Uso: MlRename <ruta.sln> <lote.json> [--dry-run]
public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Uso: MlRename <ruta.sln> <lote.json> [--dry-run]");
            return 2;
        }

        MSBuildLocator.RegisterDefaults();
        return await Runner.RunAsync(Path.GetFullPath(args[0]), Path.GetFullPath(args[1]), args.Contains("--dry-run"));
    }
}

public sealed record Entry(string Kind,
                           string? Assembly,
                           string? Type,
                           string? Name,
                           string? Match,
                           string? Replace,
                           string? New,
                           bool    Bridge,
                           string? Note);

public sealed record Batch(string Id, int Level, List<Entry> Renames);

internal sealed record Candidate(ISymbol Symbol, string OldName, string NewName, string Key);

internal static class Runner
{
    private const string BridgeMarker = "Renamed to ";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };

    public static async Task<int> RunAsync(string slnPath, string batchPath, bool dryRun)
    {
        var batch    = JsonSerializer.Deserialize<Batch>(await File.ReadAllTextAsync(batchPath), JsonOptions) ?? throw new InvalidOperationException("Lote vacio");
        var rootDir  = Path.GetDirectoryName(slnPath)!;
        var registro = Path.Combine(rootDir, "Cambios Realizados", "_registro.csv");

        using var workspace = MSBuildWorkspace.Create();
        var failures = new List<string>();
#pragma warning disable CS0618
        workspace.WorkspaceFailed += (_, e) => { if (e.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure) failures.Add(e.Diagnostic.Message); };
#pragma warning restore CS0618

        Console.WriteLine($"Abriendo {slnPath} ...");
        var original = await workspace.OpenSolutionAsync(slnPath);
        foreach (var f in failures.Distinct().Take(10)) Console.WriteLine("  [workspace] " + f);
        Console.WriteLine($"  {original.Projects.Count()} proyectos cargados.");

        var solution   = original;
        var rows       = new List<string>();
        var typeRenames = new List<(string Old, string New)>();
        var bridgeDocs = new Dictionary<string, DocumentId>();
        var processed  = new HashSet<string>();
        var errors     = 0;

        foreach (var entry in batch.Renames)
        {
            Console.WriteLine();
            Console.WriteLine($"== {entry.Kind} {entry.Assembly ?? "*"} {entry.Type ?? "*"} {entry.Name ?? entry.Match} -> {entry.New ?? entry.Replace}");

            if (dryRun)
            {
                foreach (var c in await FindCandidatesAsync(solution, entry, processed))
                {
                    var refs = await CountReferencesAsync(solution, c.Symbol);
                    Console.WriteLine($"   {Describe(c.Symbol)} :: {c.OldName} -> {c.NewName}  refs={refs}");
                }
                continue;
            }

            var guard = 0;
            var found = 0;
            while (guard++ < 5000)
            {
                var c = (await FindCandidatesAsync(solution, entry, processed)).FirstOrDefault();
                if (c is null) break;
                processed.Add(c.Key);
                found++;

                var refs    = await CountReferencesAsync(solution, c.Symbol);
                var bridges = entry.Bridge ? BuildBridges(c.Symbol, c.NewName) : new List<MemberDeclarationSyntax>();
                var project = solution.GetProject(c.Symbol.ContainingAssembly)?.Name ?? "?";

                var options = new SymbolRenameOptions(RenameOverloads: c.Symbol is IMethodSymbol, RenameInStrings: false, RenameInComments: true, RenameFile: false);
                try
                {
                    solution = await Renamer.RenameSymbolAsync(solution, c.Symbol, options, c.NewName);
                }
                catch (Exception ex)
                {
                    errors++;
                    Console.WriteLine($"   !! ERROR renombrando {Describe(c.Symbol)}: {ex.Message}");
                    continue;
                }

                processed.Add(KeyFor(entry, c.NewName, c.Symbol));
                if (c.Symbol is INamedTypeSymbol) typeRenames.Add((c.OldName, c.NewName));

                if (bridges.Count > 0)
                    solution = await AddBridgesAsync(solution, c.Symbol, bridges, bridgeDocs);

                var container = c.Symbol is INamedTypeSymbol ? c.Symbol.ContainingNamespace.ToDisplayString() : c.Symbol.ContainingSymbol.ToDisplayString();
                rows.Add(string.Join(';', batch.Id, batch.Level, KindLabel(c.Symbol), project, container, c.OldName, c.NewName, bridges.Count, refs, (entry.Note ?? "").Replace(';', ',')));
                Console.WriteLine($"   OK {KindLabel(c.Symbol),-10} {container} :: {c.OldName} -> {c.NewName}  refs={refs} puentes={bridges.Count}");
            }

            if (found == 0)
            {
                Console.WriteLine("   (sin coincidencias)");
                rows.Add(string.Join(';', batch.Id, batch.Level, entry.Kind, entry.Assembly ?? "*", entry.Type ?? "*", entry.Name ?? entry.Match, entry.New ?? entry.Replace, 0, 0, "SIN COINCIDENCIAS"));
            }
        }

        if (dryRun) return 0;

        var written = await WriteChangesAsync(original, solution);
        Console.WriteLine();
        Console.WriteLine($"Ficheros escritos: {written}");

        RenameFiles(rootDir, solution, typeRenames);

        Directory.CreateDirectory(Path.GetDirectoryName(registro)!);
        if (!File.Exists(registro))
            await File.WriteAllTextAsync(registro, "Lote;Nivel;Tipo;Proyecto;Contenedor;Antes;Despues;Puentes;Referencias;Nota" + Environment.NewLine, new UTF8Encoding(true));
        await File.AppendAllLinesAsync(registro, rows, new UTF8Encoding(false));

        Console.WriteLine($"Registro: {rows.Count} filas -> {registro}");
        return errors == 0 ? 0 : 1;
    }

    // ---------------------------------------------------------------- candidatos

    private static async Task<List<Candidate>> FindCandidatesAsync(Solution solution, Entry entry, HashSet<string> processed)
    {
        var result = new List<Candidate>();
        var kind   = entry.Kind.ToLowerInvariant();

        foreach (var project in solution.Projects.OrderBy(p => p.Name))
        {
            if (project.Name == "MlRename") continue;
            if (entry.Assembly is not null && !string.Equals(project.AssemblyName, entry.Assembly, StringComparison.OrdinalIgnoreCase)) continue;

            var compilation = await project.GetCompilationAsync();
            if (compilation is null) continue;

            foreach (var type in AllTypes(compilation.Assembly.GlobalNamespace))
            {
                if (!type.Locations.Any(l => l.IsInSource)) continue;

                switch (kind)
                {
                    case "type":
                        if (TypeFilterMatches(entry, type, matchOnTypeItself: true))
                            AddIfMatch(result, entry, type, type.Name, processed);
                        break;

                    case "member":
                        if (!TypeFilterMatches(entry, type, matchOnTypeItself: false)) break;
                        foreach (var m in type.GetMembers())
                        {
                            if (m.IsImplicitlyDeclared || IsBridge(m)) continue;
                            if (m is IMethodSymbol ms && ms.MethodKind is not (MethodKind.Ordinary or MethodKind.UserDefinedOperator)) continue;
                            AddIfMatch(result, entry, m, m.Name, processed);
                        }
                        break;

                    case "parameter":
                        if (!TypeFilterMatches(entry, type, matchOnTypeItself: false)) break;
                        foreach (var m in type.GetMembers().OfType<IMethodSymbol>())
                        {
                            if (m.IsImplicitlyDeclared || IsBridge(m)) continue;
                            foreach (var p in m.Parameters) AddIfMatch(result, entry, p, p.Name, processed);
                        }
                        foreach (var m in type.GetMembers().OfType<IPropertySymbol>().Where(p => p.IsIndexer))
                            foreach (var p in m.Parameters) AddIfMatch(result, entry, p, p.Name, processed);
                        if (type.TypeKind == TypeKind.Class && type.IsRecord)
                            foreach (var ctor in type.InstanceConstructors)
                                foreach (var p in ctor.Parameters) AddIfMatch(result, entry, p, p.Name, processed);
                        break;

                    case "typeparam":
                        if (!TypeFilterMatches(entry, type, matchOnTypeItself: false)) break;
                        foreach (var tp in type.TypeParameters) AddIfMatch(result, entry, tp, tp.Name, processed);
                        foreach (var m in type.GetMembers().OfType<IMethodSymbol>())
                        {
                            if (IsBridge(m)) continue;
                            foreach (var tp in m.TypeParameters) AddIfMatch(result, entry, tp, tp.Name, processed);
                        }
                        break;

                    default:
                        throw new InvalidOperationException("Kind desconocido: " + entry.Kind);
                }
            }
        }

        return result.GroupBy(c => c.Key).Select(g => g.First()).ToList();
    }

    private static bool TypeFilterMatches(Entry entry, INamedTypeSymbol type, bool matchOnTypeItself)
    {
        if (entry.Type is null || entry.Type == "*") return true;
        var full = FullName(type);
        if (matchOnTypeItself && entry.Type.EndsWith(".*", StringComparison.Ordinal))
            return full.StartsWith(entry.Type[..^1], StringComparison.Ordinal);
        return string.Equals(full, entry.Type, StringComparison.Ordinal);
    }

    private static void AddIfMatch(List<Candidate> result, Entry entry, ISymbol symbol, string name, HashSet<string> processed)
    {
        string? newName = null;
        if (entry.Name is not null)
        {
            if (name == entry.Name) newName = entry.New;
        }
        else if (entry.Match is not null && Regex.IsMatch(name, entry.Match))
        {
            newName = entry.New ?? Regex.Replace(name, entry.Match, entry.Replace ?? "");
        }

        if (string.IsNullOrEmpty(newName) || newName == name) return;

        var key = KeyFor(entry, name, symbol);
        if (processed.Contains(key)) return;
        result.Add(new Candidate(symbol, name, newName, key));
    }

    private static string KeyFor(Entry entry, string name, ISymbol symbol)
    {
        var owner = symbol switch
        {
            INamedTypeSymbol t => t.ContainingNamespace.ToDisplayString(),
            IParameterSymbol p => p.ContainingSymbol.ToDisplayString(),
            ITypeParameterSymbol tp => tp.ContainingSymbol.ToDisplayString(),
            _ => symbol.ContainingType?.ToDisplayString() ?? ""
        };
        return $"{symbol.Kind}|{owner}|{name}";
    }

    private static IEnumerable<INamedTypeSymbol> AllTypes(INamespaceSymbol ns)
    {
        foreach (var t in ns.GetTypeMembers())
            foreach (var n in WithNested(t)) yield return n;
        foreach (var child in ns.GetNamespaceMembers())
            foreach (var t in AllTypes(child)) yield return t;
    }

    private static IEnumerable<INamedTypeSymbol> WithNested(INamedTypeSymbol t)
    {
        yield return t;
        foreach (var n in t.GetTypeMembers())
            foreach (var x in WithNested(n)) yield return x;
    }

    private static string FullName(INamedTypeSymbol t)
        => t.ContainingType is not null ? FullName(t.ContainingType) + "." + t.Name
                                        : (t.ContainingNamespace.IsGlobalNamespace ? t.Name : t.ContainingNamespace.ToDisplayString() + "." + t.Name);

    private static bool IsBridge(ISymbol s)
        => s.GetAttributes().Any(a => a.AttributeClass?.Name == "ObsoleteAttribute"
                                   && a.ConstructorArguments.Length > 0
                                   && a.ConstructorArguments[0].Value is string msg
                                   && msg.StartsWith(BridgeMarker, StringComparison.Ordinal));

    private static string KindLabel(ISymbol s) => s switch
    {
        INamedTypeSymbol t => t.TypeKind switch { TypeKind.Interface => "Interfaz", TypeKind.Enum => "Enum", _ => t.IsRecord ? "Record" : "Tipo" },
        IMethodSymbol => "Metodo",
        IPropertySymbol => "Propiedad",
        IFieldSymbol f => f.IsConst ? "Constante" : (f.ContainingType.TypeKind == TypeKind.Enum ? "ValorEnum" : "Campo"),
        IParameterSymbol => "Parametro",
        ITypeParameterSymbol => "Generico",
        _ => s.Kind.ToString()
    };

    private static string Describe(ISymbol s) => $"{KindLabel(s)} {s.ToDisplayString()}";

    private static async Task<int> CountReferencesAsync(Solution solution, ISymbol symbol)
    {
        var symbols = symbol is IMethodSymbol m
            ? m.ContainingType.GetMembers(m.Name).Where(x => !IsBridge(x)).ToList()
            : new List<ISymbol> { symbol };

        var total = 0;
        foreach (var s in symbols)
            foreach (var r in await SymbolFinder.FindReferencesAsync(s, solution))
                total += r.Locations.Count();
        return total;
    }

    // ---------------------------------------------------------------- puentes

    private static List<MemberDeclarationSyntax> BuildBridges(ISymbol symbol, string newName)
    {
        var list = new List<MemberDeclarationSyntax>();
        if (symbol.ContainingType is null || !IsPublicApi(symbol.ContainingType)) return list;
        if (symbol.ContainingType.TypeKind == TypeKind.Interface) return list;

        var members = symbol is IMethodSymbol
            ? symbol.ContainingType.GetMembers(symbol.Name).Where(x => !IsBridge(x))
            : new[] { symbol };

        foreach (var m in members)
        {
            if (m.DeclaredAccessibility != Accessibility.Public) continue;
            if (m.IsAbstract || m.IsVirtual || m.IsOverride) continue;

            foreach (var reference in m.DeclaringSyntaxReferences)
            {
                var node = reference.GetSyntax();
                var bridge = node switch
                {
                    MethodDeclarationSyntax md => BridgeMethod(md, newName),
                    PropertyDeclarationSyntax pd => BridgeProperty(pd, newName),
                    _ => null
                };
                if (bridge is not null) list.Add(bridge);
            }
        }
        return list;
    }

    private static bool IsPublicApi(INamedTypeSymbol t)
    {
        for (var x = t; x is not null; x = x.ContainingType)
            if (x.DeclaredAccessibility != Accessibility.Public) return false;
        return true;
    }

    private static AttributeListSyntax BridgeAttributes(string newName)
        => AttributeList(SeparatedList(new[]
        {
            Attribute(ParseName("global::System.Obsolete"),
                      ParseAttributeArgumentList($"(\"{BridgeMarker}{newName}. Will be removed in 3.0.0.\", false)")),
            Attribute(ParseName("global::System.ComponentModel.EditorBrowsable"),
                      ParseAttributeArgumentList("(global::System.ComponentModel.EditorBrowsableState.Never)"))
        }));

    private static MemberDeclarationSyntax BridgeMethod(MethodDeclarationSyntax md, string newName)
    {
        var arguments = md.ParameterList.Parameters.Select(p =>
        {
            var arg = Argument(IdentifierName(p.Identifier.WithoutTrivia()));
            var refKw = p.Modifiers.FirstOrDefault(t => t.IsKind(SyntaxKind.RefKeyword) || t.IsKind(SyntaxKind.OutKeyword) || t.IsKind(SyntaxKind.InKeyword));
            return refKw == default ? arg : arg.WithRefKindKeyword(Token(refKw.Kind()));
        });

        ExpressionSyntax target = md.TypeParameterList is { Parameters.Count: > 0 } tpl
            ? GenericName(Identifier(newName), TypeArgumentList(SeparatedList<TypeSyntax>(tpl.Parameters.Select(tp => IdentifierName(tp.Identifier.WithoutTrivia())))))
            : IdentifierName(newName);

        var call = InvocationExpression(target, ArgumentList(SeparatedList(arguments)));

        return md.WithAttributeLists(SingletonList(BridgeAttributes(newName)))
                 .WithModifiers(TokenList(md.Modifiers.Where(t => !t.IsKind(SyntaxKind.AsyncKeyword))))
                 .WithBody(null)
                 .WithExpressionBody(ArrowExpressionClause(call))
                 .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                 .WithoutLeadingTrivia()
                 .WithoutTrailingTrivia();
    }

    private static MemberDeclarationSyntax BridgeProperty(PropertyDeclarationSyntax pd, string newName)
    {
        var hasSetter = pd.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)
                                                          && !a.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword) || m.IsKind(SyntaxKind.ProtectedKeyword) || m.IsKind(SyntaxKind.InternalKeyword))) == true;

        var baseDecl = pd.WithAttributeLists(SingletonList(BridgeAttributes(newName)))
                         .WithInitializer(null)
                         .WithoutLeadingTrivia()
                         .WithoutTrailingTrivia();

        if (!hasSetter)
            return baseDecl.WithAccessorList(null)
                           .WithExpressionBody(ArrowExpressionClause(IdentifierName(newName)))
                           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        return baseDecl.WithExpressionBody(null)
                       .WithSemicolonToken(default)
                       .WithAccessorList(AccessorList(List(new[]
                       {
                           AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithExpressionBody(ArrowExpressionClause(IdentifierName(newName))).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                           AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithExpressionBody(ArrowExpressionClause(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(newName), IdentifierName("value")))).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                       })));
    }

    private static async Task<Solution> AddBridgesAsync(Solution solution, ISymbol renamedSymbol, List<MemberDeclarationSyntax> bridges, Dictionary<string, DocumentId> bridgeDocs)
    {
        var originalType = renamedSymbol.ContainingType!;
        var typeKey      = solution.GetProject(originalType.ContainingAssembly)!.Id.Id + "|" + FullName(originalType);

        // Re-localizar el tipo contenedor en la solucion ya renombrada.
        var project     = solution.GetProject(solution.Projects.First(p => p.AssemblyName == originalType.ContainingAssembly.Name).Id)!;
        var compilation = (await project.GetCompilationAsync())!;
        var type        = AllTypes(compilation.Assembly.GlobalNamespace).First(t => FullName(t) == FullName(originalType));

        solution = await EnsurePartialAsync(solution, type);

        if (bridgeDocs.TryGetValue(typeKey, out var existingId) || TryFindExistingBridgeDoc(solution, type, out existingId))
        {
            var doc  = solution.GetDocument(existingId)!;
            var root = (CompilationUnitSyntax)(await doc.GetSyntaxRootAsync())!;
            var decl = root.DescendantNodes().OfType<TypeDeclarationSyntax>().First(t => t.Identifier.Text == type.Name);
            root     = root.ReplaceNode(decl, decl.AddMembers(bridges.ToArray()));
            bridgeDocs[typeKey] = existingId;
            return solution.WithDocumentSyntaxRoot(existingId, root.NormalizeWhitespace(eol: "\r\n"));
        }

        var firstDecl   = type.DeclaringSyntaxReferences.First();
        var sourceDoc   = solution.GetDocument(firstDecl.SyntaxTree)!;
        var sourceRoot  = (CompilationUnitSyntax)(await firstDecl.SyntaxTree.GetRootAsync());
        var typeSyntax  = (TypeDeclarationSyntax)(await firstDecl.GetSyntaxAsync());

        TypeDeclarationSyntax shell = typeSyntax switch
        {
            RecordDeclarationSyntax r => RecordDeclaration(r.Keyword, r.Identifier).WithClassOrStructKeyword(r.ClassOrStructKeyword).WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken)).WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken)),
            StructDeclarationSyntax s => StructDeclaration(s.Identifier),
            _ => ClassDeclaration(typeSyntax.Identifier)
        };

        var modifiers = typeSyntax.Modifiers.Where(t => !t.IsKind(SyntaxKind.PartialKeyword)).Select(t => Token(t.Kind())).ToList();
        modifiers.Add(Token(SyntaxKind.PartialKeyword));

        shell = shell.WithModifiers(TokenList(modifiers))
                     .WithTypeParameterList(typeSyntax.TypeParameterList?.WithoutTrivia())
                     .WithMembers(List(bridges));

        var nsName = type.ContainingNamespace.IsGlobalNamespace ? null : type.ContainingNamespace.ToDisplayString();
        var unit   = CompilationUnit().WithUsings(List(sourceRoot.Usings.Select(u => u.WithoutTrivia())));
        unit = nsName is null
            ? unit.WithMembers(SingletonList<MemberDeclarationSyntax>(shell))
            : unit.WithMembers(SingletonList<MemberDeclarationSyntax>(FileScopedNamespaceDeclaration(ParseName(nsName)).WithMembers(SingletonList<MemberDeclarationSyntax>(shell))));

        var header  = "// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.\r\n";
        var text    = SourceText.From(header + unit.NormalizeWhitespace(eol: "\r\n").ToFullString() + "\r\n", new UTF8Encoding(true));
        var dir     = Path.GetDirectoryName(sourceDoc.FilePath!)!;
        var path    = Path.Combine(dir, type.Name + ".Obsolete.cs");
        var newId   = DocumentId.CreateNewId(sourceDoc.Project.Id);

        bridgeDocs[typeKey] = newId;
        return solution.AddDocument(newId, type.Name + ".Obsolete.cs", text, sourceDoc.Folders, path);
    }

    private static bool TryFindExistingBridgeDoc(Solution solution, INamedTypeSymbol type, out DocumentId id)
    {
        foreach (var r in type.DeclaringSyntaxReferences)
        {
            var doc = solution.GetDocument(r.SyntaxTree);
            if (doc?.FilePath is not null && doc.FilePath.EndsWith(".Obsolete.cs", StringComparison.OrdinalIgnoreCase))
            {
                id = doc.Id;
                return true;
            }
        }
        id = null!;
        return false;
    }

    private static async Task<Solution> EnsurePartialAsync(Solution solution, INamedTypeSymbol type)
    {
        foreach (var r in type.DeclaringSyntaxReferences)
        {
            var doc  = solution.GetDocument(r.SyntaxTree)!;
            var root = await doc.GetSyntaxRootAsync();
            var decl = root!.FindNode(r.Span) as TypeDeclarationSyntax;
            if (decl is null || decl.Modifiers.Any(SyntaxKind.PartialKeyword)) continue;

            var updated = decl.AddModifiers(Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(Space));
            solution = solution.WithDocumentSyntaxRoot(doc.Id, root.ReplaceNode(decl, updated));
        }
        return solution;
    }

    // ---------------------------------------------------------------- escritura

    private static async Task<int> WriteChangesAsync(Solution original, Solution updated)
    {
        var count = 0;
        foreach (var pc in updated.GetChanges(original).GetProjectChanges())
        {
            foreach (var id in pc.GetChangedDocuments())
            {
                var doc  = updated.GetDocument(id)!;
                var text = await doc.GetTextAsync();
                WriteText(doc.FilePath!, text.ToString());
                count++;
            }
            foreach (var id in pc.GetAddedDocuments())
            {
                var doc  = updated.GetDocument(id)!;
                var text = await doc.GetTextAsync();
                WriteText(doc.FilePath!, text.ToString());
                count++;
            }
        }
        return count;
    }

    private static void WriteText(string path, string content)
    {
        var hasBom = true;
        if (File.Exists(path))
        {
            var head = new byte[3];
            using (var fs = File.OpenRead(path)) hasBom = fs.Read(head, 0, 3) == 3 && head[0] == 0xEF && head[1] == 0xBB && head[2] == 0xBF;
        }
        File.WriteAllText(path, content, new UTF8Encoding(hasBom));
    }

    private static void RenameFiles(string rootDir, Solution solution, List<(string Old, string New)> typeRenames)
    {
        if (typeRenames.Count == 0) return;

        var files = solution.Projects.SelectMany(p => p.Documents).Select(d => d.FilePath).Where(p => p is not null).Distinct().ToList();
        foreach (var path in files)
        {
            if (!File.Exists(path)) continue;
            var fileName = Path.GetFileName(path!);
            foreach (var (oldName, newName) in typeRenames.OrderByDescending(t => t.Old.Length))
            {
                if (fileName == oldName + ".cs" || fileName.StartsWith(oldName + ".", StringComparison.Ordinal))
                {
                    var target = Path.Combine(Path.GetDirectoryName(path!)!, newName + fileName[oldName.Length..]);
                    if (File.Exists(target)) break;
                    Git(rootDir, "mv", path!, target);
                    Console.WriteLine($"   git mv {fileName} -> {Path.GetFileName(target)}");
                    break;
                }
            }
        }
    }

    private static void Git(string workDir, params string[] args)
    {
        var psi = new ProcessStartInfo("git") { WorkingDirectory = workDir, RedirectStandardError = true, RedirectStandardOutput = true };
        foreach (var a in args) psi.ArgumentList.Add(a);
        using var p = Process.Start(psi)!;
        p.WaitForExit();
        if (p.ExitCode != 0)
        {
            // Fichero nuevo aun no versionado: mover sin git.
            File.Move(args[1], args[2]);
        }
    }
}
