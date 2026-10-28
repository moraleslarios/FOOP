using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MlDoc;

/// <summary>
/// Construye el texto del <c>&lt;summary&gt;</c> a partir del nombre del miembro,
/// aprovechando que la nomenclatura de MoralesLarios.OOFP es composicional:
/// [Try] + Verbo + [IfValid | IfFail[WithValue|WithException|WithoutException]] + [Always] + [Async].
/// </summary>
internal static class SummaryBuilder
{
    // ------------------------------------------------------------------ verbos

    /// <summary>Nucleo semantico de cada verbo raiz conocido del repositorio.</summary>
    private static readonly Dictionary<string, string> Verbs = new(StringComparer.Ordinal)
    {
        ["Bind"]        = "Encadena una funcion que devuelve un resultado",
        ["BindAll"]     = "Encadena una funcion sobre todos los resultados",
        ["Map"]         = "Transforma el valor",
        ["MapAll"]      = "Transforma todos los valores",
        ["Match"]       = "Reduce el resultado a un unico valor aplicando una rama para el caso valido y otra para el fallido",
        ["ExecSelf"]    = "Ejecuta una accion como efecto lateral y devuelve el resultado de origen sin modificarlo",
        ["Exec"]        = "Ejecuta la operacion indicada",
        ["Ensure"]      = "Comprueba que se cumple la condicion indicada",
        ["Validate"]    = "Valida el valor",
        ["Verify"]      = "Comprueba el valor",
        ["Check"]       = "Comprueba el valor",
        ["Filter"]      = "Filtra el valor",
        ["Where"]       = "Filtra el valor",
        ["Select"]      = "Proyecta el valor",
        ["SelectMany"]  = "Proyecta y aplana el valor",
        ["Then"]        = "Continua la cadena con la operacion indicada",
        ["Tap"]         = "Ejecuta una accion como efecto lateral y devuelve el resultado de origen sin modificarlo",
        ["Do"]          = "Ejecuta la accion indicada",
        ["Log"]         = "Registra una traza del resultado",
        ["LogAll"]      = "Registra una traza de todos los resultados",
        ["Merge"]       = "Combina los resultados",
        ["Combine"]     = "Combina los resultados",
        ["Zip"]         = "Combina los resultados en una tupla",
        ["Aggregate"]   = "Agrega los resultados",
        ["Flatten"]     = "Aplana el resultado anidado",
        ["Unwrap"]      = "Extrae el valor del resultado",
        ["Recover"]     = "Recupera el resultado fallido devolviendo un valor alternativo",
        ["OrElse"]      = "Devuelve el resultado alternativo cuando el de origen es fallido",
        ["Complete"]    = "Completa el resultado con informacion adicional",
        ["Add"]         = "Anade el elemento indicado",
        ["Append"]      = "Anade el elemento al final",
        ["Remove"]      = "Elimina el elemento indicado",
        ["Update"]      = "Actualiza el elemento indicado",
        ["Delete"]      = "Elimina el elemento indicado",
        ["Insert"]      = "Inserta el elemento indicado",
        ["Get"]         = "Obtiene el elemento solicitado",
        ["GetAll"]      = "Obtiene todos los elementos",
        ["Find"]        = "Busca el elemento solicitado",
        ["Create"]      = "Crea la instancia solicitada",
        ["Build"]       = "Construye la instancia solicitada",
        ["Parse"]       = "Convierte el texto en el tipo solicitado",
        ["TryParse"]    = "Intenta convertir el texto en el tipo solicitado",
        ["Convert"]     = "Convierte el valor al tipo solicitado",
        ["Clone"]       = "Crea una copia del elemento",
        ["Count"]       = "Cuenta los elementos",
        ["Any"]         = "Indica si existe algun elemento que cumpla la condicion",
        ["All"]         = "Indica si todos los elementos cumplen la condicion",
        ["First"]       = "Obtiene el primer elemento",
        ["Single"]      = "Obtiene el unico elemento",
        ["Save"]        = "Guarda los cambios",
        ["SaveChanges"] = "Confirma los cambios en el almacen de datos",
        ["Send"]        = "Envia la peticion",
        ["Post"]        = "Envia la peticion mediante el verbo HTTP POST",
        ["Put"]         = "Envia la peticion mediante el verbo HTTP PUT",
        ["Patch"]       = "Envia la peticion mediante el verbo HTTP PATCH",
        ["Read"]        = "Lee el contenido",
        ["Write"]       = "Escribe el contenido",
        ["Register"]    = "Registra las dependencias en el contenedor de servicios",
        ["Configure"]   = "Configura las opciones indicadas",
        ["Use"]         = "Incorpora el comportamiento indicado a la canalizacion",
    };

