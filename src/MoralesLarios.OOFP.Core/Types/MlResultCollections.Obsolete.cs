// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types;
public static partial class MlResultCollections
{
    [global::System.Obsolete("Renamed to MergeFailureErrors. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<T>> FusionFailErros<T>(this IEnumerable<MlResult<T>> source) => MergeFailureErrors<T>(source);
    [global::System.Obsolete("Renamed to MergeFailureErrorsAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>> FusionFailErrosAsync<T>(this IEnumerable<MlResult<T>> source) => MergeFailureErrorsAsync<T>(source);
    [global::System.Obsolete("Renamed to MergeFailureErrorsAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>> FusionFailErrosAsync<T>(this Task<IEnumerable<MlResult<T>>> sourceAsync) => MergeFailureErrorsAsync<T>(sourceAsync);
    [global::System.Obsolete("Renamed to MergeErrorsIfExists. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<T>> FusionErrosIfExists<T>(this IEnumerable<MlResult<T>> source) => MergeErrorsIfExists<T>(source);
    [global::System.Obsolete("Renamed to MergeErrorsIfExistsAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>> FusionErrosIfExistsAsync<T>(this IEnumerable<MlResult<T>> source) => MergeErrorsIfExistsAsync<T>(source);
    [global::System.Obsolete("Renamed to MergeErrorsIfExistsAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>> FusionErrosIfExistsAsync<T>(this Task<IEnumerable<MlResult<T>>> sourceAsync) => MergeErrorsIfExistsAsync<T>(sourceAsync);
    [global::System.Obsolete("Renamed to Traverse. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<TResult>> Projection<T, TResult>(this IEnumerable<T> source, Func<T, MlResult<TResult>> completeFuncTransform) => Traverse<T, TResult>(source, completeFuncTransform);
    [global::System.Obsolete("Renamed to Traverse. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<TResult>> Projection<T, TResult>(this IEnumerable<T> source, Func<T, int, MlResult<TResult>> completeFuncTransform) => Traverse<T, TResult>(source, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this IEnumerable<T> source, Func<T, MlResult<TResult>> completeFuncTransform) => TraverseAsync<T, TResult>(source, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this IEnumerable<T> source, Func<T, int, MlResult<TResult>> completeFuncTransform) => TraverseAsync<T, TResult>(source, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseAsync<T, TResult>(sourceAsync, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseAsync<T, TResult>(sourceAsync, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, MlResult<TResult>> completeFuncTransform) => TraverseAsync<T, TResult>(sourceAsync, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, int, MlResult<TResult>> completeFuncTransform) => TraverseAsync<T, TResult>(sourceAsync, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this IEnumerable<T> source, Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseAsync<T, TResult>(source, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this IEnumerable<T> source, Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseAsync<T, TResult>(source, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseWhile. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<TResult>> ProjectionWhile<T, TResult>(this IEnumerable<T> source, Func<T, MlResult<TResult>> completeFuncTransform) => TraverseWhile<T, TResult>(source, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseWhile. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<TResult>> ProjectionWhile<T, TResult>(this IEnumerable<T> source, Func<T, int, MlResult<TResult>> completeFuncTransform) => TraverseWhile<T, TResult>(source, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseWhileAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this IEnumerable<T> source, Func<T, MlResult<TResult>> completeFuncTransform) => TraverseWhileAsync<T, TResult>(source, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseWhileAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this IEnumerable<T> source, Func<T, int, MlResult<TResult>> completeFuncTransform) => TraverseWhileAsync<T, TResult>(source, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseWhileAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseWhileAsync<T, TResult>(sourceAsync, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseWhileAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseWhileAsync<T, TResult>(sourceAsync, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseWhileAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, MlResult<TResult>> completeFuncTransform) => TraverseWhileAsync<T, TResult>(sourceAsync, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseWhileAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, int, MlResult<TResult>> completeFuncTransform) => TraverseWhileAsync<T, TResult>(sourceAsync, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseWhileAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this IEnumerable<T> source, Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseWhileAsync<T, TResult>(source, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseWhileAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this IEnumerable<T> source, Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseWhileAsync<T, TResult>(source, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseParallelAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionParallelAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseParallelAsync<T, TResult>(sourceAsync, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseParallelAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionParallelAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseParallelAsync<T, TResult>(sourceAsync, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseParallelAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionParallelAsync<T, TResult>(this IEnumerable<T> source, Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseParallelAsync<T, TResult>(source, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseParallelAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<TResult>>> ProjectionParallelAsync<T, TResult>(this IEnumerable<T> source, Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync) => TraverseParallelAsync<T, TResult>(source, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseSplit. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)> ProjectionSplit<T, TResult>(this IEnumerable<T> source, Func<T, MlResult<TResult>> completeFuncTransform)
        where T : notnull => TraverseSplit<T, TResult>(source, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseSplitAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> ProjectionSplitAsync<T, TResult>(this IEnumerable<T> source, Func<T, MlResult<TResult>> completeFuncTransform)
        where T : notnull => TraverseSplitAsync<T, TResult>(source, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseSplitAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> ProjectionSplitAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, MlResult<TResult>> completeFuncTransform)
        where T : notnull => TraverseSplitAsync<T, TResult>(sourceAsync, completeFuncTransform);
    [global::System.Obsolete("Renamed to TraverseSplitAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> ProjectionSplitAsync<T, TResult>(this IEnumerable<T> source, Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where T : notnull => TraverseSplitAsync<T, TResult>(source, completeFuncTransformAsync);
    [global::System.Obsolete("Renamed to TraverseSplitAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> ProjectionSplitAsync<T, TResult>(this Task<IEnumerable<T>> sourceAsync, Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where T : notnull => TraverseSplitAsync<T, TResult>(sourceAsync, completeFuncTransformAsync);
}