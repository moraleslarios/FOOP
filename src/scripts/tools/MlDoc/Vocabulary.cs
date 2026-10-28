namespace MlDoc;

/// <summary>
/// Vocabulario del dominio del repositorio. Permite generar descripciones reales
/// (no genericas) porque la nomenclatura de MoralesLarios.OOFP es muy regular.
/// </summary>
internal static class Vocabulary
{
    // ---------------------------------------------------------------- parametros

    /// <summary>Descripciones por nombre EXACTO de parametro.</summary>
    private static readonly Dictionary<string, string> ParamExact = new(StringComparer.Ordinal)
    {
        // --- fuente / resultado de entrada
        ["source"]              = "Resultado de origen sobre el que se aplica la operacion.",
        ["sourceAsync"]         = "Tarea que produce el resultado de origen.",
        ["sources"]             = "Coleccion de resultados de origen.",
        ["sourcesAsync"]        = "Tarea que produce la coleccion de resultados de origen.",
        ["result"]              = "Resultado sobre el que se opera.",
        ["resultAsync"]         = "Tarea que produce el resultado sobre el que se opera.",
        ["results"]             = "Coleccion de resultados.",
        ["other"]               = "Segundo resultado que participa en la operacion.",
        ["otherAsync"]          = "Tarea que produce el segundo resultado que participa en la operacion.",

        // --- funciones
        ["func"]                = "Funcion que se ejecuta para producir el nuevo resultado.",
        ["funcAsync"]           = "Funcion asincrona que se ejecuta para producir el nuevo resultado.",
        ["funcValid"]           = "Funcion que se ejecuta cuando el resultado de origen es valido.",
        ["funcValidAsync"]      = "Funcion asincrona que se ejecuta cuando el resultado de origen es valido.",
        ["funcFail"]            = "Funcion que se ejecuta cuando el resultado de origen es fallido.",
        ["funcFailAsync"]       = "Funcion asincrona que se ejecuta cuando el resultado de origen es fallido.",
        ["funcException"]       = "Funcion que recibe la excepcion almacenada en el detalle del error.",
        ["funcExceptionAsync"]  = "Funcion asincrona que recibe la excepcion almacenada en el detalle del error.",
        ["funcError"]           = "Funcion que recibe el error del resultado de origen.",
        ["funcErrorAsync"]      = "Funcion asincrona que recibe el error del resultado de origen.",
        ["funcValue"]           = "Funcion que recibe el valor del resultado de origen.",
        ["funcValueAsync"]      = "Funcion asincrona que recibe el valor del resultado de origen.",
        ["funcResult"]          = "Funcion que produce el resultado final.",
        ["funcResultAsync"]     = "Funcion asincrona que produce el resultado final.",
        ["selector"]            = "Funcion de proyeccion aplicada a cada elemento.",
        ["selectorAsync"]       = "Funcion de proyeccion asincrona aplicada a cada elemento.",
        ["factory"]             = "Funcion que crea la instancia solicitada.",
        ["factoryAsync"]        = "Funcion asincrona que crea la instancia solicitada.",
        ["builder"]             = "Delegado de configuracion que construye el objeto.",
        ["configure"]           = "Delegado de configuracion aplicado a las opciones.",
        ["predicate"]           = "Condicion que debe cumplirse.",
        ["predicateAsync"]      = "Condicion asincrona que debe cumplirse.",
        ["condition"]           = "Condicion que debe cumplirse.",
        ["validator"]           = "Validador aplicado al valor.",

        // --- acciones
        ["action"]              = "Accion que se ejecuta como efecto lateral.",
        ["actionAsync"]         = "Accion asincrona que se ejecuta como efecto lateral.",
        ["actionValid"]         = "Accion que se ejecuta cuando el resultado de origen es valido.",
        ["actionValidAsync"]    = "Accion asincrona que se ejecuta cuando el resultado de origen es valido.",
        ["actionFail"]          = "Accion que se ejecuta cuando el resultado de origen es fallido.",
        ["actionFailAsync"]     = "Accion asincrona que se ejecuta cuando el resultado de origen es fallido.",
        ["actionException"]     = "Accion que recibe la excepcion almacenada en el detalle del error.",
        ["actionError"]         = "Accion que recibe el error del resultado de origen.",
        ["actionValue"]         = "Accion que recibe el valor del resultado de origen.",

        // --- errores
        ["error"]               = "Error que describe el fallo.",
        ["errors"]              = "Coleccion de errores que describen el fallo.",
        ["errorMessage"]        = "Mensaje descriptivo del error.",
        ["errorMessages"]       = "Mensajes descriptivos de los errores.",
        ["errorsDetails"]       = "Detalles del error, con su mensaje y metadatos asociados.",
        ["errorDetails"]        = "Detalles del error, con su mensaje y metadatos asociados.",
        ["message"]             = "Mensaje descriptivo.",
        ["messages"]            = "Mensajes descriptivos.",
        ["ex"]                  = "Excepcion capturada.",
        ["exception"]           = "Excepcion capturada.",
        ["exceptions"]          = "Excepciones capturadas.",
        ["exceptionBuilder"]    = "Funcion que construye el mensaje de error a partir de la excepcion.",
        ["errorBuilder"]        = "Funcion que construye el error a partir de la excepcion.",

        // --- valores
        ["value"]               = "Valor sobre el que se opera.",
        ["values"]              = "Valores sobre los que se opera.",
        ["valueAsync"]          = "Tarea que produce el valor sobre el que se opera.",
        ["defaultValue"]        = "Valor devuelto cuando no se obtiene ninguno.",
        ["newValue"]            = "Nuevo valor que se asigna.",
        ["key"]                 = "Clave identificativa.",
        ["keys"]                = "Claves identificativas.",
        ["id"]                  = "Identificador de la entidad.",
        ["ids"]                 = "Identificadores de las entidades.",
        ["name"]                = "Nombre del elemento.",
        ["paramName"]           = "Nombre del parametro que origina la validacion.",
        ["propertyName"]        = "Nombre de la propiedad.",
        ["entity"]              = "Entidad sobre la que se opera.",
        ["entities"]            = "Entidades sobre las que se opera.",
        ["dto"]                 = "Objeto de transferencia de datos.",
        ["dtos"]                = "Objetos de transferencia de datos.",
        ["item"]                = "Elemento sobre el que se opera.",
        ["items"]               = "Elementos sobre los que se opera.",
        ["collection"]          = "Coleccion sobre la que se opera.",
        ["details"]             = "Informacion adicional asociada.",
        ["data"]                = "Datos asociados a la operacion.",
        ["metadata"]            = "Metadatos asociados a la operacion.",
        ["options"]             = "Opciones que modifican el comportamiento de la operacion.",
        ["settings"]            = "Opciones de configuracion.",
        ["configuration"]       = "Configuracion de la aplicacion.",
        ["cancellationToken"]   = "Token para cancelar la operacion.",
        ["ct"]                  = "Token para cancelar la operacion.",

        // --- infraestructura
        ["logger"]              = "Registrador al que se envian las trazas.",
        ["logLevel"]            = "Nivel de severidad de la traza.",
        ["eventId"]             = "Identificador del evento de la traza.",
        ["services"]            = "Coleccion de servicios donde se registran las dependencias.",
        ["serviceProvider"]     = "Proveedor de servicios.",
        ["provider"]            = "Proveedor de servicios.",
        ["context"]             = "Contexto de ejecucion.",
        ["dbContext"]           = "Contexto de Entity Framework Core.",
        ["repository"]          = "Repositorio de acceso a datos.",
        ["httpClient"]          = "Cliente HTTP utilizado para la llamada.",
        ["client"]              = "Cliente utilizado para la llamada.",
        ["request"]             = "Peticion que se envia.",
        ["response"]            = "Respuesta recibida.",
        ["httpRequest"]         = "Peticion HTTP que se envia.",
        ["httpResponse"]        = "Respuesta HTTP recibida.",
        ["url"]                 = "Direccion a la que se dirige la llamada.",
        ["uri"]                 = "Direccion a la que se dirige la llamada.",
        ["route"]               = "Ruta relativa del recurso.",
        ["path"]                = "Ruta del recurso.",
        ["filePath"]            = "Ruta del fichero.",
        ["directoryPath"]       = "Ruta del directorio.",
        ["content"]             = "Contenido enviado en el cuerpo de la peticion.",
        ["body"]                = "Contenido del cuerpo de la peticion.",
        ["headers"]             = "Cabeceras que acompa\u00F1an a la peticion.",
        ["statusCode"]          = "Codigo de estado HTTP.",
        ["mediaType"]           = "Tipo de contenido de la peticion.",
        ["encoding"]            = "Codificacion de caracteres utilizada.",
        ["jsonOptions"]         = "Opciones de serializacion JSON.",

        // --- paginacion / consulta
        ["page"]                = "Numero de pagina solicitada, comenzando en uno.",
        ["pageNumber"]          = "Numero de pagina solicitada, comenzando en uno.",
        ["pageSize"]            = "Numero de elementos por pagina.",
        ["skip"]                = "Numero de elementos que se omiten.",
        ["take"]                = "Numero maximo de elementos que se devuelven.",
        ["filter"]              = "Expresion de filtrado aplicada a la consulta.",
        ["orderBy"]             = "Expresion de ordenacion aplicada a la consulta.",
        ["ascending"]           = "Indica si la ordenacion es ascendente.",
        ["includes"]            = "Propiedades de navegacion que se incluyen en la consulta.",
        ["query"]               = "Consulta sobre la que se opera.",
        ["asNoTracking"]        = "Indica si la consulta se ejecuta sin seguimiento de cambios.",
        ["saveChanges"]         = "Indica si se confirman los cambios en el almacen de datos.",
    };

