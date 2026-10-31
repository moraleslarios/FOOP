// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Métodos de extensión de <see cref="MlResult{T}"/> que registran trazas a través de un <see cref="ILogger"/>.
/// </summary>
public static partial class MlResultLogExtensions
{


    #region LogMlResultCritical

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Trace"/>.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<ILogger> LogMlResultTrace(this ILogger  logger,
                                                                string   message)
        => logger.LogMlResult(LogLevel.Trace, message);

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Trace"/>. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<ILogger>> LogMlResultTraceAsync(this ILogger  logger,
                                                                           string   message)
        => logger.LogMlResultAsync(LogLevel.Trace, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Trace"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTrace<T>(this MlResult<T>                   source, 
                                                             ILogger                       logger,
                                                             Func<T, string>               validBuildMessage = null!,
                                                             Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Trace, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Trace"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultTraceAsync<T>(this MlResult<T>                   source, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuildMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Trace, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Trace"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                              ILogger                       logger,
                                                                              Func<T, string>               validBuildMessage = null!,
                                                                              Func<MlErrorDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Trace, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Trace"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTrace<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      validMessage = null!,
                                                             string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Trace, 
                                 validBuildMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Trace"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this MlResult<T> source, 
                                                                              ILogger     logger,
                                                                              string      validMessage = null!,
                                                                              string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Trace, 
                                            validBuildMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Trace"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                              ILogger           logger,
                                                                              string            validMessage = null!,
                                                                              string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Trace, 
                                                 validBuildMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Trace"/>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTrace<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      message)
        => source.LogMlResult<T>(logger, LogLevel.Trace, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Trace"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultTraceAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Trace, message);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel <see cref="LogLevel.Trace"/>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this Task<MlResult<T>> source, 
                                                                              ILogger           logger,
                                                                              string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Trace, message);

    #endregion


    #region LogMlResultTraceIfValid

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfValid<T>(this MlResult<T> source,
                                                                    ILogger     logger,
                                                                    string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Trace, validMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultTraceIfValidAsync<T>(this MlResult<T> source,
                                                                               ILogger     logger,
                                                                               string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Trace, validMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Trace, validMessage);
    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfValid<T>(this MlResult<T>     source,
                                                                    ILogger         logger,
                                                                    Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Trace, validBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultTraceIfValidAsync<T>(this MlResult<T>     source,
                                                                               ILogger         logger,
                                                                               Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Trace, validBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Trace, validBuildMessage);

    #endregion


    #region LogMlResultTraceIfFail

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Trace, errorMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultTraceIfFailAsync<T>(this MlResult<T> source,
                                                                              ILogger     logger,
                                                                              string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Trace, errorMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Trace, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfFail<T>(this MlResult<T>                   source,
                                                                   ILogger                       logger,
                                                                   Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultTraceIfFailAsync<T>(this MlResult<T>                   source,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Trace, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> LogMlResultTraceIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                    ILogger                       logger,
                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Trace, failBuildMessage);

    #endregion


    #region LogMlResultTraceIfFailWithValue

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfFailWithValue<T>(this MlResult<T>                   source,
                                                                            ILogger                       logger,
                                                                            Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfFailWithValue<T>(this MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Trace, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                             ILogger     logger,
                                                                                             string      failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                             ILogger           logger,
                                                                                             string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                                    ILogger                               logger,
                                                                                    Func<MlErrorDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, failBuildMessage);

    #endregion


    #region LogMlResultTraceIfFailWithException

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfFailWithException<T>(this MlResult<T>                   source,
                                                                                ILogger                       logger,
                                                                                Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfFailWithException<T>(this MlResult<T> source,
                                                                                ILogger     logger,
                                                                                string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Trace, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                                 ILogger     logger,
                                                                                                 string      failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                                 ILogger           logger,
                                                                                                 string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfFailWithException<T>(this MlResult<T>                              source,
                                                                                ILogger                                  logger,
                                                                                Func<MlErrorDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    #endregion


    #region LogMlResultTraceIfFailWithoutException

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultTraceIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Trace, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                                    ILogger     logger,
                                                                                                    string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Trace, failMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel <see cref="LogLevel.Trace"/> únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                                    ILogger           logger,
                                                                                                    string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Trace, failMessage);

    #endregion

}
