namespace MoralesLarios.OOFP.Shared.Web;

/// <summary>
/// Representa <c>WebErrorDetailsKeys</c>.
/// </summary>
public static class WebErrorDetailsKeys
{
    /// <summary>
    /// Constante con problems details.
    /// </summary>
    public const string ProblemsDetails = "ProblemsDetails";

    /// <summary>
    /// Valor por defecto del miembro <c>type</c> de un ProblemDetails cuando el consumidor no
    /// proporciona una URI propia. Segun la RFC 9457, <c>about:blank</c> indica que el problema
    /// no tiene mas semantica que la del propio codigo de estado HTTP.
    /// </summary>
    public const string DefaultProblemType = "about:blank";

    /// <summary>
    /// Valor por defecto de la cabecera <c>Location</c> cuando se crea un recurso y el consumidor
    /// no proporciona una URI. Es una URI relativa valida que no filtra dominios de terceros.
    /// </summary>
    public const string DefaultCreatedLocation = "/";
}