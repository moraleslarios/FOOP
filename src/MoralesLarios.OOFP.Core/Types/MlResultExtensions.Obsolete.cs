// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types;
public static partial class MlResultExtensions
{
    [global::System.Obsolete("Renamed to GetValueOrThrow. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static T SecureValidValue<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the secure value from MlResult in Fail state") => GetValueOrThrow<T>(source, exceptionMessage);
    [global::System.Obsolete("Renamed to GetValueOrThrowAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<T> SecureValidValueAsync<T>(this Task<MlResult<T>> sourceAsync, string exceptionMessage = "Cannot obtain the secure value from MlResult in Fail state") => GetValueOrThrowAsync<T>(sourceAsync, exceptionMessage);
    [global::System.Obsolete("Renamed to GetFailureDetailsOrThrow. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlErrorDetails SecureFailErrorsDetails<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the MlErrorDetails from MlResult in Valid state") => GetFailureDetailsOrThrow<T>(source, exceptionMessage);
    [global::System.Obsolete("Renamed to GetFailureDetailsOrThrowAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlErrorDetails> SecureFailErrorsDetailsAsync<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the MlErrorDetails from MlResult in Valid state") => GetFailureDetailsOrThrowAsync<T>(source, exceptionMessage);
    [global::System.Obsolete("Renamed to GetFailureDetailsOrThrowAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlErrorDetails> SecureFailErrorsDetailsAsync<T>(this Task<MlResult<T>> sourceAsync, string exceptionMessage = "Cannot obtain the MlErrorDetails from MlResult in Valid state") => GetFailureDetailsOrThrowAsync<T>(sourceAsync, exceptionMessage);
}