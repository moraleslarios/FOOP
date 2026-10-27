// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Helpers;
public static partial class Constants
{
    [global::System.Obsolete("Renamed to ExceptionMetadataKey. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string EX_DESC_KEY => ExceptionMetadataKey;

    [global::System.Obsolete("Renamed to ValueMetadataKey. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string VALUE_KEY => ValueMetadataKey;
}