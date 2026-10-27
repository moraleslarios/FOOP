// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Types;
public static partial class MlResultActionsErrorsDetails
{
    [global::System.Obsolete("Renamed to MergeErrorsDetailsIfFailDifferentTypes. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> MergeErrorsDetailsIfFailDiferentTypes<T, T2>(this MlResult<T> source, MlResult<T2> secondary) => MergeErrorsDetailsIfFailDifferentTypes<T, T2>(source, secondary);
    [global::System.Obsolete("Renamed to MergeErrorsDetailsIfFailDifferentTypesAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T, T2>(this MlResult<T> source, MlResult<T2> secondary) => MergeErrorsDetailsIfFailDifferentTypesAsync<T, T2>(source, secondary);
    [global::System.Obsolete("Renamed to MergeErrorsDetailsIfFailDifferentTypesAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T, T2>(this Task<MlResult<T>> sourceAsync, MlResult<T2> secondary) => MergeErrorsDetailsIfFailDifferentTypesAsync<T, T2>(sourceAsync, secondary);
    [global::System.Obsolete("Renamed to MergeErrorsDetailsIfFailDifferentTypesAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T, T2>(this Task<MlResult<T>> sourceAsync, Task<MlResult<T2>> secondaryAsync) => MergeErrorsDetailsIfFailDifferentTypesAsync<T, T2>(sourceAsync, secondaryAsync);
    [global::System.Obsolete("Renamed to MergeErrorsDetailsIfFailDifferentTypesAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T, T2>(this MlResult<T> source, Task<MlResult<T2>> secondaryAsync) => MergeErrorsDetailsIfFailDifferentTypesAsync<T, T2>(source, secondaryAsync);
    [global::System.Obsolete("Renamed to GetMetadataValue. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> GetDetailValue<T>(this MlErrorDetails source) => GetMetadataValue<T>(source);
    [global::System.Obsolete("Renamed to GetMetadataValueAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> GetDetailValueAsync<T>(this MlErrorDetails source) => GetMetadataValueAsync<T>(source);
    [global::System.Obsolete("Renamed to GetMetadataValueAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> GetDetailValueAsync<T>(this Task<MlErrorDetails> sourceAsync) => GetMetadataValueAsync<T>(sourceAsync);
    [global::System.Obsolete("Renamed to GetException. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> GetDetailException<T>(this MlErrorDetails source)
        where T : Exception => GetException<T>(source);
    [global::System.Obsolete("Renamed to GetException. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<Exception> GetDetailException(this MlErrorDetails source) => GetException(source);
    [global::System.Obsolete("Renamed to GetExceptionAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> GetDetailExceptionAsync<T>(this MlErrorDetails source)
        where T : Exception => GetExceptionAsync<T>(source);
    [global::System.Obsolete("Renamed to GetExceptionAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> GetDetailExceptionAsync<T>(this Task<MlErrorDetails> sourceAsync)
        where T : Exception => GetExceptionAsync<T>(sourceAsync);
    [global::System.Obsolete("Renamed to GetExceptionAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<Exception>> GetDetailExceptionAsync(this MlErrorDetails source) => GetExceptionAsync(source);
    [global::System.Obsolete("Renamed to GetExceptionAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<Exception>> GetDetailExceptionAsync(this Task<MlErrorDetails> sourceAsync) => GetExceptionAsync(sourceAsync);
}