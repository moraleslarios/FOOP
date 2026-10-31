// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Métodos de extension de <c>MlResultLog</c>.
/// </summary>
public static partial class MlResultLogExtensions
{
    /// <summary>
    /// Registra una traza del resultado de <c>MlResultFinal</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to LogCompletedResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> LogMlResultFinal<T>(this MlResult<T> source, ILogger logger, Func<T, string> validBuildMessage, Func<MlErrorDetails, string> failBuildMessage) => LogCompletedResult<T>(source, logger, validBuildMessage, failBuildMessage);
    /// <summary>
    /// Registra una traza del resultado de <c>MlResultFinal</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to LogCompletedResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> LogMlResultFinal<T>(this MlResult<T> source, ILogger logger, string validMessage, string failMessage) => LogCompletedResult<T>(source, logger, validMessage, failMessage);
    /// <summary>
    /// Registra una traza del resultado de <c>MlResultFinal</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to LogCompletedResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> LogMlResultFinal<T>(this Task<MlResult<T>> sourceAsync, ILogger logger, string validMessage, string failMessage) => LogCompletedResult<T>(sourceAsync, logger, validMessage, failMessage);
    /// <summary>
    /// Registra una traza del resultado de <c>MlResultFinal</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to LogCompletedResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> LogMlResultFinalAsync<T>(this MlResult<T> source, ILogger logger, Func<T, string> validBuildMessage, Func<MlErrorDetails, string> failBuildMessage) => LogCompletedResultAsync<T>(source, logger, validBuildMessage, failBuildMessage);
    /// <summary>
    /// Registra una traza del resultado de <c>MlResultFinal</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to LogCompletedResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> LogMlResultFinalAsync<T>(this Task<MlResult<T>> sourceAsync, ILogger logger, Func<T, string> validBuildMessage, Func<MlErrorDetails, string> failBuildMessage) => LogCompletedResultAsync<T>(sourceAsync, logger, validBuildMessage, failBuildMessage);
}
