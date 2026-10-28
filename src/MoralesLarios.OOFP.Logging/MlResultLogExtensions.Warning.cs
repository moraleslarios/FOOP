// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Métodos de extension de <c>MlResultLog</c>.
/// </summary>
public static partial class MlResultLogExtensions
{





    #region LogMlResultWarning


    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c>.
    /// </summary>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<ILogger> LogMlResultWarning(this ILogger  logger,
                                                                string   message)
        => logger.LogMlResult(LogLevel.Warning, message);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c>. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<ILogger>> LogMlResultWarningAsync(this ILogger  logger,
                                                                           string   message)
        => logger.LogMlResultAsync(LogLevel.Warning, message);


    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuilMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarning<T>(this MlResult<T>                   source, 
                                                             ILogger                       logger,
                                                             Func<T, string>               validBuilMessage = null!,
                                                             Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Warning, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuilMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningAsync<T>(this MlResult<T>                   source, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuilMessage = null!,
                                                                        Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Warning, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage).ToAsync();


    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuilMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                              ILogger                       logger,
                                                                              Func<T, string>               validBuilMessage = null!,
                                                                              Func<MlErrorDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Warning, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);


    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarning<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      validMessage = null!,
                                                             string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Warning, 
                                 validBuilMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this MlResult<T> source, 
                                                                              ILogger     logger,
                                                                              string      validMessage = null!,
                                                                              string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Warning, 
                                            validBuilMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                              ILogger           logger,
                                                                              string            validMessage = null!,
                                                                              string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Warning, 
                                                 validBuilMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);




    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarning<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      message)
        => source.LogMlResult<T>(logger, LogLevel.Warning, message);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Warning, message);


    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>> source, 
                                                                              ILogger           logger,
                                                                              string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Warning, message);



    #endregion


    #region LogMlResultWarningIfValid


    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfValid<T>(this MlResult<T> source,
                                                                    ILogger     logger,
                                                                    string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this MlResult<T> source,
                                                                               ILogger     logger,
                                                                               string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validMessage).ToAsync();

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Warning, validMessage);
    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfValid<T>(this MlResult<T>     source,
                                                                    ILogger         logger,
                                                                    Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this MlResult<T>     source,
                                                                               ILogger         logger,
                                                                               Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Warning, validBuildMessage);


    #endregion


    #region LogMlResultWarningIfFail

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Warning, errorMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this MlResult<T> source,
                                                                              ILogger     logger,
                                                                              string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Warning, errorMessage).ToAsync();

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Warning, failMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFail<T>(this MlResult<T>                   source,
                                                                   ILogger                       logger,
                                                                   Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this MlResult<T>                   source,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Warning, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                    ILogger                       logger,
                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Warning, failBuildMessage);


    #endregion


    #region LogMlResultWarningIfFailWithValue


    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithValue<T>(this MlResult<T>                   source,
                                                                            ILogger                       logger,
                                                                            Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithValue<T>(this MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Warning, failMessage);




    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="faildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                             ILogger     logger,
                                                                                             string      faildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, faildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="faildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                             ILogger           logger,
                                                                                             string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, faildMessage);






    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                                    ILogger                               logger,
                                                                                    Func<MlErrorDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);



    #endregion


    #region LogMlResultWarningIfFailWithException


    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithException<T>(this MlResult<T>                   source,
                                                                                ILogger                       logger,
                                                                                Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithException<T>(this MlResult<T> source,
                                                                                ILogger     logger,
                                                                                string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Warning, failMessage);




    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="faildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                                 ILogger     logger,
                                                                                                 string      faildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, faildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="faildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                                 ILogger           logger,
                                                                                                 string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, faildMessage);






    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithException<T>(this MlResult<T>                              source,
                                                                                ILogger                                  logger,
                                                                                Func<MlErrorDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);




    #endregion


    #region LogMlResultWarningIfFailWithoutException


    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultWarningIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Warning, failMessage);


    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                                    ILogger     logger,
                                                                                                    string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Warning, failMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResultWarning</c> unicamente cuando el resultado de origen
    /// es fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                                    ILogger           logger,
                                                                                                    string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Warning, failMessage);


    #endregion















    #region old

    //public static MlResult<ILogger> LogMlResultWarning(this ILogger logger,
    //                                                      string message)
    //    => logger.LogMlResult(LogLevel.Warning, message);

    //public static Task<MlResult<ILogger>> LogMlResultWarningAsync(this ILogger logger,
    //                                                                 string message)
    //    => logger.LogMlResultAsync(LogLevel.Warning, message);


    //public static MlResult<T> LogMlResultWarning<T>(this MlResult<T> value,
    //                                                   ILogger logger,
    //                                                   Func<T, string> validBuildMessage = null!,
    //                                                   Func<MlErrorDetails, string> failBuildMessage = null!)
    //    => value.LogMlResult<T>(logger, LogLevel.Warning, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);



    //public static MlResult<T> LogMlResultWarning<T>(this MlResult<T> value,
    //                                                   ILogger logger,
    //                                                   string message)
    //=> value.LogMlResult<T>(logger, LogLevel.Warning, message);

    //public static Task<MlResult<T>> LogMlResultWarningAsync<T>(this MlResult<T> value,
    //                                                              ILogger logger,
    //                                                              string message)
    //    => value.LogMlResultAsync<T>(logger, LogLevel.Warning, message);


    //public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>> value,
    //                                                                    ILogger logger,
    //                                                                    string message)
    //    => await value.LogMlResultAsync<T>(logger, LogLevel.Warning, message);


    //public static MlResult<T> LogMlResultIfValidWarning<T>(this MlResult<T> value,
    //                                                          ILogger logger,
    //                                                          Func<T, string> validBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Warning, validBuilMessage: validBuildMessage);

    //public static MlResult<T> LogMlResultIfFailWarning<T>(this MlResult<T> value,
    //                                                          ILogger logger,
    //                                                          Func<MlErrorDetails, string> errorBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Warning, failBuildMessage: errorBuildMessage);


    //public static MlResult<T> LogMlResultWarning<T>(this MlResult<T> value,
    //                                                   ILogger logger,
    //                                                   string validMessage = null!,
    //                                                   string failMessage = null!)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Warning,
    //                          validBuilMessage: x => validMessage,
    //                          failBuildMessage: errors => failMessage);

    //public static MlResult<T> LogMlResultIfValidWarning<T>(this MlResult<T> value,
    //                                                          ILogger logger,
    //                                                          string validMessage)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Warning,
    //                          validBuilMessage: x => validMessage);


    //public static MlResult<T> LogMlResultIfFailWarning<T>(this MlResult<T> value,
    //                                                          ILogger logger,
    //                                                          string failMessage)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Warning,
    //                          failBuildMessage: errors => failMessage);











    //public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                    ILogger logger,
    //                                                                    Func<T, string> validBuildMessage = null!,
    //                                                                    Func<MlErrorDetails, string> failBuildMessage = null!)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Warning, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    //public static async Task<MlResult<T>> LogMlResultIfValidWarningAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                           ILogger logger,
    //                                                                           Func<T, string> validBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Warning, validBuilMessage: validBuildMessage);

    //public static async Task<MlResult<T>> LogMlResultIfFailWarningAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                           ILogger logger,
    //                                                                           Func<MlErrorDetails, string> errorBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Warning, failBuildMessage: errorBuildMessage);


    //public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                    ILogger logger,
    //                                                                    string vallidMessage = null!,
    //                                                                    string failMessage = null!)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                       LogLevel.Warning,
    //                                       validBuilMessage: x => vallidMessage,
    //                                       failBuildMessage: errors => failMessage);

    //public static async Task<MlResult<T>> LogMlResultIfValidWarningAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                           ILogger logger,
    //                                                                           string successMessage)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                       LogLevel.Warning,
    //                                       validBuilMessage: x => successMessage);


    //public static async Task<MlResult<T>> LogMlResultIfFailWarningAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                           ILogger logger,
    //                                                                           string errorMessage)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                       LogLevel.Warning,
    //                                       failBuildMessage: errors => errorMessage);




    #endregion







}

