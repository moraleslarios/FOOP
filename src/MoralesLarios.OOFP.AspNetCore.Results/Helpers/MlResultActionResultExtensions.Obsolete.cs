// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;
public static partial class MlResultActionResultExtensions
{
    [global::System.Obsolete("Renamed to ToCreatedProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToPostPdActionResult<T>(this MlResult<T> source, Uri uri) => ToCreatedProblemDetailsResult<T>(source, uri);
    [global::System.Obsolete("Renamed to ToCreatedProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToPostPdActionResult<T>(this MlResult<T> source) => ToCreatedProblemDetailsResult<T>(source);
    [global::System.Obsolete("Renamed to ToCreatedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPostPdActionResultAsync<T>(this MlResult<T> source, Uri uri) => ToCreatedProblemDetailsResultAsync<T>(source, uri);
    [global::System.Obsolete("Renamed to ToCreatedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPostPdActionResultAsync<T>(this MlResult<T> source) => ToCreatedProblemDetailsResultAsync<T>(source);
    [global::System.Obsolete("Renamed to ToOkProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToGetPdActionResult<T>(this MlResult<T> source) => ToOkProblemDetailsResult<T>(source);
    [global::System.Obsolete("Renamed to ToOkProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToGetPdActionResultAsync<T>(this MlResult<T> source) => ToOkProblemDetailsResultAsync<T>(source);
    [global::System.Obsolete("Renamed to ToOkProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToGetPdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync) => ToOkProblemDetailsResultAsync<T>(sourceAsync);
    [global::System.Obsolete("Renamed to ToNoContentProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToPutPdActionResult<T>(this MlResult<T> source) => ToNoContentProblemDetailsResult<T>(source);
    [global::System.Obsolete("Renamed to ToNoContentProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPutPdActionResultAsync<T>(this MlResult<T> source) => ToNoContentProblemDetailsResultAsync<T>(source);
    [global::System.Obsolete("Renamed to ToNoContentProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPutPdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync) => ToNoContentProblemDetailsResultAsync<T>(sourceAsync);
    [global::System.Obsolete("Renamed to ToPatchedProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToPatchPdActionResult<T>(this MlResult<T> source) => ToPatchedProblemDetailsResult<T>(source);
    [global::System.Obsolete("Renamed to ToPatchedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPatchPdActionResultAsync<T>(this MlResult<T> source) => ToPatchedProblemDetailsResultAsync<T>(source);
    [global::System.Obsolete("Renamed to ToPatchedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPatchPdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync) => ToPatchedProblemDetailsResultAsync<T>(sourceAsync);
    [global::System.Obsolete("Renamed to ToDeletedProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToDeletePdActionResult<T>(this MlResult<T> source) => ToDeletedProblemDetailsResult<T>(source);
    [global::System.Obsolete("Renamed to ToDeletedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToDeletePdActionResultAsync<T>(this MlResult<T> source) => ToDeletedProblemDetailsResultAsync<T>(source);
    [global::System.Obsolete("Renamed to ToDeletedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToDeletePdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync) => ToDeletedProblemDetailsResultAsync<T>(sourceAsync);
}