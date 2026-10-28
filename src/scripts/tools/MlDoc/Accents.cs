using System.Text;
using System.Text.RegularExpressions;

namespace MlDoc;

/// <summary>
/// Restaura la acentuacion de las palabras del texto generado, respetando el estilo
/// del repositorio (la documentacion existente esta escrita en castellano con tildes).
/// </summary>
/// <remarks>
/// El vocabulario interno se escribe sin tildes para evitar problemas de codificacion
/// en los ficheros fuente de la herramienta; esta clase las repone en el ultimo paso.
/// Nunca se modifica el contenido de <c>&lt;c&gt;...&lt;/c&gt;</c> ni de los atributos
/// <c>cref</c>/<c>name</c>, porque ahi aparecen identificadores de codigo.
/// </remarks>
internal static class Accents
{
    /// <summary>Palabras sin tilde (clave, en minuscula) y su forma acentuada.</summary>
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["funcion"]         = "funci\u00F3n",
        ["funciones"]       = "funciones",
        ["operacion"]       = "operaci\u00F3n",
        ["operaciones"]     = "operaciones",
        ["excepcion"]       = "excepci\u00F3n",
        ["excepciones"]     = "excepciones",
        ["informacion"]     = "informaci\u00F3n",
        ["configuracion"]   = "configuraci\u00F3n",
        ["representacion"]  = "representaci\u00F3n",
        ["conversion"]      = "conversi\u00F3n",
        ["coleccion"]       = "colecci\u00F3n",
        ["condicion"]       = "condici\u00F3n",
        ["validacion"]      = "validaci\u00F3n",
        ["transformacion"]  = "transformaci\u00F3n",
        ["proyeccion"]      = "proyecci\u00F3n",
        ["comparacion"]     = "comparaci\u00F3n",
        ["serializacion"]   = "serializaci\u00F3n",
        ["deserializacion"] = "deserializaci\u00F3n",
        ["paginacion"       ] = "paginaci\u00F3n",
        ["ordenacion"]      = "ordenaci\u00F3n",
        ["conexion"]        = "conexi\u00F3n",
        ["peticion"]        = "petici\u00F3n",
        ["direccion"]       = "direcci\u00F3n",
        ["descripcion"]     = "descripci\u00F3n",
        ["version"]         = "versi\u00F3n",
        ["asincrona"]       = "as\u00EDncrona",
        ["asincrono"]       = "as\u00EDncrono",
        ["sincrona"]        = "s\u00EDncrona",
        ["unico"]           = "\u00FAnico",
        ["unica"]           = "\u00FAnica",
        ["valido"]          = "v\u00E1lido",
        ["valida"]          = "v\u00E1lida",
        ["invalido"]        = "inv\u00E1lido",
        ["metodo"]          = "m\u00E9todo",
        ["metodos"]         = "m\u00E9todos",
        ["parametro"]       = "par\u00E1metro",
        ["parametros"]      = "par\u00E1metros",
        ["numero"]          = "n\u00FAmero",
        ["numerico"]        = "num\u00E9rico",
        ["indice"]          = "\u00EDndice",
        ["maximo"]          = "m\u00E1ximo",
        ["minimo"]          = "m\u00EDnimo",
        ["ultimo"]          = "\u00FAltimo",
        ["generico"]        = "gen\u00E9rico",
        ["generica"]        = "gen\u00E9rica",
        ["logico"]          = "l\u00F3gico",
        ["basico"]          = "b\u00E1sico",
        ["automaticamente"] = "autom\u00E1ticamente",
        ["explicitamente"]  = "expl\u00EDcitamente",
        ["implicita"]       = "impl\u00EDcita",
        ["explicita"]       = "expl\u00EDcita",
        ["caracter"]        = "car\u00E1cter",
        ["caracteres"]      = "caracteres",
        ["codigo"]          = "c\u00F3digo",
        ["anade"]           = "a\u00F1ade",
        ["anadido"]         = "a\u00F1adido",
        ["tambien"]         = "tambi\u00E9n",
        ["despues"]         = "despu\u00E9s",
        ["segun"]           = "seg\u00FAn",
        ["ademas"]          = "adem\u00E1s",
        ["atomico"]         = "at\u00F3mico",
        ["atomicos"]        = "at\u00F3micos",
        ["expresion"]       = "expresi\u00F3n",
        ["accion"]          = "acci\u00F3n",
        ["acciones"]        = "acciones",
        ["cancelacion"]     = "cancelaci\u00F3n",
        ["instanciacion"]   = "instanciaci\u00F3n",
        ["posicion"]        = "posici\u00F3n",
        ["opcion"]          = "opci\u00F3n",
        ["opciones"]        = "opciones",
        ["identificacion"]  = "identificaci\u00F3n",
        ["agregacion"]      = "agregaci\u00F3n",
        ["concatenacion"]   = "concatenaci\u00F3n",
        ["autenticacion"]   = "autenticaci\u00F3n",
        ["autorizacion"]    = "autorizaci\u00F3n",
        ["cabeceras"]       = "cabeceras",
        ["anidado"]         = "anidado",
        ["intencion"]       = "intenci\u00F3n",
        ["enumeracion"]     = "enumeraci\u00F3n",
        ["enumeraciones"]   = "enumeraciones",
        ["acompanan"]       = "acompa\u00F1an",
    };

    /// <summary>Divide el texto en tramos protegidos (codigo) y tramos de prosa.</summary>
    private static readonly Regex Protected = new(
        "<c>.*?</c>|<see\\s[^>]*?/>|<paramref\\s[^>]*?/>|<typeparamref\\s[^>]*?/>|(?:cref|name)=\"[^\"]*\"",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex Word = new("[A-Za-z]+", RegexOptions.Compiled);

    /// <summary>Repone las tildes del texto dejando intactos los identificadores de codigo.</summary>
    public static string Fix(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        var sb = new StringBuilder(text.Length + 16);
        var last = 0;

        foreach (var m in Protected.Matches(text).Cast<Match>())
        {
            sb.Append(FixProse(text[last..m.Index]));
            sb.Append(m.Value);
            last = m.Index + m.Length;
        }

        sb.Append(FixProse(text[last..]));
        return sb.ToString();
    }

    private static string FixProse(string prose)
        => prose.Length == 0
            ? prose
            : Word.Replace(prose, m => Map.TryGetValue(m.Value, out var fixedWord)
                                            ? Preserve(m.Value, fixedWord)
                                            : m.Value);

    /// <summary>Conserva la mayuscula inicial del original en la palabra acentuada.</summary>
    private static string Preserve(string original, string accented)
        => char.IsUpper(original[0])
            ? char.ToUpperInvariant(accented[0]) + accented[1..]
            : accented;
}
