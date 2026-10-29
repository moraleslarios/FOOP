// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Types;

/// <summary>
/// Métodos de extensión para consultar, enriquecer y combinar los detalles de error (<see cref="MlErrorDetails"/>) asociados a un <see cref="MlResult{T}"/>.
/// </summary>
public static partial class MlResultErrorDetailsExtensions
{

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Detail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="key">Clave identificativa.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> GetDetail<T>(this MlErrorDetails source, string key)
    {
        if ( ! source.Metadata.ContainsKey(key)) return source.AddError($"The key {key} does not exist in the details");
        
        var result = source.Metadata[key] is T value 
                            ? MlResult<T>.Valid(value) 
                            : source.AddError($"The key {key} does not contain a value of type {typeof(T).Name}");

        return result;
    }

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Detail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="key">Clave identificativa.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> GetDetailAsync<T>(this Task<MlErrorDetails> sourceAsync, string key)
        => (await sourceAsync).GetDetail<T>(key);



    /// <summary>
    /// Combina los resultados de <c>ErrorsDetails</c> unicamente cuando el resultado de origen es fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> MergeErrorsDetailsIfFail<T>(this MlResult<T> source,
                                                               MlResult<T> secondary)
        => source.Match(
                            fail : errorDetails => secondary.Match(
                                                                        fail : errorDetails2 => errorDetails.Merge(errorDetails2),
                                                                        valid: _             => errorDetails
                                                                    ),
                            valid: _ => source
                        );

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetails</c> unicamente cuando el resultado de origen es fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this MlResult<T> source,
                                                                                MlResult<T> secondary)
        => await source.MatchAsync(
                                    failAsync : errorDetails => secondary.MatchAsync(
                                                                                failAsync : errorDetails2 => errorDetails.Merge(errorDetails2).ToMlResultFailAsync<T>(),
                                                                                validAsync: _             => errorDetails.ToMlResultFailAsync<T>()
                                                                            ),
                                    validAsync: _ => source.ToAsync()
                                );

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetails</c> unicamente cuando el resultado de origen es fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                MlResult<T>       secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFail(secondary);

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetails</c> unicamente cuando el resultado de origen es fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="secondary">Valor de <c>secondary</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                Task<MlResult<T>> secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFail(await secondary);

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetails</c> unicamente cuando el resultado de origen es fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondaryAsync">Valor de <c>secondaryAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this MlResult<T>       source,
                                                                                Task<MlResult<T>> secondaryAsync)
        => source.MergeErrorsDetailsIfFail(await secondaryAsync);

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetailsIfFailDifferentTypes</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> MergeErrorsDetailsIfFailDifferentTypes<T,T2>(this MlResult<T > source,
                                                                               MlResult<T2> secondary)
        => source.Match(
                                    fail : errorDetails => secondary.Match(
                                                                                fail : errorDetails2 => errorDetails.Merge(errorDetails2),
                                                                                valid: _             => errorDetails
                                                                            ),
                                    valid: _ => source
                                );

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetailsIfFailDifferentTypes</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDifferentTypesAsync<T,T2>(this MlResult<T > source,
                                                                                                MlResult<T2> secondary)
        => await source.MatchAsync(
                                    failAsync : errorDetails => secondary.MatchAsync(
                                                                                        failAsync : errorDetails2 => errorDetails.Merge(errorDetails2).ToMlResultFailAsync<T>(),
                                                                                        validAsync: _             => errorDetails.ToMlResultFailAsync<T>()
                                                                            ),
                                    validAsync: _ => source.ToAsync()
                                );

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetailsIfFailDifferentTypes</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDifferentTypesAsync<T,T2>(this Task<MlResult<T>> sourceAsync,
                                                                                                MlResult<T2>      secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFailDifferentTypes(secondary);

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetailsIfFailDifferentTypes</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="secondaryAsync">Valor de <c>secondaryAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDifferentTypesAsync<T,T2>(this Task<MlResult<T>>  sourceAsync,
                                                                                                Task<MlResult<T2>> secondaryAsync)
        => (await sourceAsync).MergeErrorsDetailsIfFailDifferentTypes(await secondaryAsync);

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetailsIfFailDifferentTypes</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondaryAsync">Valor de <c>secondaryAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDifferentTypesAsync<T,T2>(this MlResult<T>       source,
                                                                                                 Task<MlResult<T2>> secondaryAsync)
        => source.MergeErrorsDetailsIfFailDifferentTypes(await secondaryAsync);


    /// <summary>
    /// Obtiene el elemento solicitado de <c>MetadataValue</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> GetMetadataValue<T>(this MlErrorDetails source)
    {
        if ( ! source.Metadata.ContainsKey(Constants.ValueMetadataKey)) return source.AddError($"The key {Constants.ValueMetadataKey} does not exist in the details");  //MlResult<T>.Fail($"The key {Constants.ValueMetadataKey} does not exist in the details");
        
        var result = source.Metadata[Constants.ValueMetadataKey] is T value 
                            ? MlResult<T>.Valid(value) 
                            : source.AddError($"The key {Constants.ValueMetadataKey} does not contain a value of type {typeof(T).Name}");

        return result;
    }


    /// <summary>
    /// Registra el valor indicado en los metadatos del error cuando el resultado de origen es fallido; si es válido, lo devuelve sin cambios.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> AddValueIfFail<T, TValue>(this MlResult<T> source,
                                                             TValue      value)
        => source.Match(
                            fail : errorsDetails => errorsDetails.AddValueMetadata(value),
                            valid: _             => source
                        );

    /// <summary>
    /// Registra el valor indicado en los metadatos del error cuando el resultado de origen es fallido; si es válido, lo devuelve sin cambios. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> AddValueIfFailAsync<T, TValue>(this MlResult<T> source,
                                                                              TValue      value)
        => await source.AddValueIfFail(value).ToAsync();

    /// <summary>
    /// Registra el valor indicado en los metadatos del error cuando el resultado de origen es fallido; si es válido, lo devuelve sin cambios. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> AddValueIfFailAsync<T, TValue>(this Task<MlResult<T>> sourceAsync,
                                                                              TValue            value)
    {
        var result = await sourceAsync.MatchAsync(
                                                    failAsync : errorsDetails => errorsDetails.AddValueMetadata(value).ToMlResultFailAsync<T>(),
                                                    validAsync: _             => sourceAsync
                                                );
        return result;
    }



    /// <summary>
    /// Obtiene el elemento solicitado de <c>MetadataValue</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> GetMetadataValueAsync<T>(this MlErrorDetails source) => source.GetMetadataValue<T>().ToAsync();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>MetadataValue</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> GetMetadataValueAsync<T>(this Task<MlErrorDetails> sourceAsync)
        => (await sourceAsync).GetMetadataValue<T>();


    /// <summary>
    /// Obtiene el elemento solicitado de <c>Exception</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> GetException<T>(this MlErrorDetails source) where T : Exception => source.GetDetail<T>(Constants.ExceptionMetadataKey);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Exception</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> GetExceptionAsync<T>(this MlErrorDetails source) where T : Exception => source.GetDetail<T>(Constants.ExceptionMetadataKey).ToAsync();
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Exception</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> GetExceptionAsync<T>(this Task<MlErrorDetails> sourceAsync) where T : Exception => (await sourceAsync).GetException<T>();
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Exception</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<Exception> GetException(this MlErrorDetails source) => source.GetDetail<Exception>(Constants.ExceptionMetadataKey);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Exception</c>. Versión asíncrona.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<Exception>> GetExceptionAsync(this MlErrorDetails source) => source.GetDetail<Exception>(Constants.ExceptionMetadataKey).ToAsync();
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Exception</c>. Versión asíncrona.
    /// </summary>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<Exception>> GetExceptionAsync(this Task<MlErrorDetails> sourceAsync) => (await sourceAsync).GetException();
}

