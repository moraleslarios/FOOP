// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System;

namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Métodos de extensión de proyección (<c>Map</c>) sobre <see cref="MlResult{T}"/>: transforman el valor cuando el origen es válido, con variantes <c>TryMap</c> que capturan las excepciones.
/// </summary>
public static class MlResultMapExtensions
{




    #region Map



    /// <summary>
    /// Transforma el valor.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> Map<T, TResult>(this MlResult<T>      source, 
                                                         Func<T, TResult> func)
        => source.Match
        (
            fail : MlResult<TResult>.Fail,
            valid: value => func(value)
        );

    /// <summary>
    /// Transforma el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapAsync<T, TResult>(this MlResult<T>      source, 
                                                                    Func<T, TResult> func)
        => source.Map(func).ToAsync();

    /// <summary>
    /// Transforma el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAsync<T, TResult>(this MlResult<T>            source,
                                                                          Func<T, Task<TResult>> funcAsync)
        => await source.MatchAsync
        (
            failAsync : errorsDetails => Task.FromResult(MlResult<TResult>.Fail(errorsDetails)),
            validAsync: value         => funcAsync.ToMlResultAsync(value)
        );


    /// <summary>
    /// Transforma el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAsync<T, TResult>(this Task<MlResult<T>>      sourceAsync,
                                                                          Func<T, Task<TResult>> funcAsync)
    {
        var partialMlResult = await sourceAsync;

        var result = await partialMlResult.MatchAsync
        (
            failAsync : errorsDetails => Task.FromResult(MlResult<TResult>.Fail(errorsDetails)),
            validAsync: value         => funcAsync.ToMlResultAsync(value)
        );

        return result;
    }

    /// <summary>
    /// Transforma el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAsync<T, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                          Func<T, TResult>  func)
        => (await sourceAsync).Map(func);


    /// <summary>
    /// Transforma el valor. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMap<T, TResult>(this MlResult<T>      source, 
                                                            Func<T, TResult> func,
                                                            string           exceptionAdditionalMessage = null!)
        => TryMap(source, func, _ => exceptionAdditionalMessage!);

    /// <summary>
    /// Transforma el valor. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapAsync<T, TResult>(this MlResult<T>      source, 
                                                                       Func<T, TResult> func,
                                                                       string           exceptionAdditionalMessage = null!)
        => source.TryMap(func, exceptionAdditionalMessage).ToAsync();

    /// <summary>
    /// Transforma el valor. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMap<T, TResult>(this MlResult<T>             source, 
                                                            Func<T, TResult>        func,
                                                            Func<Exception, string> errorMessageBuilder)
        => source.Match
        (
            fail : MlResult<TResult>.Fail,
            valid: value => func.TryToMlResult(value, errorMessageBuilder)
        );

    /// <summary>
    /// Transforma el valor. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapAsync<T, TResult>(this MlResult<T>             source, 
                                                                       Func<T, TResult>        func,
                                                                       Func<Exception, string> errorMessageBuilder)
        => source.TryMap(func, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapAsync<T, TResult>(this MlResult<T>            source,
                                                                             Func<T, Task<TResult>> funcAsync,
                                                                             string                 exceptionAdditionalMessage = null!)
        => await TryMapAsync(source, funcAsync, _ => exceptionAdditionalMessage!);

    /// <summary>
    /// Transforma el valor. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapAsync<T, TResult>(this MlResult<T>             source,
                                                                             Func<T, Task<TResult>>  funcAsync,
                                                                             Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync
        (
            failAsync :       errorsDetails => Task.FromResult(MlResult<TResult>.Fail(errorsDetails)),
            validAsync: async value         => await funcAsync.TryToMlResultAsync(value, errorMessageBuilder)
        );


    /// <summary>
    /// Transforma el valor. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapAsync<T, TResult>(this Task<MlResult<T>>      sourceAsync,
                                                                             Func<T, Task<TResult>> funcAsync,
                                                                             string                 exceptionAdditionalMessage = null!)
        => await TryMapAsync(sourceAsync, funcAsync, _ => exceptionAdditionalMessage!);

    /// <summary>
    /// Transforma el valor. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync,
                                                                             Func<T, Task<TResult>>  funcAsync,
                                                                             Func<Exception, string> errorMessageBuilder)
    {
        var partialMlResult = await sourceAsync;

        var result = await partialMlResult.MatchAsync
        (
            failAsync :       errorsDetails => Task.FromResult(MlResult<TResult>.Fail(errorsDetails)),
            validAsync: async value         => await funcAsync.TryToMlResultAsync(value, errorMessageBuilder)
        );

        return result;
    }


    /// <summary>
    /// Transforma el valor. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync,
                                                                             Func<T, TResult>        func,
                                                                             Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapAsync(func, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapAsync<T, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                             Func<T, TResult>  func,
                                                                             string            exceptionAdditionalMessage = null!)
        => await (await sourceAsync).TryMapAsync(func, exceptionAdditionalMessage);


    #endregion


    #region MapEnsure


    /// <summary>
    /// Convierte un MlResult en Fail, si no cumple con la condición de EnsureFUnc
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorDetailsResult">Detalles del error que se devuelven cuando no se cumple la condición.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> MapEnsure<T>(this MlResult<T>     source,
                                                Func<T, bool>   ensureFunc,
                                                MlErrorDetails errorDetailsResult)
        => source.MapEnsure(ensureFunc, _ => errorDetailsResult);

    /// <summary>
    /// Transforma el valor de <c>Ensure</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorDetailsResultBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> MapEnsure<T>(this MlResult<T>              source,
                                                Func<T, bool>            ensureFunc,
                                                Func<T, MlErrorDetails> errorDetailsResultBuilder)
    {
        var result = source.Match(
                                       valid: x      => ensureFunc(x) ? x : errorDetailsResultBuilder(x),
                                       fail : errors => errors.ToMlResultFail<T>()
                                 );

        return result;
    }

    /// <summary>
    /// Transforma el valor de <c>Ensure</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorMessageResult">Valor de <c>errorMessageResult</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> MapEnsure<T>(this MlResult<T>   source,
                                                Func<T, bool> ensureFunc,
                                                string        errorMessageResult)
        => source.MapEnsure(ensureFunc, errorMessageResult.ToMlErrorsDetails());

    /// <summary>
    /// Transforma el valor de <c>Ensure</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorMessageResultBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> MapEnsure<T>(this MlResult<T>               source,
                                                Func<T, bool>             ensureFunc,
                                                Func<T, string>           errorMessageResultBuilder)
        => source.MapEnsure(ensureFunc, x => errorMessageResultBuilder(x).ToMlErrorsDetails());



    /// <summary>
    /// Transforma el valor de <c>Ensure</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorDetailsResult">Valor de <c>errorDetailsResult</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapEnsureAsync<T>(this MlResult<T>     source,
                                                           Func<T, bool>   ensureFunc,
                                                           MlErrorDetails errorDetailsResult)
        => source.MapEnsure(ensureFunc, errorDetailsResult).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>Ensure</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorDetailsResultBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapEnsureAsync<T>(this MlResult<T>              source,
                                                           Func<T, bool>            ensureFunc,
                                                           Func<T, MlErrorDetails> errorDetailsResultBuilder)
        => source.MapEnsure(ensureFunc, errorDetailsResultBuilder).ToAsync();


    /// <summary>
    /// Transforma el valor de <c>Ensure</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorMessageResult">Valor de <c>errorMessageResult</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapEnsureAsync<T>(this MlResult<T> source,
                                                           Func<T, bool> ensureFunc,
                                                           string errorMessageResult)
    {
        var result = await source.MapEnsure(ensureFunc, errorMessageResult).ToAsync();

        return result;
    }


    /// <summary>
    /// Transforma el valor de <c>Ensure</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorMessageResultBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapEnsureAsync<T>(this MlResult<T>      source,
                                                           Func<T, bool>   ensureFunc,
                                                           Func<T, string> errorMessageResultBuilder)
        => source.MapEnsure(ensureFunc, errorMessageResultBuilder).ToAsync();



    /// <summary>
    /// Transforma el valor de <c>Ensure</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorDetailsResult">Valor de <c>errorDetailsResult</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapEnsureAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                 Func<T, bool>     ensureFunc,
                                                                 MlErrorDetails   errorDetailsResult)
        => await (await sourceAsync).MapEnsureAsync(ensureFunc, errorDetailsResult);

    /// <summary>
    /// Transforma el valor de <c>Ensure</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorDetailsResultBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapEnsureAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                 Func<T, bool>            ensureFunc,
                                                                 Func<T, MlErrorDetails> errorDetailsResultBuilder)
        => await (await sourceAsync).MapEnsureAsync(ensureFunc, errorDetailsResultBuilder);


    /// <summary>
    /// Transforma el valor de <c>Ensure</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorMessageResult">Valor de <c>errorMessageResult</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapEnsureAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                 Func<T, bool>     ensureFunc,
                                                                 string            errorMessageResult)
        => await (await sourceAsync).MapEnsureAsync(ensureFunc, errorMessageResult);

    /// <summary>
    /// Transforma el valor de <c>Ensure</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="ensureFunc">Función que se ejecuta durante la operación.</param>
    /// <param name="errorMessageResultBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapEnsureAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                 Func<T, bool>     ensureFunc,
                                                                 Func<T, string>   errorMessageResultBuilder)
        => await (await sourceAsync).MapEnsureAsync(ensureFunc, errorMessageResultBuilder);




    #endregion


    #region MapIf

    /// <summary>
    /// Transforma el valor de <c>If</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MapIf<T, TResult>(this MlResult<T>      source,
                                                           Func<T, bool>    condition,
                                                           Func<T, TResult> funcTrue,
                                                           Func<T, TResult> funcFalse)
    {
        var result = source.Match(
                                       valid: x => condition(x) ? funcTrue(x) : funcFalse(x),
                                       fail :      MlResult<TResult>.Fail
                                 );
        return result;
    }


    /// <summary>
    /// Transforma el valor de <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapIfAsync<T, TResult>(this MlResult<T>      source,
                                                                      Func<T, bool>    condition,
                                                                      Func<T, TResult> funcTrue,
                                                                      Func<T, TResult> funcFalse)
        => source.MapIf(condition, funcTrue, funcFalse).ToAsync();




    /// <summary>
    /// Transforma el valor de <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrueAsync">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfAsync<T, TResult>(this MlResult<T>            source,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, Task<TResult>> funcTrueAsync,
                                                                            Func<T, TResult>       funcFalse)
    {
        var result = await source.MatchAsync<T, MlResult<TResult>>(
                                           validAsync: async x => condition(x) ? await funcTrueAsync(x) : await funcFalse(x).ToAsync(),
                                           fail      :            MlResult<TResult>.Fail
                                      );
        return result;
    }

    /// <summary>
    /// Transforma el valor de <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalseAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfAsync<T, TResult>(this MlResult<T>            source,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, TResult>       funcTrue,
                                                                            Func<T, Task<TResult>> funcFalseAsync)
    {
        var result = await source.MatchAsync<T, MlResult<TResult>>(
                                           validAsync: async x => condition(x) ? await funcTrue(x).ToAsync() : await funcFalseAsync(x),
                                           fail      :            MlResult<TResult>.Fail
                                      );
        return result;
    }

   /// <summary>
   /// Transforma el valor de <c>If</c>. Versión asíncrona.
   /// </summary>
   /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
   /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
   /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
   /// <param name="condition">Condición que debe cumplirse.</param>
   /// <param name="funcTrueAsync">Función a aplicar.</param>
   /// <param name="funcFalseAsync">Función a aplicar.</param>
   /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
   public static async Task<MlResult<TResult>> MapIfAsync<T, TResult>(this MlResult<T>             source,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, Task<TResult>> funcTrueAsync,
                                                                            Func<T, Task<TResult>> funcFalseAsync)
    {
        var result = await source.MatchAsync<T, MlResult<TResult>>(
                                           validAsync: async x => condition(x) ? await funcTrueAsync(x) : await funcFalseAsync(x),
                                           fail      :            MlResult<TResult>.Fail
                                      );
        return result;
    }
    /// <summary>
    /// Transforma el valor de <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfAsync<T, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                            Func<T, bool>     condition,
                                                                            Func<T, TResult>  funcTrue,
                                                                            Func<T, TResult>  funcFalse)
        => await (await sourceAsync).MapIfAsync(condition, funcTrue, funcFalse);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrueAsync">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfAsync<T, TResult>(this Task<MlResult<T>>      sourceAsync,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, Task<TResult>> funcTrueAsync,
                                                                            Func<T, TResult>       funcFalse)
        => await (await sourceAsync).MapIfAsync(condition, funcTrueAsync, funcFalse);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalseAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfAsync<T, TResult>(this Task<MlResult<T>>      sourceAsync,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, TResult>       funcTrue,
                                                                            Func<T, Task<TResult>> funcFalseAsync)
        => await (await sourceAsync).MapIfAsync(condition, funcTrue, funcFalseAsync);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrueAsync">Función a aplicar.</param>
    /// <param name="funcFalseAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfAsync<T, TResult>(this Task<MlResult<T>>      sourceAsync,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, Task<TResult>> funcTrueAsync,
                                                                            Func<T, Task<TResult>> funcFalseAsync)
        => await (await sourceAsync).MapIfAsync(condition, funcTrueAsync, funcFalseAsync);


    // // En este grupo, no añado funcion de parámetro para cuando no se cumple la comprobación, ya que al ser el mismo tipo no seria necesario.
    // // Si quisiera hacer algo en caso de no cumplirse, me valdrían las sobrecargas de arriba

    /// <summary>
    /// Transforma el valor de <c>If</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> MapIf<T>(this MlResult<T>   source,
                                            Func<T, bool> condition,
                                            Func<T, T>    func)
    {
        var result = source.Match(
                                       valid: x => condition(x) ? func(x) : x,
                                       fail :      MlResult<T>.Fail
                                 );
        return result;
    }

    /// <summary>
    /// Transforma el valor de <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapIfAsync<T>(this MlResult<T>   source,
                                                       Func<T, bool> condition,
                                                       Func<T, T>    func)
        => source.MapIf(condition, func).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                             Func<T, bool>     condition,
                                                             Func<T, T>        func)
        => await (await sourceAsync).MapIfAsync(condition, func);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfAsync<T>(this MlResult<T>      source,
                                                             Func<T, bool>    condition,
                                                             Func<T, Task<T>> funcAsync)
    {
        var result = await source.MatchAsync<T, MlResult<T>>(
                                                               validAsync: async x => condition(x) ? await funcAsync(x) : await x.ToAsync(),
                                                               fail :      MlResult<T>.Fail
                                                            );
        return result;
    }

    /// <summary>
    /// Transforma el valor de <c>If</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                             Func<T, bool>     condition,
                                                             Func<T, Task<T>>  funcAsync)
        => await (await sourceAsync).MapIfAsync(condition, funcAsync);






    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIf<T, TResult>(this MlResult<T>             source,
                                                              Func<T, bool>           condition,
                                                              Func<T, TResult>        funcTrue,
                                                              Func<T, TResult>        funcFalse,
                                                              Func<Exception, string> errorMessageBuilder)
    {
        var result = source.Match(
                                       valid: x => condition(x) 
                                                        ? funcTrue .TryToMlResult(x, errorMessageBuilder)
                                                        : funcFalse.TryToMlResult(x, errorMessageBuilder),
                                       fail :      MlResult<TResult>.Fail
                                 );
        return result;
    }

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIf<T, TResult>(this MlResult<T>      source,
                                                              Func<T, bool>    condition,
                                                              Func<T, TResult> funcTrue,
                                                              Func<T, TResult> funcFalse,
                                                              string           exceptionAdditionalMessage = null!)
        => source.TryMapIf(condition, funcTrue, funcFalse, _ => exceptionAdditionalMessage!);


    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this MlResult<T>             source,
                                                                         Func<T, bool>           condition,
                                                                         Func<T, TResult>        funcTrue,
                                                                         Func<T, TResult>        funcFalse,
                                                                         Func<Exception, string> errorMessageBuilder)
        => source.TryMapIf(condition, funcTrue, funcFalse, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this MlResult<T>      source,
                                                                         Func<T, bool>    condition,
                                                                         Func<T, TResult> funcTrue,
                                                                         Func<T, TResult> funcFalse,
                                                                         string           exceptionAdditionalMessage = null!)
        => source.TryMapIf(condition, funcTrue, funcFalse, exceptionAdditionalMessage!).ToAsync();


    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrueAsync">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this MlResult<T>       source,
                                                                         Func<T, bool>           condition,
                                                                         Func<T, Task<TResult>>  funcTrueAsync,
                                                                         Func<T, TResult>        funcFalse,
                                                                         Func<Exception, string> errorMessageBuilder)
    {
        var result = await source.MatchAsync(
                                                 validAsync: async x => condition(x) 
                                                                          ? await funcTrueAsync.TryToMlResultAsync(x, errorMessageBuilder)
                                                                          : await funcFalse    .TryToMlResult     (x, errorMessageBuilder).ToAsync(),
                                                 fail: MlResult<TResult>.Fail
                                            );
        return result;
    }

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrueAsync">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this MlResult<T>            source,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, Task<TResult>> funcTrueAsync,
                                                                               Func<T, TResult>       funcFalse,
                                                                               string                 exceptionAdditionalMessage = null!)
        => await source.TryMapIfAsync(condition, funcTrueAsync, funcFalse, _ => exceptionAdditionalMessage!);


    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalseAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this MlResult<T>       source,
                                                                         Func<T, bool>           condition,
                                                                         Func<T, TResult>        funcTrue,
                                                                         Func<T, Task<TResult>>  funcFalseAsync,
                                                                         Func<Exception, string> errorMessageBuilder)
    {
        var result = await source.MatchAsync(
                                                 validAsync: async x => condition(x) 
                                                                          ? await funcTrue      .TryToMlResult     (x, errorMessageBuilder).ToAsync()
                                                                          : await funcFalseAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                                 fail: MlResult<TResult>.Fail
                                            );
        return result;
    }

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalseAsync">Función a aplicar.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this MlResult<T>            source,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, TResult>       funcTrue,
                                                                               Func<T, Task<TResult>> funcFalseAsync,
                                                                               string                 exceptionAdditionalMessage = null!)
        => await source.TryMapIfAsync(condition, funcTrue, funcFalseAsync, _ => exceptionAdditionalMessage!);


    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrueAsync">Función a aplicar.</param>
    /// <param name="funcFalseAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this MlResult<T>             source,
                                                                               Func<T, bool>           condition,
                                                                               Func<T, Task<TResult>>  funcTrueAsync,
                                                                               Func<T, Task<TResult>>  funcFalseAsync,
                                                                               Func<Exception, string> errorMessageBuilder)
    {
        var result = await source.MatchAsync(
                                                 validAsync: async x => condition(x) 
                                                                          ? await funcTrueAsync .TryToMlResultAsync(x, errorMessageBuilder)
                                                                          : await funcFalseAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                                 fail: MlResult<TResult>.Fail
                                            );
        return result;
    }

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrueAsync">Función a aplicar.</param>
    /// <param name="funcFalseAsync">Función a aplicar.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this MlResult<T>            source,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, Task<TResult>> funcTrueAsync,
                                                                               Func<T, Task<TResult>> funcFalseAsync,
                                                                               string                 exceptionAdditionalMessage = null!)
        => await source.TryMapIfAsync(condition, funcTrueAsync, funcFalseAsync, _ => exceptionAdditionalMessage!);



    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync,
                                                                               Func<T, bool>           condition,
                                                                               Func<T, TResult>        funcTrue,
                                                                               Func<T, TResult>        funcFalse,
                                                                               Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrue, funcFalse, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                               Func<T, bool>     condition,
                                                                               Func<T, TResult>  funcTrue,
                                                                               Func<T, TResult>  funcFalse,
                                                                               string            exceptionAdditionalMessage = null!)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrue, funcFalse, exceptionAdditionalMessage!);


    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrueAsync">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync,
                                                                               Func<T, bool>           condition,
                                                                               Func<T, Task<TResult>>  funcTrueAsync,
                                                                               Func<T, TResult>        funcFalse,
                                                                               Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrueAsync, funcFalse, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrueAsync">Función a aplicar.</param>
    /// <param name="funcFalse">Función a aplicar.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this Task<MlResult<T>>      sourceAsync,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, Task<TResult>> funcTrueAsync,
                                                                               Func<T, TResult>       funcFalse,
                                                                               string                 exceptionAdditionalMessage = null!)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrueAsync, funcFalse, _ => exceptionAdditionalMessage!);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalseAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync,
                                                                               Func<T, bool>           condition,
                                                                               Func<T, TResult>        funcTrue,
                                                                               Func<T, Task<TResult>>  funcFalseAsync,
                                                                               Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrue, funcFalseAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrue">Función a aplicar.</param>
    /// <param name="funcFalseAsync">Función a aplicar.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this Task<MlResult<T>>      sourceAsync,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, TResult>       funcTrue,
                                                                               Func<T, Task<TResult>> funcFalseAsync,
                                                                               string                 exceptionAdditionalMessage = null!)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrue, funcFalseAsync, _ => exceptionAdditionalMessage!);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrueAsync">Función a aplicar.</param>
    /// <param name="funcFalseAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync,
                                                                               Func<T, bool>           condition,
                                                                               Func<T, Task<TResult>>  funcTrueAsync,
                                                                               Func<T, Task<TResult>>  funcFalseAsync,
                                                                               Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrueAsync, funcFalseAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcTrueAsync">Función a aplicar.</param>
    /// <param name="funcFalseAsync">Función a aplicar.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfAsync<T, TResult>(this Task<MlResult<T>>      sourceAsync,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, Task<TResult>> funcTrueAsync,
                                                                               Func<T, Task<TResult>> funcFalseAsync,
                                                                               string                 exceptionAdditionalMessage = null!)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrueAsync, funcFalseAsync, _ => exceptionAdditionalMessage!);



    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIf<T>(this MlResult<T>             source,
                                               Func<T, bool>           condition,
                                               Func<T, T>              func,
                                               Func<Exception, string> errorMessageBuilder)
         => source.Match
                        (
                            valid: x => condition(x)
                                            ? func.TryToMlResult(x, errorMessageBuilder)
                                            : x.ToMlResultValid(),
                            fail: MlResult<T>.Fail
                        );

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIf<T>(this MlResult<T>   source,
                                               Func<T, bool> condition,
                                               Func<T, T>    func,
                                               string        exceptionAdditionalMessage = null!)
        => source.TryMapIf(condition, func, _ => exceptionAdditionalMessage!);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfAsync<T>(this MlResult<T>             source,
                                                          Func<T, bool>           condition,
                                                          Func<T, T>              func,
                                                          Func<Exception, string> errorMessageBuilder)
        => source.TryMapIf(condition, func, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfAsync<T>(this MlResult<T>   source,
                                                          Func<T, bool> condition,
                                                          Func<T, T>    func,
                                                          string        exceptionAdditionalMessage = null!)
        => source.TryMapIf(condition, func, _ => exceptionAdditionalMessage!).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfAsync<T>(this MlResult<T>             source,
                                                                Func<T, bool>           condition,
                                                                Func<T, Task<T>>        funcAsync,
                                                                Func<Exception, string> errorMessageBuilder)
         => await source.MatchAsync(
                                        validAsync: async x => condition(x)
                                                                    ? await funcAsync.TryToMlResultAsync(x, errorMessageBuilder)
                                                                    : await x.ToMlResultValidAsync(),
                                        fail: MlResult<T>.Fail
                                    );

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfAsync<T>(this MlResult<T>      source,
                                                                Func<T, bool>    condition,
                                                                Func<T, Task<T>> funcAsync,
                                                                string           exceptionAdditionalMessage = null!)
        => await source.TryMapIfAsync(condition, funcAsync, _ => exceptionAdditionalMessage!);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                Func<T, bool>           condition,
                                                                Func<T, Task<T>>        funcAsync,
                                                                Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfAsync(condition, funcAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>If</c>. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                Func<T, bool>     condition,
                                                                Func<T, Task<T>>  funcAsync,
                                                                string            exceptionAdditionalMessage = null!)
        => await (await sourceAsync).TryMapIfAsync(condition, funcAsync, _ => exceptionAdditionalMessage!);




    #endregion







    #region MapIfFail





    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> MapIfFail<T>(this MlResult<T>              source, 
                                                Func<MlErrorDetails, T> func)
        => source.Match
                        (
                            fail : func,
                            valid: value => value
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapIfFailAsync<T>(this MlResult<T>              source, 
                                                           Func<MlErrorDetails, T> func)
        => source.MapIfFail(func).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailAsync<T>(this MlResult<T>                    source, 
                                                                 Func<MlErrorDetails, Task<T>> funcAsync)
        => await source.MatchAsync<T, MlResult<T>>
                        (
                            failAsync : async errorDetails => await funcAsync(errorDetails),
                            validAsync:       value        =>       value.ToMlResultValidAsync()
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailAsync<T>(this Task<MlResult<T>>              sourceAsync, 
                                                                 Func<MlErrorDetails, Task<T>> funcAsync)
        => await (await sourceAsync).MapIfFailAsync(funcAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailAsync<T>(this Task<MlResult<T>>        sourceAsync, 
                                                                 Func<MlErrorDetails, T> func)
        => (await sourceAsync).MapIfFail(func);



    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFail<T>(this MlResult<T>              source, 
                                                   Func<MlErrorDetails, T> func,
                                                   Func<Exception, string>  errorMessageBuilder)
        => source.Match
                        (
                            fail : errorDetails => func.TryToMlResult(errorDetails, errorMessageBuilder),
                            valid: value => value
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailAsync<T>(this MlResult<T>              source, 
                                                              Func<MlErrorDetails, T> func,
                                                              Func<Exception, string>  errorMessageBuilder)
        => source.TryMapIfFail(func, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFail<T>(this MlResult<T>              source, 
                                                   Func<MlErrorDetails, T> func,
                                                   string                   errorMessage = null!)
        => source.TryMapIfFail(func, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailAsync<T>(this MlResult<T>              source, 
                                                              Func<MlErrorDetails, T> func,
                                                              string                   errorMessage = null!)
        => source.TryMapIfFail(func, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this MlResult<T>                    source, 
                                                                    Func<MlErrorDetails, Task<T>> funcAsync,
                                                                    Func<Exception, string>        errorMessageBuilder)
        => await source.MatchAsync
                        (
                            failAsync : errorDetails => funcAsync.TryToMlResultAsync(errorDetails, errorMessageBuilder),
                            validAsync: value        => value.ToMlResultValidAsync()
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this MlResult<T>                    source, 
                                                                    Func<MlErrorDetails, Task<T>> funcAsync,
                                                                    string                         errorMessage = null!)
        => await source.TryMapIfFailAsync(funcAsync, _ => errorMessage!);



    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this Task<MlResult<T>>              sourceAsync, 
                                                                    Func<MlErrorDetails, Task<T>> funcAsync,
                                                                    Func<Exception, string>        errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailAsync(funcAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this Task<MlResult<T>>              sourceAsync, 
                                                                    Func<MlErrorDetails, Task<T>> funcAsync,
                                                                    string                         errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailAsync(funcAsync, errorMessage);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this Task<MlResult<T>>        sourceAsync, 
                                                                    Func<MlErrorDetails, T> func,
                                                                    Func<Exception, string>  errorMessageBuilder)
        => (await sourceAsync).TryMapIfFail(func, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this Task<MlResult<T>>        sourceAsync, 
                                                                    Func<MlErrorDetails, T> func,
                                                                    string                   errorMessage = null!)
        => (await sourceAsync).TryMapIfFail(func, errorMessage);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MapIfFail<T, TResult>(this MlResult<T>                    source, 
                                                               Func<T              , TResult> funcValid,
                                                               Func<MlErrorDetails, TResult> funcFail)
        => source.Match
                        (
                            fail : funcFail,
                            valid: funcValid
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapIfFailAsync<T, TResult>(this MlResult<T>                    source, 
                                                                          Func<T              , TResult> funcValid,
                                                                          Func<MlErrorDetails, TResult> funcFail)
        => source.MapIfFail(funcValid, funcFail).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailAsync<T, TResult>(this MlResult<T>                          source, 
                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await source.MatchAsync
                        (
                            failAsync : funcFailAsync,
                            validAsync: funcValidAsync
                        );



    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await (await sourceAsync).MapIfFailAsync(funcValidAsync, funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , TResult>       funcValid,
                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await (await sourceAsync).MapIfFailAsync(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                Func<MlErrorDetails, TResult>       funcFail)
        => await (await sourceAsync).MapIfFailAsync(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                                Func<T              , TResult> funcValid,
                                                                                Func<MlErrorDetails, TResult> funcFail)
        => (await sourceAsync).MapIfFail(funcValid, funcFail);



    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFail<T, TResult>(this MlResult<T>                    source, 
                                                                  Func<T              , TResult> funcValid,
                                                                  Func<MlErrorDetails, TResult> funcFail,
                                                                  Func<Exception, string>        errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => funcFail .TryToMlResult(errorsDetails, errorMessageBuilder),
                            valid: x             => funcValid.TryToMlResult(x            , errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this MlResult<T>                    source, 
                                                                             Func<T              , TResult> funcValid,
                                                                             Func<MlErrorDetails, TResult> funcFail,
                                                                             Func<Exception, string>        errorMessageBuilder)
        => source.TryMapIfFail(funcValid, funcFail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFail<T, TResult>(this MlResult<T>                    source, 
                                                                  Func<T              , TResult> funcValid,
                                                                  Func<MlErrorDetails, TResult> funcFail,
                                                                  string                         errorMessage = null!)
        => source.TryMapIfFail(funcValid, funcFail, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this MlResult<T>                    source, 
                                                                             Func<T              , TResult> funcValid,
                                                                             Func<MlErrorDetails, TResult> funcFail,
                                                                             string                         errorMessage = null!)
        => source.TryMapIfFail(funcValid, funcFail, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this MlResult<T>                          source, 
                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await source.MatchAsync(
                            failAsync : errorsDetails => funcFailAsync .TryToMlResultAsync(errorsDetails, errorMessageBuilder),
                            validAsync: x             => funcValidAsync.TryToMlResultAsync(x            , errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this MlResult<T>                          source, 
                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                   string                               errorMessage = null!)
        => await source.TryMapIfFailAsync(funcValidAsync, funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailAsync(funcValidAsync, funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailAsync(funcValidAsync, funcFailAsync, errorMessage);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                   Func<T              , TResult>       funcValid,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                   Func<T              , TResult>       funcValid,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                   string errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                   Func<MlErrorDetails, TResult>       funcFail,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                   Func<MlErrorDetails, TResult>       funcFail,
                                                                                   string errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                                   Func<T              , TResult> funcValid,
                                                                                   Func<MlErrorDetails, TResult> funcFail,
                                                                                   Func<Exception, string>        errorMessageBuilder)
        => (await sourceAsync).TryMapIfFail(funcValid, funcFail, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                   Func<T, TResult>               funcValid,
                                                                                   Func<MlErrorDetails, TResult> funcFail,
                                                                                   string                         errorMessage = null!)
        => (await sourceAsync).TryMapIfFail(funcValid, funcFail, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailAsync<T, TResult>(this MlResult<T>                    source,
                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                Func<MlErrorDetails, TResult>       funcFail)
        => await source.MapIfFailAsync(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailAsync<T, TResult>(this MlResult<T>                    source,
                                                                                Func<T              , TResult>       funcValid,
                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await source.MapIfFailAsync(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this MlResult<T>                    source,
                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                   Func<MlErrorDetails, TResult>       funcFail,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await source.TryMapIfFailAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this MlResult<T>                    source,
                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                   Func<MlErrorDetails, TResult>       funcFail,
                                                                                   string                               errorMessage = null!)
        => await source.TryMapIfFailAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this MlResult<T>                    source,
                                                                                   Func<T              , TResult>       funcValid,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await source.TryMapIfFailAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailAsync<T, TResult>(this MlResult<T>                    source,
                                                                                   Func<T              , TResult>       funcValid,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                   string                               errorMessage = null!)
        => await source.TryMapIfFailAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);





    





    #endregion


    #region MapDefault


    /// <summary>
    /// Transforma el valor de <c>Default</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> MapDefault<T>(this object source) => "Warning, MapDefault method is only valid to debug code".ToMlResultFail<T>();

    /// <summary>
    /// Transforma el valor de <c>Default</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapDefaultAsync<T>(this object source) => await (source ?? new object()).MapDefault<T>().ToAsync();



    #endregion


    #region MapIfFailWithValue




    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// source parameter has a previous 'SuccessValue' execution
    /// </summary>
    /// <typeparam name="T">Value type of the source result.</typeparam>
    /// <param name="source">Source result.</param>
    /// <param name="func">Function executed with the value recovered from the failure details.</param>
    /// <returns>A valid result with the mapped value, or the source if it is valid.</returns>
    public static MlResult<T> MapIfFailWithValue<T>(this MlResult<T> source,
                                                          Func<T, T> func)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetMetadataValue<T>().Map(func),
                            valid: value         => value
                        );



    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithValueAsync<T>(this MlResult<T>       source,
                                                                          Func<T, Task<T>> funcAsync)
        => await source.MatchAsync
                        (
                            failAsync : errorsDetails => errorsDetails.GetMetadataValue<T>().MapAsync(funcAsync),
                            validAsync: value         => value.ToMlResultValidAsync()
                        );


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                           Func<T, Task<T>> funcAsync)
        => await (await sourceAsync).MapIfFailWithValueAsync(funcAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                           Func<T, T>       func)
        => (await sourceAsync).MapIfFailWithValue(func);



    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValue">Función que recibe el valor del resultado de origen.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithValue<T>(this MlResult<T>             source,
                                                            Func<T, T>              funcValue,
                                                            Func<Exception, string> errorMessageBuilder)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetMetadataValue<T>().Bind(x => funcValue.TryToMlResult(x, errorMessageBuilder)),
                            valid: value         => value
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValue">Función que recibe el valor del resultado de origen.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithValue<T>(this MlResult<T> source,
                                                            Func<T, T>  funcValue,
                                                            string      errorMessage = null!)
        => source.TryMapIfFailWithValue(funcValue, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValueAsync">Función asíncrona que recibe el valor del resultado de origen.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this MlResult<T>             source,
                                                                             Func<T, Task<T>>        funcValueAsync,
                                                                             Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync
                        (
                            failAsync : errorsDetails => errorsDetails.GetMetadataValue<T>().BindAsync(x => funcValueAsync.TryToMlResultAsync(x, errorMessageBuilder)),
                            validAsync: value         => value.ToMlResultValidAsync()
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValueAsync">Función asíncrona que recibe el valor del resultado de origen.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this MlResult<T>      source,
                                                                             Func<T, Task<T>> funcValueAsync,
                                                                             string           errorMessage = null!)
        => await source.TryMapIfFailWithValueAsync(funcValueAsync, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValueAsync">Función asíncrona que recibe el valor del resultado de origen.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                             Func<T, Task<T>>        funcValueAsync,
                                                                             Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValueAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValueAsync">Función asíncrona que recibe el valor del resultado de origen.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                             Func<T, Task<T>>  funcValueAsync,
                                                                             string            errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValueAsync, errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValue">Función que recibe el valor del resultado de origen.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                             Func<T, T>              funcValue,
                                                                             Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithValue(funcValue, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValue">Función que recibe el valor del resultado de origen.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                             Func<T, T>        funcValue,
                                                                             string            errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithValue(funcValue, errorMessage);




    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// source parameter has a previous 'SuccessValue' execution
    /// -- 2 funcs (valid and fail) are necessary, since they have to preserve output compatibility --
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MapIfFailWithValue<T, TValue, TResult>(this MlResult<T>            source,
                                                                                 Func<T     , TResult> funcValid,
                                                                                 Func<TValue, TResult> funcFail)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetMetadataValue<TValue>().Map(value => funcFail(value)),
                            valid: value         => funcValid(value)
                        );


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>                 source,
                                                                                                 Func<T     , Task<TResult>> funcValidAsync,
                                                                                                 Func<TValue, Task<TResult>> funcFailAlways)
        => await source.MatchAsync<T, MlResult<TResult>>
                        (
                            failAsync :       errorsDetails =>       errorsDetails.GetMetadataValueAsync<TValue>().MapAsync(value => funcFailAlways(value)),
                            validAsync: async value         => await funcValidAsync(value)
                        );


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithValueAsync<T, TValue, TResult>(this Task<MlResult<T>>            sourceAsync,
                                                                                                  Func<T     , Task<TResult>> funcValidAsync,
                                                                                                  Func<TValue, Task<TResult>> funcFailAlways)
        => await (await sourceAsync).MapIfFailWithValueAsync(funcValidAsync, funcFailAlways);







    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithValue<T, TValue, TResult>(this MlResult<T>             source,
                                                                                   Func<T     , TResult>   funcValid,
                                                                                   Func<TValue, TResult>   funcFail,
                                                                                   Func<Exception, string> errorMessageBuilder)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetMetadataValue<TValue>().Bind(value => funcFail.TryToMlResult(value, errorMessageBuilder)),  
                            valid: value         => funcValid.TryToMlResult(value, errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithValue<T, TValue, TResult>(this MlResult<T>           source,
                                                                                   Func<T     , TResult> funcValid,
                                                                                   Func<TValue, TResult> funcFail,
                                                                                   string                errorMessage = null!)
        => source.TryMapIfFailWithValue(funcValid, funcFail, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>                 source,
                                                                                                    Func<T     , Task<TResult>> funcValidAsync,
                                                                                                    Func<TValue, Task<TResult>> funcFailAsync,
                                                                                                    Func<Exception, string>     errorMessageBuilder)
        => await source.MatchAsync
                                 (
                                     failAsync : errorsDetails => errorsDetails.GetMetadataValueAsync<TValue>().BindAsync(value => funcFailAsync.TryToMlResultAsync(value, errorMessageBuilder)),
                                     validAsync: value         => funcValidAsync.TryToMlResultAsync(value, errorMessageBuilder)
                                 );


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>                 source,
                                                                                                    Func<T     , Task<TResult>> funcValidAsync,
                                                                                                    Func<TValue, Task<TResult>> funcFailAsync,
                                                                                                    string                      errorMessage = null!)
        => await source.TryMapIfFailWithValueAsync(funcValidAsync, funcFailAsync, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this Task<MlResult<T>>           sourceAsync,
                                                                                                    Func<T     , Task<TResult>> funcValidAsync,
                                                                                                    Func<TValue, Task<TResult>> funcFailAsync,
                                                                                                    Func<Exception, string>     errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValidAsync, funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this Task<MlResult<T>>            sourceAsync,
                                                                                                    Func<T     , Task<TResult>> funcValidAsync,
                                                                                                    Func<TValue, Task<TResult>> funcFailAsync,
                                                                                                    string                      errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValidAsync, funcFailAsync, errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                                                    Func<T, TResult> funcValid,
                                                                                                    Func<TValue, Task<TResult>> funcFailAsync,
                                                                                                    Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                                                    Func<T, TResult> funcValid,
                                                                                                    Func<TValue, Task<TResult>> funcFailAsync,
                                                                                                    string errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                                                    Func<T, Task<TResult>> funcValidAsync,
                                                                                                    Func<TValue, TResult> funcFail,
                                                                                                    Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this Task<MlResult<T>> sourceAsync,
                                                                                                    Func<T, Task<TResult>> funcValidAsync,
                                                                                                    Func<TValue, TResult> funcFail,
                                                                                                    string errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this Task<MlResult<T>>       sourceAsync,
                                                                                                    Func<T     , TResult>   funcValid,
                                                                                                    Func<TValue, TResult>   funcFail,
                                                                                                    Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithValue(funcValid, funcFail, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this Task<MlResult<T>>       sourceAsync,
                                                                                                    Func<T     , TResult>   funcValid,
                                                                                                    Func<TValue, TResult>   funcFail,
                                                                                                    string                  errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithValue(funcValid, funcFail, errorMessage);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                    Func<T, T>  func)
        => source.MapIfFailWithValue(func).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValue">Función que recibe el valor del resultado de origen.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this MlResult<T>             source,
                                                                       Func<T, T>              funcValue,
                                                                       Func<Exception, string> errorMessageBuilder)
        => source.TryMapIfFailWithValue(funcValue, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValue">Función que recibe el valor del resultado de origen.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                       Func<T, T>  funcValue,
                                                                       string      errorMessage = null!)
        => source.TryMapIfFailWithValue(funcValue, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>            source,
                                                                                            Func<T     , TResult> funcValid,
                                                                                            Func<TValue, TResult> funcFail)
        => source.MapIfFailWithValue<T, TValue, TResult>(funcValid, funcFail).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>                 source,
                                                                                                  Func<T     , Task<TResult>> funcValidAsync,
                                                                                                  Func<TValue, TResult>       funcFail)
        => await source.MapIfFailWithValueAsync<T, TValue, TResult>(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>                 source,
                                                                                                  Func<T     , TResult>       funcValid,
                                                                                                  Func<TValue, Task<TResult>> funcFailAsync)
        => await source.MapIfFailWithValueAsync<T, TValue, TResult>(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>             source,
                                                                                               Func<T     , TResult>   funcValid,
                                                                                               Func<TValue, TResult>   funcFail,
                                                                                               Func<Exception, string> errorMessageBuilder)
        => source.TryMapIfFailWithValue(funcValid, funcFail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>           source,
                                                                                               Func<T     , TResult> funcValid,
                                                                                               Func<TValue, TResult> funcFail,
                                                                                               string                errorMessage = null!)
        => source.TryMapIfFailWithValue(funcValid, funcFail, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>                 source,
                                                                                                     Func<T     , Task<TResult>> funcValidAsync,
                                                                                                     Func<TValue, TResult>       funcFail,
                                                                                                     Func<Exception, string>     errorMessageBuilder)
        => await source.TryMapIfFailWithValueAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>                 source,
                                                                                                     Func<T     , Task<TResult>> funcValidAsync,
                                                                                                     Func<TValue, TResult>       funcFail,
                                                                                                     string                      errorMessage = null!)
        => await source.TryMapIfFailWithValueAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>                 source,
                                                                                                     Func<T     , TResult>       funcValid,
                                                                                                     Func<TValue, Task<TResult>> funcFailAsync,
                                                                                                     Func<Exception, string>     errorMessageBuilder)
        => await source.TryMapIfFailWithValueAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido, recuperando el valor
    /// almacenado en el detalle del error. Captura cualquier excepción producida y la convierte en un
    /// resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithValueAsync<T, TValue, TResult>(this MlResult<T>                 source,
                                                                                                     Func<T     , TResult>       funcValid,
                                                                                                     Func<TValue, Task<TResult>> funcFailAsync,
                                                                                                     string                      errorMessage = null!)
        => await source.TryMapIfFailWithValueAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

















    #endregion


    #region MapIfFailWithException


    /// En el caso de MapIfFailWithException es diferente al MapIfFailWithValue. 
    /// 
    ///     1.- MapIfFailWithValue: Si recibe un MlResult Fail sin ValueDetail, añadira´un nuevo Error al que le viene de la ejecución anterior
    ///     
    ///     2.- MapIfFailWithException: Si recibe un MlResult Fail sin ExceptionDetail, Devolvera el MlResult Fail, igual que le vino


    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// source parameter has a previous Exception execution or 'ex' ErrorDetail
    /// </summary>
    /// <typeparam name="T">Value type of the source result.</typeparam>
    /// <param name="source">Source result.</param>
    /// <param name="funcException">Function executed with the exception captured in the failure details.</param>
    /// <returns>A valid result with the mapped value, or the source if it has no exception.</returns>
    public static MlResult<T> MapIfFailWithException<T>(this MlResult<T>        source,
                                                             Func<Exception, T> funcException)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(),
                                                                                                valid: ex              => funcException(ex).ToMlResultValid()
                                                                                            ),
                            valid: value         => value
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionAsync<T>(this MlResult<T>              source,
                                                                              Func<Exception, Task<T>> funcExceptionAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync().MatchAsync(
                                                                                                                            failAsync :       _  =>              errorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                            validAsync: async ex => await (await funcExceptionAsync(ex)).ToMlResultValidAsync<T>()
                                                                                                                        ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                              Func<Exception, Task<T>> funcExceptionAsync)
        => await (await sourceAsync).MapIfFailWithExceptionAsync(funcExceptionAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>  sourceAsync,
                                                                              Func<Exception, T> funcException)
        => (await sourceAsync).MapIfFailWithException(funcException);




    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithException<T>(this MlResult<T>             source,
                                                                Func<Exception, T>      funcException,
                                                                Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : _  => errorsDetails.ToMlResultFail<T>(),
                                                                                                valid: ex => funcException.TryToMlResult(ex, errorMessageBuilder)
                                                                                                                            .MergeErrorsDetailsIfFail(source)
                                                                                            ),
                            valid: value         => value
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithException<T>(this MlResult<T>        source,
                                                                Func<Exception, T> funcException,
                                                                string             errorMessage = null!)
        => source.TryMapIfFailWithException(funcException, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this MlResult<T>               source,
                                                                                 Func<Exception, Task<T>> funcExceptionAsync,
                                                                                 Func<Exception, string>  errorMessageBuilder)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync().MatchAsync(
                                                                                                                            failAsync : _  => errorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                            validAsync: ex => funcExceptionAsync.TryToMlResultAsync(ex, errorMessageBuilder)
                                                                                                                                                                .MergeErrorsDetailsIfFailAsync(source)
                                                                                                                        ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this MlResult<T>              source,
                                                                                 Func<Exception, Task<T>> funcExceptionAsync,
                                                                                 string                   errorMessage = null!)
        => await source.TryMapIfFailWithExceptionAsync(funcExceptionAsync, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                 Func<Exception, Task<T>> funcExceptionAsync,
                                                                                 Func<Exception, string>  errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithExceptionAsync(funcExceptionAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                 Func<Exception, Task<T>> funcExceptionAsync,
                                                                                 string                   errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithExceptionAsync(funcExceptionAsync, errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                                 Func<Exception, T>      funcException,
                                                                                 Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithException(funcException, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>  sourceAsync,
                                                                                 Func<Exception, T> funcException,
                                                                                 string             errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithException(funcException, errorMessage);







    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MapIfFailWithException<T, TResult>(this MlResult<T>              source,
                                                                            Func<T        , TResult> funcValid,
                                                                            Func<Exception, TResult> funcFail)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : _  => errorsDetails.ToMlResultFail<TResult>(),
                                                                                                valid: ex => funcFail(ex).ToMlResultValid()
                                                                                            ),
                            valid: x => funcValid(x).ToMlResultValid()
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult>(this MlResult<T>                    source,
                                                                                             Func<T        , Task<TResult>> funcValidAsync,
                                                                                             Func<Exception, Task<TResult>> funcFailAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetException().MatchAsync(
                                                                                                            failAsync :       _  =>              errorsDetails.ToMlResultFailAsync<TResult>(),
                                                                                                            validAsync: async ex => await (await funcFailAsync(ex)).ToMlResultValidAsync()
                                                                                                        ),
                                        validAsync: async x => await (await funcValidAsync(x)).ToMlResultValidAsync<TResult>()
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                             Func<T        , Task<TResult>> funcValidAsync,
                                                                                             Func<Exception, Task<TResult>> funcFailAsync)
        => await (await sourceAsync).MapIfFailWithExceptionAsync(funcValidAsync, funcFailAsync);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                             Func<T        , TResult>       funcValid,
                                                                                             Func<Exception, Task<TResult>> funcFailAsync)
        => await (await sourceAsync).MapIfFailWithExceptionAsync(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                             Func<T        , Task<TResult>> funcValidAsync,
                                                                                             Func<Exception, TResult>       funcFail)
        => await (await sourceAsync).MapIfFailWithExceptionAsync(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>        sourceAsync,
                                                                                             Func<T        , TResult> funcValid,
                                                                                             Func<Exception, TResult> funcFail)
        => (await sourceAsync).MapIfFailWithException(funcValid, funcFail);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithException<T, TResult>(this MlResult<T>              source,
                                                                               Func<T        , TResult> funcValid,
                                                                               Func<Exception, TResult> funcFail,
                                                                               Func<Exception, string>  errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : _  => errorsDetails.ToMlResultFail<TResult>(),
                                                                                                valid: ex => funcFail.TryToMlResult(ex, errorMessageBuilder)
                                                                                                                                  .MergeErrorsDetailsIfFailDifferentTypes(source)
                                                                                            ),
                            valid: x => funcValid.TryToMlResult(x, errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithException<T, TResult>(this MlResult<T>              source,
                                                                               Func<T        , TResult> funcValid,
                                                                               Func<Exception, TResult> funcFail,
                                                                               string                   errorMessage = null!)
        => source.TryMapIfFailWithException(funcValid, funcFail, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>        sourceAsync,
                                                                                                Func<T        , TResult> funcValid,
                                                                                                Func<Exception, TResult> funcFail,
                                                                                                Func<Exception, string>  errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithException(funcValid, funcFail, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>        sourceAsync,
                                                                                                Func<T        , TResult> funcValid,
                                                                                                Func<Exception, TResult> funcFail,
                                                                                                string                   errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithException(funcValid, funcFail, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , Task<TResult>> funcValidAsync,
                                                                                                Func<Exception, Task<TResult>> funcFailAsync,
                                                                                                Func<Exception, string>        errorMessageBuilder)
        => await sourceAsync.MatchAsync(
                            failAsync : errorsDetails => errorsDetails.GetExceptionAsync().MatchAsync(
                                                                                                failAsync :       _  =>              errorsDetails.ToMlResultFailAsync<TResult>(),
                                                                                                validAsync: async ex => await (await funcFailAsync(ex)).ToMlResultValidAsync()
                                                                                                                                                                    .MergeErrorsDetailsIfFailDifferentTypesAsync(sourceAsync)
                                                                                            ),
                            validAsync:  x => funcValidAsync.TryToMlResultAsync(x, errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , Task<TResult>> funcValidAsync,
                                                                                                Func<Exception, Task<TResult>> funcFailAsync,
                                                                                                string                         errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValidAsync, funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , TResult>       funcValid,
                                                                                                Func<Exception, Task<TResult>> funcFailAsync,
                                                                                                Func<Exception, string>        errorMessageBuilder)
        => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , TResult>       funcValid,
                                                                                                Func<Exception, Task<TResult>> funcFailAsync,
                                                                                                string                         errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , Task<TResult>> funcValidAsync,
                                                                                                Func<Exception, TResult>       funcFail,
                                                                                                Func<Exception, string>        errorMessageBuilder)
        => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , Task<TResult>> funcValidAsync,
                                                                                                Func<Exception, TResult>       funcFail,
                                                                                                string                         errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), _ => errorMessage!);


/// <summary>
/// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
/// contiene una excepción.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
/// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
/// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
/// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
public static MlResult<T> MapIfFailWithException<T, TException>(this MlResult<T>         source,
                                                                     Func<TException, T> funcException)
    where TException : Exception
    => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(),
                                                                                                            valid: ex              => funcException(ex).ToMlResultValid()
                                                                                                        ),
                            valid: value         => value
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionAsync<T, TException>(this MlResult<T>               source,
                                                                                          Func<TException, Task<T>> funcExceptionAsync)
        where TException : Exception
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync<TException>().MatchAsync(
                                                                                                                                        failAsync :       _  =>              errorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                                        validAsync: async ex => await (await funcExceptionAsync(ex)).ToMlResultValidAsync<T>()
                                                                                                                                    ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionAsync<T, TException>(this Task<MlResult<T>>         sourceAsync,
                                                                                          Func<TException, Task<T>> funcExceptionAsync)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithExceptionAsync<T, TException>(funcExceptionAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionAsync<T, TException>(this Task<MlResult<T>>   sourceAsync,
                                                                                          Func<TException, T> funcException)
        where TException : Exception
        => (await sourceAsync).MapIfFailWithException<T, TException>(funcException);




    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithException<T, TException>(this MlResult<T>              source,
                                                                            Func<TException, T>      funcException,
                                                                            Func<Exception, string>  errorMessageBuilder)
        where TException : Exception
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : _  => errorsDetails.ToMlResultFail<T>(),
                                                                                                            valid: ex => funcException.TryToMlResult(ex, errorMessageBuilder)
                                                                                                                                        .MergeErrorsDetailsIfFail(source)
                                                                                                        ),
                            valid: value         => value
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithException<T, TException>(this MlResult<T>         source,
                                                                            Func<TException, T> funcException,
                                                                            string              errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithException<T, TException>(funcException, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T, TException>(this MlResult<T>                source,
                                                                                             Func<TException, Task<T>> funcExceptionAsync,
                                                                                             Func<Exception, string>   errorMessageBuilder)
        where TException : Exception
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync<TException>().MatchAsync(
                                                                                                                                        failAsync : _  => errorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                                        validAsync: ex => funcExceptionAsync.TryToMlResultAsync(ex, errorMessageBuilder)
                                                                                                                                                                            .MergeErrorsDetailsIfFailAsync(source)
                                                                                                                                    ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T, TException>(this MlResult<T>               source,
                                                                                             Func<TException, Task<T>> funcExceptionAsync,
                                                                                             string                    errorMessage = null!)
        where TException : Exception
        => await source.TryMapIfFailWithExceptionAsync<T, TException>(funcExceptionAsync, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T, TException>(this Task<MlResult<T>>         sourceAsync,
                                                                                             Func<TException, Task<T>> funcExceptionAsync,
                                                                                             Func<Exception, string>   errorMessageBuilder)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithExceptionAsync<T, TException>(funcExceptionAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T, TException>(this Task<MlResult<T>>         sourceAsync,
                                                                                             Func<TException, Task<T>> funcExceptionAsync,
                                                                                             string                    errorMessage = null!)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithExceptionAsync<T, TException>(funcExceptionAsync, errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T, TException>(this Task<MlResult<T>>        sourceAsync,
                                                                                             Func<TException, T>      funcException,
                                                                                             Func<Exception, string>  errorMessageBuilder)
        where TException : Exception
        => (await sourceAsync).TryMapIfFailWithException<T, TException>(funcException, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T, TException>(this Task<MlResult<T>>   sourceAsync,
                                                                                             Func<TException, T> funcException,
                                                                                             string              errorMessage = null!)
        where TException : Exception
        => (await sourceAsync).TryMapIfFailWithException<T, TException>(funcException, errorMessage);







    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MapIfFailWithException<T, TResult, TException>(this MlResult<T>               source,
                                                                                        Func<T         , TResult> funcValid,
                                                                                        Func<TException, TResult> funcFail)
        where TException : Exception
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : _  => errorsDetails.ToMlResultFail<TResult>(),
                                                                                                            valid: ex => funcFail(ex).ToMlResultValid()
                                                                                                        ),
                            valid: x => funcValid(x).ToMlResultValid()
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult, TException>(this MlResult<T>                     source,
                                                                                                         Func<T         , Task<TResult>> funcValidAsync,
                                                                                                         Func<TException, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetException<TException>().MatchAsync(
                                                                                                                                    failAsync :       _  =>              errorsDetails.ToMlResultFailAsync<TResult>(),
                                                                                                                                    validAsync: async ex => await (await funcFailAsync(ex)).ToMlResultValidAsync()
                                                                                                                                ),
                                        validAsync: async x => await (await funcValidAsync(x)).ToMlResultValidAsync()
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>               sourceAsync,
                                                                                                         Func<T         , Task<TResult>> funcValidAsync,
                                                                                                         Func<TException, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithExceptionAsync<T, TResult, TException>(funcValidAsync, funcFailAsync);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>               sourceAsync,
                                                                                                         Func<T         , TResult>       funcValid,
                                                                                                         Func<TException, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>               sourceAsync,
                                                                                                         Func<T         , Task<TResult>> funcValidAsync,
                                                                                                         Func<TException, TResult>       funcFail)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>         sourceAsync,
                                                                                                         Func<T         , TResult> funcValid,
                                                                                                         Func<TException, TResult> funcFail)
        where TException : Exception
        => (await sourceAsync).MapIfFailWithException<T, TResult, TException>(funcValid, funcFail);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithException<T, TResult, TException>(this MlResult<T>               source,
                                                                                           Func<T         , TResult> funcValid,
                                                                                           Func<TException, TResult> funcFail,
                                                                                           Func<Exception, string>   errorMessageBuilder)
        where TException : Exception
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : _  => errorsDetails.ToMlResultFail<TResult>(),
                                                                                                            valid: ex => funcFail.TryToMlResult(ex, errorMessageBuilder)
                                                                                                                                              .MergeErrorsDetailsIfFailDifferentTypes(source)
                                                                                                        ),
                            valid: x => funcValid.TryToMlResult(x, errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithException<T, TResult, TException>(this MlResult<T>               source,
                                                                                           Func<T         , TResult> funcValid,
                                                                                           Func<TException, TResult> funcFail,
                                                                                           string                    errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithException<T, TResult, TException>(funcValid, funcFail, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>         sourceAsync,
                                                                                                            Func<T         , TResult> funcValid,
                                                                                                            Func<TException, TResult> funcFail,
                                                                                                            Func<Exception , string>  errorMessageBuilder)
        where TException : Exception
        => (await sourceAsync).TryMapIfFailWithException<T, TResult, TException>(funcValid, funcFail, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>         sourceAsync,
                                                                                                            Func<T         , TResult> funcValid,
                                                                                                            Func<TException, TResult> funcFail,
                                                                                                            string                    errorMessage = null!)
        where TException : Exception
        => await sourceAsync.TryMapIfFailWithExceptionAsync<T, TResult, TException>(funcValid, funcFail, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>               sourceAsync,
                                                                                                            Func<T         , Task<TResult>> funcValidAsync,
                                                                                                            Func<TException, Task<TResult>> funcFailAsync,
                                                                                                            Func<Exception , string>        errorMessageBuilder)
        where TException : Exception
        => await sourceAsync.MatchAsync(
                            failAsync : errorsDetails => errorsDetails.GetExceptionAsync<TException>().MatchAsync(
                                                                                                                            failAsync :       _  =>              errorsDetails.ToMlResultFailAsync<TResult>(),
                                                                                                                            validAsync: async ex => await (await funcFailAsync(ex)).ToMlResultValidAsync()
                                                                                                                                                                                                .MergeErrorsDetailsIfFailDifferentTypesAsync(sourceAsync)
                                                                                                                        ),
                            validAsync:  x => funcValidAsync.TryToMlResultAsync(x, errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>               sourceAsync,
                                                                                                            Func<T         , Task<TResult>> funcValidAsync,
                                                                                                            Func<TException, Task<TResult>> funcFailAsync,
                                                                                                            string                          errorMessage = null!)
        where TException : Exception
        => await sourceAsync.TryMapIfFailWithExceptionAsync<T, TResult, TException>(funcValidAsync, funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>               sourceAsync,
                                                                                                            Func<T         , TResult>       funcValid,
                                                                                                            Func<TException, Task<TResult>> funcFailAsync,
                                                                                                            Func<Exception , string>        errorMessageBuilder)
        where TException : Exception
        => await sourceAsync.TryMapIfFailWithExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>               sourceAsync,
                                                                                                            Func<T         , TResult>       funcValid,
                                                                                                            Func<TException, Task<TResult>> funcFailAsync,
                                                                                                            string                          errorMessage = null!)
        where TException : Exception
        => await sourceAsync.TryMapIfFailWithExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>               sourceAsync,
                                                                                                            Func<T         , Task<TResult>> funcValidAsync,
                                                                                                            Func<TException, TResult>       funcFail,
                                                                                                            Func<Exception, string>         errorMessageBuilder)
        where TException : Exception
        => await sourceAsync.TryMapIfFailWithExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>               sourceAsync,
                                                                                                            Func<T         , Task<TResult>> funcValidAsync,
                                                                                                            Func<TException, TResult>       funcFail,
                                                                                                            string                          errorMessage = null!)
        where TException : Exception
        => await sourceAsync.TryMapIfFailWithExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), _ => errorMessage!);







    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> MapIfFailWithExceptionError<T>(this MlResult<T>              source,
                                                             Func<MlErrorDetails, T> funcFail)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(),
                                                                                                valid: ex              => funcFail(errorsDetails).ToMlResultValid()
                                                                                            ),
                            valid: value         => value
                        );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionErrorAsync<T>(this MlResult<T>                    source,
                                                                              Func<MlErrorDetails, Task<T>> funcFailAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync().MatchAsync(
                                                                                                                            failAsync :       _  =>              errorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                            validAsync: async ex => await (await funcFailAsync(errorsDetails)).ToMlResultValidAsync<T>()
                                                                                                                        ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionErrorAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                              Func<MlErrorDetails, Task<T>> funcFailAsync)
        => await (await sourceAsync).MapIfFailWithExceptionErrorAsync(funcFailAsync);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionErrorAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                              Func<MlErrorDetails, T> funcFail)
        => (await sourceAsync).MapIfFailWithExceptionError(funcFail);




    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithExceptionError<T>(this MlResult<T>                   source,
                                                                Func<MlErrorDetails, T>      funcFail,
                                                                Func<Exception, string>       errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : _  => errorsDetails.ToMlResultFail<T>(),
                                                                                                valid: ex => funcFail.TryToMlResult(errorsDetails, errorMessageBuilder)
                                                                                                                            .MergeErrorsDetailsIfFail(source)
                                                                                            ),
                            valid: value         => value
                        );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithExceptionError<T>(this MlResult<T>              source,
                                                                Func<MlErrorDetails, T> funcFail,
                                                                string                   errorMessage = null!)
        => source.TryMapIfFailWithExceptionError(funcFail, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T>(this MlResult<T>                    source,
                                                                                 Func<MlErrorDetails, Task<T>> funcFailAsync,
                                                                                 Func<Exception, string>        errorMessageBuilder)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync().MatchAsync(
                                                                                                                            failAsync : _  => errorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                            validAsync: ex => funcFailAsync.TryToMlResultAsync(errorsDetails, errorMessageBuilder)
                                                                                                                                                                .MergeErrorsDetailsIfFailAsync(source)
                                                                                                                        ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T>(this MlResult<T>                    source,
                                                                                 Func<MlErrorDetails, Task<T>> funcFailAsync,
                                                                                 string                         errorMessage = null!)
        => await source.TryMapIfFailWithExceptionErrorAsync(funcFailAsync, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                 Func<MlErrorDetails, Task<T>> funcFailAsync,
                                                                                 Func<Exception, string>        errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithExceptionErrorAsync(funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                 Func<MlErrorDetails, Task<T>> funcFailAsync,
                                                                                 string                         errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithExceptionErrorAsync(funcFailAsync, errorMessage);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                 Func<MlErrorDetails, T> funcFail,
                                                                                 Func<Exception, string>  errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithExceptionError(funcFail, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                 Func<MlErrorDetails, T> funcFail,
                                                                                 string                   errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithExceptionError(funcFail, errorMessage);







    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MapIfFailWithExceptionError<T, TResult>(this MlResult<T>                    source,
                                                                            Func<T              , TResult> funcValid,
                                                                            Func<MlErrorDetails, TResult> funcFail)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : _  => errorsDetails.ToMlResultFail<TResult>(),
                                                                                                valid: ex => funcFail(errorsDetails).ToMlResultValid()
                                                                                            ),
                            valid: x => funcValid(x).ToMlResultValid()
                        );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult>(this MlResult<T>                          source,
                                                                                             Func<T              , Task<TResult>> funcValidAsync,
                                                                                             Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetException().MatchAsync(
                                                                                                            failAsync :       _ =>              errorsDetails.ToMlResultFailAsync<TResult>(),
                                                                                                            validAsync: async _ => await (await funcFailAsync(errorsDetails)).ToMlResultValidAsync()
                                                                                                        ),
                                        validAsync: async x => await (await funcValidAsync(x)).ToMlResultValidAsync<TResult>()
                                    );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                             Func<T              , Task<TResult>> funcValidAsync,
                                                                                             Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await (await sourceAsync).MapIfFailWithExceptionErrorAsync(funcValidAsync, funcFailAsync);


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                             Func<T              , TResult>       funcValid,
                                                                                             Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await (await sourceAsync).MapIfFailWithExceptionErrorAsync(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                             Func<T              , Task<TResult>> funcValidAsync,
                                                                                             Func<MlErrorDetails, TResult>       funcFail)
        => await (await sourceAsync).MapIfFailWithExceptionErrorAsync(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                             Func<T              , TResult> funcValid,
                                                                                             Func<MlErrorDetails, TResult> funcFail)
        => (await sourceAsync).MapIfFailWithExceptionError(funcValid, funcFail);


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithExceptionError<T, TResult>(this MlResult<T>                    source,
                                                                               Func<T              , TResult> funcValid,
                                                                               Func<MlErrorDetails, TResult> funcFail,
                                                                               Func<Exception, string>        errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : _ => errorsDetails.ToMlResultFail<TResult>(),
                                                                                                valid: _ => funcFail.TryToMlResult(errorsDetails, errorMessageBuilder)
                                                                                                                                  .MergeErrorsDetailsIfFailDifferentTypes(source)
                                                                                            ),
                            valid: x => funcValid.TryToMlResult(x, errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithExceptionError<T, TResult>(this MlResult<T>                    source,
                                                                               Func<T              , TResult> funcValid,
                                                                               Func<MlErrorDetails, TResult> funcFail,
                                                                               string                         errorMessage = null!)
        => source.TryMapIfFailWithExceptionError(funcValid, funcFail, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T              , TResult> funcValid,
                                                                                                Func<MlErrorDetails, TResult> funcFail,
                                                                                                Func<Exception, string>        errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithExceptionError(funcValid, funcFail, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T              , TResult> funcValid,
                                                                                                Func<MlErrorDetails, TResult> funcFail,
                                                                                                string                         errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithExceptionError(funcValid, funcFail, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                Func<Exception, string>              errorMessageBuilder)
        => await sourceAsync.MatchAsync(
                            failAsync : errorsDetails => errorsDetails.GetExceptionAsync().MatchAsync(
                                                                                                failAsync :       _ =>              errorsDetails.ToMlResultFailAsync<TResult>(),
                                                                                                validAsync: async _ => await (await funcFailAsync(errorsDetails)).ToMlResultValidAsync()
                                                                                                                                                                    .MergeErrorsDetailsIfFailDifferentTypesAsync(sourceAsync)
                                                                                            ),
                            validAsync:  x => funcValidAsync.TryToMlResultAsync(x, errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                string                               errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithExceptionErrorAsync(funcValidAsync, funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , TResult>       funcValid,
                                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                Func<Exception, string>              errorMessageBuilder)
        => await sourceAsync.TryMapIfFailWithExceptionErrorAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , TResult>       funcValid,
                                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                string                               errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithExceptionErrorAsync(funcValid.ToFuncTask(), funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                                Func<MlErrorDetails, TResult>       funcFail,
                                                                                                Func<Exception, string>              errorMessageBuilder)
        => await sourceAsync.TryMapIfFailWithExceptionErrorAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                                Func<MlErrorDetails, TResult>       funcFail,
                                                                                                string                               errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithExceptionErrorAsync(funcValidAsync, funcFail.ToFuncTask(), _ => errorMessage!);



/// <summary>
/// Transforma el valor de <c>IfFailWithExceptionError</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
/// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
/// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
/// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
public static MlResult<T> MapIfFailWithExceptionError<T, TException>(this MlResult<T>              source,
                                                                     Func<MlErrorDetails, T> funcFail)
        where TException : Exception
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(),
                                                                                                            valid: ex              => funcFail(errorsDetails).ToMlResultValid()
                                                                                                        ),
                            valid: value         => value
                        );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcExceptionAsync">Función asíncrona que recibe la excepción almacenada en el detalle del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionErrorAsync<T, TException>(this MlResult<T>                    source,
                                                                                          Func<MlErrorDetails, Task<T>> funcExceptionAsync)
        where TException : Exception
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync<TException>().MatchAsync(
                                                                                                                                        failAsync :       _ =>              errorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                                        validAsync: async _ => await (await funcExceptionAsync(errorsDetails)).ToMlResultValidAsync<T>()
                                                                                                                                    ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionErrorAsync<T, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                          Func<MlErrorDetails, Task<T>> funcFailAsync)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithExceptionErrorAsync<T, TException>(funcFailAsync);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithExceptionErrorAsync<T, TException>(this Task<MlResult<T>>        sourceAsync,
                                                                                          Func<MlErrorDetails, T> funcFail)
        where TException : Exception
        => (await sourceAsync).MapIfFailWithExceptionError<T, TException>(funcFail);




    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithExceptionError<T, TException>(this MlResult<T>              source,
                                                                            Func<MlErrorDetails, T> funcFail,
                                                                            Func<Exception, string>  errorMessageBuilder)
        where TException : Exception
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : _  => errorsDetails.ToMlResultFail<T>(),
                                                                                                            valid: ex => funcFail.TryToMlResult(errorsDetails, errorMessageBuilder)
                                                                                                                                        .MergeErrorsDetailsIfFail(source)
                                                                                                        ),
                            valid: value         => value
                        );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithExceptionError<T, TException>(this MlResult<T>              source,
                                                                            Func<MlErrorDetails, T> funcFail,
                                                                            string                   errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithExceptionError<T, TException>(funcFail, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T, TException>(this MlResult<T>                    source,
                                                                                             Func<MlErrorDetails, Task<T>> funcFailAsync,
                                                                                             Func<Exception, string>        errorMessageBuilder)
        where TException : Exception
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync<TException>().MatchAsync(
                                                                                                                                        failAsync : _ => errorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                                        validAsync: _ => funcFailAsync.TryToMlResultAsync(errorsDetails, errorMessageBuilder)
                                                                                                                                                                            .MergeErrorsDetailsIfFailAsync(source)
                                                                                                                                    ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T, TException>(this MlResult<T>                    source,
                                                                                             Func<MlErrorDetails, Task<T>> funcFailAsync,
                                                                                             string                         errorMessage = null!)
        where TException : Exception
        => await source.TryMapIfFailWithExceptionErrorAsync<T, TException>(funcFailAsync, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                             Func<MlErrorDetails, Task<T>> funcFailAsync,
                                                                                             Func<Exception, string>        errorMessageBuilder)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithExceptionErrorAsync<T, TException>(funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                             Func<MlErrorDetails, Task<T>> funcFailAsync,
                                                                                             string                         errorMessage = null!)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithExceptionErrorAsync<T, TException>(funcFailAsync, errorMessage);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T, TException>(this Task<MlResult<T>>        sourceAsync,
                                                                                             Func<MlErrorDetails, T> funcFail,
                                                                                             Func<Exception, string>  errorMessageBuilder)
        where TException : Exception
        => (await sourceAsync).TryMapIfFailWithExceptionError<T, TException>(funcFail, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T, TException>(this Task<MlResult<T>>        sourceAsync,
                                                                                             Func<MlErrorDetails, T> funcFail,
                                                                                             string                   errorMessage = null!)
        where TException : Exception
        => (await sourceAsync).TryMapIfFailWithExceptionError<T, TException>(funcFail, errorMessage);







    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MapIfFailWithExceptionError<T, TResult, TException>(this MlResult<T>                    source,
                                                                                        Func<T              , TResult> funcValid,
                                                                                        Func<MlErrorDetails, TResult> funcFail)
        where TException : Exception
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : _ => errorsDetails.ToMlResultFail<TResult>(),
                                                                                                            valid: _ => funcFail(errorsDetails).ToMlResultValid()
                                                                                                        ),
                            valid: x => funcValid(x).ToMlResultValid()
                        );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                         Func<T              , Task<TResult>> funcValidAsync,
                                                                                                         Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetException<TException>().MatchAsync(
                                                                                                                                    failAsync :       _ =>              errorsDetails.ToMlResultFailAsync<TResult>(),
                                                                                                                                    validAsync: async _ => await (await funcFailAsync(errorsDetails)).ToMlResultValidAsync()
                                                                                                                                ),
                                        validAsync: async x => await (await funcValidAsync(x)).ToMlResultValidAsync()
                                    );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                         Func<T              , Task<TResult>> funcValidAsync,
                                                                                                         Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValidAsync, funcFailAsync);


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                         Func<T              , TResult>       funcValid,
                                                                                                         Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                         Func<T              , Task<TResult>> funcValidAsync,
                                                                                                         Func<MlErrorDetails, TResult>       funcFail)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                                         Func<T              , TResult> funcValid,
                                                                                                         Func<MlErrorDetails, TResult> funcFail)
        where TException : Exception
        => (await sourceAsync).MapIfFailWithExceptionError<T, TResult, TException>(funcValid, funcFail);


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithExceptionError<T, TResult, TException>(this MlResult<T>                    source,
                                                                                           Func<T              , TResult> funcValid,
                                                                                           Func<MlErrorDetails, TResult> funcFail,
                                                                                           Func<Exception, string>        errorMessageBuilder)
        where TException : Exception
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : _ => errorsDetails.ToMlResultFail<TResult>(),
                                                                                                            valid: _ => funcFail.TryToMlResult(errorsDetails, errorMessageBuilder)
                                                                                                                                              .MergeErrorsDetailsIfFailDifferentTypes(source)
                                                                                                        ),
                            valid: x => funcValid.TryToMlResult(x, errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithExceptionError<T, TResult, TException>(this MlResult<T>                    source,
                                                                                           Func<T              , TResult> funcValid,
                                                                                           Func<MlErrorDetails, TResult> funcFail,
                                                                                           string                         errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithExceptionError<T, TResult, TException>(funcValid, funcFail, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                                            Func<T              , TResult> funcValid,
                                                                                                            Func<MlErrorDetails, TResult> funcFail,
                                                                                                            Func<Exception , string>       errorMessageBuilder)
        where TException : Exception
        => (await sourceAsync).TryMapIfFailWithExceptionError<T, TResult, TException>(funcValid, funcFail, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                                            Func<T              , TResult> funcValid,
                                                                                                            Func<MlErrorDetails, TResult> funcFail,
                                                                                                            string                         errorMessage = null!)
        where TException : Exception
        => (await sourceAsync).TryMapIfFailWithExceptionError<T, TResult, TException>(funcValid, funcFail, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                            Func<T              , Task<TResult>> funcValidAsync,
                                                                                                            Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                            Func<Exception , string>             errorMessageBuilder)
        where TException : Exception
        => await sourceAsync.MatchAsync(
                            failAsync : errorsDetails => errorsDetails.GetExceptionAsync<TException>().MatchAsync(
                                                                                                                            failAsync :       _ =>              errorsDetails.ToMlResultFailAsync<TResult>(),
                                                                                                                            validAsync: async _ => await (await funcFailAsync(errorsDetails)).ToMlResultValidAsync()
                                                                                                                                                                                                .MergeErrorsDetailsIfFailDifferentTypesAsync(sourceAsync)
                                                                                                                        ),
                            validAsync:  x => funcValidAsync.TryToMlResultAsync(x, errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                            Func<T              , Task<TResult>> funcValidAsync,
                                                                                                            Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                            string                               errorMessage = null!)
        where TException : Exception
        => await sourceAsync.TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValidAsync, funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                            Func<T              , TResult>       funcValid,
                                                                                                            Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                            Func<Exception , string>             errorMessageBuilder)
        where TException : Exception
        => await sourceAsync.TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                            Func<T              , TResult>       funcValid,
                                                                                                            Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                            string                               errorMessage = null!)
        where TException : Exception
        => await sourceAsync.TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                            Func<T              , Task<TResult>> funcValidAsync,
                                                                                                            Func<MlErrorDetails, TResult>       funcFail,
                                                                                                            Func<Exception, string>              errorMessageBuilder)
        where TException : Exception
        => await sourceAsync.TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                            Func<T              , Task<TResult>> funcValidAsync,
                                                                                                            Func<MlErrorDetails, TResult>       funcFail,
                                                                                                            string                               errorMessage = null!)
        where TException : Exception
        => await sourceAsync.TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), _ => errorMessage!);


    // Missing sync-source overloads for MapIfFailWithException<T>
    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapIfFailWithExceptionAsync<T>(this MlResult<T>        source,
                                                                        Func<Exception, T> funcException)
        => source.MapIfFailWithException(funcException).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this MlResult<T>             source,
                                                                           Func<Exception, T>      funcException,
                                                                           Func<Exception, string> errorMessageBuilder)
        => source.TryMapIfFailWithException(funcException, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this MlResult<T>        source,
                                                                           Func<Exception, T> funcException,
                                                                           string             errorMessage = null!)
        => source.TryMapIfFailWithException(funcException, errorMessage).ToAsync();

    // Missing sync-source overloads for MapIfFailWithException<T, TResult>
    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult>(this MlResult<T>              source,
                                                                                       Func<T        , TResult> funcValid,
                                                                                       Func<Exception, TResult> funcFail)
        => source.MapIfFailWithException(funcValid, funcFail).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult>(this MlResult<T>                    source,
                                                                                              Func<T        , Task<TResult>> funcValidAsync,
                                                                                              Func<Exception, TResult>       funcFail)
        => await source.MapIfFailWithExceptionAsync(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult>(this MlResult<T>                    source,
                                                                                              Func<T        , TResult>       funcValid,
                                                                                              Func<Exception, Task<TResult>> funcFailAsync)
        => await source.MapIfFailWithExceptionAsync(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this MlResult<T>              source,
                                                                                          Func<T        , TResult> funcValid,
                                                                                          Func<Exception, TResult> funcFail,
                                                                                          Func<Exception, string>  errorMessageBuilder)
        => source.TryMapIfFailWithException(funcValid, funcFail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this MlResult<T>              source,
                                                                                          Func<T        , TResult> funcValid,
                                                                                          Func<Exception, TResult> funcFail,
                                                                                          string                   errorMessage = null!)
        => source.TryMapIfFailWithException(funcValid, funcFail, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this MlResult<T>                    source,
                                                                                                 Func<T        , Task<TResult>> funcValidAsync,
                                                                                                 Func<Exception, TResult>       funcFail,
                                                                                                 Func<Exception, string>        errorMessageBuilder)
        => await source.ToAsync().TryMapIfFailWithExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this MlResult<T>                    source,
                                                                                                 Func<T        , Task<TResult>> funcValidAsync,
                                                                                                 Func<Exception, TResult>       funcFail,
                                                                                                 string                         errorMessage = null!)
        => await source.ToAsync().TryMapIfFailWithExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this MlResult<T>                    source,
                                                                                                 Func<T        , TResult>       funcValid,
                                                                                                 Func<Exception, Task<TResult>> funcFailAsync,
                                                                                                 Func<Exception, string>        errorMessageBuilder)
        => await source.ToAsync().TryMapIfFailWithExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult>(this MlResult<T>                    source,
                                                                                                 Func<T        , TResult>       funcValid,
                                                                                                 Func<Exception, Task<TResult>> funcFailAsync,
                                                                                                 string                         errorMessage = null!)
        => await source.ToAsync().TryMapIfFailWithExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    // Missing sync-source overloads for MapIfFailWithException<T, TException>
    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapIfFailWithExceptionAsync<T, TException>(this MlResult<T>         source,
                                                                                    Func<TException, T> funcException)
        where TException : Exception
        => source.MapIfFailWithException<T, TException>(funcException).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T, TException>(this MlResult<T>              source,
                                                                                        Func<TException, T>      funcException,
                                                                                        Func<Exception, string>  errorMessageBuilder)
        where TException : Exception
        => source.TryMapIfFailWithException<T, TException>(funcException, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcException">Función que recibe la excepción almacenada en el detalle del error.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T, TException>(this MlResult<T>         source,
                                                                                        Func<TException, T> funcException,
                                                                                        string              errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithException<T, TException>(funcException, errorMessage).ToAsync();

    // Missing sync-source overloads for MapIfFailWithException<T, TResult, TException>
    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult, TException>(this MlResult<T>               source,
                                                                                                   Func<T         , TResult> funcValid,
                                                                                                   Func<TException, TResult> funcFail)
        where TException : Exception
        => source.MapIfFailWithException<T, TResult, TException>(funcValid, funcFail).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult, TException>(this MlResult<T>                     source,
                                                                                                          Func<T         , Task<TResult>> funcValidAsync,
                                                                                                          Func<TException, TResult>       funcFail)
        where TException : Exception
        => await source.MapIfFailWithExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionAsync<T, TResult, TException>(this MlResult<T>                     source,
                                                                                                          Func<T         , TResult>       funcValid,
                                                                                                          Func<TException, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await source.MapIfFailWithExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this MlResult<T>               source,
                                                                                                       Func<T         , TResult> funcValid,
                                                                                                       Func<TException, TResult> funcFail,
                                                                                                       Func<Exception, string>   errorMessageBuilder)
        where TException : Exception
        => source.TryMapIfFailWithException<T, TResult, TException>(funcValid, funcFail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this MlResult<T>               source,
                                                                                                       Func<T         , TResult> funcValid,
                                                                                                       Func<TException, TResult> funcFail,
                                                                                                       string                    errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithException<T, TResult, TException>(funcValid, funcFail, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this MlResult<T>                     source,
                                                                                                             Func<T         , Task<TResult>> funcValidAsync,
                                                                                                             Func<TException, TResult>       funcFail,
                                                                                                             Func<Exception, string>         errorMessageBuilder)
        where TException : Exception
        => await source.ToAsync().TryMapIfFailWithExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this MlResult<T>                     source,
                                                                                                             Func<T         , Task<TResult>> funcValidAsync,
                                                                                                             Func<TException, TResult>       funcFail,
                                                                                                             string                          errorMessage = null!)
        where TException : Exception
        => await source.ToAsync().TryMapIfFailWithExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this MlResult<T>                     source,
                                                                                                             Func<T         , TResult>       funcValid,
                                                                                                             Func<TException, Task<TResult>> funcFailAsync,
                                                                                                             Func<Exception, string>         errorMessageBuilder)
        where TException : Exception
        => await source.ToAsync().TryMapIfFailWithExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionAsync<T, TResult, TException>(this MlResult<T>                     source,
                                                                                                             Func<T         , TResult>       funcValid,
                                                                                                             Func<TException, Task<TResult>> funcFailAsync,
                                                                                                             string                          errorMessage = null!)
        where TException : Exception
        => await source.ToAsync().TryMapIfFailWithExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    // Missing sync-source overloads for MapIfFailWithExceptionError<T>
    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapIfFailWithExceptionErrorAsync<T>(this MlResult<T>              source,
                                                                             Func<MlErrorDetails, T> funcFail)
        => source.MapIfFailWithExceptionError(funcFail).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T>(this MlResult<T>                   source,
                                                                                Func<MlErrorDetails, T>      funcFail,
                                                                                Func<Exception, string>       errorMessageBuilder)
        => source.TryMapIfFailWithExceptionError(funcFail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T>(this MlResult<T>              source,
                                                                                Func<MlErrorDetails, T> funcFail,
                                                                                string                   errorMessage = null!)
        => source.TryMapIfFailWithExceptionError(funcFail, errorMessage).ToAsync();

    // Missing sync-source overloads for MapIfFailWithExceptionError<T, TResult>
    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult>(this MlResult<T>                    source,
                                                                                            Func<T              , TResult> funcValid,
                                                                                            Func<MlErrorDetails, TResult> funcFail)
        => source.MapIfFailWithExceptionError(funcValid, funcFail).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                                   Func<MlErrorDetails, TResult>       funcFail)
        => await source.MapIfFailWithExceptionErrorAsync(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                   Func<T              , TResult>       funcValid,
                                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await source.MapIfFailWithExceptionErrorAsync(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this MlResult<T>                    source,
                                                                                               Func<T              , TResult> funcValid,
                                                                                               Func<MlErrorDetails, TResult> funcFail,
                                                                                               Func<Exception, string>        errorMessageBuilder)
        => source.TryMapIfFailWithExceptionError(funcValid, funcFail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this MlResult<T>                    source,
                                                                                               Func<T              , TResult> funcValid,
                                                                                               Func<MlErrorDetails, TResult> funcFail,
                                                                                               string                         errorMessage = null!)
        => source.TryMapIfFailWithExceptionError(funcValid, funcFail, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                      Func<T              , Task<TResult>> funcValidAsync,
                                                                                                      Func<MlErrorDetails, TResult>       funcFail,
                                                                                                      Func<Exception, string>              errorMessageBuilder)
        => await source.ToAsync().TryMapIfFailWithExceptionErrorAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                      Func<T              , Task<TResult>> funcValidAsync,
                                                                                                      Func<MlErrorDetails, TResult>       funcFail,
                                                                                                      string                               errorMessage = null!)
        => await source.ToAsync().TryMapIfFailWithExceptionErrorAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                      Func<T              , TResult>       funcValid,
                                                                                                      Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                      Func<Exception, string>              errorMessageBuilder)
        => await source.ToAsync().TryMapIfFailWithExceptionErrorAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                      Func<T              , TResult>       funcValid,
                                                                                                      Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                      string                               errorMessage = null!)
        => await source.ToAsync().TryMapIfFailWithExceptionErrorAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    // Missing sync-source overloads for MapIfFailWithExceptionError<T, TException>
    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapIfFailWithExceptionErrorAsync<T, TException>(this MlResult<T>              source,
                                                                                         Func<MlErrorDetails, T> funcFail)
        where TException : Exception
        => source.MapIfFailWithExceptionError<T, TException>(funcFail).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T, TException>(this MlResult<T>              source,
                                                                                             Func<MlErrorDetails, T> funcFail,
                                                                                             Func<Exception, string>  errorMessageBuilder)
        where TException : Exception
        => source.TryMapIfFailWithExceptionError<T, TException>(funcFail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithExceptionErrorAsync<T, TException>(this MlResult<T>              source,
                                                                                             Func<MlErrorDetails, T> funcFail,
                                                                                             string                   errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithExceptionError<T, TException>(funcFail, errorMessage).ToAsync();

    // Missing sync-source overloads for MapIfFailWithExceptionError<T, TResult, TException>
    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult, TException>(this MlResult<T>                    source,
                                                                                                        Func<T              , TResult> funcValid,
                                                                                                        Func<MlErrorDetails, TResult> funcFail)
        where TException : Exception
        => source.MapIfFailWithExceptionError<T, TResult, TException>(funcValid, funcFail).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                               Func<T              , Task<TResult>> funcValidAsync,
                                                                                                               Func<MlErrorDetails, TResult>       funcFail)
        where TException : Exception
        => await source.MapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithExceptionErrorAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                               Func<T              , TResult>       funcValid,
                                                                                                               Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await source.MapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this MlResult<T>                    source,
                                                                                                           Func<T              , TResult> funcValid,
                                                                                                           Func<MlErrorDetails, TResult> funcFail,
                                                                                                           Func<Exception, string>        errorMessageBuilder)
        where TException : Exception
        => source.TryMapIfFailWithExceptionError<T, TResult, TException>(funcValid, funcFail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this MlResult<T>                    source,
                                                                                                           Func<T              , TResult> funcValid,
                                                                                                           Func<MlErrorDetails, TResult> funcFail,
                                                                                                           string                         errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithExceptionError<T, TResult, TException>(funcValid, funcFail, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                                  Func<T              , Task<TResult>> funcValidAsync,
                                                                                                                  Func<MlErrorDetails, TResult>       funcFail,
                                                                                                                  Func<Exception, string>              errorMessageBuilder)
        where TException : Exception
        => await source.ToAsync().TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                                  Func<T              , Task<TResult>> funcValidAsync,
                                                                                                                  Func<MlErrorDetails, TResult>       funcFail,
                                                                                                                  string                               errorMessage = null!)
        where TException : Exception
        => await source.ToAsync().TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                                  Func<T              , TResult>       funcValid,
                                                                                                                  Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                                  Func<Exception, string>              errorMessageBuilder)
        where TException : Exception
        => await source.ToAsync().TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor de <c>IfFailWithExceptionError</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                                  Func<T              , TResult>       funcValid,
                                                                                                                  Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                                  string                               errorMessage = null!)
        where TException : Exception
        => await source.ToAsync().TryMapIfFailWithExceptionErrorAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, errorMessage);









    #endregion


    #region MapIfFailWithoutException


    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// The previous run cannot have raised an Exception. If this case occurs, the 'func' will not be executed
    /// </summary>
    /// <typeparam name="T">Value type of the source result.</typeparam>
    /// <param name="source">Source result.</param>
    /// <param name="func">Function executed when the failure details carry no exception.</param>
    /// <returns>A valid result with the mapped value, or the source failure if it carries an exception.</returns>
    public static MlResult<T> MapIfFailWithoutException<T>(this MlResult<T>              source,
                                                                Func<MlErrorDetails, T> func)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : _ => func(errorsDetails).ToMlResultValid<T>(),
                                                                                                valid: _ => errorsDetails
                                                                                            ),
                            valid: x             => x
                        );


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T>(this MlResult<T>                    source,
                                                                                 Func<MlErrorDetails, Task<T>> funcAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync().MatchAsync(
                                                                                                                            failAsync : async _ => (await funcAsync(errorsDetails)).ToMlResultValid(),
                                                                                                                            validAsync: async _ => await errorsDetails.ToMlResultFailAsync<T>()
                                                                                                                        ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                 Func<MlErrorDetails, Task<T>> funcAsync)
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync(funcAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                 Func<MlErrorDetails, T> func)
        => (await sourceAsync).MapIfFailWithoutException(func);




    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithoutException<T>(this MlResult<T>              source,
                                                                   Func<MlErrorDetails, T> func,
                                                                   Func<Exception, string>  errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : _ => func.TryToMlResult(errorsDetails, errorMessageBuilder),
                                                                                                valid: _ => errorsDetails
                                                                                            ),
                            valid: x             => x
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithoutException<T>(this MlResult<T>              source,
                                                                   Func<MlErrorDetails, T> func,
                                                                   string                   errorMessage = null!)
        => source.TryMapIfFailWithoutException(func, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this MlResult<T>                    source,
                                                                                    Func<MlErrorDetails, Task<T>> funcAsync,
                                                                                    Func<Exception, string>        errorMessageBuilder)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync().MatchAsync(
                                                                                                                            failAsync : _ => funcAsync.TryToMlResultAsync(errorsDetails, errorMessageBuilder),
                                                                                                                            validAsync: _ => errorsDetails.ToMlResultFailAsync<T>()
                                                                                                                        ),
                                        validAsync: x             => x.ToMlResultValidAsync()
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this MlResult<T>                    source,
                                                                                    Func<MlErrorDetails, Task<T>> funcAsync,
                                                                                    string                         errorMessage = null!)
        => await source.TryMapIfFailWithoutExceptionAsync(funcAsync, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                    Func<MlErrorDetails, Task<T>> funcAsync,
                                                                                    Func<Exception, string>        errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                    Func<MlErrorDetails, Task<T>> funcAsync,
                                                                                    string                         errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcAsync, errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                    Func<MlErrorDetails, T> func,
                                                                                    Func<Exception, string>  errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithoutException(func, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                    Func<MlErrorDetails, T> func,
                                                                                    string                   errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithoutException(func, errorMessage);





    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MapIfFailWithoutException<T, TResult>(this MlResult<T>                    source,
                                                                               Func<T              , TResult> funcValid,
                                                                               Func<MlErrorDetails, TResult> funcFail)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : _ => funcFail(errorsDetails).ToMlResultValid(),
                                                                                                valid: _ => errorsDetails
                                                                                            ),
                            valid: x => funcValid(x).ToMlResultValid()
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync().MatchAsync(
                                                                                                                            failAsync : async _ => (await funcFailAsync(errorsDetails)).ToMlResultValid(),
                                                                                                                            validAsync: async _ => await errorsDetails.ToMlResultFailAsync<TResult>()
                                                                                                                        ),
                                        validAsync: async x => (await funcValidAsync(x)).ToMlResultValid()
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , TResult>       funcValid,
                                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                                Func<MlErrorDetails, TResult>       funcFail)
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T              , TResult> funcValid,
                                                                                                Func<MlErrorDetails, TResult> funcFail)
        => (await sourceAsync).MapIfFailWithoutException(funcValid, funcFail);



    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithoutException<T, TResult>(this MlResult<T>                    source,
                                                                                  Func<T              , TResult> funcValid,
                                                                                  Func<MlErrorDetails, TResult> funcFail,
                                                                                  Func<Exception, string>        errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException().Match(
                                                                                                fail : _ => funcFail.TryToMlResult(errorsDetails, errorMessageBuilder),
                                                                                                valid: _ => errorsDetails
                                                                                            ),
                            valid: x             => funcValid.TryToMlResult(x, errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithoutException<T, TResult>(this MlResult<T>                    source,
                                                                                  Func<T              , TResult> funcValid,
                                                                                  Func<MlErrorDetails, TResult> funcFail,
                                                                                  string                         errorMessage = null!)
        => source.TryMapIfFailWithoutException(funcValid, funcFail, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync().MatchAsync(
                                                                                                                            failAsync : _ => funcFailAsync.TryToMlResultAsync(errorsDetails, errorMessageBuilder),
                                                                                                                            validAsync: _ => errorsDetails.ToMlResultFailAsync<TResult>()
                                                                                                                        ),
                                        validAsync: x             => funcValidAsync.TryToMlResultAsync(x, errorMessageBuilder)
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                   string                               errorMessage = null!)
        => await source.TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , TResult>       funcValid,
                                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , TResult>       funcValid,
                                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                                   Func<MlErrorDetails, TResult>       funcFail,
                                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , Task<TResult>> funcValidAsync,
                                                                                                   Func<MlErrorDetails, TResult>       funcFail,
                                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                                   Func<T              , TResult> funcValid,
                                                                                                   Func<MlErrorDetails, TResult> funcFail,
                                                                                                   Func<Exception, string>        errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithoutException(funcValid, funcFail, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                                                   Func<T              , TResult> funcValid,
                                                                                                   Func<MlErrorDetails, TResult> funcFail,
                                                                                                   string                         errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithoutException(funcValid, funcFail, _ => errorMessage!);






  /// <summary>
  /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
  /// contiene una excepción.
  /// </summary>
  /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
  /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
  /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
  /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
  /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
  public static MlResult<T> MapIfFailWithoutException<T, TException>(this MlResult<T>              source,
                                                                          Func<MlErrorDetails, T> func)
        where TException : Exception
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : _ => func(errorsDetails).ToMlResultValid<T>(),
                                                                                                            valid: _ => errorsDetails
                                                                                                        ),
                            valid: x             => x
                        );


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T, TException>(this MlResult<T>                    source,
                                                                                             Func<MlErrorDetails, Task<T>> funcAsync)
        where TException : Exception
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync<TException>().MatchAsync(
                                                                                                                                        failAsync : async _ => (await funcAsync(errorsDetails)).ToMlResultValid(),
                                                                                                                                        validAsync: async _ => await errorsDetails.ToMlResultFailAsync<T>()
                                                                                                                                    ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                             Func<MlErrorDetails, Task<T>> funcAsync)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync<T, TException>(funcAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T, TException>(this Task<MlResult<T>>        sourceAsync,
                                                                                             Func<MlErrorDetails, T> func)
        where TException : Exception
        => (await sourceAsync).MapIfFailWithoutException<T, TException>(func);




    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithoutException<T, TException>(this MlResult<T>              source,
                                                                               Func<MlErrorDetails, T> func,
                                                                               Func<Exception, string>  errorMessageBuilder)
        where TException : Exception
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : _ => func.TryToMlResult(errorsDetails, errorMessageBuilder),
                                                                                                            valid: _ => errorsDetails
                                                                                                        ),
                            valid: x             => x
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryMapIfFailWithoutException<T, TException>(this MlResult<T>              source,
                                                                               Func<MlErrorDetails, T> func,
                                                                               string                   errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithoutException<T, TException>(func, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T, TException>(this MlResult<T>                    source,
                                                                                                Func<MlErrorDetails, Task<T>> funcAsync,
                                                                                                Func<Exception, string>        errorMessageBuilder)
        where TException : Exception
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync<TException>().MatchAsync(
                                                                                                                                        failAsync : _ => funcAsync.TryToMlResultAsync(errorsDetails, errorMessageBuilder),
                                                                                                                                        validAsync: _ => errorsDetails.ToMlResultFailAsync<T>()
                                                                                                                                    ),
                                        validAsync: x             => x.ToMlResultValidAsync()
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T, TException>(this MlResult<T>                    source,
                                                                                                Func<MlErrorDetails, Task<T>> funcAsync,
                                                                                                string                         errorMessage = null!)
        where TException : Exception
        => await source.TryMapIfFailWithoutExceptionAsync<T, TException>(funcAsync, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<MlErrorDetails, Task<T>> funcAsync,
                                                                                                Func<Exception, string>        errorMessageBuilder)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync<T, TException>(funcAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAsync">Función asíncrona que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<MlErrorDetails, Task<T>> funcAsync,
                                                                                                string                         errorMessage = null!)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync<T, TException>(funcAsync, errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T, TException>(this Task<MlResult<T>>        sourceAsync,
                                                                                                Func<MlErrorDetails, T> func,
                                                                                                Func<Exception, string>  errorMessageBuilder)
        where TException : Exception
        => (await sourceAsync).TryMapIfFailWithoutException<T, TException>(func, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T, TException>(this Task<MlResult<T>>        sourceAsync,
                                                                                                Func<MlErrorDetails, T> func,
                                                                                                string                   errorMessage = null!)
        where TException : Exception
        => (await sourceAsync).TryMapIfFailWithoutException<T, TException>(func, errorMessage);





    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MapIfFailWithoutException<T, TResult, TException>(this MlResult<T>                    source,
                                                                                           Func<T              , TResult> funcValid,
                                                                                           Func<MlErrorDetails, TResult> funcFail)
        where TException : Exception
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : _ => funcFail(errorsDetails).ToMlResultValid(),
                                                                                                            valid: _ => errorsDetails
                                                                                                        ),
                            valid: x => funcValid(x).ToMlResultValid()
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                            Func<T              , Task<TResult>> funcValidAsync,
                                                                                                            Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync<TException>().MatchAsync(
                                                                                                                                        failAsync : async _ => (await funcFailAsync(errorsDetails)).ToMlResultValid(),
                                                                                                                                        validAsync: async _ => await errorsDetails.ToMlResultFailAsync<TResult>()
                                                                                                                                    ),
                                        validAsync: async x => (await funcValidAsync(x)).ToMlResultValid()
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                            Func<T              , Task<TResult>> funcValidAsync,
                                                                                                            Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValidAsync, funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                            Func<T              , TResult>       funcValid,
                                                                                                            Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                            Func<T              , Task<TResult>> funcValidAsync,
                                                                                                            Func<MlErrorDetails, TResult>       funcFail)
        where TException : Exception
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                                            Func<T              , TResult> funcValid,
                                                                                                            Func<MlErrorDetails, TResult> funcFail)
        where TException : Exception
        => (await sourceAsync).MapIfFailWithoutException<T, TResult, TException>(funcValid, funcFail);



    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithoutException<T, TResult, TException>(this MlResult<T>                    source,
                                                                                              Func<T              , TResult> funcValid,
                                                                                              Func<MlErrorDetails, TResult> funcFail,
                                                                                              Func<Exception, string>        errorMessageBuilder)
        where TException : Exception
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetException<TException>().Match(
                                                                                                            fail : _ => funcFail.TryToMlResult(errorsDetails, errorMessageBuilder),
                                                                                                            valid: _ => errorsDetails
                                                                                                        ),
                            valid: x             => funcValid.TryToMlResult(x, errorMessageBuilder)
                        );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapIfFailWithoutException<T, TResult, TException>(this MlResult<T>                    source,
                                                                                              Func<T              , TResult> funcValid,
                                                                                              Func<MlErrorDetails, TResult> funcFail,
                                                                                              string                         errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithoutException<T, TResult, TException>(funcValid, funcFail, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                               Func<T              , Task<TResult>> funcValidAsync,
                                                                                                               Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                               Func<Exception, string>              errorMessageBuilder)
        where TException : Exception
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync<TException>().MatchAsync(
                                                                                                                                        failAsync : _ => funcFailAsync.TryToMlResultAsync(errorsDetails, errorMessageBuilder),
                                                                                                                                        validAsync: _ => errorsDetails.ToMlResultFailAsync<TResult>()
                                                                                                                                    ),
                                        validAsync: x             => funcValidAsync.TryToMlResultAsync(x, errorMessageBuilder)
                                    );

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                               Func<T              , Task<TResult>> funcValidAsync,
                                                                                                               Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                               string                               errorMessage = null!)
        where TException : Exception
        => await source.TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValidAsync, funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                               Func<T              , Task<TResult>> funcValidAsync,
                                                                                                               Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                               Func<Exception, string>              errorMessageBuilder)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValidAsync, funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                               Func<T              , Task<TResult>> funcValidAsync,
                                                                                                               Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                               string                               errorMessage = null!)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValidAsync, funcFailAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                               Func<T              , TResult>       funcValid,
                                                                                                               Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                               Func<Exception, string>              errorMessageBuilder)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                               Func<T              , TResult>       funcValid,
                                                                                                               Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                               string                               errorMessage = null!)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                               Func<T              , Task<TResult>> funcValidAsync,
                                                                                                               Func<MlErrorDetails, TResult>       funcFail,
                                                                                                               Func<Exception, string>              errorMessageBuilder)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                               Func<T              , Task<TResult>> funcValidAsync,
                                                                                                               Func<MlErrorDetails, TResult>       funcFail,
                                                                                                               string                               errorMessage = null!)
        where TException : Exception
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                                               Func<T              , TResult> funcValid,
                                                                                                               Func<MlErrorDetails, TResult> funcFail,
                                                                                                               Func<Exception, string>        errorMessageBuilder)
        where TException : Exception
        => (await sourceAsync).TryMapIfFailWithoutException<T, TResult, TException>(funcValid, funcFail, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this Task<MlResult<T>>              sourceAsync,
                                                                                                               Func<T              , TResult> funcValid,
                                                                                                               Func<MlErrorDetails, TResult> funcFail,
                                                                                                               string                         errorMessage = null!)
        where TException : Exception
        => (await sourceAsync).TryMapIfFailWithoutException<T, TResult, TException>(funcValid, funcFail, _ => errorMessage!);


    // Missing sync-source overloads for MapIfFailWithoutException<T>
    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T>(this MlResult<T>              source,
                                                                           Func<MlErrorDetails, T> func)
        => source.MapIfFailWithoutException(func).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this MlResult<T>              source,
                                                                              Func<MlErrorDetails, T> func,
                                                                              Func<Exception, string>  errorMessageBuilder)
        => source.TryMapIfFailWithoutException(func, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this MlResult<T>              source,
                                                                              Func<MlErrorDetails, T> func,
                                                                              string                   errorMessage = null!)
        => source.TryMapIfFailWithoutException(func, errorMessage).ToAsync();

    // Missing sync-source overloads for MapIfFailWithoutException<T, TResult>
    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                    source,
                                                                                          Func<T              , TResult> funcValid,
                                                                                          Func<MlErrorDetails, TResult> funcFail)
        => source.MapIfFailWithoutException(funcValid, funcFail).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                 Func<T              , Task<TResult>> funcValidAsync,
                                                                                                 Func<MlErrorDetails, TResult>       funcFail)
        => await source.MapIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                 Func<T              , TResult>       funcValid,
                                                                                                 Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        => await source.MapIfFailWithoutExceptionAsync(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                    source,
                                                                                             Func<T              , TResult> funcValid,
                                                                                             Func<MlErrorDetails, TResult> funcFail,
                                                                                             Func<Exception, string>        errorMessageBuilder)
        => source.TryMapIfFailWithoutException(funcValid, funcFail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                    source,
                                                                                             Func<T              , TResult> funcValid,
                                                                                             Func<MlErrorDetails, TResult> funcFail,
                                                                                             string                         errorMessage = null!)
        => source.TryMapIfFailWithoutException(funcValid, funcFail, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                    Func<T              , Task<TResult>> funcValidAsync,
                                                                                                    Func<MlErrorDetails, TResult>       funcFail,
                                                                                                    Func<Exception, string>              errorMessageBuilder)
        => await source.TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                    Func<T              , Task<TResult>> funcValidAsync,
                                                                                                    Func<MlErrorDetails, TResult>       funcFail,
                                                                                                    string                               errorMessage = null!)
        => await source.TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                    Func<T              , TResult>       funcValid,
                                                                                                    Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                    Func<Exception, string>              errorMessageBuilder)
        => await source.TryMapIfFailWithoutExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult>(this MlResult<T>                          source,
                                                                                                    Func<T              , TResult>       funcValid,
                                                                                                    Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                    string                               errorMessage = null!)
        => await source.TryMapIfFailWithoutExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    // Missing sync-source overloads for MapIfFailWithoutException<T, TException>
    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T, TException>(this MlResult<T>              source,
                                                                                       Func<MlErrorDetails, T> func)
        where TException : Exception
        => source.MapIfFailWithoutException<T, TException>(func).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T, TException>(this MlResult<T>              source,
                                                                                           Func<MlErrorDetails, T> func,
                                                                                           Func<Exception, string>  errorMessageBuilder)
        where TException : Exception
        => source.TryMapIfFailWithoutException<T, TException>(func, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="func">Función que se ejecuta para producir el nuevo resultado.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T, TException>(this MlResult<T>              source,
                                                                                           Func<MlErrorDetails, T> func,
                                                                                           string                   errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithoutException<T, TException>(func, errorMessage).ToAsync();

    // Missing sync-source overloads for MapIfFailWithoutException<T, TResult, TException>
    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                    source,
                                                                                                      Func<T              , TResult> funcValid,
                                                                                                      Func<MlErrorDetails, TResult> funcFail)
        where TException : Exception
        => source.MapIfFailWithoutException<T, TResult, TException>(funcValid, funcFail).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                             Func<T              , Task<TResult>> funcValidAsync,
                                                                                                             Func<MlErrorDetails, TResult>       funcFail)
        where TException : Exception
        => await source.MapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask());

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                             Func<T              , TResult>       funcValid,
                                                                                                             Func<MlErrorDetails, Task<TResult>> funcFailAsync)
        where TException : Exception
        => await source.MapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                    source,
                                                                                                         Func<T              , TResult> funcValid,
                                                                                                         Func<MlErrorDetails, TResult> funcFail,
                                                                                                         Func<Exception, string>        errorMessageBuilder)
        where TException : Exception
        => source.TryMapIfFailWithoutException<T, TResult, TException>(funcValid, funcFail, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                    source,
                                                                                                         Func<T              , TResult> funcValid,
                                                                                                         Func<MlErrorDetails, TResult> funcFail,
                                                                                                         string                         errorMessage = null!)
        where TException : Exception
        => source.TryMapIfFailWithoutException<T, TResult, TException>(funcValid, funcFail, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                                                Func<MlErrorDetails, TResult>       funcFail,
                                                                                                                Func<Exception, string>              errorMessageBuilder)
        where TException : Exception
        => await source.TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAsync">Función asíncrona que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFail">Función que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                                Func<T              , Task<TResult>> funcValidAsync,
                                                                                                                Func<MlErrorDetails, TResult>       funcFail,
                                                                                                                string                               errorMessage = null!)
        where TException : Exception
        => await source.TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                                Func<T              , TResult>       funcValid,
                                                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                                Func<Exception, string>              errorMessageBuilder)
        where TException : Exception
        => await source.TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor unicamente cuando el resultado de origen es fallido y su detalle de error NO
    /// contiene una excepción. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <typeparam name="TException">Tipo de la excepción esperada.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValid">Función que se ejecuta cuando el resultado de origen es válido.</param>
    /// <param name="funcFailAsync">Función asíncrona que se ejecuta cuando el resultado de origen es fallido.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(this MlResult<T>                          source,
                                                                                                                Func<T              , TResult>       funcValid,
                                                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAsync,
                                                                                                                string                               errorMessage = null!)
        where TException : Exception
        => await source.TryMapIfFailWithoutExceptionAsync<T, TResult, TException>(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    #endregion


    #region MapAlways



    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAlways">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MapAlways<T, TResult>(this MlResult<T>   source, 
                                                               Func<TResult> funcAlways)
    {
        var partialResult = funcAlways();

        if (partialResult is null) return "The parameter 'funcAlways' returned null when executed.".ToMlResultFail<TResult>();

        var result = partialResult.ToMlResultValid();

        return result;
    }

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAlwaysAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this MlResult<T>         source, 
                                                                                Func<Task<TResult>> funcAlwaysAsync)
    {
        var partialResult = await funcAlwaysAsync();

        if (partialResult is null) return await "The parameter 'funcAlwaysAsync' returned null when executed.".ToMlResultFailAsync<TResult>();

        var result = await partialResult.ToMlResultValidAsync();

        return result;
    }


    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAlwaysAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this Task<MlResult<T>>   sourceAsync, 
                                                                                Func<Task<TResult>> funcAlwaysAsync)
    {
        var source = await sourceAsync;

        return await source.MapAlwaysAsync(funcAlwaysAsync);
    }

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAlways">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, 
                                                                                Func<TResult>     funcAlways)
        => (await sourceAsync).MapAlways(funcAlways);




    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAlways">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapAlways<T, TResult>(this MlResult<T>             source, 
                                                                  Func<TResult>           funcAlways,
                                                                  Func<Exception, string> errorMessageBuilder)
        => source.Match(valid: _            => funcAlways.TryToMlResult(errorMessageBuilder),
                        fail : errorDetails => funcAlways.TryToMlResult(errorDetails, errorMessageBuilder));

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAlways">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapAlways<T, TResult>(this MlResult<T>   source, 
                                                                  Func<TResult> funcAlways,
                                                                  string        errorMessage = null!)
        => TryMapAlways(source, funcAlways, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>             source, 
                                                                             Func<Task<TResult>>     funcAlwaysAsync,
                                                                             Func<Exception, string> errorMessageBuilder)
        => source.MatchAsync(validAsync: _            => funcAlwaysAsync.TryToMlResultAsync(errorMessageBuilder),
                             failAsync : errorDetails => funcAlwaysAsync.TryToMlResultAsync(errorDetails, errorMessageBuilder));

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>         source, 
                                                                             Func<Task<TResult>> funcAlwaysAsync,
                                                                             string              errorMessage = null!)
        => TryMapAlwaysAsync(source, funcAlwaysAsync, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync, 
                                                                                   Func<Task<TResult>>     funcAlwaysAsync,
                                                                                   Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapAlwaysAsync(funcAlwaysAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>   sourceAsync, 
                                                                                   Func<Task<TResult>> funcAlwaysAsync,
                                                                                   string              errorMessage = null!)
        => await (await sourceAsync).TryMapAlwaysAsync(funcAlwaysAsync, errorMessage);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAlways">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync, 
                                                                                   Func<TResult>           funcAlways,
                                                                                   Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryMapAlways(funcAlways, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcAlways">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, 
                                                                                   Func<TResult>     funcAlways,
                                                                                   string            errorMessage = null!)
        => (await sourceAsync).TryMapAlways(funcAlways, errorMessage);



    /// <summary>
    /// Ejecutará la acción, utilizando cada una de las funciones, según el estado del MlResult.
    /// Necesita 2 funciones:
    ///     1.- Función que se ejecutará si el MlResult es válido (facilita el value valido por parámetros)
    ///     2.- Función que se ejecutará si el MlResult es fallido (facilita el errorDetails por parámetros)
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MapAlways<T, TResult>(this MlResult<T>                    source, 
                                                               Func<T              , TResult> funcValidAlways,
                                                               Func<MlErrorDetails, TResult> funcFailAlways)
        => source.Match(valid: funcValidAlways,
                        fail : funcFailAlways);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this MlResult<T>                          source, 
                                                                                Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync)
        => await source.MatchAsync(validAsync: funcValidAlwaysAsync,
                                   failAsync : funcFailAlwaysAsync);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync)
        => await (await sourceAsync).MapAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , TResult>       funcValidAlways,
                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync)
        => await (await sourceAsync).MapAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                Func<MlErrorDetails, TResult>       funcFailAlways)
        => await (await sourceAsync).MapAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask());

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                                Func<T              , TResult> funcValidAlways,
                                                                                Func<MlErrorDetails, TResult> funcFailAlways)
        => (await sourceAsync).MapAlways(funcValidAlways, funcFailAlways);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapAlways<T, TResult>(this MlResult<T>                    source, 
                                                                  Func<T              , TResult> funcValidAlways,
                                                                  Func<MlErrorDetails, TResult> funcFailAlways,
                                                                  Func<Exception, string>        errorMessageBuilder)
        => source.Match(valid: value        => funcValidAlways.TryToMlResult(value       , errorMessageBuilder),
                        fail : errorDetails => funcFailAlways .TryToMlResult(errorDetails, errorMessageBuilder));

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryMapAlways<T, TResult>(this MlResult<T>                    source, 
                                                                  Func<T              , TResult> funcValidAlways,
                                                                  Func<MlErrorDetails, TResult> funcFailAlways,
                                                                  string                         errorMessage = null!)
        => source.TryMapAlways(funcValidAlways, funcFailAlways, _ => errorMessage!);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>                          source, 
                                                                             Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                             Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                             Func<Exception, string>              errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => funcValidAlwaysAsync.TryToMlResultAsync(value       , errorMessageBuilder),
                             failAsync : errorDetails => funcFailAlwaysAsync .TryToMlResultAsync(errorDetails, errorMessageBuilder));

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>                          source, 
                                                                             Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                             Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                             string                               errorMessage = null!)
        => source.TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync, errorMessage);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , TResult>       funcValidAlways,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , TResult>       funcValidAlways,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync, errorMessage);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                   Func<MlErrorDetails, TResult>       funcFailAlways,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                   Func<MlErrorDetails, TResult>       funcFailAlways,
                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                                   Func<T              , TResult> funcValidAlways,
                                                                                   Func<MlErrorDetails, TResult> funcFailAlways,
                                                                                   Func<Exception, string>        errorMessageBuilder)
        => (await sourceAsync).TryMapAlways(funcValidAlways, funcFailAlways, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                                   Func<T              , TResult> funcValidAlways,
                                                                                   Func<MlErrorDetails, TResult> funcFailAlways,
                                                                                   string                         errorMessage = null!)
        => (await sourceAsync).TryMapAlways(funcValidAlways, funcFailAlways, _ => errorMessage!);


    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAlways">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this MlResult<T>   source,
                                                                          Func<TResult> funcAlways)
        => source.MapAlways(funcAlways).ToAsync();

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAlways">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>             source,
                                                                             Func<TResult>           funcAlways,
                                                                             Func<Exception, string> errorMessageBuilder)
        => source.TryMapAlways(funcAlways, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcAlways">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>   source,
                                                                             Func<TResult> funcAlways,
                                                                             string        errorMessage = null!)
        => source.TryMapAlways(funcAlways, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this MlResult<T>                    source,
                                                                          Func<T              , TResult> funcValidAlways,
                                                                          Func<MlErrorDetails, TResult> funcFailAlways)
        => source.MapAlways(funcValidAlways, funcFailAlways).ToAsync();

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this MlResult<T>                          source,
                                                                                Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                Func<MlErrorDetails, TResult>       funcFailAlways)
        => await source.MapAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask());

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this MlResult<T>                          source,
                                                                                Func<T              , TResult>       funcValidAlways,
                                                                                Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync)
        => await source.MapAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>                    source,
                                                                             Func<T              , TResult> funcValidAlways,
                                                                             Func<MlErrorDetails, TResult> funcFailAlways,
                                                                             Func<Exception, string>        errorMessageBuilder)
        => source.TryMapAlways(funcValidAlways, funcFailAlways, errorMessageBuilder).ToAsync();

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>                    source,
                                                                             Func<T              , TResult> funcValidAlways,
                                                                             Func<MlErrorDetails, TResult> funcFailAlways,
                                                                             string                         errorMessage = null!)
        => source.TryMapAlways(funcValidAlways, funcFailAlways, errorMessage).ToAsync();

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>                          source,
                                                                                   Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                   Func<MlErrorDetails, TResult>       funcFailAlways,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await source.TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask(), errorMessageBuilder);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlwaysAsync">Función a aplicar.</param>
    /// <param name="funcFailAlways">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>                          source,
                                                                                   Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                   Func<MlErrorDetails, TResult>       funcFailAlways,
                                                                                   string                               errorMessage = null!)
        => await source.TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask(), errorMessage);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>                          source,
                                                                                   Func<T              , TResult>       funcValidAlways,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await source.TryMapAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync, errorMessageBuilder);

    /// <summary>
    /// Transforma el valor, ejecutando siempre la operación indicada con independencia del estado del
    /// resultado. Captura cualquier excepción producida y la convierte en un resultado fallido. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="funcValidAlways">Función a aplicar.</param>
    /// <param name="funcFailAlwaysAsync">Función a aplicar.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>                          source,
                                                                                   Func<T              , TResult>       funcValidAlways,
                                                                                   Func<MlErrorDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                                   string                               errorMessage = null!)
        => await source.TryMapAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync, errorMessage);



    #endregion


    #region Several

    



    #endregion



}

