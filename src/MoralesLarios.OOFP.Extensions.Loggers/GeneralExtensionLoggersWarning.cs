namespace MoralesLarios.OOFP.Extensions.Loggers;
public static class GeneralExtensionLoggersWarning
{





    #region LogMlResultWarning


    public static MlResult<ILogger> LogMlResultWarning(this ILogger  logger,
                                                                string   message)
        => logger.LogMlResult(LogLevel.Warning, message);

    public static Task<MlResult<ILogger>> LogMlResultWarningAsync(this ILogger  logger,
                                                                           string   message)
        => logger.LogMlResultAsync(LogLevel.Warning, message);


    public static MlResult<T> LogMlResultWarning<T>(this MlResult<T>                   source, 
                                                             ILogger                       logger,
                                                             Func<T, string>               validBuilMessage = null!,
                                                             Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Warning, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);

    public static Task<MlResult<T>> LogMlResultWarningAsync<T>(this MlResult<T>                   source, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuilMessage = null!,
                                                                        Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Warning, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage).ToAsync();


    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                              ILogger                       logger,
                                                                              Func<T, string>               validBuilMessage = null!,
                                                                              Func<MlErrorsDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Warning, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);


    public static MlResult<T> LogMlResultWarning<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      validMessage = null!,
                                                             string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Warning, 
                                 validBuilMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this MlResult<T> source, 
                                                                              ILogger     logger,
                                                                              string      validMessage = null!,
                                                                              string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Warning, 
                                            validBuilMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                              ILogger           logger,
                                                                              string            validMessage = null!,
                                                                              string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Warning, 
                                                 validBuilMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);




    public static MlResult<T> LogMlResultWarning<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      message)
        => source.LogMlResult<T>(logger, LogLevel.Warning, message);

    public static Task<MlResult<T>> LogMlResultWarningAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Warning, message);


    public static async Task<MlResult<T>> LogMlResultWarningAsync<T>(this Task<MlResult<T>> source, 
                                                                              ILogger           logger,
                                                                              string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Warning, message);



    #endregion


    #region LogMlResultWarningIfValid


    public static MlResult<T> LogMlResultWarningIfValid<T>(this MlResult<T> source,
                                                                    ILogger     logger,
                                                                    string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validMessage);

    public static Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this MlResult<T> source,
                                                                               ILogger     logger,
                                                                               string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Warning, validMessage);
    public static MlResult<T> LogMlResultWarningIfValid<T>(this MlResult<T>     source,
                                                                    ILogger         logger,
                                                                    Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validBuildMessage);

    public static Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this MlResult<T>     source,
                                                                               ILogger         logger,
                                                                               Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Warning, validBuildMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultWarningIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Warning, validBuildMessage);


    #endregion


    #region LogMlResultWarningIfFail

    public static MlResult<T> LogMlResultWarningIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Warning, errorMessage);

    public static Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this MlResult<T> source,
                                                                              ILogger     logger,
                                                                              string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Warning, errorMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Warning, failMessage);

    public static MlResult<T> LogMlResultWarningIfFail<T>(this MlResult<T>                   source,
                                                                   ILogger                       logger,
                                                                   Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Warning, failBuildMessage);

    public static Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this MlResult<T>                   source,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorsDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Warning, failBuildMessage).ToAsync();

    public async static Task<MlResult<T>> LogMlResultWarningIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                    ILogger                       logger,
                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Warning, failBuildMessage);


    #endregion


    #region LogMlResultWarningIfFailWithValue


    public static MlResult<T> LogMlResultWarningIfFailWithValue<T>(this MlResult<T>                   source,
                                                                            ILogger                       logger,
                                                                            Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Warning, failBuildMessage);

    public static MlResult<T> LogMlResultWarningIfFailWithValue<T>(this MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Warning, failMessage);




    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                             ILogger     logger,
                                                                                             string      faildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, faildMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                             ILogger           logger,
                                                                                             string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, faildMessage);






    public static MlResult<T> LogMlResultWarningIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                                    ILogger                               logger,
                                                                                    Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Warning, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Warning, failBuildMessage);



    #endregion


    #region LogMlResultWarningIfFailWithException


    public static MlResult<T> LogMlResultWarningIfFailWithException<T>(this MlResult<T>                   source,
                                                                                ILogger                       logger,
                                                                                Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Warning, failBuildMessage);

    public static MlResult<T> LogMlResultWarningIfFailWithException<T>(this MlResult<T> source,
                                                                                ILogger     logger,
                                                                                string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Warning, failMessage);




    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                                 ILogger     logger,
                                                                                                 string      faildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, faildMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                                 ILogger           logger,
                                                                                                 string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, faildMessage);






    public static MlResult<T> LogMlResultWarningIfFailWithException<T>(this MlResult<T>                              source,
                                                                                ILogger                                  logger,
                                                                                Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Warning, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Warning, failBuildMessage);




    #endregion


    #region LogMlResultWarningIfFailWithoutException


    public static MlResult<T> LogMlResultWarningIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Warning, failBuildMessage);

    public static MlResult<T> LogMlResultWarningIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Warning, failMessage);


    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                                    ILogger     logger,
                                                                                                    string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Warning, failMessage);

    public static async Task<MlResult<T>> LogMlResultWarningIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Warning, failBuildMessage);

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
    //                                                   Func<MlErrorsDetails, string> failBuildMessage = null!)
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
    //                                                          Func<MlErrorsDetails, string> errorBuildMessage)
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
    //                                                                    Func<MlErrorsDetails, string> failBuildMessage = null!)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Warning, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    //public static async Task<MlResult<T>> LogMlResultIfValidWarningAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                           ILogger logger,
    //                                                                           Func<T, string> validBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Warning, validBuilMessage: validBuildMessage);

    //public static async Task<MlResult<T>> LogMlResultIfFailWarningAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                           ILogger logger,
    //                                                                           Func<MlErrorsDetails, string> errorBuildMessage)
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
