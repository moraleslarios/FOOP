// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Métodos de extension de <c>MlResultErrorDetails</c>.
/// </summary>
public static partial class MlResultErrorDetailsExtensions
{
    /// <summary>
    /// Combina los resultados de <c>ErrorsDetailsIfFailDiferentTypes</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to MergeErrorsDetailsIfFailDifferentTypes. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> MergeErrorsDetailsIfFailDiferentTypes<T, T2>(this MlResult<T> source, MlResult<T2> secondary) => MergeErrorsDetailsIfFailDifferentTypes<T, T2>(source, secondary);
    /// <summary>
    /// Combina los resultados de <c>ErrorsDetailsIfFailDiferentTypes</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to MergeErrorsDetailsIfFailDifferentTypesAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T, T2>(this MlResult<T> source, MlResult<T2> secondary) => MergeErrorsDetailsIfFailDifferentTypesAsync<T, T2>(source, secondary);
    /// <summary>
    /// Combina los resultados de <c>ErrorsDetailsIfFailDiferentTypes</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to MergeErrorsDetailsIfFailDifferentTypesAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T, T2>(this Task<MlResult<T>> sourceAsync, MlResult<T2> secondary) => MergeErrorsDetailsIfFailDifferentTypesAsync<T, T2>(sourceAsync, secondary);
    /// <summary>
    /// Combina los resultados de <c>ErrorsDetailsIfFailDiferentTypes</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="secondaryAsync">Valor de <c>secondaryAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to MergeErrorsDetailsIfFailDifferentTypesAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T, T2>(this Task<MlResult<T>> sourceAsync, Task<MlResult<T2>> secondaryAsync) => MergeErrorsDetailsIfFailDifferentTypesAsync<T, T2>(sourceAsync, secondaryAsync);
    /// <summary>
    /// Combina los resultados de <c>ErrorsDetailsIfFailDiferentTypes</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondaryAsync">Valor de <c>secondaryAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to MergeErrorsDetailsIfFailDifferentTypesAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T, T2>(this MlResult<T> source, Task<MlResult<T2>> secondaryAsync) => MergeErrorsDetailsIfFailDifferentTypesAsync<T, T2>(source, secondaryAsync);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>DetailValue</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetMetadataValue. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> GetDetailValue<T>(this MlErrorDetails source) => GetMetadataValue<T>(source);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>DetailValue</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetMetadataValueAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> GetDetailValueAsync<T>(this MlErrorDetails source) => GetMetadataValueAsync<T>(source);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>DetailValue</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetMetadataValueAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> GetDetailValueAsync<T>(this Task<MlErrorDetails> sourceAsync) => GetMetadataValueAsync<T>(sourceAsync);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>DetailException</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetException. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> GetDetailException<T>(this MlErrorDetails source)
        where T : Exception => GetException<T>(source);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>DetailException</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetException. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<Exception> GetDetailException(this MlErrorDetails source) => GetException(source);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>DetailException</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetExceptionAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> GetDetailExceptionAsync<T>(this MlErrorDetails source)
        where T : Exception => GetExceptionAsync<T>(source);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>DetailException</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetExceptionAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> GetDetailExceptionAsync<T>(this Task<MlErrorDetails> sourceAsync)
        where T : Exception => GetExceptionAsync<T>(sourceAsync);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>DetailException</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetExceptionAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<Exception>> GetDetailExceptionAsync(this MlErrorDetails source) => GetExceptionAsync(source);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>DetailException</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetExceptionAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<Exception>> GetDetailExceptionAsync(this Task<MlErrorDetails> sourceAsync) => GetExceptionAsync(sourceAsync);
}