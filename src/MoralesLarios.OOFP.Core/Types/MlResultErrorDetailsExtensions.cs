// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Types;

public static partial class MlResultErrorDetailsExtensions
{

    public static MlResult<T> GetDetail<T>(this MlErrorDetails source, string key)
    {
        if ( ! source.Metadata.ContainsKey(key)) return source.AddError($"The key {key} does not exist in the details");
        
        var result = source.Metadata[key] is T value 
                            ? MlResult<T>.Valid(value) 
                            : source.AddError($"The key {key} does not contain a value of type {typeof(T).Name}");

        return result;
    }

    public static async Task<MlResult<T>> GetDetailAsync<T>(this Task<MlErrorDetails> sourceAsync, string key)
        => (await sourceAsync).GetDetail<T>(key);



    public static MlResult<T> MergeErrorsDetailsIfFail<T>(this MlResult<T> source,
                                                               MlResult<T> secondary)
        => source.Match(
                            fail : errorDetails => secondary.Match(
                                                                        fail : errorDetails2 => errorDetails.Merge(errorDetails2),
                                                                        valid: _             => errorDetails
                                                                    ),
                            valid: _ => source
                        );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this MlResult<T> source,
                                                                                MlResult<T> secondary)
        => await source.MatchAsync(
                                    failAsync : errorDetails => secondary.MatchAsync(
                                                                                failAsync : errorDetails2 => errorDetails.Merge(errorDetails2).ToMlResultFailAsync<T>(),
                                                                                validAsync: _             => errorDetails.ToMlResultFailAsync<T>()
                                                                            ),
                                    validAsync: _ => source.ToAsync()
                                );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                MlResult<T>       secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFail(secondary);

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                Task<MlResult<T>> secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFail(await secondary);

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this MlResult<T>       source,
                                                                                Task<MlResult<T>> secondaryAsync)
        => source.MergeErrorsDetailsIfFail(await secondaryAsync);

    public static MlResult<T> MergeErrorsDetailsIfFailDifferentTypes<T,T2>(this MlResult<T > source,
                                                                               MlResult<T2> secondary)
        => source.Match(
                                    fail : errorDetails => secondary.Match(
                                                                                fail : errorDetails2 => errorDetails.Merge(errorDetails2),
                                                                                valid: _             => errorDetails
                                                                            ),
                                    valid: _ => source
                                );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDifferentTypesAsync<T,T2>(this MlResult<T > source,
                                                                                                MlResult<T2> secondary)
        => await source.MatchAsync(
                                    failAsync : errorDetails => secondary.MatchAsync(
                                                                                        failAsync : errorDetails2 => errorDetails.Merge(errorDetails2).ToMlResultFailAsync<T>(),
                                                                                        validAsync: _             => errorDetails.ToMlResultFailAsync<T>()
                                                                            ),
                                    validAsync: _ => source.ToAsync()
                                );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDifferentTypesAsync<T,T2>(this Task<MlResult<T>> sourceAsync,
                                                                                                MlResult<T2>      secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFailDifferentTypes(secondary);

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDifferentTypesAsync<T,T2>(this Task<MlResult<T>>  sourceAsync,
                                                                                                Task<MlResult<T2>> secondaryAsync)
        => (await sourceAsync).MergeErrorsDetailsIfFailDifferentTypes(await secondaryAsync);

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDifferentTypesAsync<T,T2>(this MlResult<T>       source,
                                                                                                 Task<MlResult<T2>> secondaryAsync)
        => source.MergeErrorsDetailsIfFailDifferentTypes(await secondaryAsync);


    public static MlResult<T> GetMetadataValue<T>(this MlErrorDetails source)
    {
        if ( ! source.Metadata.ContainsKey(ValueMetadataKey)) return source.AddError($"The key {ValueMetadataKey} does not exist in the details");  //MlResult<T>.Fail($"The key {ValueMetadataKey} does not exist in the details");
        
        var result = source.Metadata[ValueMetadataKey] is T value 
                            ? MlResult<T>.Valid(value) 
                            : source.AddError($"The key {ValueMetadataKey} does not contain a value of type {typeof(T).Name}");

        return result;
    }


    public static MlResult<T> AddValueIfFail<T, TValue>(this MlResult<T> source,
                                                             TValue      value)
        => source.Match(
                            fail : errorsDetails => errorsDetails.AddValueMetadata(value),
                            valid: _             => source
                        );

    public static async Task<MlResult<T>> AddValueIfFailAsync<T, TValue>(this MlResult<T> source,
                                                                              TValue      value)
        => await source.AddValueIfFail(value).ToAsync();

    public static async Task<MlResult<T>> AddValueIfFailAsync<T, TValue>(this Task<MlResult<T>> sourceAsync,
                                                                              TValue            value)
    {
        var result = await sourceAsync.MatchAsync(
                                                    failAsync : errorsDetails => errorsDetails.AddValueMetadata(value).ToMlResultFailAsync<T>(),
                                                    validAsync: _             => sourceAsync
                                                );
        return result;
    }



    public static Task<MlResult<T>> GetMetadataValueAsync<T>(this MlErrorDetails source) => source.GetMetadataValue<T>().ToAsync();

    public static async Task<MlResult<T>> GetMetadataValueAsync<T>(this Task<MlErrorDetails> sourceAsync)
        => (await sourceAsync).GetMetadataValue<T>();


    public static MlResult<T> GetException<T>(this MlErrorDetails source) where T : Exception => source.GetDetail<T>(ExceptionMetadataKey);
    public static Task<MlResult<T>> GetExceptionAsync<T>(this MlErrorDetails source) where T : Exception => source.GetDetail<T>(ExceptionMetadataKey).ToAsync();
    public static async Task<MlResult<T>> GetExceptionAsync<T>(this Task<MlErrorDetails> sourceAsync) where T : Exception => (await sourceAsync).GetException<T>();
    public static MlResult<Exception> GetException(this MlErrorDetails source) => source.GetDetail<Exception>(ExceptionMetadataKey);
    public static Task<MlResult<Exception>> GetExceptionAsync(this MlErrorDetails source) => source.GetDetail<Exception>(ExceptionMetadataKey).ToAsync();
    public static async Task<MlResult<Exception>> GetExceptionAsync(this Task<MlErrorDetails> sourceAsync) => (await sourceAsync).GetException();
}

