// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Métodos de extensión de <see cref="MlResult{T}"/> que registran trazas a través de un <see cref="ILogger"/>.
/// </summary>
public static partial class MlResultLogExtensions
{


    #region LogMlResultDebug

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Debug"/>.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<ILogger> LogMlResultDebug(this ILogger  logger,
                                                          string   message)
        => logger.LogMlResult(LogLevel.Debug, message);

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Debug"/>. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<ILogger>> LogMlResultDebugAsync(this ILogger  logger,
                                                                     string   message)
        => logger.LogMlResultAsync(LogLevel.Debug, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Debug"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebug<T>(this MlResult<T>                   source, 
                                                       ILogger                       logger,
                                                       Func<T, string>               validBuildMessage = null!,
                                                       Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Debug, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Debug"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultDebugAsync<T>(this MlResult<T>                   source, 
                                                                  ILogger                       logger,
                                                                  Func<T, string>               validBuildMessage = null!,
                                                                  Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Debug, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Debug"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuildMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Debug, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Debug"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebug<T>(this MlResult<T> source, 
                                                       ILogger     logger,
                                                       string      validMessage = null!,
                                                       string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Debug, 
                                 validBuildMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Debug"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      validMessage = null!,
                                                                        string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Debug, 
                                            validBuildMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Debug"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                        ILogger           logger,
                                                                        string            validMessage = null!,
                                                                        string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Debug, 
                                                 validBuildMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Debug"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebug<T>(this MlResult<T> source, 
                                                       ILogger     logger,
                                                       string      message)
        => source.LogMlResult<T>(logger, LogLevel.Debug, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Debug"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultDebugAsync<T>(this MlResult<T> source, 
                                                                  ILogger     logger,
                                                                  string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Debug, message);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Debug"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugAsync<T>(this Task<MlResult<T>> source, 
                                                                        ILogger           logger,
                                                                        string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Debug, message);

    #endregion


    #region LogMlResultDebugIfValid

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfValid<T>(this MlResult<T> source,
                                                              ILogger     logger,
                                                              string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Debug, validMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultDebugIfValidAsync<T>(this MlResult<T> source,
                                                                         ILogger     logger,
                                                                         string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Debug, validMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                               ILogger           logger,
                                                                               string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Debug, validMessage);
    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfValid<T>(this MlResult<T>     source,
                                                              ILogger         logger,
                                                              Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Debug, validBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultDebugIfValidAsync<T>(this MlResult<T>     source,
                                                                         ILogger         logger,
                                                                         Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Debug, validBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                               ILogger           logger,
                                                                               Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Debug, validBuildMessage);

    #endregion


    #region LogMlResultDebugIfFail

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Debug, errorMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultDebugIfFailAsync<T>(this MlResult<T> source,
                                                                        ILogger     logger,
                                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Debug, errorMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                              ILogger           logger,
                                                                              string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Debug, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfFail<T>(this MlResult<T>                   source,
                                                             ILogger                       logger,
                                                             Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultDebugIfFailAsync<T>(this MlResult<T>                   source,
                                                                        ILogger                       logger,
                                                                        Func<MlErrorDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Debug, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> LogMlResultDebugIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Debug, failBuildMessage);

    #endregion


    #region LogMlResultDebugIfFailWithValue

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfFailWithValue<T>(this MlResult<T>                   source,
                                                                      ILogger                       logger,
                                                                      Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfFailWithValue<T>(this MlResult<T> source,
                                                                      ILogger     logger,
                                                                      string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Debug, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                       ILogger                       logger,
                                                                                       Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                       ILogger                       logger,
                                                                                       Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                                    ILogger                               logger,
                                                                                    Func<MlErrorDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                               ILogger                               logger,
                                                                                               Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                               ILogger                               logger,
                                                                                               Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, failBuildMessage);

    #endregion


    #region LogMlResultDebugIfFailWithException

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfFailWithException<T>(this MlResult<T>                   source,
                                                                          ILogger                       logger,
                                                                          Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfFailWithException<T>(this MlResult<T> source,
                                                                          ILogger     logger,
                                                                          string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Debug, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                           ILogger                       logger,
                                                                                           Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                           ILogger     logger,
                                                                                           string      failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                           ILogger                       logger,
                                                                                           Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                           ILogger           logger,
                                                                                           string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfFailWithException<T>(this MlResult<T>                              source,
                                                                          ILogger                                  logger,
                                                                          Func<MlErrorDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                           ILogger                                  logger,
                                                                                           Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                           ILogger                                  logger,
                                                                                           Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    #endregion


    #region LogMlResultDebugIfFailWithoutException

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                            ILogger                       logger,
                                                                            Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultDebugIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                             ILogger                       logger,
                                                                             string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Debug, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                              ILogger                       logger,
                                                                                              Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                              ILogger     logger,
                                                                                              string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Debug, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                              ILogger                       logger,
                                                                                              Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Debug"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                              ILogger           logger,
                                                                                              string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Debug, failMessage);

    #endregion

}
