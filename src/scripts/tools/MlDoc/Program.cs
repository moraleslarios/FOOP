using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MlDoc;

/// <summary>
/// Generador masivo de documentacion XML (<c>///</c>) para los miembros publicos
/// de MoralesLarios.OOFP que todavia no la tienen.
///
/// Trabaja a nivel de SINTAXIS (no necesita compilar la solucion) y edita los
/// ficheros por LINEAS, insertando unicamente trivia nueva: nunca reformatea,
/// reordena ni toca el codigo existente.
/// </summary>
internal static class Program
{
    private static int Main(string[] rawArgs)
    {
        var opt = Options.Parse(rawArgs);
        if (opt is null) return 1;

        var files = Discover(opt);
        if (files.Count == 0)
        {
            Console.Error.WriteLine("MlDoc: no se ha encontrado ningun fichero .cs que procesar.");
            return 1;
        }

        Console.WriteLine($"MlDoc: {files.Count} fichero(s) candidato(s).{(opt.DryRun ? "  [DRY-RUN]" : string.Empty)}");

        // --- pase 1: tipos partial que YA tienen documentacion en alguna de sus partes.
        var documentedPartials = CollectDocumentedPartials(files);

        // --- pase 2: generacion.
        var totalInserted = 0;
        var touchedFiles = 0;
        var report = new List<(string File, int Count)>();

        // Compartido entre ficheros: un tipo partial repartido en varios ficheros solo
        // debe recibir el bloque <summary>/<typeparam> UNA vez (si no, CS1710).
        var generatedPartials = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in files)
        {
            var inserted = ProcessFile(file, opt, documentedPartials, generatedPartials);
            if (inserted <= 0) continue;

            totalInserted += inserted;
            touchedFiles++;
            report.Add((file, inserted));
        }

        Console.WriteLine();
        foreach (var (file, count) in report.OrderByDescending(r => r.Count))
            Console.WriteLine($"{count,5}  {Relative(file, opt)}");

        Console.WriteLine();
        Console.WriteLine($"MlDoc: documentados={totalInserted} ficheros={touchedFiles}");

        if (!string.IsNullOrEmpty(opt.ReportPath))
        {
            var sb = new StringBuilder();
            foreach (var (file, count) in report.OrderByDescending(r => r.Count))
                sb.Append(count.ToString().PadLeft(5)).Append("  ").AppendLine(Relative(file, opt));
            sb.AppendLine().AppendLine($"documentados={totalInserted} ficheros={touchedFiles}");
            File.WriteAllText(opt.ReportPath!, sb.ToString(), new UTF8Encoding(false));
            Console.WriteLine($"MlDoc: informe en {opt.ReportPath}");
        }

