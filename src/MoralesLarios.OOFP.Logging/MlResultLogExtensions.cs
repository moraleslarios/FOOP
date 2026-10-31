// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Métodos de extensión de <see cref="MlResult{T}"/> que registran trazas a través de un <see cref="ILogger"/>.
/// </summary>
public static partial class MlResultLogExtensions
{


    #region LogMlResult

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel que se indique.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<ILogger> LogMlResult(this ILogger  logger, 
                                                     LogLevel logLevel, 
                                                     string   message)
    {
        if (logger  is null) return MlResultLogGuards.NullLoggerFail();
        if (message is null) return logger.ToMlResultValid();

        logger.Log(logLevel, message);

        return logger.ToMlResultValid();
    }

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<ILogger>> LogMlResultAsync(this ILogger  logger, 
                                                                LogLevel logLevel, 
                                                                string   message)
    {
        if (logger  is null) return MlResultLogGuards.NullLoggerFailAsync();
        if (message is null) return logger.ToMlResultValidAsync();

        logger.Log(logLevel, message);

        return logger.ToMlResultValidAsync();
    }

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    /// <remarks>
    /// Este método no lanza excepciones: si <paramref name="logger"/> es nulo se devuelve el resultado de origen sin registrar nada.
    /// </remarks>
    /// <example>
    /// <code>
    /// // 'origen' es cualquier MlResult&lt;int&gt; y 'logger' un ILogger ya disponible.
    /// MlResult&lt;int&gt; resultado = origen.LogMlResult(logger,
    ///                                              logLevel         : LogLevel.Information,
    ///                                              validBuildMessage: valor  =&gt; $"Valor calculado: {valor}",
    ///                                              failBuildMessage : errors =&gt; $"Fallo al calcular: {errors.GetErrorsSummary()}");
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResult<T>(this MlResult<T>                   source, 
                                                  ILogger                       logger, 
                                                  LogLevel                      logLevel, 
                                                  Func<T, string>               validBuildMessage = null!,
                                                  Func<MlErrorDetails, string> failBuildMessage = null!)
    {
        if (! MlResultLogGuards.IsEnabled(logger, logLevel)) return source;

        source.Match(
                        valid: x      => validBuildMessage != null ? logger.LogMlResult(logLevel, validBuildMessage(x))    : null!,
                        fail : errors => failBuildMessage != null ? logger.LogMlResult(logLevel, failBuildMessage(errors)) : null!
                    );

        return source;
    }

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T>                   source, 
                                                             ILogger                       logger, 
                                                             LogLevel                      logLevel, 
                                                             Func<T, string>               validBuildMessage = null!,
                                                             Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, logLevel, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                   ILogger                       logger, 
                                                                   LogLevel                      logLevel, 
                                                                   Func<T, string>               validBuildMessage = null!,
                                                                   Func<MlErrorDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, logLevel, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResult<T>(this MlResult<T> source, 
                                                  ILogger     logger, 
                                                  LogLevel    logLevel, 
                                                  string      validMessage = null!,
                                                  string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 logLevel, 
                                 validBuildMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T> source, 
                                                                   ILogger     logger, 
                                                                   LogLevel    logLevel, 
                                                                   string      validMessage = null!,
                                                                   string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            logLevel, 
                                            validBuildMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                   ILogger           logger, 
                                                                   LogLevel          logLevel, 
                                                                   string            validMessage = null!,
                                                                   string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 logLevel, 
                                                 validBuildMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResult<T>(this MlResult<T> source, 
                                                  ILogger     logger, 
                                                  LogLevel    logLevel, 
                                                  string      message)
    {
        if (logger is null || message is null) return source;

        logger.Log(logLevel, message);

        return source;
    }

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T> source, 
                                                             ILogger     logger, 
                                                             LogLevel    logLevel, 
                                                             string      message)
    {
        if (logger is null || message is null) return source.ToAsync();

        logger.Log(logLevel, message);

        return source.ToAsync();
    }

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea con el nivel que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual, sin construirlo a partir del resultado. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>> source, 
                                                                   ILogger           logger, 
                                                                   LogLevel          logLevel, 
                                                                   string            message)
    {
        if (logger is null || message is null) return await source;

        logger.Log(logLevel, message);

        return await source;
    }

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResult<T>(this MlResult<T>                                                     source, 
                                                  ILogger                                                         logger, 
                                                  (LogLevel logLevel, Func<T              , string> buildMessage) validBuildMessage,
                                                  (LogLevel logLevel, Func<MlErrorDetails, string> buildMessage) failBuildMessage)
    {
        if (logger is null) return source;

        source.Match(
                        valid: x      => logger.LogMlResult(validBuildMessage.logLevel, MlResultLogGuards.Build(logger, validBuildMessage.logLevel, validBuildMessage.buildMessage, x)),
                        fail : errors => logger.LogMlResult(failBuildMessage .logLevel, MlResultLogGuards.Build(logger, failBuildMessage .logLevel, failBuildMessage.buildMessage, errors))
                    );

        return source;
    }

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T>                                                     source, 
                                                             ILogger                                                         logger, 
                                                             (LogLevel logLevel, Func<T              , string> buildMessage) validBuildMessage,
                                                             (LogLevel logLevel, Func<MlErrorDetails, string> buildMessage) failBuildMessage)
        => source.LogMlResult(logger, validBuildMessage, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>>                                               sourceAsync, 
                                                                   ILogger                                                         logger, 
                                                                   (LogLevel logLevel, Func<T              , string> buildMessage) validBuildMessage,
                                                                   (LogLevel logLevel, Func<MlErrorDetails, string> buildMessage) failBuildMessage)
        => (await sourceAsync).LogMlResult(logger, validBuildMessage, failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Information"/> si es válido y <see cref="LogLevel.Error"/> si es fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    /// <remarks>
    /// Este método no lanza excepciones: si <paramref name="logger"/> es nulo se devuelve el resultado de origen sin registrar nada.
    /// </remarks>
    /// <example>
    /// <code>
    /// // 'origen' es cualquier MlResult&lt;int&gt; y 'logger' un ILogger ya disponible.
    /// MlResult&lt;int&gt; resultado = origen.LogCompletedResult(logger,
    ///                                                     validBuildMessage: valor  =&gt; $"Operación completada con {valor}",
    ///                                                     failBuildMessage : errors =&gt; $"Operación fallida: {errors.GetErrorsSummary()}");
    /// </code>
    /// </example>
    public static MlResult<T> LogCompletedResult<T>(this MlResult<T>                   source,
                                                       ILogger                       logger,
                                                       Func<T, string>               validBuildMessage,
                                                       Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResult(logger,
                              (LogLevel.Information, validBuildMessage),
                              (LogLevel.Error      , failBuildMessage));

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Information"/> si es válido y <see cref="LogLevel.Error"/> si es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogCompletedResultAsync<T>(this MlResult<T>                   source,
                                                                  ILogger                       logger,
                                                                  Func<T, string>               validBuildMessage,
                                                                  Func<MlErrorDetails, string> failBuildMessage)
        => source.LogCompletedResult(logger, validBuildMessage, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Information"/> si es válido y <see cref="LogLevel.Error"/> si es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogCompletedResultAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuildMessage,
                                                                        Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogCompletedResultAsync(logger, validBuildMessage, failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Information"/> si es válido y <see cref="LogLevel.Error"/> si es fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogCompletedResult<T>(this MlResult<T>  source,
                                                       ILogger      logger,
                                                       string       validMessage,
                                                       string       failMessage)
        => source.LogCompletedResult(logger,
                                   _ => validMessage,
                                   _ => failMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Information"/> si es válido y <see cref="LogLevel.Error"/> si es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogCompletedResult<T>(this Task<MlResult<T>>  sourceAsync,
                                                                   ILogger            logger,
                                                                   string             validMessage,
                                                                   string             failMessage)
        => await sourceAsync.LogCompletedResultAsync(logger, _ => validMessage, _ => failMessage);

    #endregion


    #region LogMlResultIfValid

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfValid<T>(this MlResult<T> source,
                                                         ILogger     logger,
                                                         LogLevel    logLevel,
                                                         string      validMessage)
        => source.LogMlResult<T>(logger, 
                              logLevel, 
                              validBuildMessage: x => validMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultIfValidAsync<T>(this MlResult<T> source,
                                                                    ILogger     logger,
                                                                    LogLevel    logLevel,
                                                                    string      validMessage)
        => source.LogMlResultIfValid(logger, logLevel, validMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje literal que se registra cuando el resultado es válido. Si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                          ILogger           logger,
                                                                          LogLevel          logLevel,
                                                                          string            validMessage)
        => (await sourceAsync).LogMlResult<T>(logger, 
                                           logLevel, 
                                           validBuildMessage: x => validMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfValid<T>(this MlResult<T>     source,
                                                         ILogger         logger,
                                                         LogLevel        logLevel,
                                                         Func<T, string> validBuildMessage)
        => source.Match(
                            valid: x      => source.LogMlResult(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, validBuildMessage, x)),
                            fail : errors => source
                        );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultIfValidAsync<T>(this MlResult<T>     source,
                                                                    ILogger         logger,
                                                                    LogLevel        logLevel,
                                                                    Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, logLevel, validBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                          ILogger           logger,
                                                                          LogLevel          logLevel,
                                                                          Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, logLevel, validBuildMessage);

    #endregion


    #region LogMlResultIfFail

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    /// <remarks>
    /// Este método no lanza excepciones: si <paramref name="logger"/> es nulo se devuelve el resultado de origen sin registrar nada.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Solo se registra la traza si 'origen' es fallido.
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultIfFail(logger,
    ///                                                    logLevel    : LogLevel.Error,
    ///                                                    errorMessage: "El cálculo no se ha podido completar");
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        LogLevel    logLevel,
                                                        string      errorMessage)
        => source.LogMlResult<T>(logger, 
                              logLevel, 
                              failBuildMessage: errors => errorMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailAsync<T>(this MlResult<T> source,
                                                                   ILogger     logger,
                                                                   LogLevel    logLevel,
                                                                   string      errorMessage)
        => source.LogMlResultIfFail(logger, logLevel, errorMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                         ILogger           logger,
                                                                         LogLevel          logLevel,
                                                                         string            failMessage)
        => (await sourceAsync).LogMlResult<T>(logger, 
                                              logLevel, 
                                              failBuildMessage: errors => failMessage);
    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFail<T>(this MlResult<T>                   source,
                                                        ILogger                       logger,
                                                        LogLevel                      logLevel,
                                                        Func<MlErrorDetails, string> failBuildMessage)
        => source.Match(
                            valid: x      => source,

                            fail : errors => source.LogMlResult(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, failBuildMessage, errors))
                        );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailAsync<T>(this MlResult<T>                   source,
                                                                   ILogger                       logger,
                                                                   LogLevel                      logLevel,
                                                                   Func<MlErrorDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, logLevel, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                         ILogger                       logger,
                                                                         LogLevel                      logLevel,
                                                                         Func<MlErrorDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, logLevel, failBuildMessage);

    #endregion


    #region LogMlResultIfFailWithValue

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    /// <remarks>
    /// Este método no lanza excepciones: si <paramref name="logger"/> es nulo se devuelve el resultado de origen sin registrar nada.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Solo se registra la traza si 'origen' es fallido y su detalle de error lleva un valor adicional.
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultIfFailWithValue(logger,
    ///                                                             logLevel        : LogLevel.Warning,
    ///                                                             failBuildMessage: errors =&gt; $"Valor rechazado: {errors.GetErrorsSummary()}");
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultIfFailWithValue<T>(this MlResult<T>                   source,
                                                                 ILogger                       logger,
                                                                 LogLevel                      logLevel,
                                                                 Func<MlErrorDetails, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetMetadataValue<T>()
                                                        .Bind( _ => source.LogMlResult(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, failBuildMessage, errorsDetails)))
                        );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithValue<T>(this MlResult<T> source,
                                                                 ILogger     logger,
                                                                 LogLevel    logLevel,
                                                                 string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, logLevel, _ => failMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                  ILogger                       logger,
                                                                                  LogLevel                      logLevel,
                                                                                  Func<MlErrorDetails, string> failBuildMessage)
        => await source.MatchAsync(
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetMetadataValueAsync<T>()
                                                                        .BindAsync( _ => source.LogMlResultAsync(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, failBuildMessage, errorsDetails)))
                                    );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                  ILogger     logger,
                                                                                  LogLevel    logLevel,
                                                                                  string      failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, logLevel, _ => failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                  ILogger                       logger,
                                                                                  LogLevel                      logLevel,
                                                                                  Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, logLevel, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                  ILogger           logger,
                                                                                  LogLevel          logLevel,
                                                                                  string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, logLevel, _ => failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                         ILogger                               logger,
                                                                         LogLevel                              logLevel,
                                                                         Func<MlErrorDetails, TValue, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetMetadataValue<TValue>()
                                                        .Bind( value => source.LogMlResult(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, failBuildMessage, errorsDetails, value)))
                        );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                          ILogger                               logger,
                                                                                          LogLevel                              logLevel,
                                                                                          Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await source.MatchAsync(           
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetMetadataValueAsync<TValue>()
                                                                        .BindAsync( value => source.LogMlResultAsync(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, failBuildMessage, errorsDetails, value)))
                                    );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor adicional almacenado en los detalles del error, que <b>no</b> es el tipo del resultado. Se infiere de la llamada, por lo que una inferencia inesperada puede ocultar un error de invocación.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                          ILogger                               logger,
                                                                                          LogLevel                              logLevel,
                                                                                          Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, logLevel, failBuildMessage);

    #endregion


    #region LogMlResultIfFailWithException

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    /// <remarks>
    /// Este método no lanza excepciones: si <paramref name="logger"/> es nulo se devuelve el resultado de origen sin registrar nada.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Solo se registra la traza si 'origen' es fallido y su detalle de error lleva una excepción.
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultIfFailWithException(logger,
    ///                                                                 logLevel        : LogLevel.Error,
    ///                                                                 failBuildMessage: errors =&gt; $"Excepción capturada: {errors.GetErrorsSummary()}");
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultIfFailWithException<T>(this MlResult<T>                   source,
                                                                     ILogger                       logger,
                                                                     LogLevel                      logLevel,
                                                                     Func<MlErrorDetails, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetException()
                                                        .Bind( _ => source.LogMlResult(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, failBuildMessage, errorsDetails)))
                        );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithException<T>(this MlResult<T> source,
                                                                     ILogger     logger,
                                                                     LogLevel    logLevel,
                                                                     string      failMessage)
        => source.LogMlResultIfFailWithException(logger, logLevel, _ => failMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                      ILogger                       logger,
                                                                                      LogLevel                      logLevel,
                                                                                      Func<MlErrorDetails, string> failBuildMessage)
        => await source.MatchAsync(
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync()
                                                                        .BindAsync( _ => source.LogMlResultAsync(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, failBuildMessage, errorsDetails)))
                                    );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                      ILogger     logger,
                                                                                      LogLevel    logLevel,
                                                                                      string      failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, logLevel, _ => failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                      ILogger                       logger,
                                                                                      LogLevel                      logLevel,
                                                                                      Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, logLevel, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                      ILogger           logger,
                                                                                      LogLevel          logLevel,
                                                                                      string            failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, logLevel, _ => failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithException<T>(this MlResult<T>                              source,
                                                                     ILogger                                  logger,
                                                                     LogLevel                                 logLevel,
                                                                     Func<MlErrorDetails, Exception, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetException()
                                                        .Bind( ex => source.LogMlResult(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, failBuildMessage, errorsDetails, ex)))
                        );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                      ILogger                                  logger,
                                                                                      LogLevel                                 logLevel,
                                                                                      Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await source.MatchAsync(           
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync()
                                                                        .BindAsync( ex => source.LogMlResultAsync(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, failBuildMessage, errorsDetails, ex)))
                                    );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                            sourceAsync,
                                                                                          ILogger                                  logger,
                                                                                          LogLevel                                 logLevel,
                                                                                          Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, logLevel, failBuildMessage);

    #endregion


    #region LogMlResultIfFailWithoutException

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                        ILogger                       logger,
                                                                        LogLevel                      logLevel,
                                                                        Func<MlErrorDetails, string> failBuildMessage)
        => source.Match(
                            valid: x            => source,
                            fail: errorsDetails => errorsDetails.GetException()
                                                        .BindIfFail(
                                                                        funcFail : _ => source.LogMlResult(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, failBuildMessage, errorsDetails)),
                                                                        funcValid: _ => source
                                                                    )
                        );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                        ILogger                       logger,
                                                                        LogLevel                      logLevel,
                                                                        string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, logLevel, _ => failMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                         ILogger                       logger,
                                                                                         LogLevel                      logLevel,
                                                                                         Func<MlErrorDetails, string> failBuildMessage)
        => await source.MatchAsync(
                                    validAsync: x             => source.ToAsync(),
                                    failAsync : errorsDetails => errorsDetails.GetExceptionAsync()
                                                                .BindIfFailAsync(
                                                                                    funcFailAsync : _ => source.LogMlResultAsync(logger, logLevel, MlResultLogGuards.Build(logger, logLevel, failBuildMessage, errorsDetails)),
                                                                                    funcValidAsync: _ => source.ToAsync()
                                                                                )
                        );

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                         ILogger     logger,
                                                                                         LogLevel    logLevel,
                                                                                         string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, logLevel, _ => failMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los datos del fallo · los detalles del error, el valor adicional o la excepción, según la sobrecarga ·. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                         ILogger                       logger,
                                                                                         LogLevel                      logLevel,
                                                                                         Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, logLevel, failBuildMessage);

    /// <summary>
    /// Registra una traza con el nivel que se indique únicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje literal que se registra cuando el resultado es fallido. Si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                         ILogger           logger,
                                                                                         LogLevel          logLevel,
                                                                                         string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, logLevel, _ => failMessage);

    #endregion


    #region LogGeneralErrorIfFail

    /// <summary>
    /// Registra el resumen de errores con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogGeneralErrorIfFail<T>(this MlResult<T> source, ILogger logger)
        => source.LogMlResultErrorIfFail(logger, failBuildMessage : errorDetails => errorDetails.GetErrorsSummary());

    /// <summary>
    /// Registra el resumen de errores con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogGeneralErrorIfFailAsync<T>(this MlResult<T> source, ILogger logger)
        => source.LogMlResultErrorIfFail(logger, failBuildMessage : errorDetails => errorDetails.GetErrorsSummary()).ToAsync();

    /// <summary>
    /// Registra el resumen de errores con el nivel <see cref="LogLevel.Error"/> únicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogGeneralErrorIfFailAsync<T>(this Task<MlResult<T>> sourceAsync, ILogger logger)
        => (await sourceAsync).LogMlResultErrorIfFail(logger, failBuildMessage : errorDetails => errorDetails.GetErrorsSummary());

    #endregion


}
