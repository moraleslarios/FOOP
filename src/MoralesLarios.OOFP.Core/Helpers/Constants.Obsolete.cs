// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Helpers;
/// <summary>
/// Miembros obsoletos de <see cref="Constants"/>, conservados como alias de compatibilidad con la versión 2.0.
/// </summary>
public static partial class Constants
{
    /// <summary>
    /// Obtiene el valor de <c>EX_DESC_KEY</c>.
    /// </summary>
    [global::System.Obsolete("Renamed to ExceptionMetadataKey. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string EX_DESC_KEY => ExceptionMetadataKey;

    /// <summary>
    /// Obtiene el valor de <c>VALUE_KEY</c>.
    /// </summary>
    [global::System.Obsolete("Renamed to ValueMetadataKey. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string VALUE_KEY => ValueMetadataKey;

    // Puentes [Obsolete] de la v2.1.1 (epica 1): los nombres con guion bajo disparaban CA1707.
    // Los cuatro que eran `public const` pasan aqui a propiedad de solo lectura: un `const` no puede
    // inicializarse con el valor de otro miembro `static readonly`. Es una ruptura binaria asumida
    // conscientemente (decision D-01).

    /// <summary>
    /// Obtiene el valor de <c>DEFAULT_ERROR_MESSAGE</c>.
    /// </summary>
    [global::System.Obsolete("Renamed to DefaultErrorMessage. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string DEFAULT_ERROR_MESSAGE => DefaultErrorMessage;

    /// <summary>
    /// Obtiene el valor de <c>NOT_FOUND_KEY</c>.
    /// </summary>
    [global::System.Obsolete("Renamed to NotFoundKey. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string NOT_FOUND_KEY => NotFoundKey;

    /// <summary>
    /// Obtiene el valor de <c>PARAM_NAME_KEY</c>.
    /// </summary>
    [global::System.Obsolete("Renamed to ParameterNameKey. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string PARAM_NAME_KEY => ParameterNameKey;

    /// <summary>
    /// Obtiene el valor de <c>FAILED_INDEXES_KEY</c>.
    /// </summary>
    [global::System.Obsolete("Renamed to FailedIndexesKey. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string FAILED_INDEXES_KEY => FailedIndexesKey;

    /// <summary>
    /// Obtiene el valor de <c>EXPECTED_KEY</c>.
    /// </summary>
    [global::System.Obsolete("Renamed to ExpectedKey. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string EXPECTED_KEY => ExpectedKey;

    /// <summary>
    /// Construye el mensaje de error por defecto para una excepcion capturada.
    /// </summary>
    /// <param name="ex">Excepcion capturada.</param>
    /// <returns>Mensaje de error compuesto.</returns>
    [global::System.Obsolete("Renamed to EnsureFpMessages.DefaultExceptionError. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string DEFAULT_EX_ERROR_MESSAGE(Exception ex) => EnsureFpMessages.DefaultExceptionError(ex);
}