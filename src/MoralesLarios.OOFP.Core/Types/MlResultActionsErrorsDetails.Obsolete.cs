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
}