// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;


namespace MoralesLarios.OOFP.Validation.DataAnnotations.Helpers;
public static partial class Extensions
{

    public static IEnumerable<ValidationResult> ValidateDataAnnotations(this object source)
    {
        ValidationContext valContext = new ValidationContext(source, null, null);
        var result     = new List<ValidationResult>();
        Validator.TryValidateObject(source, valContext, result, true);
        
        return result;
    } 


    public static MlResult<T> ValidateWithDataAnnotations<T>(this T source)
    {
        var result = source!.ValidateDataAnnotations().ToMlResultValid()
                                .Map ( valResults => valResults.Select(x => x.ErrorMessage))
                                .Bind( errors     => errors.Any() ? errors!.ToMlResultFail<T>() : source.ToMlResultValid<T>());
        return result;

    }

    public static Task<MlResult<T>> ValidateWithDataAnnotationsAsync<T>(this T source)
        => source!.ValidateWithDataAnnotations().ToAsync();

    public static async Task<MlResult<T>> ValidateWithDataAnnotationsAsync<T>(this Task<T> sourceAsync)
        => await (await sourceAsync).ValidateWithDataAnnotationsAsync();


    public static MlResult<IEnumerable<T>> ValidateWithDataAnnotations<T>(this IEnumerable<T> source)
    {
        var result = source.Select(x => x.ValidateWithDataAnnotations())
                            .MergeErrorsIfExists();
        return result;
    }

    public static Task<MlResult<IEnumerable<T>>> ValidateWithDataAnnotationsAsync<T>(this IEnumerable<T> source)
        => source.ValidateWithDataAnnotations().ToAsync();

    public static async Task<MlResult<IEnumerable<T>>> ValidateWithDataAnnotationsAsync<T>(this Task<IEnumerable<T>> sourceAsync)
        => await (await sourceAsync).ValidateWithDataAnnotationsAsync();




}
