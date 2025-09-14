namespace MoralesLarios.OOFP.Extensions.Loggers;
public static class GeneralExtensionLoggersError
{


    

    #region LogMlResultError


    public static MlResult<ILogger> LogMlResultError(this ILogger  logger,
                                                          string   message)
        => logger.LogMlResult(LogLevel.Error, message);

    public static Task<MlResult<ILogger>> LogMlResultErrorAsync(this ILogger  logger,
                                                                     string   message)
        => logger.LogMlResultAsync(LogLevel.Error, message);


    public static MlResult<T> LogMlResultError<T>(this MlResult<T>                   source, 
                                                       ILogger                       logger,
                                                       Func<T, string>               validBuilMessage = null!,
                                                       Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Error, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);

    public static Task<MlResult<T>> LogMlResultErrorAsync<T>(this MlResult<T>                   source, 
                                                                  ILogger                       logger,
                                                                  Func<T, string>               validBuilMessage = null!,
                                                                  Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Error, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage).ToAsync();


    public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuilMessage = null!,
                                                                        Func<MlErrorsDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Error, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);


    public static MlResult<T> LogMlResultError<T>(this MlResult<T> source, 
                                                       ILogger     logger,
                                                       string      validMessage = null!,
                                                       string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Error, 
                                 validBuilMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      validMessage = null!,
                                                                        string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Error, 
                                            validBuilMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                        ILogger           logger,
                                                                        string            validMessage = null!,
                                                                        string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Error, 
                                                 validBuilMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);




    public static MlResult<T> LogMlResultError<T>(this MlResult<T> source, 
                                                       ILogger     logger,
                                                       string      message)
        => source.LogMlResult<T>(logger, LogLevel.Error, message);

    public static Task<MlResult<T>> LogMlResultErrorAsync<T>(this MlResult<T> source, 
                                                                  ILogger     logger,
                                                                  string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Error, message);


    public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this Task<MlResult<T>> source, 
                                                                        ILogger           logger,
                                                                        string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Error, message);



    #endregion


    #region LogMlResultErrorIfValid


    public static MlResult<T> LogMlResultErrorIfValid<T>(this MlResult<T> source,
                                                              ILogger     logger,
                                                              string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Error, validMessage);

    public static Task<MlResult<T>> LogMlResultErrorIfValidAsync<T>(this MlResult<T> source,
                                                                         ILogger     logger,
                                                                         string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Error, validMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultErrorIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                               ILogger           logger,
                                                                               string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Error, validMessage);
    public static MlResult<T> LogMlResultErrorIfValid<T>(this MlResult<T>     source,
                                                              ILogger         logger,
                                                              Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Error, validBuildMessage);

    public static Task<MlResult<T>> LogMlResultErrorIfValidAsync<T>(this MlResult<T>     source,
                                                                         ILogger         logger,
                                                                         Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Error, validBuildMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultErrorIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                               ILogger           logger,
                                                                               Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Error, validBuildMessage);


    #endregion


    #region LogMlResultErrorIfFail

    public static MlResult<T> LogMlResultErrorIfFail<T>(this MlResult<T> source,
                                                             ILogger     logger,
                                                             string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Error, errorMessage);

    public static Task<MlResult<T>> LogMlResultErrorIfFailAsync<T>(this MlResult<T> source,
                                                                        ILogger     logger,
                                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Error, errorMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultErrorIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                              ILogger           logger,
                                                                              string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Error, failMessage);

    public static MlResult<T> LogMlResultErrorIfFail<T>(this MlResult<T>                   source,
                                                             ILogger                       logger,
                                                             Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Error, failBuildMessage);

    public static Task<MlResult<T>> LogMlResultErrorIfFailAsync<T>(this MlResult<T>                   source,
                                                                        ILogger                       logger,
                                                                        Func<MlErrorsDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Error, failBuildMessage).ToAsync();

    public async static Task<MlResult<T>> LogMlResultErrorIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorsDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Error, failBuildMessage);


    #endregion


    #region LogMlResultErrorIfFailWithValue


    public static MlResult<T> LogMlResultErrorIfFailWithValue<T>(this MlResult<T>                   source,
                                                                      ILogger                       logger,
                                                                      Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Error, failBuildMessage);

    public static MlResult<T> LogMlResultErrorIfFailWithValue<T>(this MlResult<T> source,
                                                                      ILogger     logger,
                                                                      string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Error, failMessage);




    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                       ILogger                       logger,
                                                                                       Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      faildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, faildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                       ILogger                       logger,
                                                                                       Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                             ILogger           logger,
                                                                                             string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, faildMessage);






    public static MlResult<T> LogMlResultErrorIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                              ILogger                               logger,
                                                                              Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Error, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                               ILogger                               logger,
                                                                                               Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                               ILogger                               logger,
                                                                                               Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Error, failBuildMessage);



    #endregion


    #region LogMlResultErrorIfFailWithException


    public static MlResult<T> LogMlResultErrorIfFailWithException<T>(this MlResult<T>                   source,
                                                                          ILogger                       logger,
                                                                          Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Error, failBuildMessage);

    public static MlResult<T> LogMlResultErrorIfFailWithException<T>(this MlResult<T> source,
                                                                          ILogger     logger,
                                                                          string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Error, failMessage);




    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                           ILogger                       logger,
                                                                                           Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                           ILogger     logger,
                                                                                           string      faildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, faildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                           ILogger                       logger,
                                                                                           Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                           ILogger           logger,
                                                                                           string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, faildMessage);






    public static MlResult<T> LogMlResultErrorIfFailWithException<T>(this MlResult<T>                              source,
                                                                          ILogger                                  logger,
                                                                          Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Error, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                           ILogger                                  logger,
                                                                                           Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                           ILogger                                  logger,
                                                                                           Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Error, failBuildMessage);




    #endregion


    #region LogMlResultErrorIfFailWithoutException


    public static MlResult<T> LogMlResultErrorIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                             ILogger                       logger,
                                                                             Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Error, failBuildMessage);

    public static MlResult<T> LogMlResultErrorIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                             ILogger                       logger,
                                                                             string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Error, failMessage);


    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                              ILogger                       logger,
                                                                                              Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                              ILogger     logger,
                                                                                              string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Error, failMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                              ILogger                       logger,
                                                                                              Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Error, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultErrorIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                              ILogger           logger,
                                                                                              string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Error, failMessage);


    #endregion





    #region old

    //public static MlResult<ILogger> LogMlResultError(this ILogger logger,
    //                                                    string message)
    //    => logger.LogMlResult(LogLevel.Error, message);

    //public static Task<MlResult<ILogger>> LogMlResultErrorAsync(this ILogger logger,
    //                                                               string message)
    //    => logger.LogMlResultAsync(LogLevel.Error, message);


    //public static MlResult<T> LogMlResultError<T>(this MlResult<T> value,
    //                                                 ILogger logger,
    //                                                 Func<T, string> validBuildMessage = null!,
    //                                                 Func<MlErrorsDetails, string> failBuildMessage = null!)
    //    => value.LogMlResult<T>(logger, LogLevel.Error, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);



    //public static MlResult<T> LogMlResultError<T>(this MlResult<T> value,
    //                                                 ILogger logger,
    //                                                 string message)
    //=> value.LogMlResult<T>(logger, LogLevel.Error, message);

    //public static Task<MlResult<T>> LogMlResultErrorAsync<T>(this MlResult<T> value,
    //                                                            ILogger logger,
    //                                                            string message)
    //    => value.LogMlResultAsync<T>(logger, LogLevel.Error, message);


    //public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this Task<MlResult<T>> value,
    //                                                                  ILogger logger,
    //                                                                  string message)
    //    => await value.LogMlResultAsync<T>(logger, LogLevel.Error, message);


    //public static MlResult<T> LogMlResultIfValidError<T>(this MlResult<T> value,
    //                                                        ILogger logger,
    //                                                        Func<T, string> validBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Error, validBuilMessage: validBuildMessage);

    //public static MlResult<T> LogMlResultIfFailError<T>(this MlResult<T> value,
    //                                                        ILogger logger,
    //                                                        Func<MlErrorsDetails, string> failBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Error, failBuildMessage: failBuildMessage);


    //public static MlResult<T> LogMlResultError<T>(this MlResult<T> value,
    //                                                 ILogger logger,
    //                                                 string validMessage = null!,
    //                                                 string failMessage = null!)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Error,
    //                          validBuilMessage: x => validMessage,
    //                          failBuildMessage: errors => failMessage);

    //public static MlResult<T> LogMlResultIfValidError<T>(this MlResult<T> value,
    //                                                        ILogger logger,
    //                                                        string validMessage)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Error,
    //                          validBuilMessage: x => validMessage);


    //public static MlResult<T> LogMlResultIfFailError<T>(this MlResult<T> value,
    //                                                        ILogger logger,
    //                                                        string failMessage)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Error,
    //                          failBuildMessage: errors => failMessage);











    //public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                  ILogger logger,
    //                                                                  Func<T, string> validBuildMessage = null!,
    //                                                                  Func<MlErrorsDetails, string> failBuildMessage = null!)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Error, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    //public static async Task<MlResult<T>> LogMlResultIfValidErrorAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                         ILogger logger,
    //                                                                         Func<T, string> validBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Error, validBuilMessage: validBuildMessage);

    //public static async Task<MlResult<T>> LogMlResultIfFailErrorAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                         ILogger logger,
    //                                                                         Func<MlErrorsDetails, string> failBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Error, failBuildMessage: failBuildMessage);


    //public static async Task<MlResult<T>> LogMlResultErrorAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                  ILogger logger,
    //                                                                  string vallidMessage = null!,
    //                                                                  string failMessage = null!)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                       LogLevel.Error,
    //                                       validBuilMessage: x => vallidMessage,
    //                                       failBuildMessage: errors => failMessage);

    //public static async Task<MlResult<T>> LogMlResultIfValidErrorAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                         ILogger logger,
    //                                                                         string successMessage)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                       LogLevel.Error,
    //                                       validBuilMessage: x => successMessage);


    //public static async Task<MlResult<T>> LogMlResultIfFailErrorAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                         ILogger logger,
    //                                                                         string errorMessage)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                       LogLevel.Error,
    //                                       failBuildMessage: errors => errorMessage);



    #endregion





}
