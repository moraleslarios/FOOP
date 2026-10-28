// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Métodos de extension de <c>MlResult</c>.
/// </summary>
public static partial class MlResultExtensions
{
    /// <summary>
    /// Ejecuta <c>SecureValidValue</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="exceptionMessage">Mensaje descriptivo.</param>
    /// <returns>Valor de tipo <c>T</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to GetValueOrThrow. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static T SecureValidValue<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the secure value from MlResult in Fail state") => GetValueOrThrow<T>(source, exceptionMessage);
    /// <summary>
    /// Ejecuta <c>SecureValidValue</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="exceptionMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el valor de tipo <c>T</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to GetValueOrThrowAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<T> SecureValidValueAsync<T>(this Task<MlResult<T>> sourceAsync, string exceptionMessage = "Cannot obtain the secure value from MlResult in Fail state") => GetValueOrThrowAsync<T>(sourceAsync, exceptionMessage);
    /// <summary>
    /// Ejecuta <c>SecureFailErrorsDetails</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="exceptionMessage">Mensaje descriptivo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to GetFailureDetailsOrThrow. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlErrorDetails SecureFailErrorsDetails<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the MlErrorDetails from MlResult in Valid state") => GetFailureDetailsOrThrow<T>(source, exceptionMessage);
    /// <summary>
    /// Ejecuta <c>SecureFailErrorsDetails</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="exceptionMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to GetFailureDetailsOrThrowAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlErrorDetails> SecureFailErrorsDetailsAsync<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the MlErrorDetails from MlResult in Valid state") => GetFailureDetailsOrThrowAsync<T>(source, exceptionMessage);
    /// <summary>
    /// Ejecuta <c>SecureFailErrorsDetails</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="exceptionMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to GetFailureDetailsOrThrowAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlErrorDetails> SecureFailErrorsDetailsAsync<T>(this Task<MlResult<T>> sourceAsync, string exceptionMessage = "Cannot obtain the MlErrorDetails from MlResult in Valid state") => GetFailureDetailsOrThrowAsync<T>(sourceAsync, exceptionMessage);
}