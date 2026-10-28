// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Validation.FluentValidation.Helpers;
/// <summary>
/// Representa <c>Extensions</c>.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Válida el valor de <c>WithFluentValidations</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ValidateWithFluentValidation. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> ValidateWithFluentValidations<T, TValidator>(this T source)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidation<T, TValidator>(source);
    /// <summary>
    /// Válida el valor de <c>WithFluentValidations</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ValidateWithFluentValidation. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<T>> ValidateWithFluentValidations<T, TValidator>(this IEnumerable<T> source)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidation<T, TValidator>(source);
    /// <summary>
    /// Válida el valor de <c>WithFluentValidations</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ValidateWithFluentValidationAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> ValidateWithFluentValidationsAsync<T, TValidator>(this T source)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidationAsync<T, TValidator>(source);
    /// <summary>
    /// Válida el valor de <c>WithFluentValidations</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ValidateWithFluentValidationAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> ValidateWithFluentValidationsAsync<T, TValidator>(this Task<T> sourceAsync)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidationAsync<T, TValidator>(sourceAsync);
    /// <summary>
    /// Válida el valor de <c>WithFluentValidations</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ValidateWithFluentValidationAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>> ValidateWithFluentValidationsAsync<T, TValidator>(this IEnumerable<T> source)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidationAsync<T, TValidator>(source);
    /// <summary>
    /// Válida el valor de <c>WithFluentValidations</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ValidateWithFluentValidationAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>> ValidateWithFluentValidationsAsync<T, TValidator>(this Task<IEnumerable<T>> sourceAsync)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidationAsync<T, TValidator>(sourceAsync);
}