// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Sobrecargas obsoletas de <see cref="MlResultBindExtensions"/>, conservadas como alias de compatibilidad con la versión 2.0.
/// </summary>
public static partial class MlResultBindExtensions
{
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to BindSaveValueInDetailsIfFailedFuncResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<TResult> BindSaveValueInDetailsIfFaildFuncResult<T, TResult>(this MlResult<T> source, Func<T, MlResult<TResult>> func) => BindSaveValueInDetailsIfFailedFuncResult<T, TResult>(source, func);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to BindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this MlResult<T> source, Func<T, Task<MlResult<TResult>>> funcAsync) => BindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(source, funcAsync);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to BindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, Func<T, Task<MlResult<TResult>>> funcAsync) => BindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(sourceAsync, funcAsync);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to BindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, Func<T, MlResult<TResult>> func) => BindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(sourceAsync, func);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to BindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this MlResult<T> source, Func<T, MlResult<TResult>> func) => BindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(source, func);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TryBindSaveValueInDetailsIfFailedFuncResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<TResult> TryBindSaveValueInDetailsIfFaildFuncResult<T, TResult>(this MlResult<T> source, Func<T, MlResult<TResult>> func, Func<Exception, string> errorMessageBuilder) => TryBindSaveValueInDetailsIfFailedFuncResult<T, TResult>(source, func, errorMessageBuilder);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TryBindSaveValueInDetailsIfFailedFuncResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<TResult> TryBindSaveValueInDetailsIfFaildFuncResult<T, TResult>(this MlResult<T> source, Func<T, MlResult<TResult>> func, string errorMessage = null !) => TryBindSaveValueInDetailsIfFailedFuncResult<T, TResult>(source, func, errorMessage);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TryBindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this MlResult<T> source, Func<T, MlResult<TResult>> func, Func<Exception, string> errorMessageBuilder) => TryBindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(source, func, errorMessageBuilder);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TryBindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this MlResult<T> source, Func<T, MlResult<TResult>> func, string errorMessage = null !) => TryBindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(source, func, errorMessage);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TryBindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, Func<T, MlResult<TResult>> func, Func<Exception, string> errorMessageBuilder) => TryBindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(sourceAsync, func, errorMessageBuilder);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TryBindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, Func<T, MlResult<TResult>> func, string errorMessage = null !) => TryBindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(sourceAsync, func, errorMessage);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TryBindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, Func<T, Task<MlResult<TResult>>> funcAsync, Func<Exception, string> errorMessageBuilder) => TryBindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(sourceAsync, funcAsync, errorMessageBuilder);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TryBindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, Func<T, Task<MlResult<TResult>>> funcAsync, string errorMessage = null !) => TryBindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(sourceAsync, funcAsync, errorMessage);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TryBindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this MlResult<T> source, Func<T, Task<MlResult<TResult>>> funcAsync, Func<Exception, string> errorMessageBuilder) => TryBindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(source, funcAsync, errorMessageBuilder);
    /// <summary>
    /// Encadena una función que devuelve un resultado de <c>SaveValueInDetailsIfFaildFuncResult</c>.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TryBindSaveValueInDetailsIfFailedFuncResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TResult>(this MlResult<T> source, Func<T, Task<MlResult<TResult>>> funcAsync, string errorMessage = null !) => TryBindSaveValueInDetailsIfFailedFuncResultAsync<T, TResult>(source, funcAsync, errorMessage);
}