// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Sobrecargas de los métodos de extensión de registro que aceptan un registrador tipado por categoría, para poder
/// usar directamente el registrador que inyecta el contenedor sin convertirlo al tipo base.
/// </summary>
/// <remarks>
/// Estas sobrecargas son <b>aditivas</b>: no modifican ninguna firma anterior. El alcance responde a la decisión
/// <c>D-20</c> opción (b) de la épica 06 de la versión 2.2.0 · se ofrecen en la familia genérica <c>LogMlResult</c> y
/// en las seis familias de nivel. Cuando el argumento es un registrador tipado, el compilador elige estas
/// sobrecargas porque la conversión es de identidad; cuando es un registrador sin categoría, elige las anteriores.
/// No se combinan con el identificador de evento para no multiplicar la superficie pública.
/// </remarks>
public static partial class MlResultLogExtensions
{


    #region LogMlResult con registrador tipado

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel que se indique, usando un registrador tipado por
    /// categoría.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    /// <remarks>
    /// Los constructores de mensaje sólo se invocan si el registrador acepta el nivel indicado, igual que en la
    /// sobrecarga sin categoría.
    /// </remarks>
    /// <example>
    /// <code>
    /// // 'logger' es el ILogger&lt;PedidosService&gt; que inyecta el contenedor.
    /// var resultado = origen.LogMlResult(logger,
    ///                                    LogLevel.Information,
    ///                                    validBuildMessage: valor  =&gt; $"Valor calculado: {valor}",
    ///                                    failBuildMessage : errors =&gt; errors.GetErrorsSummary());
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResult<T, TCategory>(this MlResult<T>                  source,
                                                             ILogger<TCategory>           logger,
                                                             LogLevel                     logLevel,
                                                             Func<T, string>              validBuildMessage = null!,
                                                             Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, logLevel, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel que se indique, usando un registrador tipado por
    /// categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultAsync<T, TCategory>(this MlResult<T>                  source,
                                                                        ILogger<TCategory>           logger,
                                                                        LogLevel                     logLevel,
                                                                        Func<T, string>              validBuildMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, logLevel, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel que se indique, usando
    /// un registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultAsync<T, TCategory>(this Task<MlResult<T>>            sourceAsync,
                                                                              ILogger<TCategory>           logger,
                                                                              LogLevel                     logLevel,
                                                                              Func<T, string>              validBuildMessage = null!,
                                                                              Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResult((ILogger)logger, logLevel, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultTrace con registrador tipado

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Trace"/>, usando un
    /// registrador tipado por categoría.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    /// <example>
    /// <code>
    /// // 'logger' es el ILogger&lt;PedidosService&gt; que inyecta el contenedor.
    /// var resultado = origen.LogMlResultTrace(logger, validBuildMessage: valor =&gt; $"Paso intermedio: {valor}");
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultTrace<T, TCategory>(this MlResult<T>                  source,
                                                                  ILogger<TCategory>           logger,
                                                                  Func<T, string>              validBuildMessage = null!,
                                                                  Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Trace, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Trace"/>, usando un
    /// registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultTraceAsync<T, TCategory>(this MlResult<T>                  source,
                                                                             ILogger<TCategory>           logger,
                                                                             Func<T, string>              validBuildMessage = null!,
                                                                             Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Trace, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Trace"/>, usando un registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultTraceAsync<T, TCategory>(this Task<MlResult<T>>            sourceAsync,
                                                                                   ILogger<TCategory>           logger,
                                                                                   Func<T, string>              validBuildMessage = null!,
                                                                                   Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResult((ILogger)logger, LogLevel.Trace, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultDebug con registrador tipado

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Debug"/>, usando un
    /// registrador tipado por categoría.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    /// <example>
    /// <code>
    /// // 'logger' es el ILogger&lt;PedidosService&gt; que inyecta el contenedor.
    /// var resultado = origen.LogMlResultDebug(logger, validBuildMessage: valor =&gt; $"Consulta resuelta: {valor}");
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultDebug<T, TCategory>(this MlResult<T>                  source,
                                                                  ILogger<TCategory>           logger,
                                                                  Func<T, string>              validBuildMessage = null!,
                                                                  Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Debug, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Debug"/>, usando un
    /// registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultDebugAsync<T, TCategory>(this MlResult<T>                  source,
                                                                             ILogger<TCategory>           logger,
                                                                             Func<T, string>              validBuildMessage = null!,
                                                                             Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Debug, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Debug"/>, usando un registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultDebugAsync<T, TCategory>(this Task<MlResult<T>>            sourceAsync,
                                                                                   ILogger<TCategory>           logger,
                                                                                   Func<T, string>              validBuildMessage = null!,
                                                                                   Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResult((ILogger)logger, LogLevel.Debug, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultInformation con registrador tipado

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Information"/>, usando un
    /// registrador tipado por categoría.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    /// <example>
    /// <code>
    /// // 'logger' es el ILogger&lt;PedidosService&gt; que inyecta el contenedor.
    /// var resultado = origen.LogMlResultInformation(logger, validBuildMessage: valor =&gt; $"Pedido {valor} creado");
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultInformation<T, TCategory>(this MlResult<T>                  source,
                                                                        ILogger<TCategory>           logger,
                                                                        Func<T, string>              validBuildMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Information, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Information"/>, usando un
    /// registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultInformationAsync<T, TCategory>(this MlResult<T>                  source,
                                                                                   ILogger<TCategory>           logger,
                                                                                   Func<T, string>              validBuildMessage = null!,
                                                                                   Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Information, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Information"/>, usando un registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultInformationAsync<T, TCategory>(this Task<MlResult<T>>            sourceAsync,
                                                                                         ILogger<TCategory>           logger,
                                                                                         Func<T, string>              validBuildMessage = null!,
                                                                                         Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResult((ILogger)logger, LogLevel.Information, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultWarning con registrador tipado

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Warning"/>, usando un
    /// registrador tipado por categoría.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    /// <example>
    /// <code>
    /// // 'logger' es el ILogger&lt;PedidosService&gt; que inyecta el contenedor.
    /// var resultado = origen.LogMlResultWarning(logger, failBuildMessage: errors =&gt; errors.GetErrorsSummary());
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultWarning<T, TCategory>(this MlResult<T>                  source,
                                                                    ILogger<TCategory>           logger,
                                                                    Func<T, string>              validBuildMessage = null!,
                                                                    Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Warning, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Warning"/>, usando un
    /// registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultWarningAsync<T, TCategory>(this MlResult<T>                  source,
                                                                               ILogger<TCategory>           logger,
                                                                               Func<T, string>              validBuildMessage = null!,
                                                                               Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Warning, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Warning"/>, usando un registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningAsync<T, TCategory>(this Task<MlResult<T>>            sourceAsync,
                                                                                     ILogger<TCategory>           logger,
                                                                                     Func<T, string>              validBuildMessage = null!,
                                                                                     Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResult((ILogger)logger, LogLevel.Warning, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultError con registrador tipado

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Error"/>, usando un
    /// registrador tipado por categoría.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    /// <example>
    /// <code>
    /// // 'logger' es el ILogger&lt;PedidosService&gt; que inyecta el contenedor.
    /// var resultado = origen.LogMlResultError(logger, failBuildMessage: errors =&gt; errors.GetErrorsSummary());
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultError<T, TCategory>(this MlResult<T>                  source,
                                                                  ILogger<TCategory>           logger,
                                                                  Func<T, string>              validBuildMessage = null!,
                                                                  Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Error, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Error"/>, usando un
    /// registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultErrorAsync<T, TCategory>(this MlResult<T>                  source,
                                                                             ILogger<TCategory>           logger,
                                                                             Func<T, string>              validBuildMessage = null!,
                                                                             Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Error, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Error"/>, usando un registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultErrorAsync<T, TCategory>(this Task<MlResult<T>>            sourceAsync,
                                                                                   ILogger<TCategory>           logger,
                                                                                   Func<T, string>              validBuildMessage = null!,
                                                                                   Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResult((ILogger)logger, LogLevel.Error, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


    #region LogMlResultCritical con registrador tipado

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Critical"/>, usando un
    /// registrador tipado por categoría.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>El resultado de origen, sin alterar.</returns>
    /// <example>
    /// <code>
    /// // 'logger' es el ILogger&lt;PedidosService&gt; que inyecta el contenedor.
    /// var resultado = origen.LogMlResultCritical(logger, failBuildMessage: errors =&gt; errors.GetErrorsSummary());
    /// </code>
    /// </example>
    public static MlResult<T> LogMlResultCritical<T, TCategory>(this MlResult<T>                  source,
                                                                     ILogger<TCategory>           logger,
                                                                     Func<T, string>              validBuildMessage = null!,
                                                                     Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Critical, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza de <see cref="MlResult{T}"/> con el nivel <see cref="LogLevel.Critical"/>, usando un
    /// registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static Task<MlResult<T>> LogMlResultCriticalAsync<T, TCategory>(this MlResult<T>                  source,
                                                                                ILogger<TCategory>           logger,
                                                                                Func<T, string>              validBuildMessage = null!,
                                                                                Func<MlErrorDetails, string> failBuildMessage  = null!)
        => source.LogMlResult((ILogger)logger, LogLevel.Critical, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza de la <see cref="MlResult{T}"/> producida por una tarea, con el nivel
    /// <see cref="LogLevel.Critical"/>, usando un registrador tipado por categoría. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TCategory">Categoría del registrador tipado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador tipado al que se envían las trazas.</param>
    /// <param name="validBuildMessage">Constructor del mensaje que se registra cuando el resultado es válido; recibe el valor transportado. Es opcional: si es nulo no se registra nada para el caso válido.</param>
    /// <param name="failBuildMessage">Constructor del mensaje que se registra cuando el resultado es fallido; recibe los detalles del error. Es opcional: si es nulo no se registra nada para el caso fallido.</param>
    /// <returns>Tarea que produce el resultado de origen, sin alterar.</returns>
    public static async Task<MlResult<T>> LogMlResultCriticalAsync<T, TCategory>(this Task<MlResult<T>>            sourceAsync,
                                                                                      ILogger<TCategory>           logger,
                                                                                      Func<T, string>              validBuildMessage = null!,
                                                                                      Func<MlErrorDetails, string> failBuildMessage  = null!)
        => (await sourceAsync).LogMlResult((ILogger)logger, LogLevel.Critical, validBuildMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    #endregion


}
