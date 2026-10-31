// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Métodos de extensión de <see cref="MlResult{T}"/> que registran trazas a través de un <see cref="ILogger"/>.
/// </summary>
public static partial class MlResultLogExtensions
{


    #region LogMlResultError

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Error"/>.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<ILogger> LogMlResultError(this ILogger  logger,
                                                          string   message)
        => logger.LogMlResult(LogLevel.Error, message);

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Error"/>. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<ILogger>> LogMlResultErrorAsync(this ILogger  logger,
                                                                     string   message)
        => logger.LogMlResultAsync(LogLevel.Error, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Error"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultError<T>(this MlResult<T>                   source, 
                                                       ILogger                       logger,
                                                       Func<T, string>               validBuildMessage = null!,
                                                       Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Error, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Error"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultErrorAsync<T>(this MlResult<T>                   source, 
                                                                  ILogger                       logger,
                                                                  Func<T, string>               validBuildMessage = null!,
                                                                  Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Error, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Error"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuildMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Error, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Error"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultError<T>(this MlResult<T> source, 
                                                       ILogger     logger,
                                                       string      validMessage = null!,
                                                       string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Error, 
                                 validBuildMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Error"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      validMessage = null!,
                                                                        string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Error, 
                                            validBuildMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Error"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                        ILogger           logger,
                                                                        string            validMessage = null!,
                                                                        string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Error, 
                                                 validBuildMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Error"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultError<T>(this MlResult<T> source, 
                                                       ILogger     logger,
                                                       string      message)
        => source.LogMlResult<T>(logger, LogLevel.Error, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Error"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultErrorAsync<T>(this MlResult<T> source, 
                                                                  ILogger     logger,
                                                                  string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Error, message);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Error"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this Task<MlResult<T>> source, 
                                                                        ILogger           logger,
                                                                        string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Error, message);

    #endregion


    #region LogMlResultErrorIfValid

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfValid<T>(this MlResult<T> source,
                                                              ILogger     logger,
                                                              string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Error, validMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfValidAsync<T>(this MlResult<T> source,
                                                                         ILogger     logger,
                                                                         string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Error, validMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                               ILogger           logger,
                                                                               string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Error, validMessage);
    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfValid<T>(this MlResult<T>     source,
                                                              ILogger         logger,
                                                              Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Error, validBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfValidAsync<T>(this MlResult<T>     source,
                                                                         ILogger         logger,
                                                                         Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Error, validBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                               ILogger           logger,
                                                                               Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Error, validBuildMessage);

    #endregion


    #region LogMlResultErrorIfFail

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfFail<T>(this MlResult<T> source,
                                                             ILogger     logger,
                                                             string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Error, errorMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailAsync<T>(this MlResult<T> source,
                                                                        ILogger     logger,
                                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Error, errorMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                              ILogger           logger,
                                                                              string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Error, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfFail<T>(this MlResult<T>                   source,
                                                             ILogger                       logger,
                                                             Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailAsync<T>(this MlResult<T>                   source,
                                                                        ILogger                       logger,
                                                                        Func<MlErrorDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Error, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> LogMlResultErrorIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Error, failBuildMessage);

    #endregion


    #region LogMlResultErrorIfFailWithValue

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfFailWithValue<T>(this MlResult<T>                   source,
                                                                      ILogger                       logger,
                                                                      Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfFailWithValue<T>(this MlResult<T> source,
                                                                      ILogger     logger,
                                                                      string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Error, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                       ILogger                       logger,
                                                                                       Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                       ILogger                       logger,
                                                                                       Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                             ILogger           logger,
                                                                                             string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                              ILogger                               logger,
                                                                              Func<MlErrorDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                               ILogger                               logger,
                                                                                               Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                               ILogger                               logger,
                                                                                               Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, failBuildMessage);

    #endregion


    #region LogMlResultErrorIfFailWithException

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfFailWithException<T>(this MlResult<T>                   source,
                                                                          ILogger                       logger,
                                                                          Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfFailWithException<T>(this MlResult<T> source,
                                                                          ILogger     logger,
                                                                          string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Error, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                           ILogger                       logger,
                                                                                           Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                           ILogger     logger,
                                                                                           string      failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                           ILogger                       logger,
                                                                                           Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                           ILogger           logger,
                                                                                           string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfFailWithException<T>(this MlResult<T>                              source,
                                                                          ILogger                                  logger,
                                                                          Func<MlErrorDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                           ILogger                                  logger,
                                                                                           Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                           ILogger                                  logger,
                                                                                           Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    #endregion


    #region LogMlResultErrorIfFailWithoutException

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                             ILogger                       logger,
                                                                             Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultErrorIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                             ILogger                       logger,
                                                                             string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Error, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                              ILogger                       logger,
                                                                                              Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                              ILogger     logger,
                                                                                              string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Error, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                              ILogger                       logger,
                                                                                              Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                              ILogger           logger,
                                                                                              string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Error, failMessage);

    #endregion


    #region LogMlResultErrorTemplate

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Error"/>, pasando
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
    /// logger.LogMlResultErrorTemplate("hola a las {Hora}", DateTime.Now);
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultErrorTemplate(this ILogger      logger,
                                                                     string       messageTemplate,
                                                              params object?[]    args)
        => logger.LogMlResultTemplate(LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Error"/>, pasando
    /// la plantilla y sus argumentos por separado al registrador. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce un resultado válido que transporta el propio registrador.</returns>
    public static Task<MlResult<ILogger>> LogMlResultErrorTemplateAsync(this ILogger      logger,
                                                                               string       messageTemplate,
                                                                        params object?[]    args)
        => logger.LogMlResultTemplateAsync(LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Error"/>, pasando la plantilla y sus argumentos por separado.
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
    ///                     .LogMlResultErrorTemplate(logger, "cliente {ClienteId} consultado", 17);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultErrorTemplate<T>(this MlResult<T>      source,
                                                                   ILogger         logger,
                                                                   string          messageTemplate,
                                                            params object?[]       args)
        => source.LogMlResultTemplate(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Error"/>, pasando la plantilla y sus argumentos por separado.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultErrorTemplateAsync<T>(this MlResult<T>      source,
                                                                             ILogger         logger,
                                                                             string          messageTemplate,
                                                                      params object?[]       args)
        => source.LogMlResultTemplateAsync(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Error"/>, pasando la plantilla y sus argumentos por separado.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultErrorTemplateAsync<T>(this Task<MlResult<T>>      sourceAsync,
                                                                             ILogger                logger,
                                                                             string                 messageTemplate,
                                                                      params object?[]              args)
        => sourceAsync.LogMlResultTemplateAsync(logger, LogLevel.Error, messageTemplate, args);

    #endregion

    #region LogMlResultErrorIfFailTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultErrorIfFailTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultErrorIfFailTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailTemplate(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultErrorIfFailTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultErrorIfFailTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailTemplate(logger, LogLevel.Error, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailTemplateAsync(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailTemplateAsync(logger, LogLevel.Error, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailTemplateAsync(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailTemplateAsync(logger, LogLevel.Error, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultErrorIfFailWithValueTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultErrorIfFailWithValueTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultErrorIfFailWithValueTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailWithValueTemplate(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultErrorIfFailWithValueTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultErrorIfFailWithValueTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithValueTemplate(logger, LogLevel.Error, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithValueTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithValueTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Error, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithValueTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithValueTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Error, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultErrorIfFailWithExceptionTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultErrorIfFailWithExceptionTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultErrorIfFailWithExceptionTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailWithExceptionTemplate(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultErrorIfFailWithExceptionTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultErrorIfFailWithExceptionTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithExceptionTemplate(logger, LogLevel.Error, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Error, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Error, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultErrorIfFailWithoutExceptionTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultErrorIfFailWithoutExceptionTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultErrorIfFailWithoutExceptionTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailWithoutExceptionTemplate(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultErrorIfFailWithoutExceptionTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultErrorIfFailWithoutExceptionTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithoutExceptionTemplate(logger, LogLevel.Error, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Error, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Error, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Error</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Error, messageTemplate, argsBuilder);

    #endregion


}
