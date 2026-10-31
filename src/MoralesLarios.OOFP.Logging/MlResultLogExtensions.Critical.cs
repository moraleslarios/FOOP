// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Net.Http.Headers;

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Métodos de extensión de <see cref="MlResult{T}"/> que registran trazas a través de un <see cref="ILogger"/>.
/// </summary>
public static partial class MlResultLogExtensions
{


    #region LogMlResultCritical

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Critical"/>.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<ILogger> LogMlResultCritical(this ILogger  logger,
                                                                string   message)
        => logger.LogMlResult(LogLevel.Critical, message);

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Critical"/>. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<ILogger>> LogMlResultCriticalAsync(this ILogger  logger,
                                                                           string   message)
        => logger.LogMlResultAsync(LogLevel.Critical, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Critical"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCritical<T>(this MlResult<T>                   source, 
                                                             ILogger                       logger,
                                                             Func<T, string>               validBuildMessage = null!,
                                                             Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Critical, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Critical"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalAsync<T>(this MlResult<T>                   source, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuildMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Critical, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Critical"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                              ILogger                       logger,
                                                                              Func<T, string>               validBuildMessage = null!,
                                                                              Func<MlErrorDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Critical, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Critical"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCritical<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      validMessage = null!,
                                                             string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Critical, 
                                 validBuildMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Critical"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalAsync<T>(this MlResult<T> source, 
                                                                              ILogger     logger,
                                                                              string      validMessage = null!,
                                                                              string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Critical, 
                                            validBuildMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Critical"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                              ILogger           logger,
                                                                              string            validMessage = null!,
                                                                              string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Critical, 
                                                 validBuildMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Critical"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCritical<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      message)
        => source.LogMlResult<T>(logger, LogLevel.Critical, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Critical"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Critical, message);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Critical"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalAsync<T>(this Task<MlResult<T>> source, 
                                                                              ILogger           logger,
                                                                              string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Critical, message);

    #endregion


    #region LogMlResultCriticalIfValid

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfValid<T>(this MlResult<T> source,
                                                                    ILogger     logger,
                                                                    string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Critical, validMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfValidAsync<T>(this MlResult<T> source,
                                                                               ILogger     logger,
                                                                               string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Critical, validMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Critical, validMessage);
    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfValid<T>(this MlResult<T>     source,
                                                                    ILogger         logger,
                                                                    Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Critical, validBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfValidAsync<T>(this MlResult<T>     source,
                                                                               ILogger         logger,
                                                                               Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Critical, validBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Critical, validBuildMessage);

    #endregion


    #region LogMlResultCriticalIfFail

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Critical, errorMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailAsync<T>(this MlResult<T> source,
                                                                              ILogger     logger,
                                                                              string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Critical, errorMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Critical, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfFail<T>(this MlResult<T>                   source,
                                                                   ILogger                       logger,
                                                                   Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailAsync<T>(this MlResult<T>                   source,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Critical, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> LogMlResultCriticalIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                    ILogger                       logger,
                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Critical, failBuildMessage);

    #endregion


    #region LogMlResultCriticalIfFailWithValue

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfFailWithValue<T>(this MlResult<T>                   source,
                                                                            ILogger                       logger,
                                                                            Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfFailWithValue<T>(this MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Critical, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                             ILogger     logger,
                                                                                             string      failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                             ILogger           logger,
                                                                                             string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                                    ILogger                               logger,
                                                                                    Func<MlErrorDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, failBuildMessage);

    #endregion


    #region LogMlResultCriticalIfFailWithException

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfFailWithException<T>(this MlResult<T>                   source,
                                                                                ILogger                       logger,
                                                                                Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfFailWithException<T>(this MlResult<T> source,
                                                                                ILogger     logger,
                                                                                string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Critical, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                                 ILogger     logger,
                                                                                                 string      failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                                 ILogger           logger,
                                                                                                 string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfFailWithException<T>(this MlResult<T>                              source,
                                                                                ILogger                                  logger,
                                                                                Func<MlErrorDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    #endregion


    #region LogMlResultCriticalIfFailWithoutException

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultCriticalIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Critical, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                                    ILogger     logger,
                                                                                                    string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Critical, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Critical"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                                    ILogger           logger,
                                                                                                    string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Critical, failMessage);

    #endregion


    #region LogMlResultCriticalTemplate

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Critical"/>, pasando
    /// la plantilla y sus argumentos por separado al registrador.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre · <c>"hola a las {Hora}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Resultado válido que transporta el propio registrador, para poder encadenar.</returns>
    /// <remarks>
    /// La plantilla <b>no se interpola</b>: llega intacta al registrador, que la publica como
    /// <c>{OriginalFormat}</c> y expone cada marcador como un campo consultable en el destino.
    /// </remarks>
    /// <example>
    /// <code>
    /// logger.LogMlResultCriticalTemplate("hola a las {Hora}", DateTime.Now);
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultCriticalTemplate(this ILogger      logger,
                                                                     string       messageTemplate,
                                                              params object?[]    args)
        => logger.LogMlResultTemplate(LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Critical"/>, pasando
    /// la plantilla y sus argumentos por separado al registrador. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce un resultado válido que transporta el propio registrador.</returns>
    public static Task<MlResult<ILogger>> LogMlResultCriticalTemplateAsync(this ILogger      logger,
                                                                               string       messageTemplate,
                                                                        params object?[]    args)
        => logger.LogMlResultTemplateAsync(LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Critical"/>, pasando la plantilla y sus argumentos por separado.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>El resultado de origen, inalterado.</returns>
    /// <remarks>
    /// La traza se emite siempre, con independencia de que el resultado sea válido o fallido, y el
    /// resultado de origen se devuelve intacto.
    /// </remarks>
    /// <example>
    /// <code>
    /// var resultado = ObtenerCliente(17)
    ///                     .LogMlResultCriticalTemplate(logger, "cliente {ClienteId} consultado", 17);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultCriticalTemplate<T>(this MlResult<T>      source,
                                                                   ILogger         logger,
                                                                   string          messageTemplate,
                                                            params object?[]       args)
        => source.LogMlResultTemplate(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Critical"/>, pasando la plantilla y sus argumentos por separado.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalTemplateAsync<T>(this MlResult<T>      source,
                                                                             ILogger         logger,
                                                                             string          messageTemplate,
                                                                      params object?[]       args)
        => source.LogMlResultTemplateAsync(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Critical"/>, pasando la plantilla y sus argumentos por separado.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalTemplateAsync<T>(this Task<MlResult<T>>      sourceAsync,
                                                                             ILogger                logger,
                                                                             string                 messageTemplate,
                                                                      params object?[]              args)
        => sourceAsync.LogMlResultTemplateAsync(logger, LogLevel.Critical, messageTemplate, args);

    #endregion

    #region LogMlResultCriticalIfFailTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos los aporta el llamante.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultCriticalIfFailTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultCriticalIfFailTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailTemplate(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos se derivan del detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultCriticalIfFailTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultCriticalIfFailTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailTemplate(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailTemplateAsync(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailTemplateAsync(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailTemplateAsync(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailTemplateAsync(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultCriticalIfFailWithValueTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos los aporta el llamante.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultCriticalIfFailWithValueTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultCriticalIfFailWithValueTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailWithValueTemplate(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos se derivan del detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultCriticalIfFailWithValueTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultCriticalIfFailWithValueTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithValueTemplate(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithValueTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithValueTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithValueTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithValueTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultCriticalIfFailWithExceptionTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos los aporta el llamante.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultCriticalIfFailWithExceptionTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultCriticalIfFailWithExceptionTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailWithExceptionTemplate(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos se derivan del detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultCriticalIfFailWithExceptionTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultCriticalIfFailWithExceptionTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithExceptionTemplate(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultCriticalIfFailWithoutExceptionTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos los aporta el llamante.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultCriticalIfFailWithoutExceptionTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultCriticalIfFailWithoutExceptionTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailWithoutExceptionTemplate(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos se derivan del detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultCriticalIfFailWithoutExceptionTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultCriticalIfFailWithoutExceptionTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithoutExceptionTemplate(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Critical, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Critical</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Critical, messageTemplate, argsBuilder);

    #endregion


}
