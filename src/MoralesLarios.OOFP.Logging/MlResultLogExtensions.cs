// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0




using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Métodos de extension de <c>MlResultLog</c>.
/// </summary>
public static partial class MlResultLogExtensions
{



    #region LogMlResult


    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>.
    /// </summary>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<ILogger> LogMlResult(this ILogger  logger, 
                                                     LogLevel logLevel, 
                                                     string   message)
    {
        logger.Log(logLevel, message);

        return logger.ToMlResultValid();
    }

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<ILogger>> LogMlResultAsync(this ILogger  logger, 
                                                                LogLevel logLevel, 
                                                                string   message)
    {
        logger.Log(logLevel, message);

        return logger.ToMlResultValidAsync();
    }


    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuilMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResult<T>(this MlResult<T>                   source, 
                                                  ILogger                       logger, 
                                                  LogLevel                      logLevel, 
                                                  Func<T, string>               validBuilMessage = null!,
                                                  Func<MlErrorDetails, string> failBuildMessage = null!)
    {
        source.Match(
                        valid: x      => validBuilMessage != null ? logger.LogMlResult(logLevel, validBuilMessage(x))    : null!,
                        fail : errors => failBuildMessage != null ? logger.LogMlResult(logLevel, failBuildMessage(errors)) : null!
                    );

        return source;
    }

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuilMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T>                   source, 
                                                             ILogger                       logger, 
                                                             LogLevel                      logLevel, 
                                                             Func<T, string>               validBuilMessage = null!,
                                                             Func<MlErrorDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, logLevel, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage).ToAsync();


    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuilMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                   ILogger                       logger, 
                                                                   LogLevel                      logLevel, 
                                                                   Func<T, string>               validBuilMessage = null!,
                                                                   Func<MlErrorDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, logLevel, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);


    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResult<T>(this MlResult<T> source, 
                                                  ILogger     logger, 
                                                  LogLevel    logLevel, 
                                                  string      validMessage = null!,
                                                  string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 logLevel, 
                                 validBuilMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T> source, 
                                                                   ILogger     logger, 
                                                                   LogLevel    logLevel, 
                                                                   string      validMessage = null!,
                                                                   string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            logLevel, 
                                            validBuilMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                   ILogger           logger, 
                                                                   LogLevel          logLevel, 
                                                                   string            validMessage = null!,
                                                                   string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 logLevel, 
                                                 validBuilMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);




    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResult<T>(this MlResult<T> source, 
                                                  ILogger     logger, 
                                                  LogLevel    logLevel, 
                                                  string      message)
    {
        logger.Log(logLevel, message);

        return source;
    }

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T> source, 
                                                             ILogger     logger, 
                                                             LogLevel    logLevel, 
                                                             string      message)
    {
        logger.Log(logLevel, message);

        return source.ToAsync();
    }


    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>> source, 
                                                                   ILogger           logger, 
                                                                   LogLevel          logLevel, 
                                                                   string            message)
    {
        _ = await source;

        logger.Log(logLevel, message);

        return await source;
    }





    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResult<T>(this MlResult<T>                                                     source, 
                                                  ILogger                                                         logger, 
                                                  (LogLevel logLevel, Func<T              , string> buildMessage) validBuildMessage,
                                                  (LogLevel logLevel, Func<MlErrorDetails, string> buildMessage) failBuildMessage)
    {
        source.Match(
                        valid: x      => logger.LogMlResult(validBuildMessage.logLevel, validBuildMessage.buildMessage(x)),
                        fail : errors => logger.LogMlResult(failBuildMessage .logLevel, failBuildMessage .buildMessage(errors))
                    );

        return source;
    }


    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T>                                                     source, 
                                                             ILogger                                                         logger, 
                                                             (LogLevel logLevel, Func<T              , string> buildMessage) validBuildMessage,
                                                             (LogLevel logLevel, Func<MlErrorDetails, string> buildMessage) failBuildMessage)
        => source.LogMlResult(logger, validBuildMessage, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>>                                               sourceAsync, 
                                                                   ILogger                                                         logger, 
                                                                   (LogLevel logLevel, Func<T              , string> buildMessage) validBuildMessage,
                                                                   (LogLevel logLevel, Func<MlErrorDetails, string> buildMessage) failBuildMessage)
        => (await sourceAsync).LogMlResult(logger, validBuildMessage, failBuildMessage);



    /// <summary>
    /// Registra una traza del resultado de <c>CompletedResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogCompletedResult<T>(this MlResult<T>                   source,
                                                       ILogger                       logger,
                                                       Func<T, string>               validBuildMessage,
                                                       Func<MlErrorDetails, string> failBuildMessage)
        => source.LogMlResult(logger,
                              (LogLevel.Information, validBuildMessage),
                              (LogLevel.Error      , failBuildMessage));

    /// <summary>
    /// Registra una traza del resultado de <c>CompletedResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogCompletedResultAsync<T>(this MlResult<T>                   source,
                                                                  ILogger                       logger,
                                                                  Func<T, string>               validBuildMessage,
                                                                  Func<MlErrorDetails, string> failBuildMessage)
        => source.LogCompletedResult(logger, validBuildMessage, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza del resultado de <c>CompletedResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogCompletedResultAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuildMessage,
                                                                        Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogCompletedResultAsync(logger, validBuildMessage, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>CompletedResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogCompletedResult<T>(this MlResult<T>  source,
                                                       ILogger      logger,
                                                       string       validMessage,
                                                       string       failMessage)
        => source.LogCompletedResult(logger,
                                   _ => validMessage,
                                   _ => failMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>CompletedResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogCompletedResult<T>(this Task<MlResult<T>>  sourceAsync,
                                                                   ILogger            logger,
                                                                   string             validMessage,
                                                                   string             failMessage)
        => await sourceAsync.LogCompletedResultAsync(logger, _ => validMessage, _ => failMessage);





    #endregion


    #region LogMlResultIfValid


    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfValid<T>(this MlResult<T> source,
                                                         ILogger     logger,
                                                         LogLevel    logLevel,
                                                         string      validMessage)
        => source.LogMlResult<T>(logger, 
                              logLevel, 
                              validBuilMessage: x => validMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultIfValidAsync<T>(this MlResult<T> source,
                                                                    ILogger     logger,
                                                                    LogLevel    logLevel,
                                                                    string      validMessage)
        => source.LogMlResultIfValid(logger, logLevel, validMessage).ToAsync();

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                          ILogger           logger,
                                                                          LogLevel          logLevel,
                                                                          string            validMessage)
        => (await sourceAsync).LogMlResult<T>(logger, 
                                           logLevel, 
                                           validBuilMessage: x => validMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// válido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfValid<T>(this MlResult<T>     source,
                                                         ILogger         logger,
                                                         LogLevel        logLevel,
                                                         Func<T, string> validBuildMessage)
        => source.Match(
                            valid: x      => source.LogMlResult(logger, logLevel, validBuildMessage(x)),
                            fail : errors => source
                        );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultIfValidAsync<T>(this MlResult<T>     source,
                                                                    ILogger         logger,
                                                                    LogLevel        logLevel,
                                                                    Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, logLevel, validBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// válido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="validBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                          ILogger           logger,
                                                                          LogLevel          logLevel,
                                                                          Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, logLevel, validBuildMessage);


    #endregion


    #region LogMlResultIfFail

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        LogLevel    logLevel,
                                                        string      errorMessage)
        => source.LogMlResult<T>(logger, 
                              logLevel, 
                              failBuildMessage: errors => errorMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailAsync<T>(this MlResult<T> source,
                                                                   ILogger     logger,
                                                                   LogLevel    logLevel,
                                                                   string      errorMessage)
        => source.LogMlResultIfFail(logger, logLevel, errorMessage).ToAsync();

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                         ILogger           logger,
                                                                         LogLevel          logLevel,
                                                                         string            failMessage)
        => (await sourceAsync).LogMlResult<T>(logger, 
                                              logLevel, 
                                              failBuildMessage: errors => failMessage);
    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFail<T>(this MlResult<T>                   source,
                                                        ILogger                       logger,
                                                        LogLevel                      logLevel,
                                                        Func<MlErrorDetails, string> failBuildMessage)
        => source.Match(
                            valid: x      => source,

                            fail : errors => source.LogMlResult(logger, logLevel, failBuildMessage(errors))
                        );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogMlResultIfFailAsync<T>(this MlResult<T>                   source,
                                                                   ILogger                       logger,
                                                                   LogLevel                      logLevel,
                                                                   Func<MlErrorDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, logLevel, failBuildMessage).ToAsync();

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public async static Task<MlResult<T>> LogMlResultIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                         ILogger                       logger,
                                                                         LogLevel                      logLevel,
                                                                         Func<MlErrorDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, logLevel, failBuildMessage);


    #endregion


    #region LogMlResultIfFailWithValue


    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithValue<T>(this MlResult<T>                   source,
                                                                 ILogger                       logger,
                                                                 LogLevel                      logLevel,
                                                                 Func<MlErrorDetails, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetMetadataValue<T>()
                                                        .Bind( _ => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails)))
                        );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithValue<T>(this MlResult<T> source,
                                                                 ILogger     logger,
                                                                 LogLevel    logLevel,
                                                                 string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, logLevel, _ => failMessage);




    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                  ILogger                       logger,
                                                                                  LogLevel                      logLevel,
                                                                                  Func<MlErrorDetails, string> failBuildMessage)
        => await source.MatchAsync(
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetMetadataValueAsync<T>()
                                                                        .BindAsync( _ => source.LogMlResultAsync(logger, logLevel, failBuildMessage(errorsDetails)))
                                    );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="faildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                  ILogger     logger,
                                                                                  LogLevel    logLevel,
                                                                                  string      faildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, logLevel, _ => faildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                  ILogger                       logger,
                                                                                  LogLevel                      logLevel,
                                                                                  Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, logLevel, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="faildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                  ILogger           logger,
                                                                                  LogLevel          logLevel,
                                                                                  string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, logLevel, _ => faildMessage);






    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                         ILogger                               logger,
                                                                         LogLevel                              logLevel,
                                                                         Func<MlErrorDetails, TValue, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetMetadataValue<TValue>()
                                                        .Bind( value => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails, value)))
                        );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                          ILogger                               logger,
                                                                                          LogLevel                              logLevel,
                                                                                          Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await source.MatchAsync(           
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetMetadataValueAsync<TValue>()
                                                                        .BindAsync( value => source.LogMlResultAsync(logger, logLevel, failBuildMessage(errorsDetails, value)))
                                    );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido, recuperando el valor almacenado en el detalle del error. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TValue">Tipo del valor.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                          ILogger                               logger,
                                                                                          LogLevel                              logLevel,
                                                                                          Func<MlErrorDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, logLevel, failBuildMessage);



    #endregion


    #region LogMlResultIfFailWithException


    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithException<T>(this MlResult<T>                   source,
                                                                     ILogger                       logger,
                                                                     LogLevel                      logLevel,
                                                                     Func<MlErrorDetails, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetException()
                                                        .Bind( _ => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails)))
                        );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithException<T>(this MlResult<T> source,
                                                                     ILogger     logger,
                                                                     LogLevel    logLevel,
                                                                     string      failMessage)
        => source.LogMlResultIfFailWithException(logger, logLevel, _ => failMessage);




    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                      ILogger                       logger,
                                                                                      LogLevel                      logLevel,
                                                                                      Func<MlErrorDetails, string> failBuildMessage)
        => await source.MatchAsync(
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync()
                                                                        .BindAsync( _ => source.LogMlResultAsync(logger, logLevel, failBuildMessage(errorsDetails)))
                                    );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="faildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                      ILogger     logger,
                                                                                      LogLevel    logLevel,
                                                                                      string      faildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, logLevel, _ => faildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                      ILogger                       logger,
                                                                                      LogLevel                      logLevel,
                                                                                      Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, logLevel, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="faildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                      ILogger           logger,
                                                                                      LogLevel          logLevel,
                                                                                      string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, logLevel, _ => faildMessage);






    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithException<T>(this MlResult<T>                              source,
                                                                     ILogger                                  logger,
                                                                     LogLevel                                 logLevel,
                                                                     Func<MlErrorDetails, Exception, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetException()
                                                        .Bind( ex => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails, ex)))
                        );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                      ILogger                                  logger,
                                                                                      LogLevel                                 logLevel,
                                                                                      Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await source.MatchAsync(           
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetExceptionAsync()
                                                                        .BindAsync( ex => source.LogMlResultAsync(logger, logLevel, failBuildMessage(errorsDetails, ex)))
                                    );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                            sourceAsync,
                                                                                          ILogger                                  logger,
                                                                                          LogLevel                                 logLevel,
                                                                                          Func<MlErrorDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, logLevel, failBuildMessage);




    #endregion


    #region LogMlResultIfFailWithoutException


    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                        ILogger                       logger,
                                                                        LogLevel                      logLevel,
                                                                        Func<MlErrorDetails, string> failBuildMessage)
        => source.Match(
                            valid: x            => source,
                            fail: errorsDetails => errorsDetails.GetException()
                                                        .BindIfFail(
                                                                        funcFail : _ => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails)),
                                                                        funcValid: _ => source
                                                                    )
                        );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogMlResultIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                        ILogger                       logger,
                                                                        LogLevel                      logLevel,
                                                                        string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, logLevel, _ => failMessage);


    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                         ILogger                       logger,
                                                                                         LogLevel                      logLevel,
                                                                                         Func<MlErrorDetails, string> failBuildMessage)
        => await source.MatchAsync(
                                    validAsync: x             => source.ToAsync(),
                                    failAsync : errorsDetails => errorsDetails.GetExceptionAsync()
                                                                .BindIfFailAsync(
                                                                                    funcFailAsync : _ => source.LogMlResultAsync(logger, logLevel, failBuildMessage(errorsDetails)),
                                                                                    funcValidAsync: _ => source.ToAsync()
                                                                                )
                        );

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                         ILogger     logger,
                                                                                         LogLevel    logLevel,
                                                                                         string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, logLevel, _ => failMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failBuildMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                         ILogger                       logger,
                                                                                         LogLevel                      logLevel,
                                                                                         Func<MlErrorDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, logLevel, failBuildMessage);

    /// <summary>
    /// Registra una traza del resultado de <c>MlResult</c> unicamente cuando el resultado de origen es
    /// fallido y su detalle de error NO contiene una excepción. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="logLevel">Nivel de severidad de la traza.</param>
    /// <param name="failMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                         ILogger           logger,
                                                                                         LogLevel          logLevel,
                                                                                         string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, logLevel, _ => failMessage);


    #endregion


    #region LogGeneralErrorIfFail


    /// <summary>
    /// Registra una traza del resultado de <c>GeneralError</c> unicamente cuando el resultado de origen es
    /// fallido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogGeneralErrorIfFail<T>(this MlResult<T> source, ILogger logger)
        => source.LogMlResultErrorIfFail(logger, failBuildMessage : errorDetals => errorDetals.GetErrorsSummary());

    /// <summary>
    /// Registra una traza del resultado de <c>GeneralError</c> unicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> LogGeneralErrorIfFailAsync<T>(this MlResult<T> source, ILogger logger)
        => source.LogMlResultErrorIfFail(logger, failBuildMessage : errorDetals => errorDetals.GetErrorsSummary()).ToAsync();

    /// <summary>
    /// Registra una traza del resultado de <c>GeneralError</c> unicamente cuando el resultado de origen es
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogGeneralErrorIfFailAsync<T>(this Task<MlResult<T>> sourceAsync, ILogger logger)
        => (await sourceAsync).LogMlResultErrorIfFail(logger, failBuildMessage : errorDetals => errorDetals.GetErrorsSummary());



    #endregion




}

