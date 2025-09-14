namespace MoralesLarios.OOFP.Extensions.Loggers;
public static class GeneralExtensionLoggersTrace
{




    #region LogMlResultCritical


    public static MlResult<ILogger> LogMlResultTrace(this ILogger  logger,
                                                                string   message)
        => logger.LogMlResult(LogLevel.Trace, message);

    public static Task<MlResult<ILogger>> LogMlResultTraceAsync(this ILogger  logger,
                                                                           string   message)
        => logger.LogMlResultAsync(LogLevel.Trace, message);


    public static MlResult<T> LogMlResultTrace<T>(this MlResult<T>                   source, 
                                                             ILogger                       logger,
                                                             Func<T, string>               validBuilMessage = null!,
                                                             Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Trace, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);

    public static Task<MlResult<T>> LogMlResultTraceAsync<T>(this MlResult<T>                   source, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuilMessage = null!,
                                                                        Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Trace, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage).ToAsync();


    public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                              ILogger                       logger,
                                                                              Func<T, string>               validBuilMessage = null!,
                                                                              Func<MlErrorsDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Trace, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);


    public static MlResult<T> LogMlResultTrace<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      validMessage = null!,
                                                             string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Trace, 
                                 validBuilMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this MlResult<T> source, 
                                                                              ILogger     logger,
                                                                              string      validMessage = null!,
                                                                              string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Trace, 
                                            validBuilMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                              ILogger           logger,
                                                                              string            validMessage = null!,
                                                                              string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Trace, 
                                                 validBuilMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);




    public static MlResult<T> LogMlResultTrace<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      message)
        => source.LogMlResult<T>(logger, LogLevel.Trace, message);

    public static Task<MlResult<T>> LogMlResultTraceAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Trace, message);


    public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this Task<MlResult<T>> source, 
                                                                              ILogger           logger,
                                                                              string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Trace, message);



    #endregion


    #region LogMlResultTraceIfValid


    public static MlResult<T> LogMlResultTraceIfValid<T>(this MlResult<T> source,
                                                                    ILogger     logger,
                                                                    string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Trace, validMessage);

    public static Task<MlResult<T>> LogMlResultTraceIfValidAsync<T>(this MlResult<T> source,
                                                                               ILogger     logger,
                                                                               string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Trace, validMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultTraceIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Trace, validMessage);
    public static MlResult<T> LogMlResultTraceIfValid<T>(this MlResult<T>     source,
                                                                    ILogger         logger,
                                                                    Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Trace, validBuildMessage);

    public static Task<MlResult<T>> LogMlResultTraceIfValidAsync<T>(this MlResult<T>     source,
                                                                               ILogger         logger,
                                                                               Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Trace, validBuildMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultTraceIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Trace, validBuildMessage);


    #endregion


    #region LogMlResultTraceIfFail

    public static MlResult<T> LogMlResultTraceIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Trace, errorMessage);

    public static Task<MlResult<T>> LogMlResultTraceIfFailAsync<T>(this MlResult<T> source,
                                                                              ILogger     logger,
                                                                              string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Trace, errorMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultTraceIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Trace, failMessage);

    public static MlResult<T> LogMlResultTraceIfFail<T>(this MlResult<T>                   source,
                                                                   ILogger                       logger,
                                                                   Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Trace, failBuildMessage);

    public static Task<MlResult<T>> LogMlResultTraceIfFailAsync<T>(this MlResult<T>                   source,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorsDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Trace, failBuildMessage).ToAsync();

    public async static Task<MlResult<T>> LogMlResultTraceIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                    ILogger                       logger,
                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Trace, failBuildMessage);


    #endregion


    #region LogMlResultTraceIfFailWithValue


    public static MlResult<T> LogMlResultTraceIfFailWithValue<T>(this MlResult<T>                   source,
                                                                            ILogger                       logger,
                                                                            Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Trace, failBuildMessage);

    public static MlResult<T> LogMlResultTraceIfFailWithValue<T>(this MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Trace, failMessage);




    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                             ILogger     logger,
                                                                                             string      faildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, faildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                             ILogger           logger,
                                                                                             string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, faildMessage);






    public static MlResult<T> LogMlResultTraceIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                                    ILogger                               logger,
                                                                                    Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Trace, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Trace, failBuildMessage);



    #endregion


    #region LogMlResultTraceIfFailWithException


    public static MlResult<T> LogMlResultTraceIfFailWithException<T>(this MlResult<T>                   source,
                                                                                ILogger                       logger,
                                                                                Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Trace, failBuildMessage);

    public static MlResult<T> LogMlResultTraceIfFailWithException<T>(this MlResult<T> source,
                                                                                ILogger     logger,
                                                                                string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Trace, failMessage);




    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                                 ILogger     logger,
                                                                                                 string      faildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, faildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                                 ILogger           logger,
                                                                                                 string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, faildMessage);






    public static MlResult<T> LogMlResultTraceIfFailWithException<T>(this MlResult<T>                              source,
                                                                                ILogger                                  logger,
                                                                                Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Trace, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Trace, failBuildMessage);




    #endregion


    #region LogMlResultTraceIfFailWithoutException


    public static MlResult<T> LogMlResultTraceIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Trace, failBuildMessage);

    public static MlResult<T> LogMlResultTraceIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Trace, failMessage);


    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                                    ILogger     logger,
                                                                                                    string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Trace, failMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Trace, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultTraceIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                                    ILogger           logger,
                                                                                                    string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Trace, failMessage);


    #endregion











    #region old

    //public static MlResult<ILogger> LogMlResultTrace(this ILogger logger,
    //                                                    string message)
    //    => logger.LogMlResult(LogLevel.Trace, message);

    //public static Task<MlResult<ILogger>> LogMlResultTraceAsync(this ILogger logger,
    //                                                               string message)
    //    => logger.LogMlResultAsync(LogLevel.Trace, message);


    //public static MlResult<T> LogMlResultTrace<T>(this MlResult<T> value,
    //                                                 ILogger logger,
    //                                                 Func<T, string> validBuildMessage = null!,
    //                                                 Func<MlErrorsDetails, string> failBuildMessage = null!)
    //    => value.LogMlResult<T>(logger, LogLevel.Trace, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);



    //public static MlResult<T> LogMlResultTrace<T>(this MlResult<T> value,
    //                                                 ILogger logger,
    //                                                 string message)
    //=> value.LogMlResult<T>(logger, LogLevel.Trace, message);

    //public static Task<MlResult<T>> LogMlResultTraceAsync<T>(this MlResult<T> value,
    //                                                            ILogger logger,
    //                                                            string message)
    //    => value.LogMlResultAsync<T>(logger, LogLevel.Trace, message);


    //public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this Task<MlResult<T>> value,
    //                                                                  ILogger logger,
    //                                                                  string message)
    //    => await value.LogMlResultAsync<T>(logger, LogLevel.Trace, message);


    //public static MlResult<T> LogMlResultIfValidTrace<T>(this MlResult<T> value,
    //                                                        ILogger logger,
    //                                                        Func<T, string> validBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Trace, validBuilMessage: validBuildMessage);

    //public static MlResult<T> LogMlResultIfFailTrace<T>(this MlResult<T> value,
    //                                                        ILogger logger,
    //                                                        Func<MlErrorsDetails, string> errorBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Trace, failBuildMessage: errorBuildMessage);


    //public static MlResult<T> LogMlResultTrace<T>(this MlResult<T> value,
    //                                                 ILogger logger,
    //                                                 string validMessage = null!,
    //                                                 string failMessage = null!)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Trace,
    //                          validBuilMessage: x => validMessage,
    //                          failBuildMessage: errors => failMessage);

    //public static MlResult<T> LogMlResultIfValidTrace<T>(this MlResult<T> value,
    //                                                        ILogger logger,
    //                                                        string validMessage)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Trace,
    //                          validBuilMessage: x => validMessage);


    //public static MlResult<T> LogMlResultIfFailTrace<T>(this MlResult<T> value,
    //                                                        ILogger logger,
    //                                                        string failMessage)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Trace,
    //                          failBuildMessage: errors => failMessage);











    //public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                  ILogger logger,
    //                                                                  Func<T, string> validBuildMessage = null!,
    //                                                                  Func<MlErrorsDetails, string> failBuildMessage = null!)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Trace, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    //public static async Task<MlResult<T>> LogMlResultIfValidTraceAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                         ILogger logger,
    //                                                                         Func<T, string> validBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Trace, validBuilMessage: validBuildMessage);

    //public static async Task<MlResult<T>> LogMlResultIfFailTraceAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                         ILogger logger,
    //                                                                         Func<MlErrorsDetails, string> errorBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Trace, failBuildMessage: errorBuildMessage);


    //public static async Task<MlResult<T>> LogMlResultTraceAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                  ILogger logger,
    //                                                                  string vallidMessage = null!,
    //                                                                  string failMessage = null!)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                       LogLevel.Trace,
    //                                       validBuilMessage: x => vallidMessage,
    //                                       failBuildMessage: errors => failMessage);

    //public static async Task<MlResult<T>> LogMlResultIfValidTraceAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                         ILogger logger,
    //                                                                         string successMessage)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                       LogLevel.Trace,
    //                                       validBuilMessage: x => successMessage);


    //public static async Task<MlResult<T>> LogMlResultIfFailTraceAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                         ILogger logger,
    //                                                                         string errorMessage)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                       LogLevel.Trace,
    //                                       failBuildMessage: errors => errorMessage);




    #endregion











}
