// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Métodos de extensión de <see cref="MlResult{T}"/> que registran trazas a través de un <see cref="ILogger"/>.
/// </summary>
public static partial class MlResultLogExtensions
{


    #region LogMlResultWarning

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Warning"/>.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<ILogger> LogMlResultWarning(this ILogger  logger,
                                                                string   message)
        => logger.LogMlResult(LogLevel.Warning, message);

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Warning"/>. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<ILogger>> LogMlResultWarningAsync(this ILogger  logger,
                                                                           string   message)
        => logger.LogMlResultAsync(LogLevel.Warning, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Warning"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarning<T>(this MlResult<T>                   source, 
                                                             ILogger                       logger,
                                                             Func<T, string>               validBuildMessage = null!,
                                                             Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Warning, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Warning"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningAsync<T>(this MlResult<T>                   source, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuildMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Warning, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Warning"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                              ILogger                       logger,
                                                                              Func<T, string>               validBuildMessage = null!,
                                                                              Func<MlErrorDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Warning, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Warning"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarning<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      validMessage = null!,
                                                             string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Warning, 
                                 validBuildMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Warning"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this MlResult<T> source, 
                                                                              ILogger     logger,
                                                                              string      validMessage = null!,
                                                                              string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Warning, 
                                            validBuildMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Warning"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                              ILogger           logger,
                                                                              string            validMessage = null!,
                                                                              string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Warning, 
                                                 validBuildMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Warning"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarning<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      message)
        => source.LogMlResult<T>(logger, LogLevel.Warning, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Warning"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Warning, message);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Warning"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>> source, 
                                                                              ILogger           logger,
                                                                              string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Warning, message);

    #endregion


    #region LogMlResultWarningIfValid

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfValid<T>(this MlResult<T> source,
                                                                    ILogger     logger,
                                                                    string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this MlResult<T> source,
                                                                               ILogger     logger,
                                                                               string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Warning, validMessage);
    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfValid<T>(this MlResult<T>     source,
                                                                    ILogger         logger,
                                                                    Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this MlResult<T>     source,
                                                                               ILogger         logger,
                                                                               Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Warning, validBuildMessage);

    #endregion


    #region LogMlResultWarningIfFail

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Warning, errorMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this MlResult<T> source,
                                                                              ILogger     logger,
                                                                              string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Warning, errorMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Warning, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFail<T>(this MlResult<T>                   source,
                                                                   ILogger                       logger,
                                                                   Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this MlResult<T>                   source,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Warning, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                    ILogger                       logger,
                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Warning, failBuildMessage);

    #endregion


    #region LogMlResultWarningIfFailWithValue

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithValue<T>(this MlResult<T>                   source,
                                                                            ILogger                       logger,
                                                                            Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithValue<T>(this MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Warning, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                             ILogger     logger,
                                                                                             string      failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                             ILogger           logger,
                                                                                             string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                                    ILogger                               logger,
                                                                                    Func<MlErrorDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    #endregion


    #region LogMlResultWarningIfFailWithException

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithException<T>(this MlResult<T>                   source,
                                                                                ILogger                       logger,
                                                                                Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithException<T>(this MlResult<T> source,
                                                                                ILogger     logger,
                                                                                string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Warning, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                                 ILogger     logger,
                                                                                                 string      failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                                 ILogger           logger,
                                                                                                 string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithException<T>(this MlResult<T>                              source,
                                                                                ILogger                                  logger,
                                                                                Func<MlErrorDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    #endregion


    #region LogMlResultWarningIfFailWithoutException

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Warning, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                                    ILogger     logger,
                                                                                                    string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Warning, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Warning"/> únicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                                    ILogger           logger,
                                                                                                    string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Warning, failMessage);

    #endregion


    #region LogMlResultWarningTemplate

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Warning"/>, pasando
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
    /// logger.LogMlResultWarningTemplate("hola a las {Hora}", DateTime.Now);
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultWarningTemplate(this ILogger      logger,
                                                                     [StringSyntax("MessageTemplate")]
                                                                     string       messageTemplate,
                                                              params object?[]    args)
        => logger.LogMlResultTemplate(LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Warning"/>, pasando
    /// la plantilla y sus argumentos por separado al registrador. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce un resultado válido que transporta el propio registrador.</returns>
    public static Task<MlResult<ILogger>> LogMlResultWarningTemplateAsync(this ILogger      logger,
                                                                               [StringSyntax("MessageTemplate")]
                                                                               string       messageTemplate,
                                                                        params object?[]    args)
        => logger.LogMlResultTemplateAsync(LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Warning"/>, pasando la plantilla y sus argumentos por separado.
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
    ///                     .LogMlResultWarningTemplate(logger, "cliente {ClienteId} consultado", 17);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultWarningTemplate<T>(this MlResult<T>      source,
                                                                   ILogger         logger,
                                                                   [StringSyntax("MessageTemplate")]
                                                                   string          messageTemplate,
                                                            params object?[]       args)
        => source.LogMlResultTemplate(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Warning"/>, pasando la plantilla y sus argumentos por separado.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultWarningTemplateAsync<T>(this MlResult<T>      source,
                                                                             ILogger         logger,
                                                                             [StringSyntax("MessageTemplate")]
                                                                             string          messageTemplate,
                                                                      params object?[]       args)
        => source.LogMlResultTemplateAsync(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Warning"/>, pasando la plantilla y sus argumentos por separado.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultWarningTemplateAsync<T>(this Task<MlResult<T>>      sourceAsync,
                                                                             ILogger                logger,
                                                                             [StringSyntax("MessageTemplate")]
                                                                             string                 messageTemplate,
                                                                      params object?[]              args)
        => sourceAsync.LogMlResultTemplateAsync(logger, LogLevel.Warning, messageTemplate, args);

    #endregion

    #region LogMlResultWarningIfFailTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultWarningIfFailTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultWarningIfFailTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            [StringSyntax("MessageTemplate")]
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailTemplate(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultWarningIfFailTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultWarningIfFailTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          [StringSyntax("MessageTemplate")]
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailTemplate(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       [StringSyntax("MessageTemplate")]
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailTemplateAsync(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     [StringSyntax("MessageTemplate")]
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailTemplateAsync(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       [StringSyntax("MessageTemplate")]
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailTemplateAsync(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     [StringSyntax("MessageTemplate")]
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailTemplateAsync(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultWarningIfFailWithValueTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultWarningIfFailWithValueTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultWarningIfFailWithValueTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            [StringSyntax("MessageTemplate")]
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailWithValueTemplate(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultWarningIfFailWithValueTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultWarningIfFailWithValueTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          [StringSyntax("MessageTemplate")]
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithValueTemplate(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithValueTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       [StringSyntax("MessageTemplate")]
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithValueTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     [StringSyntax("MessageTemplate")]
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithValueTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       [StringSyntax("MessageTemplate")]
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithValueTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     [StringSyntax("MessageTemplate")]
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailWithValueTemplateAsync(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultWarningIfFailWithExceptionTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultWarningIfFailWithExceptionTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultWarningIfFailWithExceptionTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            [StringSyntax("MessageTemplate")]
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailWithExceptionTemplate(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultWarningIfFailWithExceptionTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultWarningIfFailWithExceptionTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          [StringSyntax("MessageTemplate")]
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithExceptionTemplate(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       [StringSyntax("MessageTemplate")]
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     [StringSyntax("MessageTemplate")]
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       [StringSyntax("MessageTemplate")]
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     [StringSyntax("MessageTemplate")]
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailWithExceptionTemplateAsync(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultWarningIfFailWithoutExceptionTemplate

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultWarningIfFailWithoutExceptionTemplate(logger,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultWarningIfFailWithoutExceptionTemplate<T>(this   MlResult<T> source,
                                                                            ILogger     logger,
                                                                            [StringSyntax("MessageTemplate")]
                                                                            string      messageTemplate,
                                                                     params object?[]   args)
        => source.LogMlResultIfFailWithoutExceptionTemplate(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
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
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultWarningIfFailWithoutExceptionTemplate(logger,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultWarningIfFailWithoutExceptionTemplate<T>(this MlResult<T>                    source,
                                                                          ILogger                        logger,
                                                                          [StringSyntax("MessageTemplate")]
                                                                          string                         messageTemplate,
                                                                          Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithoutExceptionTemplate(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionTemplateAsync<T>(this   MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       [StringSyntax("MessageTemplate")]
                                                                                       string      messageTemplate,
                                                                                params object?[]   args)
        => source.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionTemplateAsync<T>(this MlResult<T>                    source,
                                                                                     ILogger                        logger,
                                                                                     [StringSyntax("MessageTemplate")]
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos los aporta el llamante. Versión
    /// asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       [StringSyntax("MessageTemplate")]
                                                                                       string            messageTemplate,
                                                                                params object?[]         args)
        => sourceAsync.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Warning, messageTemplate, args);

    /// <summary>
    /// Registra una traza de nivel <c>Warning</c> con plantilla de mensaje y campos
    /// estructurados, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                     ILogger                        logger,
                                                                                     [StringSyntax("MessageTemplate")]
                                                                                     string                         messageTemplate,
                                                                                     Func<MlErrorDetails, object?[]> argsBuilder)
        => sourceAsync.LogMlResultIfFailWithoutExceptionTemplateAsync(logger, LogLevel.Warning, messageTemplate, argsBuilder);

    #endregion


}
