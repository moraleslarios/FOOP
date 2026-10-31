// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;

public static partial class MlResultLogExtensions
{

    #region LogMlResultIfFailTemplate

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido. Los argumentos los aporta el llamante.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <remarks>
    /// Este método no lanza excepciones: si <paramref name="logger"/> o
    /// <paramref name="messageTemplate"/> son nulos se devuelve el resultado de origen sin registrar
    /// nada. La plantilla nunca se interpola: se entrega al registrador tal cual para que el
    /// <i>sink</i> publique cada marcador como un campo propio.
    /// </remarks>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultIfFailTemplate(logger,
    ///                                                               LogLevel.Error,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultIfFailTemplate<T>(this   MlResult<T> source,
                                                                   ILogger     logger,
                                                                   LogLevel    logLevel,
                                                                   [StringSyntax("MessageTemplate")]
                                                                   string      messageTemplate,
                                                            params object?[]   args)
        => LogIfFailTemplateCore(source, logger, logLevel, messageTemplate, _ => args, PredicateIfFail<T>);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido. Los argumentos se derivan del detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <remarks>
    /// Los nombres de campo canónicos de la rama de fallo están en
    /// <see cref="MlResultLogErrorFields"/>, y son contrato: un <i>sink</i> que indexe por ellos
    /// seguirá encontrándolos.
    /// </remarks>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultIfFailTemplate(logger,
    ///                                                               LogLevel.Error,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultIfFailTemplate<T>(this MlResult<T>                    source,
                                                                 ILogger                        logger,
                                                                 LogLevel                       logLevel,
                                                                 [StringSyntax("MessageTemplate")]
                                                                 string                         messageTemplate,
                                                                 Func<MlErrorDetails, object?[]> argsBuilder)
        => LogIfFailTemplateCore(source, logger, logLevel, messageTemplate, argsBuilder, PredicateIfFail<T>);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido. Los argumentos los aporta el llamante. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailTemplateAsync<T>(this   MlResult<T> source,
                                                                              ILogger     logger,
                                                                              LogLevel    logLevel,
                                                                              [StringSyntax("MessageTemplate")]
                                                                              string      messageTemplate,
                                                                       params object?[]   args)
        => source.LogMlResultIfFailTemplate(logger, logLevel, messageTemplate, args).ToAsync();

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailTemplateAsync<T>(this MlResult<T>                    source,
                                                                            ILogger                        logger,
                                                                            LogLevel                       logLevel,
                                                                            [StringSyntax("MessageTemplate")]
                                                                            string                         messageTemplate,
                                                                            Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailTemplate(logger, logLevel, messageTemplate, argsBuilder).ToAsync();

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido. Los argumentos los aporta el llamante. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    LogLevel          logLevel,
                                                                                    [StringSyntax("MessageTemplate")]
                                                                                    string            messageTemplate,
                                                                             params object?[]         args)
        => (await sourceAsync).LogMlResultIfFailTemplate(logger, logLevel, messageTemplate, args);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                  ILogger                        logger,
                                                                                  LogLevel                       logLevel,
                                                                                  [StringSyntax("MessageTemplate")]
                                                                                  string                         messageTemplate,
                                                                                  Func<MlErrorDetails, object?[]> argsBuilder)
        => (await sourceAsync).LogMlResultIfFailTemplate(logger, logLevel, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultIfFailWithValueTemplate

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos los aporta el llamante.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <remarks>
    /// Este método no lanza excepciones: si <paramref name="logger"/> o
    /// <paramref name="messageTemplate"/> son nulos se devuelve el resultado de origen sin registrar
    /// nada. La plantilla nunca se interpola: se entrega al registrador tal cual para que el
    /// <i>sink</i> publique cada marcador como un campo propio.
    /// </remarks>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultIfFailWithValueTemplate(logger,
    ///                                                               LogLevel.Error,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultIfFailWithValueTemplate<T>(this   MlResult<T> source,
                                                                   ILogger     logger,
                                                                   LogLevel    logLevel,
                                                                   [StringSyntax("MessageTemplate")]
                                                                   string      messageTemplate,
                                                            params object?[]   args)
        => LogIfFailTemplateCore(source, logger, logLevel, messageTemplate, _ => args, PredicateIfFailWithValue<T>);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos se derivan del detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <remarks>
    /// Los nombres de campo canónicos de la rama de fallo están en
    /// <see cref="MlResultLogErrorFields"/>, y son contrato: un <i>sink</i> que indexe por ellos
    /// seguirá encontrándolos.
    /// </remarks>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultIfFailWithValueTemplate(logger,
    ///                                                               LogLevel.Error,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultIfFailWithValueTemplate<T>(this MlResult<T>                    source,
                                                                 ILogger                        logger,
                                                                 LogLevel                       logLevel,
                                                                 [StringSyntax("MessageTemplate")]
                                                                 string                         messageTemplate,
                                                                 Func<MlErrorDetails, object?[]> argsBuilder)
        => LogIfFailTemplateCore(source, logger, logLevel, messageTemplate, argsBuilder, PredicateIfFailWithValue<T>);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos los aporta el llamante. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailWithValueTemplateAsync<T>(this   MlResult<T> source,
                                                                              ILogger     logger,
                                                                              LogLevel    logLevel,
                                                                              [StringSyntax("MessageTemplate")]
                                                                              string      messageTemplate,
                                                                       params object?[]   args)
        => source.LogMlResultIfFailWithValueTemplate(logger, logLevel, messageTemplate, args).ToAsync();

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailWithValueTemplateAsync<T>(this MlResult<T>                    source,
                                                                            ILogger                        logger,
                                                                            LogLevel                       logLevel,
                                                                            [StringSyntax("MessageTemplate")]
                                                                            string                         messageTemplate,
                                                                            Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithValueTemplate(logger, logLevel, messageTemplate, argsBuilder).ToAsync();

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos los aporta el llamante. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    LogLevel          logLevel,
                                                                                    [StringSyntax("MessageTemplate")]
                                                                                    string            messageTemplate,
                                                                             params object?[]         args)
        => (await sourceAsync).LogMlResultIfFailWithValueTemplate(logger, logLevel, messageTemplate, args);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva un valor en los metadatos. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                  ILogger                        logger,
                                                                                  LogLevel                       logLevel,
                                                                                  [StringSyntax("MessageTemplate")]
                                                                                  string                         messageTemplate,
                                                                                  Func<MlErrorDetails, object?[]> argsBuilder)
        => (await sourceAsync).LogMlResultIfFailWithValueTemplate(logger, logLevel, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultIfFailWithExceptionTemplate

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos los aporta el llamante.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <remarks>
    /// Este método no lanza excepciones: si <paramref name="logger"/> o
    /// <paramref name="messageTemplate"/> son nulos se devuelve el resultado de origen sin registrar
    /// nada. La plantilla nunca se interpola: se entrega al registrador tal cual para que el
    /// <i>sink</i> publique cada marcador como un campo propio.
    /// </remarks>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultIfFailWithExceptionTemplate(logger,
    ///                                                               LogLevel.Error,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultIfFailWithExceptionTemplate<T>(this   MlResult<T> source,
                                                                   ILogger     logger,
                                                                   LogLevel    logLevel,
                                                                   [StringSyntax("MessageTemplate")]
                                                                   string      messageTemplate,
                                                            params object?[]   args)
        => LogIfFailTemplateCore(source, logger, logLevel, messageTemplate, _ => args, PredicateIfFailWithException<T>);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos se derivan del detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <remarks>
    /// Los nombres de campo canónicos de la rama de fallo están en
    /// <see cref="MlResultLogErrorFields"/>, y son contrato: un <i>sink</i> que indexe por ellos
    /// seguirá encontrándolos.
    /// </remarks>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultIfFailWithExceptionTemplate(logger,
    ///                                                               LogLevel.Error,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultIfFailWithExceptionTemplate<T>(this MlResult<T>                    source,
                                                                 ILogger                        logger,
                                                                 LogLevel                       logLevel,
                                                                 [StringSyntax("MessageTemplate")]
                                                                 string                         messageTemplate,
                                                                 Func<MlErrorDetails, object?[]> argsBuilder)
        => LogIfFailTemplateCore(source, logger, logLevel, messageTemplate, argsBuilder, PredicateIfFailWithException<T>);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos los aporta el llamante. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailWithExceptionTemplateAsync<T>(this   MlResult<T> source,
                                                                              ILogger     logger,
                                                                              LogLevel    logLevel,
                                                                              [StringSyntax("MessageTemplate")]
                                                                              string      messageTemplate,
                                                                       params object?[]   args)
        => source.LogMlResultIfFailWithExceptionTemplate(logger, logLevel, messageTemplate, args).ToAsync();

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailWithExceptionTemplateAsync<T>(this MlResult<T>                    source,
                                                                            ILogger                        logger,
                                                                            LogLevel                       logLevel,
                                                                            [StringSyntax("MessageTemplate")]
                                                                            string                         messageTemplate,
                                                                            Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithExceptionTemplate(logger, logLevel, messageTemplate, argsBuilder).ToAsync();

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos los aporta el llamante. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    LogLevel          logLevel,
                                                                                    [StringSyntax("MessageTemplate")]
                                                                                    string            messageTemplate,
                                                                             params object?[]         args)
        => (await sourceAsync).LogMlResultIfFailWithExceptionTemplate(logger, logLevel, messageTemplate, args);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error lleva una excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                  ILogger                        logger,
                                                                                  LogLevel                       logLevel,
                                                                                  [StringSyntax("MessageTemplate")]
                                                                                  string                         messageTemplate,
                                                                                  Func<MlErrorDetails, object?[]> argsBuilder)
        => (await sourceAsync).LogMlResultIfFailWithExceptionTemplate(logger, logLevel, messageTemplate, argsBuilder);

    #endregion


    #region LogMlResultIfFailWithoutExceptionTemplate

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos los aporta el llamante.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <remarks>
    /// Este método no lanza excepciones: si <paramref name="logger"/> o
    /// <paramref name="messageTemplate"/> son nulos se devuelve el resultado de origen sin registrar
    /// nada. La plantilla nunca se interpola: se entrega al registrador tal cual para que el
    /// <i>sink</i> publique cada marcador como un campo propio.
    /// </remarks>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultIfFailWithoutExceptionTemplate(logger,
    ///                                                               LogLevel.Error,
    ///                                                               "El pedido {PedidoId} no se ha podido procesar",
    ///                                                               pedidoId);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultIfFailWithoutExceptionTemplate<T>(this   MlResult<T> source,
                                                                   ILogger     logger,
                                                                   LogLevel    logLevel,
                                                                   [StringSyntax("MessageTemplate")]
                                                                   string      messageTemplate,
                                                            params object?[]   args)
        => LogIfFailTemplateCore(source, logger, logLevel, messageTemplate, _ => args, PredicateIfFailWithoutException<T>);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos se derivan del detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <remarks>
    /// Los nombres de campo canónicos de la rama de fallo están en
    /// <see cref="MlResultLogErrorFields"/>, y son contrato: un <i>sink</i> que indexe por ellos
    /// seguirá encontrándolos.
    /// </remarks>
    /// <example>
    /// <code>
    /// MlResult&lt;int&gt; resultado = origen.LogMlResultIfFailWithoutExceptionTemplate(logger,
    ///                                                               LogLevel.Error,
    ///                                                               MlResultLogErrorFields.DefaultTemplate,
    ///                                                               MlResultLogErrorFields.From);
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultIfFailWithoutExceptionTemplate<T>(this MlResult<T>                    source,
                                                                 ILogger                        logger,
                                                                 LogLevel                       logLevel,
                                                                 [StringSyntax("MessageTemplate")]
                                                                 string                         messageTemplate,
                                                                 Func<MlErrorDetails, object?[]> argsBuilder)
        => LogIfFailTemplateCore(source, logger, logLevel, messageTemplate, argsBuilder, PredicateIfFailWithoutException<T>);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos los aporta el llamante. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailWithoutExceptionTemplateAsync<T>(this   MlResult<T> source,
                                                                              ILogger     logger,
                                                                              LogLevel    logLevel,
                                                                              [StringSyntax("MessageTemplate")]
                                                                              string      messageTemplate,
                                                                       params object?[]   args)
        => source.LogMlResultIfFailWithoutExceptionTemplate(logger, logLevel, messageTemplate, args).ToAsync();

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailWithoutExceptionTemplateAsync<T>(this MlResult<T>                    source,
                                                                            ILogger                        logger,
                                                                            LogLevel                       logLevel,
                                                                            [StringSyntax("MessageTemplate")]
                                                                            string                         messageTemplate,
                                                                            Func<MlErrorDetails, object?[]> argsBuilder)
        => source.LogMlResultIfFailWithoutExceptionTemplate(logger, logLevel, messageTemplate, argsBuilder).ToAsync();

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos los aporta el llamante. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="args">Valores que rellenan los marcadores de la plantilla, en el mismo orden en que aparecen.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionTemplateAsync<T>(this   Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    LogLevel          logLevel,
                                                                                    [StringSyntax("MessageTemplate")]
                                                                                    string            messageTemplate,
                                                                             params object?[]         args)
        => (await sourceAsync).LogMlResultIfFailWithoutExceptionTemplate(logger, logLevel, messageTemplate, args);

    /// <summary>
    /// Registra una traza con plantilla de mensaje y campos estructurados, con el nivel que se
    /// indique, únicamente cuando el resultado es fallido y su detalle de error NO lleva ninguna excepción. Los argumentos se derivan del detalle del error.
    /// Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, con marcadores con nombre. Debe ser una constante, nunca una cadena interpolada.</param>
    /// <param name="argsBuilder">Constructor de los valores que rellenan los marcadores; recibe el detalle del error. Sólo se ejecuta si el nivel está habilitado.</param>
    /// <returns>Tarea que produce el resultado de origen, sin modificar.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionTemplateAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                  ILogger                        logger,
                                                                                  LogLevel                       logLevel,
                                                                                  [StringSyntax("MessageTemplate")]
                                                                                  string                         messageTemplate,
                                                                                  Func<MlErrorDetails, object?[]> argsBuilder)
        => (await sourceAsync).LogMlResultIfFailWithoutExceptionTemplate(logger, logLevel, messageTemplate, argsBuilder);

    #endregion


    #region Núcleo privado de la rama de fallo

    /// <summary>
    /// Decide si la traza con plantilla debe emitirse para un resultado de origen, y la emite.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen.</param>
    /// <param name="logger">Registrador, que puede ser nulo.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="messageTemplate">Plantilla del mensaje, que puede ser nula.</param>
    /// <param name="argsBuilder">Constructor de los argumentos, que puede ser nulo.</param>
    /// <param name="shouldLog">Condición que el detalle del error debe cumplir para que se registre la traza.</param>
    /// <returns>El resultado de origen, sin modificar.</returns>
    /// <remarks>
    /// Único punto del fichero que mira el estado del resultado, para que las cuatro familias
    /// compartan exactamente la misma política de pereza, de nulos y de retorno.
    /// </remarks>
    private static MlResult<T> LogIfFailTemplateCore<T>(MlResult<T>                     source,
                                                        ILogger                         logger,
                                                        LogLevel                        logLevel,
                                                        [StringSyntax("MessageTemplate")]
                                                        string                          messageTemplate,
                                                        Func<MlErrorDetails, object?[]> argsBuilder,
                                                        Func<MlErrorDetails, bool>      shouldLog)
        => source.Match(
                            valid: _             => source,
                            fail : errorsDetails =>
                            {
                                if (MlResultLogGuards.IsEnabled(logger, logLevel) && shouldLog(errorsDetails))
                                {
                                    LogTemplateCore(logger,
                                                    logLevel,
                                                    messageTemplate,
                                                    MlResultLogGuards.BuildArgs(logger, logLevel, argsBuilder, errorsDetails));
                                }

                                return source;
                            }
                        );

    /// <summary>Condición de la familia <c>IfFail</c>: basta con que el resultado sea fallido.</summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errorsDetails">Detalle del error.</param>
    /// <returns>Siempre <c>true</c>.</returns>
    private static bool PredicateIfFail<T>(MlErrorDetails errorsDetails) => true;

    /// <summary>Condición de la familia <c>IfFailWithValue</c>: el detalle lleva un valor en los metadatos.</summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errorsDetails">Detalle del error.</param>
    /// <returns><c>true</c> si el detalle lleva un valor del tipo indicado.</returns>
    private static bool PredicateIfFailWithValue<T>(MlErrorDetails errorsDetails)
        => errorsDetails.GetMetadataValue<T>().IsSuccess;

    /// <summary>Condición de la familia <c>IfFailWithException</c>: el detalle lleva una excepción.</summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errorsDetails">Detalle del error.</param>
    /// <returns><c>true</c> si el detalle lleva una excepción.</returns>
    private static bool PredicateIfFailWithException<T>(MlErrorDetails errorsDetails)
        => errorsDetails.GetException().IsSuccess;

    /// <summary>Condición de la familia <c>IfFailWithoutException</c>: el detalle NO lleva excepción.</summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errorsDetails">Detalle del error.</param>
    /// <returns><c>true</c> si el detalle no lleva excepción.</returns>
    private static bool PredicateIfFailWithoutException<T>(MlErrorDetails errorsDetails)
        => errorsDetails.GetException().IsFailure;

    #endregion

}