    /// <summary>Descripciones por SUFIJO de nombre de parametro (se evaluan en orden).</summary>
    private static readonly (string Suffix, string Text)[] ParamSuffix =
    [
        ("CancellationToken", "Token para cancelar la operacion."),
        ("ErrorMessage",      "Mensaje descriptivo del error."),
        ("ErrorsDetails",     "Detalles del error, con su mensaje y metadatos asociados."),
        ("Exception",         "Excepcion capturada."),
        ("Predicate",         "Condicion que debe cumplirse."),
        ("Selector",          "Funcion de proyeccion aplicada a cada elemento."),
        ("Factory",           "Funcion que crea la instancia solicitada."),
        ("Options",           "Opciones que modifican el comportamiento de la operacion."),
        ("Logger",            "Registrador al que se envian las trazas."),
        ("Repository",        "Repositorio de acceso a datos."),
        ("Context",           "Contexto de ejecucion."),
        ("Message",           "Mensaje descriptivo."),
        ("Name",              "Nombre del elemento."),
        ("Path",              "Ruta del recurso."),
        ("Url",               "Direccion a la que se dirige la llamada."),
        ("Id",                "Identificador del elemento."),
        ("Key",               "Clave identificativa."),
        ("Value",             "Valor sobre el que se opera."),
        ("Func",              "Funcion que se ejecuta durante la operacion."),
        ("Action",            "Accion que se ejecuta como efecto lateral."),
    ];

