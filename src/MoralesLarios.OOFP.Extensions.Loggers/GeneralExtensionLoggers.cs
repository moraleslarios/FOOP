


using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.Extensions.Loggers;
public static class GeneralExtensionLoggers
{



    #region LogMlResult


    public static MlResult<ILogger> LogMlResult(this ILogger  logger, 
                                                     LogLevel logLevel, 
                                                     string   message)
    {
        logger.Log(logLevel, message);

        return logger.ToMlResultValid();
    }

    public static Task<MlResult<ILogger>> LogMlResultAsync(this ILogger  logger, 
                                                                LogLevel logLevel, 
                                                                string   message)
    {
        logger.Log(logLevel, message);

        return logger.ToMlResultValidAsync();
    }


    public static MlResult<T> LogMlResult<T>(this MlResult<T>                   source, 
                                                  ILogger                       logger, 
                                                  LogLevel                      logLevel, 
                                                  Func<T, string>               validBuilMessage = null!,
                                                  Func<MlErrorsDetails, string> failBuildMessage = null!)
    {
        source.Match(
                        valid: x      => validBuilMessage != null ? logger.LogMlResult(logLevel, validBuilMessage(x))    : null!,
                        fail : errors => failBuildMessage != null ? logger.LogMlResult(logLevel, failBuildMessage(errors)) : null!
                    );

        return source;
    }

