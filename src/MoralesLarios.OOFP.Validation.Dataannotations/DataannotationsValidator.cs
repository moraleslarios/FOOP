// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Validation.Dataannotations;

public static class DataannotationsValidator
{

    public static MlResult<T> Validate<T>(T source) => ValidateSource(source);


    public static Task<MlResult<T>> ValidateAsync<T>(T source) => ValidateSource(source).ToAsync();

    public static async Task<MlResult<T>> ValidateAsync<T>(Task<T> sourceAsync)
    {
        var source = await sourceAsync;
        return await ValidateSource(source).ToAsync();
    }


    public static MlResult<IEnumerable<T>> Validate<T>(IEnumerable<T> source) => ValidateSource(source);

    public static Task<MlResult<IEnumerable<T>>> ValidateAsync<T>(IEnumerable<T> source) => ValidateSource(source).ToAsync();

    public static async Task<MlResult<IEnumerable<T>>> ValidateAsync<T>(Task<IEnumerable<T>> sourceAsync)
    {
        var source = await sourceAsync;
        return await ValidateSource(source).ToAsync();
    }


    private static MlResult<T> ValidateSource<T>(T source)
    {
        var result = EnsureFp.NotNull(source, $"{nameof(source)} no be null")
                                .Bind(_ => source.ValidateWithDataannotations());

        return result;
    }

    private static MlResult<IEnumerable<T>> ValidateSource<T>(IEnumerable<T> source)
    {
        var result = EnsureFp.NotNull(source, $"{nameof(source)} no be null")
                                .Bind(_ => EnsureFp.NotEmpty(source, $"{nameof(source)} no be empty"))
                                .Bind(_ => source.ValidateWithDataannotations());
        return result;
    }
}
