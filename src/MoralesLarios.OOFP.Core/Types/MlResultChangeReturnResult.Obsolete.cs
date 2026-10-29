// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Sobrecargas obsoletas de <see cref="MlResultChangeReturnResult"/>, conservadas como alias de compatibilidad con la versión 2.0.
/// </summary>
public static partial class MlResultChangeReturnResult
{
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisValid</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValidValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysValid. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<TResult> ChangeReturnResultAlwaisValid<T, TResult>(this MlResult<T> source, TResult validValue, TResult failValidValue) => ChangeReturnResultAlwaysValid<T, TResult>(source, validValue, failValidValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisValid</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValidValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysValidAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> ChangeReturnResultAlwaisValidAsync<T, TResult>(this MlResult<T> source, TResult validValue, TResult failValidValue) => ChangeReturnResultAlwaysValidAsync<T, TResult>(source, validValue, failValidValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisValid</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validValue">Valor sobre el que se opera.</param>
    /// <param name="failValidValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysValidAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> ChangeReturnResultAlwaisValidAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, TResult validValue, TResult failValidValue) => ChangeReturnResultAlwaysValidAsync<T, TResult>(sourceAsync, validValue, failValidValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFail. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<TResult> ChangeReturnResultAlwaisFail<T, TResult>(this MlResult<T> source, MlErrorDetails validFailValue, MlErrorDetails failValue) => ChangeReturnResultAlwaysFail<T, TResult>(source, validFailValue, failValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFail. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<TResult> ChangeReturnResultAlwaisFail<T, TResult>(this MlResult<T> source, MlError validFailValue, MlError failValue) => ChangeReturnResultAlwaysFail<T, TResult>(source, validFailValue, failValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFail. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<TResult> ChangeReturnResultAlwaisFail<T, TResult>(this MlResult<T> source, IEnumerable<string> validFailValue, IEnumerable<string> failValue) => ChangeReturnResultAlwaysFail<T, TResult>(source, validFailValue, failValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFail. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<TResult> ChangeReturnResultAlwaisFail<T, TResult>(this MlResult<T> source, string validFailValue, string failValue) => ChangeReturnResultAlwaysFail<T, TResult>(source, validFailValue, failValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFailAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> ChangeReturnResultAlwaisFailAsync<T, TResult>(this MlResult<T> source, MlErrorDetails validFailValue, MlErrorDetails failValue) => ChangeReturnResultAlwaysFailAsync<T, TResult>(source, validFailValue, failValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFailAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> ChangeReturnResultAlwaisFailAsync<T, TResult>(this MlResult<T> source, MlError validFailValue, MlError failValue) => ChangeReturnResultAlwaysFailAsync<T, TResult>(source, validFailValue, failValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFailAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> ChangeReturnResultAlwaisFailAsync<T, TResult>(this MlResult<T> source, IEnumerable<string> validFailValue, IEnumerable<string> failValue) => ChangeReturnResultAlwaysFailAsync<T, TResult>(source, validFailValue, failValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFailAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> ChangeReturnResultAlwaisFailAsync<T, TResult>(this MlResult<T> source, string validFailValue, string failValue) => ChangeReturnResultAlwaysFailAsync<T, TResult>(source, validFailValue, failValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFailAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> ChangeReturnResultAlwaisFailAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, MlErrorDetails validFailValue, MlErrorDetails failValue) => ChangeReturnResultAlwaysFailAsync<T, TResult>(sourceAsync, validFailValue, failValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFailAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> ChangeReturnResultAlwaisFailAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, MlError validFailValue, MlError failValue) => ChangeReturnResultAlwaysFailAsync<T, TResult>(sourceAsync, validFailValue, failValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFailAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> ChangeReturnResultAlwaisFailAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, IEnumerable<string> validFailValue, IEnumerable<string> failValue) => ChangeReturnResultAlwaysFailAsync<T, TResult>(sourceAsync, validFailValue, failValue);
    /// <summary>
    /// Ejecuta <c>ChangeReturnResultAlwaisFail</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validFailValue">Valor sobre el que se opera.</param>
    /// <param name="failValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ChangeReturnResultAlwaysFailAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> ChangeReturnResultAlwaisFailAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, string validFailValue, string failValue) => ChangeReturnResultAlwaysFailAsync<T, TResult>(sourceAsync, validFailValue, failValue);
}