        return 0;
    }

    // ------------------------------------------------------------- descubrimiento

    private static readonly string[] ExcludedDirs = ["bin", "obj", ".git", ".vs", "node_modules", "packages"];

    private static List<string> Discover(Options opt)
    {
        var result = new List<string>();

        foreach (var f in opt.Files)
            if (File.Exists(f)) result.Add(Path.GetFullPath(f));

        foreach (var root in opt.Roots)
        {
            if (!Directory.Exists(root)) continue;

            foreach (var f in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                var parts = Path.GetRelativePath(root, f).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (parts.Any(p => ExcludedDirs.Contains(p, StringComparer.OrdinalIgnoreCase))) continue;
                if (Path.GetFileName(f).EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)) continue;
                if (Path.GetFileName(f).EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase)) continue;
                if (Path.GetFileName(f).Equals("GlobalUsings.cs", StringComparison.OrdinalIgnoreCase)) continue;
                result.Add(Path.GetFullPath(f));
            }
        }

        var filtered = result.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();

        if (opt.Only.Count > 0)
            filtered = filtered.Where(f => opt.Only.Any(o => f.Contains(o, StringComparison.OrdinalIgnoreCase))).ToList();

        if (opt.MaxFiles > 0 && filtered.Count > opt.MaxFiles)
            filtered = filtered.Take(opt.MaxFiles).ToList();

        return filtered;
    }

    private static string Relative(string file, Options opt)
    {
        foreach (var root in opt.Roots)
            if (file.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                return Path.GetRelativePath(root, file);
        return file;
    }

    // ---------------------------------------------------------------- partials

    private static HashSet<string> CollectDocumentedPartials(List<string> files)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in files)
        {
            var root = Parse(file, out _);
            if (root is null) continue;

            foreach (var type in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
            {
                if (type is not TypeDeclarationSyntax td) continue;
                if (!td.Modifiers.Any(SyntaxKind.PartialKeyword)) continue;
                if (!HasDocumentation(td)) continue;
                set.Add(FullName(td));
            }
        }

        return set;
    }

    /// <summary>
    /// Devuelve el nombre completo del tipo incluyendo la aridad generica, de modo
    /// que <c>Foo&lt;T&gt;</c> y <c>Foo&lt;T1, T2&gt;</c> no se consideren el mismo tipo parcial.
    /// </summary>
    private static string FullName(BaseTypeDeclarationSyntax type)
    {
        var parts = new List<string> { Named(type) };
        foreach (var anc in type.Ancestors())
        {
            switch (anc)
            {
                case BaseTypeDeclarationSyntax t: parts.Insert(0, Named(t)); break;
                case BaseNamespaceDeclarationSyntax ns: parts.Insert(0, ns.Name.ToString()); break;
            }
        }
        return string.Join('.', parts);

        static string Named(BaseTypeDeclarationSyntax t)
        {
            var arity = (t as TypeDeclarationSyntax)?.TypeParameterList?.Parameters.Count ?? 0;
            return arity == 0 ? t.Identifier.ValueText : t.Identifier.ValueText + "`" + arity;
        }
    }

    // ---------------------------------------------------------------- parseo

    private static CompilationUnitSyntax? Parse(string file, out string text)
    {
        try
        {
            text = File.ReadAllText(file, Encoding.UTF8);
            var tree = CSharpSyntaxTree.ParseText(text, new CSharpParseOptions(LanguageVersion.Preview));
            return tree.GetCompilationUnitRoot();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"MlDoc: no se pudo leer {file}: {ex.Message}");
            text = string.Empty;
            return null;
        }
    }

    // ---------------------------------------------------------------- proceso

    private static int ProcessFile(string file, Options opt, HashSet<string> documentedPartials, HashSet<string> generatedPartials)
    {
        var root = Parse(file, out var text);
        if (root is null) return 0;
        if (root.ContainsDiagnostics && root.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error))
        {
            Console.Error.WriteLine($"MlDoc: SALTADO (errores de sintaxis) {file}");
            return 0;
        }

        // Salto de linea que se usara para las lineas NUEVAS. Si el fichero contiene
        // algun CRLF se prefiere CRLF (estilo del repositorio) aunque haya LF sueltos,
        // porque hay ficheros con finales de linea mixtos.
        var newline = DominantNewline(text);

        // IMPORTANTE: hay ficheros en el repo con finales de linea MIXTOS (CRLF + LF
        // sueltos). Si se trocea con un unico separador los indices de linea no cuadran
        // con los que devuelve Roslyn y no se inserta nada. Por eso se trocea
        // CONSERVANDO el terminador real de cada linea.
        var lines = SplitLinesKeepEnds(text);

        // (linea de insercion, bloque de lineas)
        var inserts = new List<(int Line, List<string> Block)>();

        foreach (var member in root.DescendantNodes().OfType<MemberDeclarationSyntax>())
        {
            if (member is BaseNamespaceDeclarationSyntax) continue;
            if (HasDocumentation(member)) continue;
            if (!Visibility.IsExternallyVisible(member)) continue;

            // tipos partial: documentar una sola vez
            if (member is TypeDeclarationSyntax td && td.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                var fqn = FullName(td);
                if (documentedPartials.Contains(fqn) || !generatedPartials.Add(fqn)) continue;
            }

            // metodos partial: la doc va en la parte declarativa
            if (member is MethodDeclarationSyntax pm && pm.Modifiers.Any(SyntaxKind.PartialKeyword) && pm.Body is not null) continue;

            var span = member.GetLocation().GetLineSpan();
            var line = span.StartLinePosition.Line;
            if (line < 0 || line >= lines.Count) continue;

            var indent = Indentation(lines[line]);
            var block = DocBlock.Build(member, indent, opt);
            if (block.Count == 0) continue;

            inserts.Add((line, block));
        }

        if (inserts.Count == 0) return 0;

        if (!opt.DryRun)
        {
            foreach (var (line, block) in inserts.OrderByDescending(i => i.Line))
                lines.InsertRange(line, block.Select(b => b + newline));

            var output = string.Concat(lines);
            File.WriteAllText(file, output, new UTF8Encoding(HasUtf8Bom(file)));
        }

        return inserts.Count;
    }

    /// <summary>
    /// Salto de linea que se usara para las lineas NUEVAS. Si el fichero contiene
    /// algun CRLF se prefiere CRLF (estilo del repositorio) aunque haya LF sueltos,
    /// porque hay ficheros con finales de linea mixtos.
    /// </summary>
    private static string DominantNewline(string text)
    {
        var crlf = 0;
        var lf = 0;
        var cr = 0;

        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\r')
            {
                if (i + 1 < text.Length && text[i + 1] == '\n') { crlf++; i++; }
                else cr++;
            }
            else if (text[i] == '\n') lf++;
        }

        if (crlf > 0) return "\r\n";
        return lf >= cr ? "\n" : "\r";
    }

    /// <summary>
    /// Trocea el texto en lineas CONSERVANDO el terminador de cada una, de forma que
    /// los indices coincidan con los que calcula Roslyn aunque el fichero mezcle
    /// CRLF, LF y CR. La ultima linea puede no llevar terminador.
    /// </summary>
    private static List<string> SplitLinesKeepEnds(string text)
    {
        var result = new List<string>();
        var start = 0;

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c != '\r' && c != '\n') continue;

            var end = i + 1;
            if (c == '\r' && end < text.Length && text[end] == '\n') end++;

            result.Add(text[start..end]);
            i = end - 1;
            start = end;
        }

        if (start < text.Length) result.Add(text[start..]);
        if (result.Count == 0) result.Add(string.Empty);

        return result;
    }

    private static string Indentation(string line)
    {
        var i = 0;
        while (i < line.Length && (line[i] == ' ' || line[i] == '\t')) i++;
        return line[..i];
    }

    internal static bool HasDocumentation(SyntaxNode node)
        => node.GetLeadingTrivia().Any(t =>
               t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
            || t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));

    /// <summary>Indica si el fichero empieza por la marca BOM de UTF-8.</summary>
    private static bool HasUtf8Bom(string file)
    {
        try
        {
            using var fs = File.OpenRead(file);
            Span<byte> head = stackalloc byte[3];
            return fs.Read(head) == 3 && head[0] == 0xEF && head[1] == 0xBB && head[2] == 0xBF;
        }
        catch
        {
            return false;
        }
    }
}
