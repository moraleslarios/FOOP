namespace MlDoc;

/// <summary>Opciones de linea de comandos de MlDoc.</summary>
internal sealed class Options
{
    public List<string> Roots { get; } = [];
    public List<string> Files { get; } = [];
    public List<string> Only  { get; } = [];
    public bool DryRun { get; private set; }
    public int MaxFiles { get; private set; }
    public string? ReportPath { get; private set; }

    /// <summary>
    /// Cuando se indica, se exploran los <c>.csproj</c> por debajo de esta ruta y se
    /// toman como raices unicamente las carpetas de los proyectos que declaran
    /// <c>GenerateDocumentationFile = true</c> (es decir, los publicables).
    /// </summary>
    public string? SolutionRoot { get; private set; }

    public static Options? Parse(string[] args)
    {
        var o = new Options();

        for (var i = 0; i < args.Length; i++)
        {
            var a = args[i];
            switch (a)
            {
                case "--root":
                    if (++i >= args.Length) return Fail("--root requiere una ruta.");
                    o.Roots.Add(Path.GetFullPath(args[i]));
                    break;

                case "--docs-only":
                    if (++i >= args.Length) return Fail("--docs-only requiere la ruta raiz de la solucion.");
                    o.SolutionRoot = Path.GetFullPath(args[i]);
                    break;

                case "--file":
                    if (++i >= args.Length) return Fail("--file requiere una ruta.");
                    o.Files.Add(Path.GetFullPath(args[i]));
                    break;

                case "--only":
                    if (++i >= args.Length) return Fail("--only requiere un patron.");
                    o.Only.Add(args[i]);
                    break;

                case "--max-files":
                    if (++i >= args.Length || !int.TryParse(args[i], out var n)) return Fail("--max-files requiere un numero.");
                    o.MaxFiles = n;
                    break;

                case "--report":
                    if (++i >= args.Length) return Fail("--report requiere una ruta.");
                    o.ReportPath = Path.GetFullPath(args[i]);
                    break;

                case "--dry-run":
                    o.DryRun = true;
                    break;

                case "-h":
                case "--help":
                    Usage();
                    return null;

                default:
                    return Fail("argumento desconocido: " + a);
            }
        }

        if (o.SolutionRoot is not null)
            ResolveDocumentableProjects(o);

        if (o.Roots.Count == 0 && o.Files.Count == 0) return Fail("debe indicar al menos --root, --file o --docs-only.");
        return o;
    }

    /// <summary>
    /// Anade como raices las carpetas de los proyectos que generan fichero de documentacion.
    /// </summary>
    private static void ResolveDocumentableProjects(Options o)
    {
        var skip = new[] { "bin", "obj", ".git", ".vs" };

        foreach (var csproj in Directory.EnumerateFiles(o.SolutionRoot!, "*.csproj", SearchOption.AllDirectories))
        {
            var rel = Path.GetRelativePath(o.SolutionRoot!, csproj)
                          .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (rel.Any(p => skip.Contains(p, StringComparer.OrdinalIgnoreCase))) continue;

            string content;
            try { content = File.ReadAllText(csproj); }
            catch { continue; }

            if (!content.Contains("<GenerateDocumentationFile>true</GenerateDocumentationFile>", StringComparison.OrdinalIgnoreCase))
                continue;

            var dir = Path.GetDirectoryName(Path.GetFullPath(csproj));
            if (dir is not null) o.Roots.Add(dir);
        }

        Console.WriteLine($"MlDoc: {o.Roots.Count} proyecto(s) con documentacion XML activada.");
        foreach (var r in o.Roots.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            Console.WriteLine("        " + Path.GetFileName(r));
    }

    private static Options? Fail(string message)
    {
        Console.Error.WriteLine("MlDoc: " + message);
        Usage();
        return null;
    }

    private static void Usage()
    {
        Console.Error.WriteLine("""
            Uso: MlDoc (--docs-only <dir> | --root <dir> | --file <fichero>) [opciones]

              --docs-only <dir>   Explora <dir> y procesa SOLO los proyectos que declaran
                                  GenerateDocumentationFile=true (los publicables).
              --root <dir>        Directorio a recorrer recursivamente (excluye bin/obj).
              --file <fichero>    Fichero .cs concreto.
              --only <patron>     Procesa solo ficheros cuya ruta contenga el patron (repetible).
              --max-files <n>     Limita el numero de ficheros procesados.
              --report <fichero>  Escribe el informe por fichero.
              --dry-run           No escribe nada; solo informa.
            """);
    }
}
