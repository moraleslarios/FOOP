// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;
/// <summary>
/// Representa <c>MlResultActionResultExtensionsLegacy</c>.
/// </summary>
public static partial class MlResultActionResultExtensionsLegacy
{
    /// <summary>
    /// Ejecuta <c>ToSimpleRepoPostActionResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="controllerBase">Valor de <c>controllerBase</c>.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToCreatedResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToSimpleRepoPostActionResult<T>(this MlResult<T> source, ControllerBase controllerBase) => ToCreatedResult<T>(source, controllerBase);
    /// <summary>
    /// Ejecuta <c>ToSimpleRepoPostActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="controllerBase">Valor de <c>controllerBase</c>.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToCreatedResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToSimpleRepoPostActionResultAsync<T>(this MlResult<T> source, ControllerBase controllerBase) => ToCreatedResultAsync<T>(source, controllerBase);
    /// <summary>
    /// Ejecuta <c>ToSimpleRepoPostActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="controllerBase">Valor de <c>controllerBase</c>.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToCreatedResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToSimpleRepoPostActionResultAsync<T>(this Task<MlResult<T>> sourceAsync, ControllerBase controllerBase) => ToCreatedResultAsync<T>(sourceAsync, controllerBase);
}