// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Validation.FluentValidation.Helpers;

namespace MoralesLarios.OOFP.Validation.FluentValidation;

/// <summary>
/// Representa <c>FluentValidationValidator</c>.
/// </summary>
public static class FluentValidationValidator
{
    /// <summary>
    /// Válida el valor.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Validate<T, TValidator>(T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => EnsureFp.NotNull(source, $"{nameof(source)} must not be null")
                   .Bind(_ => source.ValidateWithFluentValidation<T, TValidator>());

    /// <summary>
    /// Válida el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ValidateAsync<T, TValidator>(T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => Validate<T, TValidator>(source).ToAsync();

    /// <summary>
    /// Válida el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ValidateAsync<T, TValidator>(Task<T> sourceAsync)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => await ValidateAsync<T, TValidator>(await sourceAsync);

    /// <summary>
    /// Válida el valor.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>> Validate<T, TValidator>(IEnumerable<T> source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => EnsureFp.NotNull(source, $"{nameof(source)} must not be null")
                   .Bind(_ => EnsureFp.NotEmpty(source, $"{nameof(source)} must not be empty"))
                   .Bind(_ => source.ValidateWithFluentValidation<T, TValidator>());

    /// <summary>
    /// Válida el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<IEnumerable<T>>> ValidateAsync<T, TValidator>(IEnumerable<T> source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => Validate<T, TValidator>(source).ToAsync();

    /// <summary>
    /// Válida el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValidator">Tipo genérico TValidator.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<T>>> ValidateAsync<T, TValidator>(Task<IEnumerable<T>> sourceAsync)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => await ValidateAsync<T, TValidator>(await sourceAsync);
}