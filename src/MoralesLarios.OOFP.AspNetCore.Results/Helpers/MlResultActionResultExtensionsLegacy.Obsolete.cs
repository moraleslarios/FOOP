// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;
public static partial class MlResultActionResultExtensionsLegacy
{
    [global::System.Obsolete("Renamed to ToCreatedResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToSimpleRepoPostActionResult<T>(this MlResult<T> source, ControllerBase controllerBase) => ToCreatedResult<T>(source, controllerBase);
    [global::System.Obsolete("Renamed to ToCreatedResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToSimpleRepoPostActionResultAsync<T>(this MlResult<T> source, ControllerBase controllerBase) => ToCreatedResultAsync<T>(source, controllerBase);
    [global::System.Obsolete("Renamed to ToCreatedResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToSimpleRepoPostActionResultAsync<T>(this Task<MlResult<T>> sourceAsync, ControllerBase controllerBase) => ToCreatedResultAsync<T>(sourceAsync, controllerBase);
}