    // ---------------------------------------------------------------- genericos

    private static readonly Dictionary<string, string> TypeParamExact = new(StringComparer.Ordinal)
    {
        ["T"]           = "Tipo del valor transportado por el resultado.",
        ["T1"]          = "Tipo del primer valor.",
        ["T2"]          = "Tipo del segundo valor.",
        ["T3"]          = "Tipo del tercer valor.",
        ["T4"]          = "Tipo del cuarto valor.",
        ["T5"]          = "Tipo del quinto valor.",
        ["TSource"]     = "Tipo del valor de origen.",
        ["TResult"]     = "Tipo del valor devuelto.",
        ["TReturn"]     = "Tipo del valor devuelto.",
        ["TValue"]      = "Tipo del valor.",
        ["TNew"]        = "Tipo del nuevo valor.",
        ["TOut"]        = "Tipo del valor de salida.",
        ["TIn"]         = "Tipo del valor de entrada.",
        ["TException"]  = "Tipo de la excepcion esperada.",
        ["TError"]      = "Tipo del error.",
        ["TEntity"]     = "Tipo de la entidad de datos.",
        ["TDto"]        = "Tipo del objeto de transferencia de datos.",
        ["TKey"]        = "Tipo de la clave de la entidad.",
        ["TId"]         = "Tipo del identificador de la entidad.",
        ["TContext"]    = "Tipo del contexto de datos.",
        ["TOptions"]    = "Tipo de las opciones de configuracion.",
        ["TService"]    = "Tipo del servicio.",
        ["TRepository"] = "Tipo del repositorio.",
        ["TEnum"]       = "Tipo de la enumeraci\u00F3n.",
        ["TSelf"]       = "Tipo que implementa el contrato, usado para el patron de tipo curiosamente recurrente.",
        ["TCollection"] = "Tipo de la coleccion.",
        ["TItem"]       = "Tipo de los elementos de la coleccion.",
        ["TProperty"]   = "Tipo de la propiedad.",
        ["TRequest"]    = "Tipo de la peticion.",
        ["TResponse"]   = "Tipo de la respuesta.",
    };

    public static string ParamText(string name)
    {
        if (ParamExact.TryGetValue(name, out var exact)) return exact;

        // normalizacion: quitar el sufijo Async y reintentar
        if (name.EndsWith("Async", StringComparison.Ordinal))
        {
            var core = name[..^"Async".Length];
            if (ParamExact.TryGetValue(core, out var baseText))
                return AsAsync(baseText);
        }

        foreach (var (suffix, text) in ParamSuffix)
            if (name.Length > suffix.Length && name.EndsWith(suffix, StringComparison.Ordinal))
                return text;

        return Humanize(name) + ".";
    }

    public static string TypeParamText(string name)
        => TypeParamExact.TryGetValue(name, out var t) ? t : "Tipo generico " + name + ".";

    private static string AsAsync(string text)
    {
        if (text.StartsWith("Funcion ", StringComparison.Ordinal))
            return "Funcion asincrona " + text["Funcion ".Length..];
        if (text.StartsWith("Accion ", StringComparison.Ordinal))
            return "Accion asincrona " + text["Accion ".Length..];
        return text;
    }

    /// <summary>Convierte un identificador camelCase/PascalCase en una frase legible.</summary>
    public static string Humanize(string identifier)
    {
        var words = Words.Split(identifier);
        if (words.Count == 0) return identifier;
        var joined = string.Join(' ', words.Select(w => w.ToLowerInvariant()));
        return char.ToUpperInvariant(joined[0]) + joined[1..];
    }
}
