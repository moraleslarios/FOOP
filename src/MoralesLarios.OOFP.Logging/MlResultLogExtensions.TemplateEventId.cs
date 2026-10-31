// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;

/// <summary>
/// Cruce de los dos ejes nuevos de la versión 2.3.0 · la <b>plantilla</b> de registro estructurado y el
/// <see cref="EventId"/> ·, para poder filtrar y correlacionar trazas por evento sin renunciar a los campos.
/// </summary>
/// <remarks>
/// El alcance responde a la decisión <c>D-28</c> opción (b) de la épica 08 de la versión 2.3.0, que replica el
/// precedente de <c>D-20</c>: el <see cref="EventId"/> se ofrece en la familia genérica
/// <c>LogMlResultTemplate</c> y en las seis familias de nivel, y <b>no</b> en las familias condicionales ni
/// cruzado con <c>ILogger&lt;TCategory&gt;</c>. Todas las firmas de este fichero son <b>aditivas</b>.
/// </remarks>
public static partial class MlResultLogExtensions
{

    #region LogMlResultTemplate con EventId

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel y el identificador de evento que se indiquen,
    /// pasando la plantilla y sus argumentos por separado al registrador.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre · <c>"hola a las {Hora}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Resultado válido que transporta el propio registrador, para poder encadenar.</returns>
    /// <remarks>
    /// La plantilla <b>no se interpola</b>: llega intacta al registrador, que la publica como
    /// <c>{OriginalFormat}</c> y expone cada marcador como un campo consultable en el destino. El
    /// <paramref name="eventId"/> viaja además como campo propio de la traza.
    /// </remarks>
    /// <example>
    /// <code>
    /// logger.LogMlResultTemplate(LogLevel.Information, new EventId(1001, "PedidoCreado"),
    ///                            "pedido {PedidoId} creado a las {Hora}", 17, DateTime.Now);
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultTemplate(this ILogger      logger,
                                                             LogLevel     logLevel,
                                                             EventId      eventId,
                                                             [StringSyntax("MessageTemplate")]
                                                             string       messageTemplate,
                                                      params object?[]    args)
    {
        LogTemplateCore(logger, logLevel, eventId, messageTemplate, args);

        return logger.ToMlResultValid();
    }

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel y el identificador de evento que se indiquen,
    /// pasando la plantilla y sus argumentos por separado al registrador. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce un resultado válido que transporta el propio registrador.</returns>
    public static Task<MlResult<ILogger>> LogMlResultTemplateAsync(this ILogger      logger,
                                                                       LogLevel     logLevel,
                                                                       EventId      eventId,
                                                                       [StringSyntax("MessageTemplate")]
                                                                       string       messageTemplate,
                                                                params object?[]    args)
        => logger.LogMlResultTemplate(logLevel, eventId, messageTemplate, args).ToAsync();

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel y el identificador de
    /// evento que se indiquen, pasando la plantilla y sus argumentos por separado al registrador.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
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
    ///                     .LogMlResultTemplate(logger, LogLevel.Information, new EventId(1001, "ClienteConsultado"),
    ///                                          "cliente {ClienteId} consultado", 17);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultTemplate<T>(this MlResult<T>  source,
                                                          ILogger      logger,
                                                          LogLevel     logLevel,
                                                          EventId      eventId,
                                                          [StringSyntax("MessageTemplate")]
                                                          string       messageTemplate,
                                                   params object?[]    args)
    {
        LogTemplateCore(logger, logLevel, eventId, messageTemplate, args);

        return source;
    }

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel y el identificador de
    /// evento que se indiquen, pasando la plantilla y sus argumentos por separado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultTemplateAsync<T>(this MlResult<T>  source,
                                                                     ILogger      logger,
                                                                     LogLevel     logLevel,
                                                                     EventId      eventId,
                                                                     [StringSyntax("MessageTemplate")]
                                                                     string       messageTemplate,
                                                              params object?[]    args)
        => source.LogMlResultTemplate(logger, logLevel, eventId, messageTemplate, args).ToAsync();

    /// <summary>
    /// Registra una traza <b>estructurada</b> de la <see cref="MlResult{T}"/> producida por una tarea, con el
    /// nivel y el identificador de evento que se indiquen. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static async Task<MlResult<T>> LogMlResultTemplateAsync<T>(this Task<MlResult<T>>  sourceAsync,
                                                                           ILogger            logger,
                                                                           LogLevel           logLevel,
                                                                           EventId            eventId,
                                                                           [StringSyntax("MessageTemplate")]
                                                                           string             messageTemplate,
                                                                    params object?[]          args)
        => (await sourceAsync).LogMlResultTemplate(logger, logLevel, eventId, messageTemplate, args);

    #endregion


    #region LogMlResultTraceTemplate con EventId

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Trace"/> y el identificador
    /// de evento que se indique, pasando la plantilla y sus argumentos por separado al registrador.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre · <c>"hola a las {Hora}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Resultado válido que transporta el propio registrador, para poder encadenar.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultTraceTemplate(new EventId(1001, "PedidoCreado"), "pedido {PedidoId} creado", 17);
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultTraceTemplate(this ILogger      logger,
                                                                      EventId      eventId,
                                                                      [StringSyntax("MessageTemplate")]
                                                                      string       messageTemplate,
                                                               params object?[]    args)
        => logger.LogMlResultTemplate(LogLevel.Trace, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Trace"/> y el identificador
    /// de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce un resultado válido que transporta el propio registrador.</returns>
    public static Task<MlResult<ILogger>> LogMlResultTraceTemplateAsync(this ILogger      logger,
                                                                                EventId      eventId,
                                                                                [StringSyntax("MessageTemplate")]
                                                                                string       messageTemplate,
                                                                         params object?[]    args)
        => logger.LogMlResultTemplateAsync(LogLevel.Trace, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Trace"/> y el identificador de evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>El resultado de origen, inalterado.</returns>
    /// <example>
    /// <code>
    /// var resultado = ObtenerCliente(17)
    ///                     .LogMlResultTraceTemplate(logger, new EventId(1001, "ClienteConsultado"),
    ///                                                "cliente {ClienteId} consultado", 17);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultTraceTemplate<T>(this MlResult<T>  source,
                                                                   ILogger      logger,
                                                                   EventId      eventId,
                                                                   [StringSyntax("MessageTemplate")]
                                                                   string       messageTemplate,
                                                            params object?[]    args)
        => source.LogMlResultTemplate(logger, LogLevel.Trace, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Trace"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultTraceTemplateAsync<T>(this MlResult<T>  source,
                                                                              ILogger      logger,
                                                                              EventId      eventId,
                                                                              [StringSyntax("MessageTemplate")]
                                                                              string       messageTemplate,
                                                                       params object?[]    args)
        => source.LogMlResultTemplateAsync(logger, LogLevel.Trace, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> de la <see cref="MlResult{T}"/> producida por una tarea, con el
    /// nivel <see cref="LogLevel.Trace"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceTemplateAsync<T>(this Task<MlResult<T>>  sourceAsync,
                                                                                    ILogger            logger,
                                                                                    EventId            eventId,
                                                                                    [StringSyntax("MessageTemplate")]
                                                                                    string             messageTemplate,
                                                                             params object?[]          args)
        => (await sourceAsync).LogMlResultTraceTemplate(logger, eventId, messageTemplate, args);

    #endregion


    #region LogMlResultDebugTemplate con EventId

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Debug"/> y el identificador
    /// de evento que se indique, pasando la plantilla y sus argumentos por separado al registrador.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre · <c>"hola a las {Hora}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Resultado válido que transporta el propio registrador, para poder encadenar.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultDebugTemplate(new EventId(1001, "PedidoCreado"), "pedido {PedidoId} creado", 17);
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultDebugTemplate(this ILogger      logger,
                                                                      EventId      eventId,
                                                                      [StringSyntax("MessageTemplate")]
                                                                      string       messageTemplate,
                                                               params object?[]    args)
        => logger.LogMlResultTemplate(LogLevel.Debug, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Debug"/> y el identificador
    /// de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce un resultado válido que transporta el propio registrador.</returns>
    public static Task<MlResult<ILogger>> LogMlResultDebugTemplateAsync(this ILogger      logger,
                                                                                EventId      eventId,
                                                                                [StringSyntax("MessageTemplate")]
                                                                                string       messageTemplate,
                                                                         params object?[]    args)
        => logger.LogMlResultTemplateAsync(LogLevel.Debug, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Debug"/> y el identificador de evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>El resultado de origen, inalterado.</returns>
    /// <example>
    /// <code>
    /// var resultado = ObtenerCliente(17)
    ///                     .LogMlResultDebugTemplate(logger, new EventId(1001, "ClienteConsultado"),
    ///                                                "cliente {ClienteId} consultado", 17);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultDebugTemplate<T>(this MlResult<T>  source,
                                                                   ILogger      logger,
                                                                   EventId      eventId,
                                                                   [StringSyntax("MessageTemplate")]
                                                                   string       messageTemplate,
                                                            params object?[]    args)
        => source.LogMlResultTemplate(logger, LogLevel.Debug, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Debug"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultDebugTemplateAsync<T>(this MlResult<T>  source,
                                                                              ILogger      logger,
                                                                              EventId      eventId,
                                                                              [StringSyntax("MessageTemplate")]
                                                                              string       messageTemplate,
                                                                       params object?[]    args)
        => source.LogMlResultTemplateAsync(logger, LogLevel.Debug, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> de la <see cref="MlResult{T}"/> producida por una tarea, con el
    /// nivel <see cref="LogLevel.Debug"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugTemplateAsync<T>(this Task<MlResult<T>>  sourceAsync,
                                                                                    ILogger            logger,
                                                                                    EventId            eventId,
                                                                                    [StringSyntax("MessageTemplate")]
                                                                                    string             messageTemplate,
                                                                             params object?[]          args)
        => (await sourceAsync).LogMlResultDebugTemplate(logger, eventId, messageTemplate, args);

    #endregion


    #region LogMlResultInformationTemplate con EventId

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Information"/> y el identificador
    /// de evento que se indique, pasando la plantilla y sus argumentos por separado al registrador.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre · <c>"hola a las {Hora}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Resultado válido que transporta el propio registrador, para poder encadenar.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultInformationTemplate(new EventId(1001, "PedidoCreado"), "pedido {PedidoId} creado", 17);
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultInformationTemplate(this ILogger      logger,
                                                                      EventId      eventId,
                                                                      [StringSyntax("MessageTemplate")]
                                                                      string       messageTemplate,
                                                               params object?[]    args)
        => logger.LogMlResultTemplate(LogLevel.Information, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Information"/> y el identificador
    /// de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce un resultado válido que transporta el propio registrador.</returns>
    public static Task<MlResult<ILogger>> LogMlResultInformationTemplateAsync(this ILogger      logger,
                                                                                EventId      eventId,
                                                                                [StringSyntax("MessageTemplate")]
                                                                                string       messageTemplate,
                                                                         params object?[]    args)
        => logger.LogMlResultTemplateAsync(LogLevel.Information, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Information"/> y el identificador de evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>El resultado de origen, inalterado.</returns>
    /// <example>
    /// <code>
    /// var resultado = ObtenerCliente(17)
    ///                     .LogMlResultInformationTemplate(logger, new EventId(1001, "ClienteConsultado"),
    ///                                                "cliente {ClienteId} consultado", 17);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultInformationTemplate<T>(this MlResult<T>  source,
                                                                   ILogger      logger,
                                                                   EventId      eventId,
                                                                   [StringSyntax("MessageTemplate")]
                                                                   string       messageTemplate,
                                                            params object?[]    args)
        => source.LogMlResultTemplate(logger, LogLevel.Information, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Information"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultInformationTemplateAsync<T>(this MlResult<T>  source,
                                                                              ILogger      logger,
                                                                              EventId      eventId,
                                                                              [StringSyntax("MessageTemplate")]
                                                                              string       messageTemplate,
                                                                       params object?[]    args)
        => source.LogMlResultTemplateAsync(logger, LogLevel.Information, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> de la <see cref="MlResult{T}"/> producida por una tarea, con el
    /// nivel <see cref="LogLevel.Information"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static async Task<MlResult<T>> LogMlResultInformationTemplateAsync<T>(this Task<MlResult<T>>  sourceAsync,
                                                                                    ILogger            logger,
                                                                                    EventId            eventId,
                                                                                    [StringSyntax("MessageTemplate")]
                                                                                    string             messageTemplate,
                                                                             params object?[]          args)
        => (await sourceAsync).LogMlResultInformationTemplate(logger, eventId, messageTemplate, args);

    #endregion


    #region LogMlResultWarningTemplate con EventId

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Warning"/> y el identificador
    /// de evento que se indique, pasando la plantilla y sus argumentos por separado al registrador.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre · <c>"hola a las {Hora}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Resultado válido que transporta el propio registrador, para poder encadenar.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultWarningTemplate(new EventId(1001, "PedidoCreado"), "pedido {PedidoId} creado", 17);
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultWarningTemplate(this ILogger      logger,
                                                                      EventId      eventId,
                                                                      [StringSyntax("MessageTemplate")]
                                                                      string       messageTemplate,
                                                               params object?[]    args)
        => logger.LogMlResultTemplate(LogLevel.Warning, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Warning"/> y el identificador
    /// de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce un resultado válido que transporta el propio registrador.</returns>
    public static Task<MlResult<ILogger>> LogMlResultWarningTemplateAsync(this ILogger      logger,
                                                                                EventId      eventId,
                                                                                [StringSyntax("MessageTemplate")]
                                                                                string       messageTemplate,
                                                                         params object?[]    args)
        => logger.LogMlResultTemplateAsync(LogLevel.Warning, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Warning"/> y el identificador de evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>El resultado de origen, inalterado.</returns>
    /// <example>
    /// <code>
    /// var resultado = ObtenerCliente(17)
    ///                     .LogMlResultWarningTemplate(logger, new EventId(1001, "ClienteConsultado"),
    ///                                                "cliente {ClienteId} consultado", 17);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultWarningTemplate<T>(this MlResult<T>  source,
                                                                   ILogger      logger,
                                                                   EventId      eventId,
                                                                   [StringSyntax("MessageTemplate")]
                                                                   string       messageTemplate,
                                                            params object?[]    args)
        => source.LogMlResultTemplate(logger, LogLevel.Warning, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Warning"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultWarningTemplateAsync<T>(this MlResult<T>  source,
                                                                              ILogger      logger,
                                                                              EventId      eventId,
                                                                              [StringSyntax("MessageTemplate")]
                                                                              string       messageTemplate,
                                                                       params object?[]    args)
        => source.LogMlResultTemplateAsync(logger, LogLevel.Warning, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> de la <see cref="MlResult{T}"/> producida por una tarea, con el
    /// nivel <see cref="LogLevel.Warning"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningTemplateAsync<T>(this Task<MlResult<T>>  sourceAsync,
                                                                                    ILogger            logger,
                                                                                    EventId            eventId,
                                                                                    [StringSyntax("MessageTemplate")]
                                                                                    string             messageTemplate,
                                                                             params object?[]          args)
        => (await sourceAsync).LogMlResultWarningTemplate(logger, eventId, messageTemplate, args);

    #endregion


    #region LogMlResultErrorTemplate con EventId

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Error"/> y el identificador
    /// de evento que se indique, pasando la plantilla y sus argumentos por separado al registrador.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre · <c>"hola a las {Hora}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Resultado válido que transporta el propio registrador, para poder encadenar.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultErrorTemplate(new EventId(1001, "PedidoCreado"), "pedido {PedidoId} creado", 17);
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultErrorTemplate(this ILogger      logger,
                                                                      EventId      eventId,
                                                                      [StringSyntax("MessageTemplate")]
                                                                      string       messageTemplate,
                                                               params object?[]    args)
        => logger.LogMlResultTemplate(LogLevel.Error, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Error"/> y el identificador
    /// de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce un resultado válido que transporta el propio registrador.</returns>
    public static Task<MlResult<ILogger>> LogMlResultErrorTemplateAsync(this ILogger      logger,
                                                                                EventId      eventId,
                                                                                [StringSyntax("MessageTemplate")]
                                                                                string       messageTemplate,
                                                                         params object?[]    args)
        => logger.LogMlResultTemplateAsync(LogLevel.Error, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Error"/> y el identificador de evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>El resultado de origen, inalterado.</returns>
    /// <example>
    /// <code>
    /// var resultado = ObtenerCliente(17)
    ///                     .LogMlResultErrorTemplate(logger, new EventId(1001, "ClienteConsultado"),
    ///                                                "cliente {ClienteId} consultado", 17);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultErrorTemplate<T>(this MlResult<T>  source,
                                                                   ILogger      logger,
                                                                   EventId      eventId,
                                                                   [StringSyntax("MessageTemplate")]
                                                                   string       messageTemplate,
                                                            params object?[]    args)
        => source.LogMlResultTemplate(logger, LogLevel.Error, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Error"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultErrorTemplateAsync<T>(this MlResult<T>  source,
                                                                              ILogger      logger,
                                                                              EventId      eventId,
                                                                              [StringSyntax("MessageTemplate")]
                                                                              string       messageTemplate,
                                                                       params object?[]    args)
        => source.LogMlResultTemplateAsync(logger, LogLevel.Error, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> de la <see cref="MlResult{T}"/> producida por una tarea, con el
    /// nivel <see cref="LogLevel.Error"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorTemplateAsync<T>(this Task<MlResult<T>>  sourceAsync,
                                                                                    ILogger            logger,
                                                                                    EventId            eventId,
                                                                                    [StringSyntax("MessageTemplate")]
                                                                                    string             messageTemplate,
                                                                             params object?[]          args)
        => (await sourceAsync).LogMlResultErrorTemplate(logger, eventId, messageTemplate, args);

    #endregion


    #region LogMlResultCriticalTemplate con EventId

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Critical"/> y el identificador
    /// de evento que se indique, pasando la plantilla y sus argumentos por separado al registrador.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre · <c>"hola a las {Hora}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Resultado válido que transporta el propio registrador, para poder encadenar.</returns>
    /// <example>
    /// <code>
    /// logger.LogMlResultCriticalTemplate(new EventId(1001, "PedidoCreado"), "pedido {PedidoId} creado", 17);
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultCriticalTemplate(this ILogger      logger,
                                                                      EventId      eventId,
                                                                      [StringSyntax("MessageTemplate")]
                                                                      string       messageTemplate,
                                                               params object?[]    args)
        => logger.LogMlResultTemplate(LogLevel.Critical, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel <see cref="LogLevel.Critical"/> y el identificador
    /// de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce un resultado válido que transporta el propio registrador.</returns>
    public static Task<MlResult<ILogger>> LogMlResultCriticalTemplateAsync(this ILogger      logger,
                                                                                EventId      eventId,
                                                                                [StringSyntax("MessageTemplate")]
                                                                                string       messageTemplate,
                                                                         params object?[]    args)
        => logger.LogMlResultTemplateAsync(LogLevel.Critical, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Critical"/> y el identificador de evento que se indique.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>El resultado de origen, inalterado.</returns>
    /// <example>
    /// <code>
    /// var resultado = ObtenerCliente(17)
    ///                     .LogMlResultCriticalTemplate(logger, new EventId(1001, "ClienteConsultado"),
    ///                                                "cliente {ClienteId} consultado", 17);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultCriticalTemplate<T>(this MlResult<T>  source,
                                                                   ILogger      logger,
                                                                   EventId      eventId,
                                                                   [StringSyntax("MessageTemplate")]
                                                                   string       messageTemplate,
                                                            params object?[]    args)
        => source.LogMlResultTemplate(logger, LogLevel.Critical, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel
    /// <see cref="LogLevel.Critical"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalTemplateAsync<T>(this MlResult<T>  source,
                                                                              ILogger      logger,
                                                                              EventId      eventId,
                                                                              [StringSyntax("MessageTemplate")]
                                                                              string       messageTemplate,
                                                                       params object?[]    args)
        => source.LogMlResultTemplateAsync(logger, LogLevel.Critical, eventId, messageTemplate, args);

    /// <summary>
    /// Registra una traza <b>estructurada</b> de la <see cref="MlResult{T}"/> producida por una tarea, con el
    /// nivel <see cref="LogLevel.Critical"/> y el identificador de evento que se indique. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalTemplateAsync<T>(this Task<MlResult<T>>  sourceAsync,
                                                                                    ILogger            logger,
                                                                                    EventId            eventId,
                                                                                    [StringSyntax("MessageTemplate")]
                                                                                    string             messageTemplate,
                                                                             params object?[]          args)
        => (await sourceAsync).LogMlResultCriticalTemplate(logger, eventId, messageTemplate, args);

    #endregion


    #region Núcleo privado con EventId

    /// <summary>
    /// Núcleo común de todas las sobrecargas con plantilla y <see cref="EventId"/>.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
    /// <param name="eventId">Identificador de evento que acompaña a la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, por posición.</param>
    /// <remarks>
    /// Consulta <c>IsEnabled</c> antes de entregar nada al registrador, de modo que con el nivel apagado no
    /// se paga ningún coste. El aviso <c>CA2254</c> se silencia a propósito: que la plantilla la aporte el
    /// consumidor en ejecución es justamente el objetivo de esta API.
    /// </remarks>
    private static void LogTemplateCore(ILogger   logger,
                                        LogLevel  logLevel,
                                        EventId   eventId,
                                        [StringSyntax("MessageTemplate")]
                                        string    messageTemplate,
                                        object?[] args)
    {
        if (messageTemplate is null) return;
        if (! MlResultLogGuards.IsEnabled(logger, logLevel)) return;

#pragma warning disable CA2254 // La plantilla la aporta el consumidor en ejecución; es el objetivo de la API.
        logger.Log(logLevel, eventId, messageTemplate, args ?? []);
#pragma warning restore CA2254
    }

    #endregion

}
