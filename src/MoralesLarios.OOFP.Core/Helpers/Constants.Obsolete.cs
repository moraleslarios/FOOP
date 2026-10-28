// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Helpers;
/// <summary>
/// Representa <c>Constants</c>.
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
}