// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types;

/// <summary>
/// Métodos de extension de <c>MlResult</c>.
/// </summary>
public static partial class MlResultExtensions
{

    /// <summary>
    /// Añade el elemento indicado de <c>MlErrorDetail</c> unicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorKey">Clave identificativa.</param>
    /// <param name="errorValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> AddMlErrorDetailIfFail<T>(this MlResult<T> source, 
                                                             string      errorKey, 
                                                             object      errorValue)
        => source.Match
                    (
                        fail : errorsDetails => errorsDetails.AddMetadataOrThrow(errorKey, errorValue),
                        valid: _ => source
                    );


    /// <summary>
    /// Añade el elemento indicado de <c>ValueDetail</c> unicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> AddValueDetailIfFail<T>(this MlResult<T> source, 
                                                           object      errorValue)
        => source.AddMlErrorDetailIfFail(Constants.ValueMetadataKey, errorValue);

    /// <summary>
    /// Añade el elemento indicado de <c>MlErrorDetail</c> unicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorKey">Clave identificativa.</param>
    /// <param name="errorValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> AddMlErrorDetailIfFailAsync<T>(this MlResult<T> source, 
                                                                              string      errorKey, 
                                                                              object      errorValue)
        => source.AddMlErrorDetailIfFail(errorKey, errorValue).ToAsync();


    /// <summary>
    /// Añade el elemento indicado de <c>MlErrorDetail</c> unicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorKey">Clave identificativa.</param>
    /// <param name="errorValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> AddMlErrorDetailIfFailAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                              string            errorKey, 
                                                                              object            errorValue)
        => await (await sourceAsync).AddMlErrorDetailIfFailAsync(errorKey, errorValue);

    /// <summary>
    /// Añade el elemento indicado de <c>ValueDetail</c> unicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> AddValueDetailIfFailAsync<T>(this MlResult<T> source,
                                                                            object      errorValue)
        => source.AddValueDetailIfFail(errorValue).ToAsync();


    /// <summary>
    /// Añade el elemento indicado de <c>ValueDetail</c> unicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> AddValueDetailIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                            object            errorValue)
        => await (await sourceAsync).AddValueDetailIfFailAsync(errorValue);










    #region CompleteWithValue

    /// <summary>
    /// Returns a new MlResult with the value of the source and the value passed as a parameter.
    /// </summary>
    /// <typeparam name="T">Value type of the source result.</typeparam>
    /// <typeparam name="TResult">Value type of the returned result.</typeparam>
    /// <param name="source">Source result.</param>
    /// <param name="completeFunc">Function that builds the new value from the source value.</param>
    /// <returns>A valid result with the new value, or the source failure.</returns>
    public static MlResult<TResult> CompleteWithDataValueIfValid<T, TResult>(this MlResult<T>      source,
                                                                                  Func<T, TResult> completeFunc)
        => source.Match(
                            fail : errorDetails => errorDetails.ToMlResultFail<TResult>(),
                            valid: x            => MlResult<TResult>.Valid(completeFunc(x))
                        );



    /// <summary>
    /// Completa el resultado con información adicional de <c>WithDataValue</c> unicamente cuando el
    /// resultado de origen es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFuncAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> CompleteWithDataValueIfValidAsync<T, TResult>(this MlResult<T>            source,
                                                                                                   Func<T, Task<TResult>> completeFuncAsync)
        => await source.MatchAsync(
                                        failAsync :       errorDetails => errorDetails.ToMlResultFailAsync<TResult>(),
                                        validAsync: async x            => MlResult<TResult>.Valid(await completeFuncAsync(x))
                                    );


    /// <summary>
    /// Completa el resultado con información adicional de <c>WithDataValue</c> unicamente cuando el
    /// resultado de origen es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFuncAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> CompleteWithDataValueIfValidAsync<T, TResult>(this Task<MlResult<T>>      sourceAsync,
                                                                                                   Func<T, Task<TResult>> completeFuncAsync)
        => await (await sourceAsync).CompleteWithDataValueIfValidAsync(completeFuncAsync);

    /// <summary>
    /// Completa el resultado con información adicional de <c>WithDataValue</c> unicamente cuando el
    /// resultado de origen es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="completeFunc">Función que se ejecuta durante la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> CompleteWithDataValueIfValidAsync<T, TResult>(this MlResult<T>      source,
                                                                                              Func<T, TResult> completeFunc)
        => source.CompleteWithDataValueIfValid(completeFunc).ToAsync();

    /// <summary>
    /// Completa el resultado con información adicional de <c>WithDataValue</c> unicamente cuando el
    /// resultado de origen es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="completeFunc">Función que se ejecuta durante la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> CompleteWithDataValueIfValidAsync<T, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                                                    Func<T, TResult> completeFunc)
        => await (await sourceAsync).CompleteWithDataValueIfValidAsync(completeFunc);





    /// <summary>
    /// Populate the ErrorDetails information with a Key value entry, if the MlResult result is fail
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns>Fail MlResult with value ErrorDetailData</returns>
    public static MlResult<T> CompleteWithDetailsValueIfFail<T, TValue>(this MlResult<T> source, 
                                                                             TValue      value)
        => source.Match (
                            fail : errorDetails => errorDetails.AddValueMetadata(value),
                            valid: x            => source
                        );

    /// <summary>
    /// Completa el resultado con información adicional de <c>WithDetailsValue</c> unicamente cuando el
    /// resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> CompleteWithDetailsValueIfFailAsync<T, TValue>(this MlResult<T> source, 
                                                                                        TValue      value)
        => source.CompleteWithDetailsValueIfFail(value).ToAsync();

    /// <summary>
    /// Completa el resultado con información adicional de <c>WithDetailsValue</c> unicamente cuando el
    /// resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> CompleteWithDetailsValueIfFailAsync<T, TValue>(this Task<MlResult<T>> sourceAsync, 
                                                                                              TValue            value)
        => (await sourceAsync).CompleteWithDetailsValueIfFail(value);




    /// <summary>
    /// If the source is valid, it returns a new MlResult with the value of the source and the value passed as a parameter.
    /// If the source is fail, populate the ErrorDetails information with a Key value entry, if the MlResult result is fail
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="completeFunc"></param>
    /// <returns></returns>
    public static MlResult<TResult> CompleteWithDataValue<T, TValue, TResult>(this MlResult<T>      source, 
                                                                                   TValue           value,
                                                                                   Func<T, TResult> completeFunc)
        => source.Match(
                            fail : errorDetails => errorDetails.AddValueMetadata(value).ToMlResultFail<TResult>(),
                            valid: x            => MlResult<TResult>.Valid(completeFunc(x))
                        );


    /// <summary>
    /// Completa el resultado con información adicional de <c>WithDataValue</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="completeFuncAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> CompleteWithDataValueAsync<T, TValue, TResult>(this MlResult<T>            source, 
                                                                                                    TValue                 value,
                                                                                                    Func<T, Task<TResult>> completeFuncAsync)
        => await source.MatchAsync(
                                        failAsync :       errorDetails => errorDetails.AddValueMetadata(value).ToMlResultFailAsync<TResult>(),
                                        validAsync: async x            => MlResult<TResult>.Valid(await completeFuncAsync(x))
                                    );


    /// <summary>
    /// Completa el resultado con información adicional de <c>WithDataValue</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="completeFuncAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> CompleteWithDataValueAsync<T, TValue, TResult>(this Task<MlResult<T>>      sourceAsync, 
                                                                                                    TValue                 value,
                                                                                                    Func<T, Task<TResult>> completeFuncAsync)
        => await (await sourceAsync).CompleteWithDataValueAsync(value, completeFuncAsync);

    /// <summary>
    /// Completa el resultado con información adicional de <c>WithDataValue</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="completeFunc">Función que se ejecuta durante la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> CompleteWithDataValueAsync<T, TValue, TResult>(this MlResult<T>      source,
                                                                                               TValue           value,
                                                                                               Func<T, TResult> completeFunc)
        => source.CompleteWithDataValue(value, completeFunc).ToAsync();

    /// <summary>
    /// Completa el resultado con información adicional de <c>WithDataValue</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="completeFunc">Función que se ejecuta durante la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> CompleteWithDataValueAsync<T, TValue, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                                                     TValue           value,
                                                                                                     Func<T, TResult> completeFunc)
        => await (await sourceAsync).CompleteWithDataValueAsync(value, completeFunc);


    #endregion CompleteWithValue




    #region GetValueOrThrow



    /// <summary>
    /// Obtiene el elemento solicitado de <c>ValueOrThrow</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="exceptionMessage">Mensaje descriptivo.</param>
    /// <returns>Valor de tipo <c>T</c> obtenido por la operación.</returns>
    public static T GetValueOrThrow<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the secure value from MlResult in Fail state")
        => source.Match(valid: x => x,
                        fail : _ => throw new InvalidProgramException(exceptionMessage));

    /// <summary>
    /// Obtiene el elemento solicitado de <c>ValueOrThrow</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="exceptionMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el valor de tipo <c>T</c> obtenido por la operación.</returns>
    public static async Task<T> GetValueOrThrowAsync<T>(this Task<MlResult<T>> sourceAsync, string exceptionMessage = "Cannot obtain the secure value from MlResult in Fail state")
        => await sourceAsync.MatchAsync(valid     : x => x,
                                        failAsync : _ => throw new InvalidProgramException(exceptionMessage));

    /// <summary>
    /// Obtiene el elemento solicitado de <c>FailureDetailsOrThrow</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="exceptionMessage">Mensaje descriptivo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails GetFailureDetailsOrThrow<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the MlErrorDetails from MlResult in Valid state")
        => source.Match(valid: _ => throw new InvalidProgramException(exceptionMessage),
                        fail : errors => errors);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>FailureDetailsOrThrow</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="exceptionMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static Task<MlErrorDetails> GetFailureDetailsOrThrowAsync<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the MlErrorDetails from MlResult in Valid state")
        => source.GetFailureDetailsOrThrow(exceptionMessage).ToAsync();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>FailureDetailsOrThrow</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="exceptionMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static async Task<MlErrorDetails> GetFailureDetailsOrThrowAsync<T>(this Task<MlResult<T>> sourceAsync, string exceptionMessage = "Cannot obtain the MlErrorDetails from MlResult in Valid state")
        => await sourceAsync.GetFailureDetailsOrThrowAsync(exceptionMessage);


    #endregion





    #region CreateCompleteMlResult



    // **************************** MLResult 2 Generics

    /// <summary>
    /// Crea la instancia solicitada de <c>CompleteMlResult</c>.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source1">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="source2">Resultado sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(T1, T2)> CreateCompleteMlResult<T1, T2>(this MlResult<T1> source1,
                                                                         MlResult<T2> source2)
        => source1.Match(
                            fail : errorDetails1 => errorDetails1.MergeErrorsDetails<T2, (T1, T2)>(source2),
                            valid: x1 => source2.Match(
                                                            fail : errorDetails2 => errorDetails2.MergeErrorsDetails<T1, (T1, T2)>(source1),
                                                            valid: x2            => MlResult<(T1, T2)>.Valid((x1, x2))
                                                      )
                        );


    /// <summary>
    /// Crea la instancia solicitada de <c>CompleteMlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source1">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="source2">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this MlResult<T1> source1,
                                                                                    MlResult<T2> source2)
        => source1.CreateCompleteMlResult(source2).ToAsync();

    /// <summary>
    /// Crea la instancia solicitada de <c>CompleteMlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source1Async">Tarea que produce el resultado de origen.</param>
    /// <param name="source2">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this Task<MlResult<T1>> source1Async,
                                                                                          MlResult<T2>       source2)
    {
        var source1 = await source1Async;

        var result = await source1Async.MatchAsync(
                                                        failAsync : errorDetails1 => errorDetails1.MergeErrorsDetailsAsync<T2, (T1, T2)>(source2),
                                                        validAsync: x1            => source2.Match(
                                                                                                       fail : errorDetails2 => errorDetails2.MergeErrorsDetails<T1, (T1, T2)>(source1),
                                                                                                       valid: x2            => MlResult<(T1, T2)>.Valid((x1, x2))
                                                                                                   ).ToAsync()
                                                    );

        return result;
    }


    /// <summary>
    /// Crea la instancia solicitada de <c>CompleteMlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source1">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="source2Async">Valor de <c>source2Async</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this MlResult<T1>       source1,
                                                                                          Task<MlResult<T2>> source2Async)
    {
        var source2 = await source2Async;

        var result = source1.Match(
                                    fail : errorDetails1 => errorDetails1.MergeErrorsDetailsAsync<T2, (T1, T2)>(source2),
                                    valid: x1            => source2Async.MatchAsync(
                                                                                       failAsync : errorDetails2 => errorDetails2.MergeErrorsDetailsAsync<T1, (T1, T2)>(source1),
                                                                                       validAsync: x2 => MlResult<(T1, T2)>.ValidAsync((x1, x2))
                                                                                   )
                                );

        return await result;
    }                   

    /// <summary>
    /// Crea la instancia solicitada de <c>CompleteMlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source1Async">Tarea que produce el resultado de origen.</param>
    /// <param name="source2Async">Valor de <c>source2Async</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this Task<MlResult<T1>> source1Async,
                                                                                    Task<MlResult<T2>> source2Async)
        => source1Async.MatchAsync(
                                    failAsync : errorDetails1 => errorDetails1.MergeErrorsDetailsAsync<T2, (T1, T2)>(source2Async),
                                    validAsync: x1            => source2Async.MatchAsync(
                                                                                            failAsync : errorDetails2 => errorDetails2.MergeErrorsDetailsAsync<T1, (T1, T2)>(source1Async),
                                                                                            validAsync: x2            => MlResult<(T1, T2)>.ValidAsync((x1, x2))
                                                                                        )
                                );





    // **************************** MLResult 3 Generics



    /// <summary>
    /// Crea la instancia solicitada de <c>CompleteMlResult</c>.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <typeparam name="T3">Tipo del tercer valor.</typeparam>
    /// <param name="source1">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="source2">Resultado sobre el que se opera.</param>
    /// <param name="source3">Resultado sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(T1, T2, T3)> CreateCompleteMlResult<T1, T2, T3>(this MlResult<T1> source1,
                                                                                 MlResult<T2> source2,
                                                                                 MlResult<T3> source3)
        => source1.Match(
                            fail : errorDetails1 => errorDetails1.ToMlResultFail<(T1, T2, T3)>(),
                            valid: x1            => source2.Match(
                                                                        fail : errorDetails2 => errorDetails2.ToMlResultFail<(T1, T2, T3)>(),
                                                                        valid: x2            => source3.Match(
                                                                                                                   fail : errorDetails3 => errorDetails3.ToMlResultFail<(T1, T2, T3)>(),
                                                                                                                   valid: x3            => MlResult<(T1, T2, T3)>.Valid((x1, x2, x3))
                                                                                                               )
                                                                  )
                        );













    // ******************************* Object 2 Generics


    /// <summary>
    /// Crea la instancia solicitada de <c>CompleteMlResult</c>.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source1">Source 1.</param>
    /// <param name="source2">Resultado sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(T1, T2)> CreateCompleteMlResult<T1, T2>(this T1           source1,
                                                                         MlResult<T2> source2)
        => source2.Match(
                            fail : errorDetails2 => errorDetails2.ToMlResultFail<(T1, T2)>(),
                            valid: x2            => MlResult<(T1, T2)>.Valid((source1, x2))
                        );

    /// <summary>
    /// Crea la instancia solicitada de <c>CompleteMlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source1">Source 1.</param>
    /// <param name="source2">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this T1           source1,
                                                                                    MlResult<T2> source2)
        => source2.Match(
                            fail : errorDetails2 => errorDetails2.ToMlResultFail<(T1, T2)>(),
                            valid: x2            => MlResult<(T1, T2)>.Valid((source1, x2))
                        ).ToAsync();

    /// <summary>
    /// Crea la instancia solicitada de <c>CompleteMlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source1Async">Source 1 async.</param>
    /// <param name="source2">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this Task<T1>     source1Async,
                                                                                          MlResult<T2> source2)
    {
        var source1 = await source1Async;

        var result = source2.Match(
                                        fail : errorDetails2 => errorDetails2.ToMlResultFail<(T1, T2)>(),
                                        valid: x2            => MlResult<(T1, T2)>.Valid((source1, x2))
                                  );

        return result;
    }
                                 

    /// <summary>
    /// Crea la instancia solicitada de <c>CompleteMlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source1">Source 1.</param>
    /// <param name="source2Async">Valor de <c>source2Async</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this T1                 source1,
                                                                                    Task<MlResult<T2>> source2Async)
        => source2Async.MatchAsync(
                                      failAsync : errorDetails2 => errorDetails2.ToMlResultFailAsync<(T1, T2)>(),
                                      validAsync: x2            => MlResult<(T1, T2)>.ValidAsync((source1, x2))
                                  );

    /// <summary>
    /// Crea la instancia solicitada de <c>CompleteMlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor.</typeparam>
    /// <param name="source1Async">Source 1 async.</param>
    /// <param name="source2Async">Valor de <c>source2Async</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this Task<T1>           source1Async,
                                                                                          Task<MlResult<T2>> source2Async)
    {
        var source1 = await source1Async;

        var result = await source1.CreateCompleteMlResultAsync(source2Async);

        return result;
    }







    #endregion




}

