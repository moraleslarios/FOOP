// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.Validation.FluentValidation.Helpers;
/// <summary>
/// Métodos de extension de <c></c>.
/// </summary>
public static partial class Extensions
{

    /// <summary>
    /// Válida el valor de <c>WithFluentValidationResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>global::FluentValidation.Results.ValidationResult</c> obtenido por la operación.</returns>
    public static global::FluentValidation.Results.ValidationResult ValidateWithFluentValidationResult<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => new TValidator().Validate(source);

    /// <summary>
    /// Válida el valor de <c>WithFluentValidation</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ValidateWithFluentValidation<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
    {
        var result = MlResult.Empty()
                                .TryMap( _          => Activator.CreateInstance<TValidator>(), $"Problems with automatic create instance of {typeof(TValidator).Name}")
                                .TryMap( validator  => validator.Validate(source))
                                .Map   ( valResults => valResults.Errors.Select(x => x.ErrorMessage))
                                .Bind  ( errors     => errors.Any() ? errors.ToMlResultFail<T>() : source.ToMlResultValid<T>());
        return result;
    }

    /// <summary>
    /// Válida el valor de <c>WithFluentValidation</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ValidateWithFluentValidationAsync<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => source.ValidateWithFluentValidation<T, TValidator>().ToAsync();

    /// <summary>
    /// Válida el valor de <c>WithFluentValidation</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ValidateWithFluentValidationAsync<T, TValidator>(this Task<T> sourceAsync)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => await (await sourceAsync).ValidateWithFluentValidationAsync<T, TValidator>();

    /// <summary>
    /// Válida el valor de <c>WithFluentValidation</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>> ValidateWithFluentValidation<T, TValidator>(this IEnumerable<T> source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => source.Select(item => item.ValidateWithFluentValidation<T, TValidator>())
                     .MergeErrorsIfExists();

    /// <summary>
    /// Válida el valor de <c>WithFluentValidation</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<IEnumerable<T>>> ValidateWithFluentValidationAsync<T, TValidator>(this IEnumerable<T> source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => source.ValidateWithFluentValidation<T, TValidator>().ToAsync();

    /// <summary>
    /// Válida el valor de <c>WithFluentValidation</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<T>>> ValidateWithFluentValidationAsync<T, TValidator>(this Task<IEnumerable<T>> sourceAsync)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => await (await sourceAsync).ValidateWithFluentValidationAsync<T, TValidator>();

    /// <summary>
    /// Válida el valor de <c>WitHFluentValidations</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [Obsolete("Use ValidateWithFluentValidation instead.")]
    public static MlResult<T> ValidateWitHFluentValidations<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => source.ValidateWithFluentValidation<T, TValidator>();

    /// <summary>
    /// Válida el valor de <c>WitHFluentValidations</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [Obsolete("Use ValidateWithFluentValidationAsync instead.")]
    public static Task<MlResult<T>> ValidateWitHFluentValidationsAsync<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => source.ValidateWithFluentValidationAsync<T, TValidator>();

}
