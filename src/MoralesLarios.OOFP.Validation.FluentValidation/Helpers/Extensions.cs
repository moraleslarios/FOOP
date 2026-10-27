// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.Validation.FluentValidation.Helpers;
public static partial class Extensions
{

    public static global::FluentValidation.Results.ValidationResult ValidateWithFluentValidationResult<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
        => new TValidator().Validate(source);

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

    public static Task<MlResult<T>> ValidateWithFluentValidationAsync<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => source.ValidateWithFluentValidation<T, TValidator>().ToAsync();

    public static async Task<MlResult<T>> ValidateWithFluentValidationAsync<T, TValidator>(this Task<T> sourceAsync)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => await (await sourceAsync).ValidateWithFluentValidationAsync<T, TValidator>();

    public static MlResult<IEnumerable<T>> ValidateWithFluentValidation<T, TValidator>(this IEnumerable<T> source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => source.Select(item => item.ValidateWithFluentValidation<T, TValidator>())
                     .MergeErrorsIfExists();

    public static Task<MlResult<IEnumerable<T>>> ValidateWithFluentValidationAsync<T, TValidator>(this IEnumerable<T> source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => source.ValidateWithFluentValidation<T, TValidator>().ToAsync();

    public static async Task<MlResult<IEnumerable<T>>> ValidateWithFluentValidationAsync<T, TValidator>(this Task<IEnumerable<T>> sourceAsync)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => await (await sourceAsync).ValidateWithFluentValidationAsync<T, TValidator>();

    [Obsolete("Use ValidateWithFluentValidations instead.")]
    public static MlResult<T> ValidateWitHFluentValidations<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => source.ValidateWithFluentValidation<T, TValidator>();

    [Obsolete("Use ValidateWithFluentValidationsAsync instead.")]
    public static Task<MlResult<T>> ValidateWitHFluentValidationsAsync<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new()
            => source.ValidateWithFluentValidationAsync<T, TValidator>();

}