    public static Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T>                   source, 
                                                             ILogger                       logger, 
                                                             LogLevel                      logLevel, 
                                                             Func<T, string>               validBuilMessage = null!,
                                                             Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, logLevel, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage).ToAsync();


    public static async Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                   ILogger                       logger, 
                                                                   LogLevel                      logLevel, 
                                                                   Func<T, string>               validBuilMessage = null!,
                                                                   Func<MlErrorsDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, logLevel, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);


    public static MlResult<T> LogMlResult<T>(this MlResult<T> source, 
                                                  ILogger     logger, 
                                                  LogLevel    logLevel, 
                                                  string      validMessage = null!,
                                                  string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 logLevel, 
                                 validBuilMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T> source, 
                                                                   ILogger     logger, 
                                                                   LogLevel    logLevel, 
                                                                   string      validMessage = null!,
                                                                   string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            logLevel, 
                                            validBuilMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                   ILogger           logger, 
                                                                   LogLevel          logLevel, 
                                                                   string            validMessage = null!,
                                                                   string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 logLevel, 
                                                 validBuilMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);




    public static MlResult<T> LogMlResult<T>(this MlResult<T> source, 
                                                  ILogger     logger, 
                                                  LogLevel    logLevel, 
                                                  string      message)
    {
        logger.Log(logLevel, message);

        return source;
    }

    public static Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T> source, 
                                                             ILogger     logger, 
                                                             LogLevel    logLevel, 
                                                             string      message)
    {
        logger.Log(logLevel, message);

        return source.ToAsync();
    }


    public static async Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>> source, 
                                                                   ILogger           logger, 
                                                                   LogLevel          logLevel, 
                                                                   string            message)
    {
        _ = await source;

        logger.Log(logLevel, message);

        return await source;
    }





    public static MlResult<T> LogMlResult<T>(this MlResult<T>                                                     source, 
                                                  ILogger                                                         logger, 
                                                  (LogLevel logLevel, Func<T              , string> buildMessage) validBuildMessage,
                                                  (LogLevel logLevel, Func<MlErrorsDetails, string> buildMessage) failBuildMessage)
    {
        source.Match(
                        valid: x      => logger.LogMlResult(validBuildMessage.logLevel, validBuildMessage.buildMessage(x)),
                        fail : errors => logger.LogMlResult(failBuildMessage .logLevel, failBuildMessage .buildMessage(errors))
                    );

        return source;
    }


    public static Task<MlResult<T>> LogMlResultAsync<T>(this MlResult<T>                                                     source, 
                                                             ILogger                                                         logger, 
                                                             (LogLevel logLevel, Func<T              , string> buildMessage) validBuildMessage,
                                                             (LogLevel logLevel, Func<MlErrorsDetails, string> buildMessage) failBuildMessage)
        => source.LogMlResult(logger, validBuildMessage, failBuildMessage).ToAsync();

    public async static Task<MlResult<T>> LogMlResultAsync<T>(this Task<MlResult<T>>                                               sourceAsync, 
                                                                   ILogger                                                         logger, 
                                                                   (LogLevel logLevel, Func<T              , string> buildMessage) validBuildMessage,
                                                                   (LogLevel logLevel, Func<MlErrorsDetails, string> buildMessage) failBuildMessage)
        => (await sourceAsync).LogMlResult(logger, validBuildMessage, failBuildMessage);



    public static MlResult<T> LogMlResultFinal<T>(this MlResult<T>                   source,
                                                       ILogger                       logger,
                                                       Func<T, string>               validBuildMessage,
                                                       Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResult(logger,
                              (LogLevel.Information, validBuildMessage),
                              (LogLevel.Error      , failBuildMessage));

    public static Task<MlResult<T>> LogMlResultFinalAsync<T>(this MlResult<T>                   source,
                                                                  ILogger                       logger,
                                                                  Func<T, string>               validBuildMessage,
                                                                  Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultFinal(logger, validBuildMessage, failBuildMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultFinalAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuildMessage,
                                                                        Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultFinalAsync(logger, validBuildMessage, failBuildMessage);

    public static MlResult<T> LogMlResultFinal<T>(this MlResult<T>  source,
                                                       ILogger      logger,
                                                       string       validMessage,
                                                       string       failMessage)
        => source.LogMlResultFinal(logger,
                                   _ => validMessage,
                                   _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultFinal<T>(this Task<MlResult<T>>  sourceAsync,
                                                                   ILogger            logger,
                                                                   string             validMessage,
                                                                   string             failMessage)
        => await sourceAsync.LogMlResultFinalAsync(logger, _ => validMessage, _ => failMessage);





    #endregion


    #region LogMlResultIfValid


    public static MlResult<T> LogMlResultIfValid<T>(this MlResult<T> source,
                                                         ILogger     logger,
                                                         LogLevel    logLevel,
                                                         string      validMessage)
        => source.LogMlResult<T>(logger, 
                              logLevel, 
                              validBuilMessage: x => validMessage);

    public static Task<MlResult<T>> LogMlResultIfValidAsync<T>(this MlResult<T> source,
                                                                    ILogger     logger,
                                                                    LogLevel    logLevel,
                                                                    string      validMessage)
        => source.LogMlResultIfValid(logger, logLevel, validMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                          ILogger           logger,
                                                                          LogLevel          logLevel,
                                                                          string            validMessage)
        => (await sourceAsync).LogMlResult<T>(logger, 
                                           logLevel, 
                                           validBuilMessage: x => validMessage);

    public static MlResult<T> LogMlResultIfValid<T>(this MlResult<T>     source,
                                                         ILogger         logger,
                                                         LogLevel        logLevel,
                                                         Func<T, string> validBuildMessage)
        => source.Match(
                            valid: x      => source.LogMlResult(logger, logLevel, validBuildMessage(x)),
                            fail : errors => source
                        );

    public static Task<MlResult<T>> LogMlResultIfValidAsync<T>(this MlResult<T>     source,
                                                                    ILogger         logger,
                                                                    LogLevel        logLevel,
                                                                    Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, logLevel, validBuildMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                          ILogger           logger,
                                                                          LogLevel          logLevel,
                                                                          Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, logLevel, validBuildMessage);


    #endregion


    #region LogMlResultIfFail

    public static MlResult<T> LogMlResultIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        LogLevel    logLevel,
                                                        string      errorMessage)
        => source.LogMlResult<T>(logger, 
                              logLevel, 
                              failBuildMessage: errors => errorMessage);

    public static Task<MlResult<T>> LogMlResultIfFailAsync<T>(this MlResult<T> source,
                                                                   ILogger     logger,
                                                                   LogLevel    logLevel,
                                                                   string      errorMessage)
        => source.LogMlResultIfFail(logger, logLevel, errorMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                         ILogger           logger,
                                                                         LogLevel          logLevel,
                                                                         string            failMessage)
        => (await sourceAsync).LogMlResult<T>(logger, 
                                              logLevel, 
                                              failBuildMessage: errors => failMessage);
    public static MlResult<T> LogMlResultIfFail<T>(this MlResult<T>                   source,
                                                        ILogger                       logger,
                                                        LogLevel                      logLevel,
                                                        Func<MlErrorsDetails, string> failBuildMessage)
        => source.Match(
                            valid: x      => source,

                            fail : errors => source.LogMlResult(logger, logLevel, failBuildMessage(errors))
                        );

    public static Task<MlResult<T>> LogMlResultIfFailAsync<T>(this MlResult<T>                   source,
                                                                   ILogger                       logger,
                                                                   LogLevel                      logLevel,
                                                                   Func<MlErrorsDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, logLevel, failBuildMessage).ToAsync();

    public async static Task<MlResult<T>> LogMlResultIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                         ILogger                       logger,
                                                                         LogLevel                      logLevel,
                                                                         Func<MlErrorsDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, logLevel, failBuildMessage);


    #endregion


    #region LogMlResultIfFailWithValue


    public static MlResult<T> LogMlResultIfFailWithValue<T>(this MlResult<T>                   source,
                                                                 ILogger                       logger,
                                                                 LogLevel                      logLevel,
                                                                 Func<MlErrorsDetails, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetDetailValue<T>()
                                                        .Bind( _ => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails)))
                        );

    public static MlResult<T> LogMlResultIfFailWithValue<T>(this MlResult<T> source,
                                                                 ILogger     logger,
                                                                 LogLevel    logLevel,
                                                                 string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, logLevel, _ => failMessage);




    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                  ILogger                       logger,
                                                                                  LogLevel                      logLevel,
                                                                                  Func<MlErrorsDetails, string> failBuildMessage)
        => await source.MatchAsync(
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetDetailValueAsync<T>()
                                                                        .BindAsync( _ => source.LogMlResultAsync(logger, logLevel, failBuildMessage(errorsDetails)))
                                    );

    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                  ILogger     logger,
                                                                                  LogLevel    logLevel,
                                                                                  string      faildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, logLevel, _ => faildMessage);

    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                  ILogger                       logger,
                                                                                  LogLevel                      logLevel,
                                                                                  Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, logLevel, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                  ILogger           logger,
                                                                                  LogLevel          logLevel,
                                                                                  string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, logLevel, _ => faildMessage);






    public static MlResult<T> LogMlResultIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                         ILogger                               logger,
                                                                         LogLevel                              logLevel,
                                                                         Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetDetailValue<TValue>()
                                                        .Bind( value => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails, value)))
                        );

    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                          ILogger                               logger,
                                                                                          LogLevel                              logLevel,
                                                                                          Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await source.MatchAsync(           
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetDetailValueAsync<TValue>()
                                                                        .BindAsync( value => source.LogMlResultAsync(logger, logLevel, failBuildMessage(errorsDetails, value)))
                                    );

    public static async Task<MlResult<T>> LogMlResultIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                          ILogger                               logger,
                                                                                          LogLevel                              logLevel,
                                                                                          Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, logLevel, failBuildMessage);



    #endregion


    #region LogMlResultIfFailWithException


    public static MlResult<T> LogMlResultIfFailWithException<T>(this MlResult<T>                   source,
                                                                     ILogger                       logger,
                                                                     LogLevel                      logLevel,
                                                                     Func<MlErrorsDetails, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetDetailException()
                                                        .Bind( _ => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails)))
                        );

    public static MlResult<T> LogMlResultIfFailWithException<T>(this MlResult<T> source,
                                                                     ILogger     logger,
                                                                     LogLevel    logLevel,
                                                                     string      failMessage)
        => source.LogMlResultIfFailWithException(logger, logLevel, _ => failMessage);




    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                      ILogger                       logger,
                                                                                      LogLevel                      logLevel,
                                                                                      Func<MlErrorsDetails, string> failBuildMessage)
        => await source.MatchAsync(
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync()
                                                                        .BindAsync( _ => source.LogMlResultAsync(logger, logLevel, failBuildMessage(errorsDetails)))
                                    );

    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                      ILogger     logger,
                                                                                      LogLevel    logLevel,
                                                                                      string      faildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, logLevel, _ => faildMessage);

    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                      ILogger                       logger,
                                                                                      LogLevel                      logLevel,
                                                                                      Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, logLevel, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                      ILogger           logger,
                                                                                      LogLevel          logLevel,
                                                                                      string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, logLevel, _ => faildMessage);






    public static MlResult<T> LogMlResultIfFailWithException<T>(this MlResult<T>                              source,
                                                                     ILogger                                  logger,
                                                                     LogLevel                                 logLevel,
                                                                     Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => source.Match(
                            valid: x             => source,
                            fail : errorsDetails => errorsDetails.GetDetailException()
                                                        .Bind( ex => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails, ex)))
                        );

    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                      ILogger                                  logger,
                                                                                      LogLevel                                 logLevel,
                                                                                      Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await source.MatchAsync(           
                                        validAsync: x             => source.ToAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync()
                                                                        .BindAsync( ex => source.LogMlResultAsync(logger, logLevel, failBuildMessage(errorsDetails, ex)))
                                    );

    public static async Task<MlResult<T>> LogMlResultIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                            sourceAsync,
                                                                                          ILogger                                  logger,
                                                                                          LogLevel                                 logLevel,
                                                                                          Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, logLevel, failBuildMessage);




    #endregion


    #region LogMlResultIfFailWithoutException


    public static MlResult<T> LogMlResultIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                        ILogger                       logger,
                                                                        LogLevel                      logLevel,
                                                                        Func<MlErrorsDetails, string> failBuildMessage)
        => source.Match(
                            valid: x            => source,
                            fail: errorsDetails => errorsDetails.GetDetailException()
                                                        .BindIfFail(
                                                                        funcFail : _ => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails)),
                                                                        funcValid: _ => source
                                                                    )
                        );

    public static MlResult<T> LogMlResultIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                        ILogger                       logger,
                                                                        LogLevel                      logLevel,
                                                                        string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, logLevel, _ => failMessage);


    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                         ILogger                       logger,
                                                                                         LogLevel                      logLevel,
                                                                                         Func<MlErrorsDetails, string> failBuildMessage)
        => await source.MatchAsync(
                                    validAsync: x             => source.ToAsync(),
                                    failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync()
                                                                .BindIfFailAsync(
                                                                                    funcFailAsync : _ => source.LogMlResultAsync(logger, logLevel, failBuildMessage(errorsDetails)),
                                                                                    funcValidAsync: _ => source.ToAsync()
                                                                                )
                        );

    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                         ILogger     logger,
                                                                                         LogLevel    logLevel,
                                                                                         string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, logLevel, _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                         ILogger                       logger,
                                                                                         LogLevel                      logLevel,
                                                                                         Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, logLevel, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                         ILogger           logger,
                                                                                         LogLevel          logLevel,
                                                                                         string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, logLevel, _ => failMessage);


    #endregion


    #region LogGeneralErrorIfFail


    public static MlResult<T> LogGeneralErrorIfFail<T>(this MlResult<T> source, ILogger logger)
        => source.LogMlResultErrorIfFail(logger, failBuildMessage : errorDetals => errorDetals.ToErrorsDescription());

    public static Task<MlResult<T>> LogGeneralErrorIfFailAsync<T>(this MlResult<T> source, ILogger logger)
        => source.LogMlResultErrorIfFail(logger, failBuildMessage : errorDetals => errorDetals.ToErrorsDescription()).ToAsync();

    public static async Task<MlResult<T>> LogGeneralErrorIfFailAsync<T>(this Task<MlResult<T>> sourceAsync, ILogger logger)
        => (await sourceAsync).LogMlResultErrorIfFail(logger, failBuildMessage : errorDetals => errorDetals.ToErrorsDescription());



    #endregion




}
