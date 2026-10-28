// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;
/// <summary>
/// Métodos de extension de <c>MlResultActionResult</c>.
/// </summary>
public static partial class MlResultActionResultExtensions
{
    /// <summary>
    /// Ejecuta <c>ToPostPdActionResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="uri">Dirección a la que se dirige la llamada.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToCreatedProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToPostPdActionResult<T>(this MlResult<T> source, Uri uri) => ToCreatedProblemDetailsResult<T>(source, uri);
    /// <summary>
    /// Ejecuta <c>ToPostPdActionResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToCreatedProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToPostPdActionResult<T>(this MlResult<T> source) => ToCreatedProblemDetailsResult<T>(source);
    /// <summary>
    /// Ejecuta <c>ToPostPdActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="uri">Dirección a la que se dirige la llamada.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToCreatedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPostPdActionResultAsync<T>(this MlResult<T> source, Uri uri) => ToCreatedProblemDetailsResultAsync<T>(source, uri);
    /// <summary>
    /// Ejecuta <c>ToPostPdActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToCreatedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPostPdActionResultAsync<T>(this MlResult<T> source) => ToCreatedProblemDetailsResultAsync<T>(source);
    /// <summary>
    /// Ejecuta <c>ToGetPdActionResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToOkProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToGetPdActionResult<T>(this MlResult<T> source) => ToOkProblemDetailsResult<T>(source);
    /// <summary>
    /// Ejecuta <c>ToGetPdActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToOkProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToGetPdActionResultAsync<T>(this MlResult<T> source) => ToOkProblemDetailsResultAsync<T>(source);
    /// <summary>
    /// Ejecuta <c>ToGetPdActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToOkProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToGetPdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync) => ToOkProblemDetailsResultAsync<T>(sourceAsync);
    /// <summary>
    /// Ejecuta <c>ToPutPdActionResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToNoContentProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToPutPdActionResult<T>(this MlResult<T> source) => ToNoContentProblemDetailsResult<T>(source);
    /// <summary>
    /// Ejecuta <c>ToPutPdActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToNoContentProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPutPdActionResultAsync<T>(this MlResult<T> source) => ToNoContentProblemDetailsResultAsync<T>(source);
    /// <summary>
    /// Ejecuta <c>ToPutPdActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToNoContentProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPutPdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync) => ToNoContentProblemDetailsResultAsync<T>(sourceAsync);
    /// <summary>
    /// Ejecuta <c>ToPatchPdActionResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToPatchedProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToPatchPdActionResult<T>(this MlResult<T> source) => ToPatchedProblemDetailsResult<T>(source);
    /// <summary>
    /// Ejecuta <c>ToPatchPdActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToPatchedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPatchPdActionResultAsync<T>(this MlResult<T> source) => ToPatchedProblemDetailsResultAsync<T>(source);
    /// <summary>
    /// Ejecuta <c>ToPatchPdActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToPatchedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToPatchPdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync) => ToPatchedProblemDetailsResultAsync<T>(sourceAsync);
    /// <summary>
    /// Ejecuta <c>ToDeletePdActionResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToDeletedProblemDetailsResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IActionResult ToDeletePdActionResult<T>(this MlResult<T> source) => ToDeletedProblemDetailsResult<T>(source);
    /// <summary>
    /// Ejecuta <c>ToDeletePdActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToDeletedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToDeletePdActionResultAsync<T>(this MlResult<T> source) => ToDeletedProblemDetailsResultAsync<T>(source);
    /// <summary>
    /// Ejecuta <c>ToDeletePdActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ToDeletedProblemDetailsResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<IActionResult> ToDeletePdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync) => ToDeletedProblemDetailsResultAsync<T>(sourceAsync);
}