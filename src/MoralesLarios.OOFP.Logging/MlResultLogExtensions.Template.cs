// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;

public static partial class MlResultLogExtensions
{

    #region LogMlResultTemplate

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel que se indique, pasando la plantilla y
    /// sus argumentos por separado al registrador.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre · <c>"hola a las {Hora}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Resultado válido que transporta el propio registrador, para poder encadenar.</returns>
    /// <remarks>
    /// A diferencia de las sobrecargas que reciben un <c>message</c> ya compuesto, aquí la plantilla
    /// <b>no se interpola</b>: llega intacta al registrador, que la publica como
    /// <c>{OriginalFormat}</c> y expone cada marcador como un campo consultable en el destino.
    /// </remarks>
    /// <example>
    /// <code>
    /// logger.LogMlResultTemplate(LogLevel.Information, "hola a las {Hora}", DateTime.Now);
    /// </code>
    /// </example>
    public static MlResult<ILogger> LogMlResultTemplate(this ILogger          logger,
                                                            LogLevel         logLevel,
                                                            [StringSyntax("MessageTemplate")]
                                                            string           messageTemplate,
                                                     params object?[]        args)
    {
        LogTemplateCore(logger, logLevel, messageTemplate, args);

        return logger.ToMlResultValid();
    }

    /// <summary>
    /// Registra una traza <b>estructurada</b> con el nivel que se indique, pasando la plantilla y
    /// sus argumentos por separado al registrador. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce un resultado válido que transporta el propio registrador.</returns>
    public static Task<MlResult<ILogger>> LogMlResultTemplateAsync(this ILogger          logger,
                                                                       LogLevel         logLevel,
                                                                       [StringSyntax("MessageTemplate")]
                                                                       string           messageTemplate,
                                                                params object?[]        args)
        => logger.LogMlResultTemplate(logLevel, messageTemplate, args).ToAsync();

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel que se indique,
    /// pasando la plantilla y sus argumentos por separado al registrador.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
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
    ///                     .LogMlResultTemplate(logger, LogLevel.Information,
    ///                                          "cliente {ClienteId} consultado a las {Hora}", 17, DateTime.Now);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultTemplate<T>(this MlResult<T>      source,
                                                          ILogger         logger,
                                                          LogLevel        logLevel,
                                                          [StringSyntax("MessageTemplate")]
                                                          string          messageTemplate,
                                                   params object?[]       args)
    {
        LogTemplateCore(logger, logLevel, messageTemplate, args);

        return source;
    }

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel que se indique,
    /// pasando la plantilla y sus argumentos por separado al registrador. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static Task<MlResult<T>> LogMlResultTemplateAsync<T>(this MlResult<T>      source,
                                                                     ILogger         logger,
                                                                     LogLevel        logLevel,
                                                                     [StringSyntax("MessageTemplate")]
                                                                     string          messageTemplate,
                                                              params object?[]       args)
        => source.LogMlResultTemplate(logger, logLevel, messageTemplate, args).ToAsync();

    /// <summary>
    /// Registra una traza <b>estructurada</b> del resultado de origen con el nivel que se indique,
    /// pasando la plantilla y sus argumentos por separado al registrador. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envía la traza. Si es nulo no se registra nada.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="args"/>. Si es nula no se registra nada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <returns>Tarea que produce el resultado de origen, inalterado.</returns>
    public static async Task<MlResult<T>> LogMlResultTemplateAsync<T>(this Task<MlResult<T>>      sourceAsync,
                                                                           ILogger               logger,
                                                                           LogLevel              logLevel,
                                                                           [StringSyntax("MessageTemplate")]
                                                                           string                messageTemplate,
                                                                    params object?[]             args)
        => (await sourceAsync).LogMlResultTemplate(logger, logLevel, messageTemplate, args);

    #endregion


    #region Núcleo privado

    /// <summary>
    /// Emite la traza estructurada respetando las tres políticas de la librería · registrador nulo,
    /// plantilla nula y cortocircuito por <see cref="ILogger.IsEnabled(LogLevel)"/> ·.
    /// </summary>
    /// <param name="logger">Registrador al que se envía la traza, que puede ser nulo.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registra la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, que puede ser nula.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla.</param>
    /// <remarks>
    /// La plantilla se entrega sin interpolar, que es justamente lo que permite al destino exponer
    /// cada marcador como un campo. Por eso se suprime <c>CA2254</c>: la plantilla la aporta el
    /// consumidor en tiempo de ejecución y no puede ser una constante desde este punto.
    /// </remarks>
    private static void LogTemplateCore(ILogger    logger,
                                        LogLevel   logLevel,
                                        [StringSyntax("MessageTemplate")]
                                        string     messageTemplate,
                                        object?[]  args)
    {
        if (messageTemplate is null) return;

        if (! MlResultLogGuards.IsEnabled(logger, logLevel)) return;

#pragma warning disable CA2254 // La plantilla la aporta el consumidor en ejecución; es el objetivo de la API.
        logger.Log(logLevel, messageTemplate, args ?? []);
#pragma warning restore CA2254
    }

    #endregion

}