    // ------------------------------------------------------- sufijos modificadores

    /// <summary>
    /// Modificadores reconocidos, en orden de evaluacion (del mas especifico al menos).
    /// El texto se concatena al nucleo del verbo.
    /// </summary>
    private static readonly (string Token, string Text)[] Modifiers =
    [
        ("IfFailWithoutException", " unicamente cuando el resultado de origen es fallido y su detalle de error NO contiene una excepcion"),
        ("IfFailWithException",    " unicamente cuando el resultado de origen es fallido y su detalle de error contiene una excepcion"),
        ("IfFailWithValue",        " unicamente cuando el resultado de origen es fallido, recuperando el valor almacenado en el detalle del error"),
        ("IfFailWithoutValue",     " unicamente cuando el resultado de origen es fallido y su detalle de error NO contiene un valor"),
        ("IfValidWithValue",       " unicamente cuando el resultado de origen es valido, usando su valor"),
        ("IfFailAlways",           " cuando el resultado de origen es fallido, ejecutando siempre la operacion indicada"),
        ("IfValidAlways",          " cuando el resultado de origen es valido, ejecutando siempre la operacion indicada"),
        ("IfFail",                 " unicamente cuando el resultado de origen es fallido"),
        ("IfValid",                " unicamente cuando el resultado de origen es valido"),
        ("IfNotNull",              " unicamente cuando el valor no es nulo"),
        ("IfNull",                 " unicamente cuando el valor es nulo"),
        ("IfEmpty",                " unicamente cuando la coleccion esta vacia"),
        ("IfTrue",                 " unicamente cuando la condicion se cumple"),
        ("IfFalse",                " unicamente cuando la condicion no se cumple"),
        ("Always",                 ", ejecutando siempre la operacion indicada con independencia del estado del resultado"),
    ];

    private const string TryPrefix =
        "Ejecuta la operacion de forma protegida, capturando cualquier excepcion y convirtiendola en un resultado fallido";

    /// <summary>Construye el summary de un metodo.</summary>
    public static string ForMethod(string name, bool isAsync, bool isExtension, bool returnsMlResult)
    {
        var core = Describe(name, out var isTry, out var hadVerb);

        var sb = new System.Text.StringBuilder();
        sb.Append(core);
        if (!core.EndsWith('.')) sb.Append('.');

        if (isTry && hadVerb)
            sb.Append(" Captura cualquier excepcion producida y la convierte en un resultado fallido.");

        if (isAsync)
            sb.Append(" Version asincrona.");

        return sb.ToString();
    }

    /// <summary>Construye el summary de una propiedad.</summary>
    public static string ForProperty(string name, bool isBoolean)
    {
        if (isBoolean && (name.StartsWith("Is", StringComparison.Ordinal)
                       || name.StartsWith("Has", StringComparison.Ordinal)
                       || name.StartsWith("Can", StringComparison.Ordinal)
                       || name.StartsWith("Are", StringComparison.Ordinal)))
            return "Indica si " + LowerFirst(StripBoolPrefix(name)) + ".";

        return "Obtiene el valor de " + Code(name) + ".";
    }

    /// <summary>Construye el summary de un campo o constante.</summary>
    public static string ForField(string name)
        => "Valor de " + Code(name) + ".";

