// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Sobrecargas de los métodos de extensión de registro que aceptan un <see cref="EventId"/>, para poder filtrar y
/// correlacionar trazas sin depender del texto del mensaje.
/// </summary>
/// <remarks>
/// Estas sobrecargas son <b>aditivas</b>: no modifican ninguna firma anterior. El alcance responde a la decisión
/// <c>D-20</c> opción (b) de la épica 06 de la versión 2.2.0 · el <see cref="EventId"/> se ofrece en la familia
/// genérica <c>LogMlResult</c> y en las seis familias de nivel, no en las 339 firmas públicas.
/// </remarks>
public static partial class MlResultLogExtensions
{


    #region LogMlResult con EventId

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel y el identificador de evento que se indiquen.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    /// <remarks>
    /// Este método no lanza excepciones: si <paramref name="logger"/> es nulo se devuelve un resultado fallido sin
    /// registrar nada.
    /// </remarks>
    /// <example>
    /// <code>
    /// logger.LogMlResult(LogLevel.Information, new EventId(1001, "PedidoCreado"), "Pedido creado");
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResult(this ILogger  logger,
                                                     LogLevel logLevel,
                                                     EventId  eventId,
                                                     string   message)
    {
        if (logger  is null) return MlResultLogGuards.NullLoggerFail();
        if (message is null) return logger.ToMlResultValid();

        logger.Log(logLevel, eventId, message);

        return logger.ToMlResultValid();
    }

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel y el identificador de evento que se indiquen.
    /// Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    public static Task<MlResult<ILogger>> LogMlResultAsync(this ILogger  logger,
                                                                LogLevel logLevel,
                                                                EventId  eventId,
                                                                string   message)
        => logger.LogMlResult(logLevel, eventId, message).ToAsync();

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel y el identificador de evento que se indiquen.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    /// <remarks>
    /// Los constructores de mensaje sólo se invocan si el registrador acepta el nivel indicado · se consulta
    /// <c>IsEnabled</c> antes de construir el mensaje ·, de modo que con el nivel apagado no se paga ningún coste.
    /// </remarks>
    /// <example>
    /// <code>
    /// // 'origen' es cualquier MlResult&lt;int&gt; y 'logger' un ILogger ya disponible.
    /// MlResult&lt;int&gt; resultado = origen.LogMlResult(logger,
    ///                                              LogLevel.Information,
    ///                                              new EventId(1001, "PedidoCreado"),
    ///                                              validBuildMessage: valor  =&gt; $"Valor calculado: {valor}",
    ///                                              failBuildMessage : errors =&gt; $"Fallo al calcular: {errors.GetErrorsSummary()}");
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResult<T>(this MlResult<T>                  source,
                                                  ILogger                      logger,
                                                  LogLevel                     logLevel,
                                                  EventId                      eventId,
                                                  Func<T, string>              validBuildMessage = null!,
                                                  Func<MlErrorDetails, string> failBuildMessage  = null!)
    {
        if (! MlResultLogGuards.IsEnabled(logger, logLevel)) return source;

        source.Match(
                        valid: x      => validBuildMessage != null ? logger.LogMlResult(logLevel, eventId, validBuildMessage(x))     : null!,
                        fail : errors => failBuildMessage  != null ? logger.LogMlResult(logLevel, eventId, failBuildMessage(errors)) : null!
                    );

        return source;
    }

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel y el identificador de evento que se indiquen.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T>                  source,
                                                             ILogger                      logger,
                                                             LogLevel                     logLevel,
                                                             EventId                      eventId,
                                                             Func<T, string>              validBuildMessage = null!,
                                                             Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult(logger, logLevel, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel y el identificador de
    /// evento que se indiquen. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>>            sourceAsync,
                                                                   ILogger                      logger,
                                                                   LogLevel                     logLevel,
                                                                   EventId                      eventId,
                                                                   Func<T, string>              validBuildMessage = null!,
                                                                   Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResult(logger, logLevel, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultTrace con EventId

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Trace"/> y el identificador de
    /// evento que se indique.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultTrace(new EventId(1001, "PasoIntermedio"), "Entrando en el paso intermedio");
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultTrace(this ILogger logger,
                                                          EventId eventId,
                                                          string  message)
        => logger.LogMlResult(LogLevel.Trace, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Trace"/> y el identificador de
    /// evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    public static Task<MlResult<ILogger>> LogMlResultTraceAsync(this ILogger logger,
                                                                     EventId eventId,
                                                                     string  message)
        => logger.LogMlResultAsync(LogLevel.Trace, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Trace"/> y el identificador de
    /// evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    public static MlResult<T> LogMlResultTrace<T>(this MlResult<T>                  source,
                                                       ILogger                      logger,
                                                       EventId                      eventId,
                                                       Func<T, string>              validBuildMessage = null!,
                                                       Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult(logger, LogLevel.Trace, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Trace"/> y el identificador de
    /// evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultTraceAsync<T>(this MlResult<T>                  source,
                                                                  ILogger                      logger,
                                                                  EventId                      eventId,
                                                                  Func<T, string>              validBuildMessage = null!,
                                                                  Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResultAsync(logger, LogLevel.Trace, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Trace"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this Task<MlResult<T>>            sourceAsync,
                                                                        ILogger                      logger,
                                                                        EventId                      eventId,
                                                                        Func<T, string>              validBuildMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResultTrace(logger, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultDebug con EventId

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Debug"/> y el identificador de
    /// evento que se indique.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultDebug(new EventId(2001, "ConsultaLanzada"), "Consulta lanzada contra el repositorio");
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultDebug(this ILogger logger,
                                                          EventId eventId,
                                                          string  message)
        => logger.LogMlResult(LogLevel.Debug, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Debug"/> y el identificador de
    /// evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    public static Task<MlResult<ILogger>> LogMlResultDebugAsync(this ILogger logger,
                                                                     EventId eventId,
                                                                     string  message)
        => logger.LogMlResultAsync(LogLevel.Debug, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Debug"/> y el identificador de
    /// evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    public static MlResult<T> LogMlResultDebug<T>(this MlResult<T>                  source,
                                                       ILogger                      logger,
                                                       EventId                      eventId,
                                                       Func<T, string>              validBuildMessage = null!,
                                                       Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult(logger, LogLevel.Debug, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Debug"/> y el identificador de
    /// evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultDebugAsync<T>(this MlResult<T>                  source,
                                                                  ILogger                      logger,
                                                                  EventId                      eventId,
                                                                  Func<T, string>              validBuildMessage = null!,
                                                                  Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResultAsync(logger, LogLevel.Debug, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Debug"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugAsync<T>(this Task<MlResult<T>>            sourceAsync,
                                                                        ILogger                      logger,
                                                                        EventId                      eventId,
                                                                        Func<T, string>              validBuildMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResultDebug(logger, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultInformation con EventId

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Information"/> y el identificador
    /// de evento que se indique.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultInformation(new EventId(3001, "PedidoCreado"), "Pedido creado correctamente");
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultInformation(this ILogger logger,
                                                                EventId eventId,
                                                                string  message)
        => logger.LogMlResult(LogLevel.Information, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Information"/> y el identificador
    /// de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    public static Task<MlResult<ILogger>> LogMlResultInformationAsync(this ILogger logger,
                                                                           EventId eventId,
                                                                           string  message)
        => logger.LogMlResultAsync(LogLevel.Information, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Information"/> y el
    /// identificador de evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    public static MlResult<T> LogMlResultInformation<T>(this MlResult<T>                  source,
                                                             ILogger                      logger,
                                                             EventId                      eventId,
                                                             Func<T, string>              validBuildMessage = null!,
                                                             Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult(logger, LogLevel.Information, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Information"/> y el
    /// identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultInformationAsync<T>(this MlResult<T>                  source,
                                                                        ILogger                      logger,
                                                                        EventId                      eventId,
                                                                        Func<T, string>              validBuildMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResultAsync(logger, LogLevel.Information, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Information"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultInformationAsync<T>(this Task<MlResult<T>>            sourceAsync,
                                                                              ILogger                      logger,
                                                                              EventId                      eventId,
                                                                              Func<T, string>              validBuildMessage = null!,
                                                                              Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResultInformation(logger, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultWarning con EventId

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Warning"/> y el identificador de
    /// evento que se indique.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultWarning(new EventId(4001, "ReintentoNecesario"), "Ha hecho falta reintentar la operacion");
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultWarning(this ILogger logger,
                                                            EventId eventId,
                                                            string  message)
        => logger.LogMlResult(LogLevel.Warning, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Warning"/> y el identificador de
    /// evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    public static Task<MlResult<ILogger>> LogMlResultWarningAsync(this ILogger logger,
                                                                       EventId eventId,
                                                                       string  message)
        => logger.LogMlResultAsync(LogLevel.Warning, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Warning"/> y el identificador
    /// de evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    public static MlResult<T> LogMlResultWarning<T>(this MlResult<T>                  source,
                                                         ILogger                      logger,
                                                         EventId                      eventId,
                                                         Func<T, string>              validBuildMessage = null!,
                                                         Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult(logger, LogLevel.Warning, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Warning"/> y el identificador
    /// de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningAsync<T>(this MlResult<T>                  source,
                                                                    ILogger                      logger,
                                                                    EventId                      eventId,
                                                                    Func<T, string>              validBuildMessage = null!,
                                                                    Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResultAsync(logger, LogLevel.Warning, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Warning"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>>            sourceAsync,
                                                                          ILogger                      logger,
                                                                          EventId                      eventId,
                                                                          Func<T, string>              validBuildMessage = null!,
                                                                          Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResultWarning(logger, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultError con EventId

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Error"/> y el identificador de
    /// evento que se indique.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultError(new EventId(5001, "PedidoRechazado"), "El pedido ha sido rechazado");
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultError(this ILogger logger,
                                                          EventId eventId,
                                                          string  message)
        => logger.LogMlResult(LogLevel.Error, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Error"/> y el identificador de
    /// evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    public static Task<MlResult<ILogger>> LogMlResultErrorAsync(this ILogger logger,
                                                                     EventId eventId,
                                                                     string  message)
        => logger.LogMlResultAsync(LogLevel.Error, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Error"/> y el identificador de
    /// evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    public static MlResult<T> LogMlResultError<T>(this MlResult<T>                  source,
                                                       ILogger                      logger,
                                                       EventId                      eventId,
                                                       Func<T, string>              validBuildMessage = null!,
                                                       Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult(logger, LogLevel.Error, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Error"/> y el identificador de
    /// evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorAsync<T>(this MlResult<T>                  source,
                                                                  ILogger                      logger,
                                                                  EventId                      eventId,
                                                                  Func<T, string>              validBuildMessage = null!,
                                                                  Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResultAsync(logger, LogLevel.Error, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Error"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this Task<MlResult<T>>            sourceAsync,
                                                                        ILogger                      logger,
                                                                        EventId                      eventId,
                                                                        Func<T, string>              validBuildMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResultError(logger, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultCritical con EventId

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Critical"/> y el identificador de
    /// evento que se indique.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultCritical(new EventId(6001, "ServicioCaido"), "El servicio no responde");
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultCritical(this ILogger logger,
                                                             EventId eventId,
                                                             string  message)
        => logger.LogMlResult(LogLevel.Critical, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="ILogger"/> con el nivel <see cref="LogLevel.Critical"/> y el identificador de
    /// evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="message">Mensaje literal que se registra tal cual. Si es nulo no se registra nada.</param>
    /// <returns>Tarea que produce el resultado válido con el registrador, o resultado fallido si el registrador es nulo.</returns>
    public static Task<MlResult<ILogger>> LogMlResultCriticalAsync(this ILogger logger,
                                                                        EventId eventId,
                                                                        string  message)
        => logger.LogMlResultAsync(LogLevel.Critical, eventId, message);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Critical"/> y el identificador
    /// de evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    public static MlResult<T> LogMlResultCritical<T>(this MlResult<T>                  source,
                                                          ILogger                      logger,
                                                          EventId                      eventId,
                                                          Func<T, string>              validBuildMessage = null!,
                                                          Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult(logger, LogLevel.Critical, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Critical"/> y el identificador
    /// de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalAsync<T>(this MlResult<T>                  source,
                                                                     ILogger                      logger,
                                                                     EventId                      eventId,
                                                                     Func<T, string>              validBuildMessage = null!,
                                                                     Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResultAsync(logger, LogLevel.Critical, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Critical"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalAsync<T>(this Task<MlResult<T>>            sourceAsync,
                                                                           ILogger                      logger,
                                                                           EventId                      eventId,
                                                                           Func<T, string>              validBuildMessage = null!,
                                                                           Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResultCritical(logger, eventId, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


}
