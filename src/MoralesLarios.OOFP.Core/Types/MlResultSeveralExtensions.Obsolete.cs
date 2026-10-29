// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using MoralesLarios.OOFP.Types.Errors;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Sobrecargas obsoletas de <see cref="MlResultSeveralExtensions"/>, conservadas como alias de compatibilidad con la versión 2.0.
/// </summary>
public static partial class MlResultSeveralExtensions
{
    /// <summary>
    /// Ejecuta <c>EmptyToFailed</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfEmpty. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T> items, MlError error) => FailIfEmpty<T>(items, error);
    /// <summary>
    /// Ejecuta <c>EmptyToFailed</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfEmpty. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T> items, MlErrorDetails errorsDetails) => FailIfEmpty<T>(items, errorsDetails);
    /// <summary>
    /// Ejecuta <c>EmptyToFailed</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="messageError">Valor de <c>messageError</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfEmpty. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T> items, string messageError) => FailIfEmpty<T>(items, messageError);
    /// <summary>
    /// Ejecuta <c>EmptyToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to FailIfEmptyAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this IEnumerable<T> items, MlError error) => FailIfEmptyAsync<T>(items, error);
    /// <summary>
    /// Ejecuta <c>EmptyToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to FailIfEmptyAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this IEnumerable<T> items, MlErrorDetails errorsDetails) => FailIfEmptyAsync<T>(items, errorsDetails);
    /// <summary>
    /// Ejecuta <c>EmptyToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="messageError">Valor de <c>messageError</c>.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to FailIfEmptyAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this IEnumerable<T> items, string messageError) => FailIfEmptyAsync<T>(items, messageError);
    /// <summary>
    /// Ejecuta <c>EmptyToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="itemsAsync">Elementos sobre los que se opera.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to FailIfEmptyAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this Task<IEnumerable<T>> itemsAsync, MlError error) => FailIfEmptyAsync<T>(itemsAsync, error);
    /// <summary>
    /// Ejecuta <c>EmptyToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="itemsAsync">Elementos sobre los que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to FailIfEmptyAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this Task<IEnumerable<T>> itemsAsync, MlErrorDetails errorsDetails) => FailIfEmptyAsync<T>(itemsAsync, errorsDetails);
    /// <summary>
    /// Ejecuta <c>EmptyToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="itemsAsync">Elementos sobre los que se opera.</param>
    /// <param name="messageError">Valor de <c>messageError</c>.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to FailIfEmptyAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this Task<IEnumerable<T>> itemsAsync, string messageError) => FailIfEmptyAsync<T>(itemsAsync, messageError);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNull. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> NullToFailed<T>(this T source, MlErrorDetails errorsDetails) => FailIfNull<T>(source, errorsDetails);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNull. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> NullToFailed<T>(this T source, MlError error) => FailIfNull<T>(source, error);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNull. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> NullToFailed<T>(this T source, string errorMessage) => FailIfNull<T>(source, errorMessage);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNull. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> NullToFailed<T>(this T source, IEnumerable<string> errorsMessage) => FailIfNull<T>(source, errorsMessage);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNullAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> NullToFailedAsync<T>(this T source, MlError error) => FailIfNullAsync<T>(source, error);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNullAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> NullToFailedAsync<T>(this T source, MlErrorDetails errorsDetails) => FailIfNullAsync<T>(source, errorsDetails);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNullAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> NullToFailedAsync<T>(this T source, string errorMessage) => FailIfNullAsync<T>(source, errorMessage);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNullAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> NullToFailedAsync<T>(this T source, IEnumerable<string> errorsMessage) => FailIfNullAsync<T>(source, errorsMessage);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNullAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> NullToFailedAsync<T>(this Task<T> sourceAsync, string errorMessage) => FailIfNullAsync<T>(sourceAsync, errorMessage);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNullAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> NullToFailedAsync<T>(this Task<T> sourceAsync, MlError error) => FailIfNullAsync<T>(sourceAsync, error);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNullAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> NullToFailedAsync<T>(this Task<T> sourceAsync, MlErrorDetails errorsDetails) => FailIfNullAsync<T>(sourceAsync, errorsDetails);
    /// <summary>
    /// Ejecuta <c>NullToFailed</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfNullAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> NullToFailedAsync<T>(this Task<T> sourceAsync, IEnumerable<string> errorsMessage) => FailIfNullAsync<T>(sourceAsync, errorsMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalse. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> BoolToResult<T>(this T source, bool condition, MlErrorDetails errorsDetails) => FailIfFalse<T>(source, condition, errorsDetails);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalse. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> BoolToResult<T>(this T source, bool condition, MlError error) => FailIfFalse<T>(source, condition, error);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalse. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> BoolToResult<T>(this T source, bool condition, string errorMessage) => FailIfFalse<T>(source, condition, errorMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalse. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> BoolToResult<T>(this T source, bool condition, IEnumerable<string> errorsMessage) => FailIfFalse<T>(source, condition, errorsMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalse. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<bool> BoolToResult(this bool source, MlErrorDetails errorsDetails) => FailIfFalse(source, errorsDetails);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalse. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<bool> BoolToResult(this bool source, MlError error) => FailIfFalse(source, error);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalse. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<bool> BoolToResult(this bool source, string errorMessage) => FailIfFalse(source, errorMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalse. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<bool> BoolToResult(this bool source, IEnumerable<string> errorsMessage) => FailIfFalse(source, errorsMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> BoolToResultAsync<T>(this T source, bool condition, MlError error) => FailIfFalseAsync<T>(source, condition, error);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> BoolToResultAsync<T>(this T source, bool condition, MlErrorDetails errorsDetails) => FailIfFalseAsync<T>(source, condition, errorsDetails);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> BoolToResultAsync<T>(this T source, bool condition, string errorMessage) => FailIfFalseAsync<T>(source, condition, errorMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> BoolToResultAsync<T>(this T source, bool condition, IEnumerable<string> errorsMessage) => FailIfFalseAsync<T>(source, condition, errorsMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> BoolToResultAsync<T>(this Task<T> sourceAsync, bool condition, string errorMessage) => FailIfFalseAsync<T>(sourceAsync, condition, errorMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> BoolToResultAsync<T>(this Task<T> sourceAsync, bool condition, MlError error) => FailIfFalseAsync<T>(sourceAsync, condition, error);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> BoolToResultAsync<T>(this Task<T> sourceAsync, bool condition, MlErrorDetails errorsDetails) => FailIfFalseAsync<T>(sourceAsync, condition, errorsDetails);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> BoolToResultAsync<T>(this Task<T> sourceAsync, bool condition, IEnumerable<string> errorsMessage) => FailIfFalseAsync<T>(sourceAsync, condition, errorsMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<bool>> BoolToResultAsync(this bool source, MlErrorDetails errorsDetails) => FailIfFalseAsync(source, errorsDetails);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<bool>> BoolToResultAsync(this bool source, MlError error) => FailIfFalseAsync(source, error);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<bool>> BoolToResultAsync(this bool source, string errorMessage) => FailIfFalseAsync(source, errorMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<bool>> BoolToResultAsync(this bool source, IEnumerable<string> errorsMessage) => FailIfFalseAsync(source, errorsMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<bool>> BoolToResultAsync(this Task<bool> sourceAsync, MlErrorDetails errorsDetails) => FailIfFalseAsync(sourceAsync, errorsDetails);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<bool>> BoolToResultAsync(this Task<bool> sourceAsync, MlError error) => FailIfFalseAsync(sourceAsync, error);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<bool>> BoolToResultAsync(this Task<bool> sourceAsync, string errorMessage) => FailIfFalseAsync(sourceAsync, errorMessage);
    /// <summary>
    /// Ejecuta <c>BoolToResult</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FailIfFalseAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<bool>> BoolToResultAsync(this Task<bool> sourceAsync, IEnumerable<string> errorsMessage) => FailIfFalseAsync(sourceAsync, errorsMessage);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="otherValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1 value1, TResult2 value2)> Combine<TResult1, TResult2>(this MlResult<TResult1> source, TResult2 otherValue) => CombineAll<TResult1, TResult2>(source, otherValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3)> Combine<TResult1, TResult2, TResult3>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2) values) => CombineAll<TResult1, TResult2, TResult3>(source, values);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3, TResult4)> Combine<TResult1, TResult2, TResult3, TResult4>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2, TResult4 value3) values) => CombineAll<TResult1, TResult2, TResult3, TResult4>(source, values);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5>(source, values);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(source, values);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(source, values);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(source, values);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1 value1, TResult2 value2)> Combine<TResult1, TResult2>(this TResult1 source, MlResult<TResult2> mlResultValue) => CombineAll<TResult1, TResult2>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)> Combine<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source, MlResult<TResult3> mlResultValue) => CombineAll<TResult1, TResult2, TResult3>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)> Combine<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source, MlResult<TResult4> mlResultValue) => CombineAll<TResult1, TResult2, TResult3, TResult4>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source, MlResult<TResult5> mlResultValue) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source, MlResult<TResult6> mlResultValue) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source, MlResult<TResult7> mlResultValue) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source, MlResult<TResult8> mlResultValue) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2)> Combine<TResult1, TResult2>(this TResult1 source, TResult2 value) => CombineAll<TResult1, TResult2>(source, value);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3)> Combine<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source, TResult3 newValue) => CombineAll<TResult1, TResult2, TResult3>(source, newValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3, TResult4)> Combine<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source, TResult4 newValue) => CombineAll<TResult1, TResult2, TResult3, TResult4>(source, newValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source, TResult5 newValue) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5>(source, newValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source, TResult6 newValue) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(source, newValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source, TResult7 newValue) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(source, newValue);
    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAll. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source, TResult8 newValue) => CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(source, newValue);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="otherValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAsync<TResult1, TResult2>(this MlResult<TResult1> source, TResult2 otherValue) => CombineAllAsync<TResult1, TResult2>(source, otherValue);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="otherValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAsync<TResult1, TResult2>(this Task<MlResult<TResult1>> sourceAsync, TResult2 otherValue) => CombineAllAsync<TResult1, TResult2>(sourceAsync, otherValue);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3)>> CombineAsync<TResult1, TResult2, TResult3>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2) values) => CombineAllAsync<TResult1, TResult2, TResult3>(source, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3)>> CombineAsync<TResult1, TResult2, TResult3>(this Task<MlResult<TResult1>> sourceAsync, (TResult2 value1, TResult3 value2) values) => CombineAllAsync<TResult1, TResult2, TResult3>(sourceAsync, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4)>> CombineAsync<TResult1, TResult2, TResult3, TResult4>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2, TResult4 value3) values) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(source, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4)>> CombineAsync<TResult1, TResult2, TResult3, TResult4>(this Task<MlResult<TResult1>> sourceAsync, (TResult2 value1, TResult3 value2, TResult4 value3) values) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(sourceAsync, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(source, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this Task<MlResult<TResult1>> sourceAsync, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(sourceAsync, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(source, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this Task<MlResult<TResult1>> sourceAsync, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(sourceAsync, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(source, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this Task<MlResult<TResult1>> sourceAsync, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(sourceAsync, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this MlResult<TResult1> source, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(source, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this Task<MlResult<TResult1>> sourceAsync, (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(sourceAsync, values);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAsync<TResult1, TResult2>(this TResult1 source, MlResult<TResult2> mlResultValue) => CombineAllAsync<TResult1, TResult2>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAsync<TResult1, TResult2>(this TResult1 source, Task<MlResult<TResult2>> mlResultValueAsync) => CombineAllAsync<TResult1, TResult2>(source, mlResultValueAsync);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)>> CombineAsync<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source, MlResult<TResult3> mlResultValue) => CombineAllAsync<TResult1, TResult2, TResult3>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)>> CombineAsync<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source, Task<MlResult<TResult3>> mlResultValueAsync) => CombineAllAsync<TResult1, TResult2, TResult3>(source, mlResultValueAsync);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)>> CombineAsync<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source, MlResult<TResult4> mlResultValue) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)>> CombineAsync<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source, Task<MlResult<TResult4>> mlResultValueAsync) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(source, mlResultValueAsync);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source, MlResult<TResult5> mlResultValue) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source, Task<MlResult<TResult5>> mlResultValueAsync) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(source, mlResultValueAsync);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source, MlResult<TResult6> mlResultValue) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source, Task<MlResult<TResult6>> mlResultValueAsync) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(source, mlResultValueAsync);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source, MlResult<TResult7> mlResultValue) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source, Task<MlResult<TResult7>> mlResultValueAsync) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(source, mlResultValueAsync);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source, MlResult<TResult8> mlResultValue) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(source, mlResultValue);
    /// <summary>
    /// Combina los resultados. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to CombineAllAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source, Task<MlResult<TResult8>> mlResultValueAsync) => CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(source, mlResultValueAsync);
    /// <summary>
    /// Ejecuta la acción indicada.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to Pipe. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<TResult> Do<T, TResult>(this MlResult<T> source, Func<MlResult<T>, MlResult<TResult>> action) => Pipe<T, TResult>(source, action);
    /// <summary>
    /// Ejecuta la acción indicada. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="actionAsync">Acción asíncrona que se ejecuta como efecto lateral.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to PipeAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> DoAsync<T, TResult>(this MlResult<T> source, Func<MlResult<T>, Task<MlResult<TResult>>> actionAsync) => PipeAsync<T, TResult>(source, actionAsync);
    /// <summary>
    /// Ejecuta la acción indicada. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="actionAsync">Acción asíncrona que se ejecuta como efecto lateral.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to PipeAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<TResult>> DoAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, Func<Task<MlResult<T>>, Task<MlResult<TResult>>> actionAsync) => PipeAsync<T, TResult>(sourceAsync, actionAsync);
}