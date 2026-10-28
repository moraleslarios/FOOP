using System.Text;

namespace MlDoc;

/// <summary>
/// Troceado de identificadores PascalCase / camelCase en palabras, respetando
/// los acronimos y los prefijos de la nomenclatura del repositorio.
/// </summary>
internal static class Words
{
    /// <summary>Acronimos que NO deben trocearse ni pasarse a minuscula entera.</summary>
    private static readonly string[] Acronyms =
    [
        "HTTP", "HTTPS", "JSON", "XML", "URL", "URI", "API", "DTO", "SQL", "EF",
        "ID", "CRUD", "OOFP", "FP", "ML", "DB", "IO", "UTC", "CSV", "HTML",
    ];

    public static List<string> Split(string identifier)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(identifier)) return result;

        var s = identifier.TrimStart('_', '@');
        if (s.Length == 0) return result;

        var sb = new StringBuilder();
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c == '_')
            {
                Flush(sb, result);
                continue;
            }

            if (sb.Length == 0)
            {
                sb.Append(c);
                continue;
            }

            var prev = s[i - 1];
            var startsWord =
                   (char.IsUpper(c) && !char.IsUpper(prev))                                  // aB   -> a | B
                || (char.IsUpper(c) && char.IsUpper(prev)
                        && i + 1 < s.Length && char.IsLower(s[i + 1]))                        // ABc  -> AB | c
                || (char.IsDigit(c) && !char.IsDigit(prev))                                   // a1   -> a | 1
                || (!char.IsDigit(c) && char.IsDigit(prev));                                  // 1a   -> 1 | a

            if (startsWord) Flush(sb, result);
            sb.Append(c);
        }
        Flush(sb, result);

        // reagrupar acronimos partidos (p.ej. H | T | T | P no ocurre, pero Html -> Html)
        return result;
    }

    private static void Flush(StringBuilder sb, List<string> result)
    {
        if (sb.Length == 0) return;
        result.Add(sb.ToString());
        sb.Clear();
    }

    /// <summary>Indica si la palabra es un acronimo conocido.</summary>
    public static bool IsAcronym(string word)
        => Acronyms.Contains(word.ToUpperInvariant());
}
