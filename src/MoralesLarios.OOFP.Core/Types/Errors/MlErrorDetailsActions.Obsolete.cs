// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types.Errors;
public static partial class MlErrorDetailsActions
{
    [global::System.Obsolete("Renamed to ContainsMetadata. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static bool HasKeyDetails(this MlErrorDetails source, string key) => ContainsMetadata(source, key);
    [global::System.Obsolete("Renamed to ContainsValueMetadata. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static bool HasValueDetails(this MlErrorDetails source) => ContainsValueMetadata(source);
    [global::System.Obsolete("Renamed to ContainsValueMetadataAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<bool> HasValueDetailsAsync(this MlErrorDetails source) => ContainsValueMetadataAsync(source);
    [global::System.Obsolete("Renamed to ContainsException. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static bool HasExceptionDetails(this MlErrorDetails source) => ContainsException(source);
    [global::System.Obsolete("Renamed to ContainsExceptionAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<bool> HasExceptionDetailsAsync(this MlErrorDetails source) => ContainsExceptionAsync(source);
    [global::System.Obsolete("Renamed to AddMetadataOrThrow. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlErrorDetails AddDetail<T>(this MlErrorDetails source, string key, T value) => AddMetadataOrThrow<T>(source, key, value);
    [global::System.Obsolete("Renamed to AddMetadataRange. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlErrorDetails AddDetails(this MlErrorDetails source, Dictionary<string, object> otherDetails) => AddMetadataRange(source, otherDetails);
    [global::System.Obsolete("Renamed to AddMetadataRange. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlErrorDetails AddDetails(this MlErrorDetails source, params (string key, object value)[] otherDetails) => AddMetadataRange(source, otherDetails);
    [global::System.Obsolete("Renamed to AddValueMetadata. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlErrorDetails AddDetailValue<T>(this MlErrorDetails source, T value) => AddValueMetadata<T>(source, value);
}