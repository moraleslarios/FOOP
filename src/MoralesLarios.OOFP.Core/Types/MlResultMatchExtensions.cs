// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Métodos de extensión de resolución (<c>Match</c>) que reducen un <see cref="MlResult{T}"/> a un único valor aplicando una función distinta según sea válido o fallido.
/// </summary>
public static class MlResultMatchExtensions
{

    #region Match


    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <returns>Valor de tipo <c>TResult</c> obtenido por la operación.</returns>
    public static TResult Match<T, TResult>(this MlResult<T>                    source,
                                                 Func<T              , TResult> valid,
                                                 Func<MlErrorDetails, TResult> fail)
        => source.IsSuccess ? valid(source.SuccessValue) : fail(source.FailureDetails);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el valor de tipo <c>TResult</c> obtenido por la operación.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(this MlResult<T>                          source,
                                                                  Func<T              , Task<TResult>> validAsync,
                                                                  Func<MlErrorDetails, Task<TResult>> failAsync)
        => source.IsSuccess ? await validAsync(source.SuccessValue) : await failAsync(source.FailureDetails);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el valor de tipo <c>TResult</c> obtenido por la operación.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(this MlResult<T>                          source,
                                                                  Func<T              ,      TResult>  valid,
                                                                  Func<MlErrorDetails, Task<TResult>> failAsync)
        => source.IsSuccess ? valid(source.SuccessValue) : await failAsync(source.FailureDetails);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <returns>Tarea que produce el valor de tipo <c>TResult</c> obtenido por la operación.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(this MlResult<T>                          source,
                                                                  Func<T              , Task<TResult>> validAsync,
                                                                  Func<MlErrorDetails,      TResult>  fail)
        => source.IsSuccess ? await validAsync(source.SuccessValue) : fail(source.FailureDetails);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <returns>Tarea que produce el valor de tipo <c>TResult</c> obtenido por la operación.</returns>
    public static Task<TResult> MatchAsync<T, TResult>(this MlResult<T>                    source,
                                                            Func<T              , TResult> valid,
                                                            Func<MlErrorDetails, TResult> fail)
        => source.Match(valid, fail).ToAsync();



    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el valor de tipo <c>TResult</c> obtenido por la operación.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                  Func<T              , Task<TResult>> validAsync,
                                                                  Func<MlErrorDetails, Task<TResult>> failAsync)
    {
        var partialResult = (await sourceAsync);

        var result = partialResult.IsSuccess ? (await validAsync(partialResult.SuccessValue)) : (await failAsync(partialResult.FailureDetails));

        return result;
    }

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el valor de tipo <c>TResult</c> obtenido por la operación.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                  Func<T              ,      TResult>  valid,
                                                                  Func<MlErrorDetails, Task<TResult>> failAsync)
    {
        var partialResult = (await sourceAsync);

        var result = partialResult.IsSuccess ? valid(partialResult.SuccessValue) : await failAsync(partialResult.FailureDetails);

        return result;
    }

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <returns>Tarea que produce el valor de tipo <c>TResult</c> obtenido por la operación.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                  Func<T              , Task<TResult>> validAsync,
                                                                  Func<MlErrorDetails,      TResult>  fail)
    {
        var partialResult = (await sourceAsync);

        var result = partialResult.IsSuccess ? await validAsync(partialResult.SuccessValue) : fail(partialResult.FailureDetails);

        return result;
    }

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <returns>Tarea que produce el valor de tipo <c>TResult</c> obtenido por la operación.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                  Func<T              , TResult> valid,
                                                                  Func<MlErrorDetails, TResult> fail)
        => (await sourceAsync).Match(valid, fail);














    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMatch<T, TResult>(this MlResult<T>                    source, 
                                                              Func<T,TResult>                valid,
                                                              Func<MlErrorDetails, TResult> fail,
                                                              Func<Exception, string>        errorMessageBuilder)
        => source.Match(valid: value        => valid.TryToMlResult(value       , errorMessageBuilder),
                        fail : errorDetails => fail .TryToMlResult(errorDetails, errorMessageBuilder));


    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMatch<T, TResult>(this MlResult<T>                    source, 
                                                              Func<T,TResult>                valid,
                                                              Func<MlErrorDetails, TResult> fail,
                                                              string                         errorMessage = null!)
        => source.TryMatch(valid, fail, _ => errorMessage);



    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,Task<TResult>>                validAsync,
                                                                         Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                         Func<Exception, string>              errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => validAsync.TryToMlResultAsync(value       , errorMessageBuilder),
                             failAsync : errorDetails => failAsync .TryToMlResultAsync(errorDetails, errorMessageBuilder));

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,Task<TResult>>                validAsync,
                                                                         Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                         string                               errorMessage = null!)
        => source.TryMatchAsync(validAsync, failAsync, _ => errorMessage);




    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                    source, 
                                                                         Func<T,Task<TResult>>          validAsync,
                                                                         Func<MlErrorDetails, TResult> fail,
                                                                         Func<Exception, string>        errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => validAsync.TryToMlResultAsync(value       , errorMessageBuilder),
                             failAsync : errorDetails => fail      .TryToMlResult     (errorDetails, errorMessageBuilder).ToAsync());


    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                    source, 
                                                                         Func<T,Task<TResult>>          validAsync,
                                                                         Func<MlErrorDetails, TResult> fail,
                                                                         string                         errorMessage = null!)
        => source.TryMatchAsync(validAsync, fail, _ => errorMessage);


    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,TResult>                      valid,
                                                                         Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                         Func<Exception, string>              errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => valid     .TryToMlResult     (value       , errorMessageBuilder).ToAsync(),
                             failAsync : errorDetails => failAsync .TryToMlResultAsync(errorDetails, errorMessageBuilder));

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,TResult>                      valid,
                                                                         Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                         string                               errorMessage = null!)
        => source.TryMatchAsync(valid, failAsync, _ => errorMessage);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>              source,
                                                                         Func<T, TResult>         valid,
                                                                         Func<MlErrorDetails, TResult> fail,
                                                                         Func<Exception, string>  errorMessageBuilder)
        => source.TryMatch(valid, fail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>              source,
                                                                         Func<T, TResult>         valid,
                                                                         Func<MlErrorDetails, TResult> fail,
                                                                         string                    errorMessage = null!)
        => source.TryMatchAsync(valid, fail, _ => errorMessage);




    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                               Func<T,Task<TResult>>                validAsync,
                                                                               Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                               Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMatchAsync(validAsync, failAsync, errorMessageBuilder);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                               Func<T,Task<TResult>>                validAsync,
                                                                               Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                               string                               errorMessage = null!)
        => await (await sourceAsync).TryMatchAsync(validAsync, failAsync, errorMessage);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                               Func<T,Task<TResult>>          validAsync,
                                                                               Func<MlErrorDetails, TResult> fail,
                                                                               Func<Exception, string>        errorMessageBuilder)
        => await (await sourceAsync).TryMatchAsync(validAsync, fail, errorMessageBuilder);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="validAsync">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                               Func<T,Task<TResult>>          validAsync,
                                                                               Func<MlErrorDetails, TResult> fail,
                                                                               string                         errorMessage = null!)
        => await (await sourceAsync).TryMatchAsync(validAsync, fail, errorMessage);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                               Func<T, TResult>                     valid,
                                                                               Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                               Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMatchAsync(valid, failAsync, errorMessageBuilder);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="failAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                               Func<T, TResult>                     valid,
                                                                               Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                               string                               errorMessage = null!)
        => await (await sourceAsync).TryMatchAsync(valid, failAsync, errorMessage);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                               Func<T, TResult>               valid,
                                                                               Func<MlErrorDetails, TResult> fail,
                                                                               Func<Exception, string>        errorMessageBuilder)
        => (await sourceAsync).TryMatch(valid, fail,errorMessageBuilder);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="valid">Función a aplicar.</param>
    /// <param name="fail">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                               Func<T, TResult>               valid,
                                                                               Func<MlErrorDetails, TResult> fail,
                                                                               string                         errorMessage = null!)
        => (await sourceAsync).TryMatch(valid, fail, errorMessage);



    #endregion Match




    #region MatchAll



    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAll">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> Match<T, TResult>(this MlResult<T>   source, 
                                                           Func<TResult> funcAll)
        => funcAll();

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAllAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MatchAsync<T, TResult>(this MlResult<T>         source, 
                                                                            Func<Task<TResult>> funcAllAsync)
        => await funcAllAsync();

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAll">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MatchAsync<T, TResult>(this MlResult<T>   source,
                                                                      Func<TResult> funcAll)
        => source.Match(funcAll).ToAsync();


    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAllAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MatchAsync<T, TResult>(this Task<MlResult<T>>   sourceAsync, 
                                                                            Func<Task<TResult>> funcAllAsync)
        => await funcAllAsync();

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAll">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MatchAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, 
                                                                            Func<TResult>     funcAll)
        => (await sourceAsync).Match(funcAll);





    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAll">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMatch<T, TResult>(this MlResult<T>             source, 
                                                              Func<TResult>           funcAll,
                                                              Func<Exception, string> errorMessageBuilder)
        => source.Match(valid: _            => funcAll.TryToMlResult(errorMessageBuilder),
                        fail : errorDetails => funcAll.TryToMlResult(errorDetails, errorMessageBuilder));

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAll">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMatch<T, TResult>(this MlResult<T>   source, 
                                                              Func<TResult> funcAll,
                                                              string        errorMessage = null!)
        => TryMatch(source, funcAll, _ => errorMessage);


    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAllAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>             source, 
                                                                         Func<Task<TResult>>     funcAllAsync,
                                                                         Func<Exception, string> errorMessageBuilder)
        => source.MatchAsync(validAsync: _            => funcAllAsync.TryToMlResultAsync(errorMessageBuilder),
                             failAsync : errorDetails => funcAllAsync.TryToMlResultAsync(errorDetails, errorMessageBuilder));

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAllAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>         source, 
                                                                         Func<Task<TResult>> funcAllAsync,
                                                                         string              errorMessage = null!)
        => TryMatchAsync(source, funcAllAsync, _ => errorMessage);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAllAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync, 
                                                                               Func<Task<TResult>>     funcAllAsync,
                                                                               Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMatchAsync(funcAllAsync, errorMessageBuilder);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAllAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>   sourceAsync, 
                                                                               Func<Task<TResult>> funcAllAsync,
                                                                               string              errorMessage = null!)
        => await (await sourceAsync).TryMatchAsync(funcAllAsync, errorMessage);


    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAll">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync, 
                                                                               Func<TResult>           funcAll,
                                                                               Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryMatch(funcAll, errorMessageBuilder);

    /// <summary>
    /// Reduce el resultado a un único valor aplicando una rama para el caso válido y otra para el fallido.
    /// Captura cualquier excepción producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAll">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, 
                                                                               Func<TResult>     funcAll,
                                                                               string            errorMessage = null!)
        => (await sourceAsync).TryMatch(funcAll, errorMessage);



    #endregion MatchAll



}

