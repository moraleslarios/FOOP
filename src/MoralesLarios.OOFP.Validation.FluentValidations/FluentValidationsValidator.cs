// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Validation.FluentValidations.Helpers;

namespace MoralesLarios.OOFP.Validation.FluentValidations;

public static class FluentValidationsValidator
{
    public static MlResult<T> Validate<T, TValidator>(T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => EnsureFp.NotNull(source, $"{nameof(source)} no be null")
                   .Bind(_ => source.ValidateWithFluentValidations<T, TValidator>());

    public static Task<MlResult<T>> ValidateAsync<T, TValidator>(T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => Validate<T, TValidator>(source).ToAsync();

    public static async Task<MlResult<T>> ValidateAsync<T, TValidator>(Task<T> sourceAsync)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => await ValidateAsync<T, TValidator>(await sourceAsync);

    public static MlResult<IEnumerable<T>> Validate<T, TValidator>(IEnumerable<T> source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => EnsureFp.NotNull(source, $"{nameof(source)} no be null")
                   .Bind(_ => EnsureFp.NotEmpty(source, $"{nameof(source)} no be empty"))
                   .Bind(_ => source.ValidateWithFluentValidations<T, TValidator>());

    public static Task<MlResult<IEnumerable<T>>> ValidateAsync<T, TValidator>(IEnumerable<T> source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => Validate<T, TValidator>(source).ToAsync();

    public static async Task<MlResult<IEnumerable<T>>> ValidateAsync<T, TValidator>(Task<IEnumerable<T>> sourceAsync)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => await ValidateAsync<T, TValidator>(await sourceAsync);
}