    /// <summary>Construye el summary de un tipo.</summary>
    public static string ForType(TypeDeclarationSyntax type)
    {
        var name = TrimGenericMarkers(type.Identifier.ValueText);

        return type switch
        {
            InterfaceDeclarationSyntax => "Contrato de " + Code(name) + ".",
            _ when name.EndsWith("Extensions", StringComparison.Ordinal)
                                       => "Metodos de extension de " + Code(name[..^"Extensions".Length]) + ".",
            _ when name.EndsWith("Exception", StringComparison.Ordinal)
                                       => "Representa la excepcion " + Code(name) + ".",
            _ when name.EndsWith("Options", StringComparison.Ordinal)
                                       => "Opciones de configuracion de " + Code(name[..^"Options".Length]) + ".",
            _ when name.EndsWith("Attribute", StringComparison.Ordinal)
                                       => "Atributo " + Code(name) + ".",
            _                          => "Representa " + Code(name) + ".",
        };
    }

    /// <summary>Construye el summary de una enumeracion.</summary>
    public static string ForEnum(string name)
        => "Enumera los valores de " + Code(TrimGenericMarkers(name)) + ".";

    /// <summary>Construye el summary de un constructor.</summary>
    public static string ForConstructor(string typeName)
        => "Inicializa una nueva instancia de <see cref=\"" + typeName + "\"/>.";

    /// <summary>Construye el summary de un valor de enumeracion.</summary>
    public static string ForEnumMember(string name)
        => "Valor " + Code(name) + " de la enumeraci\u00F3n.";

    // ------------------------------------------------------------------ interno

    /// <summary>Descompone el nombre en verbo + modificadores y devuelve la frase.</summary>
    private static string Describe(string name, out bool isTry, out bool hadVerb)
    {
        isTry = false;
        hadVerb = false;

        var work = name;

        // sufijo Async (se trata aparte)
        if (work.EndsWith("Async", StringComparison.Ordinal) && work.Length > "Async".Length)
            work = work[..^"Async".Length];

        // prefijo Try (pero NO TryParse, que es un verbo propio)
        if (work.StartsWith("Try", StringComparison.Ordinal)
            && work.Length > 3
            && char.IsUpper(work[3])
            && !Verbs.ContainsKey(work))
        {
            isTry = true;
            work = work[3..];
        }

        // modificador conocido al final
        var modifier = string.Empty;
        foreach (var (token, text) in Modifiers)
        {
            if (!work.EndsWith(token, StringComparison.Ordinal) || work.Length <= token.Length) continue;
            modifier = text;
            work = work[..^token.Length];
            break;
        }

        // verbo raiz conocido
        if (Verbs.TryGetValue(work, out var verbText))
        {
            hadVerb = true;
            return verbText + modifier;
        }

        // verbo raiz conocido como prefijo + complemento libre
        foreach (var verb in Verbs.Keys.OrderByDescending(k => k.Length))
        {
            if (!work.StartsWith(verb, StringComparison.Ordinal) || work.Length <= verb.Length) continue;
            if (!char.IsUpper(work[verb.Length])) continue;

            hadVerb = true;
            var rest = work[verb.Length..];
            return Verbs[verb] + " de " + Code(rest) + modifier;
        }

        if (isTry)
            return TryPrefix + (modifier.Length > 0 ? modifier : " indicada por " + Vocabulary.Humanize(work).ToLowerInvariant());

        // sin verbo reconocido: se construye una frase a partir del propio nombre
        return "Ejecuta " + Code(work) + modifier;
    }

    private static string StripBoolPrefix(string name)
    {
        foreach (var p in new[] { "Is", "Has", "Can", "Are" })
            if (name.StartsWith(p, StringComparison.Ordinal) && name.Length > p.Length && char.IsUpper(name[p.Length]))
                return Vocabulary.Humanize(name[p.Length..]);
        return Vocabulary.Humanize(name);
    }

    private static string LowerFirst(string text)
        => text.Length == 0 ? text : char.ToLowerInvariant(text[0]) + text[1..];

    /// <summary>Devuelve el identificador en forma legible envuelto en <c>&lt;c&gt;</c>.</summary>
    public static string Code(string identifier)
        => "<c>" + identifier + "</c>";

    /// <summary>Pone la frase en minuscula inicial y le anade un articulo si procede.</summary>
    private static string LowerFirstArticle(string text)
        => "el " + LowerFirst(text);

    private static string TrimGenericMarkers(string name)
    {
        var i = name.IndexOf('`');
        return i < 0 ? name : name[..i];
    }
}
