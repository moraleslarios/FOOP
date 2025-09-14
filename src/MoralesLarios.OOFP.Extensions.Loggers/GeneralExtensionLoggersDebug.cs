namespace MoralesLarios.OOFP.Extensions.Loggers;
public static class GeneralExtensionLoggersDebug
{





    #region LogMlResultDebug


    public static MlResult<ILogger> LogMlResultDebug(this ILogger  logger,
                                                          string   message)
        => logger.LogMlResult(LogLevel.Debug, message);

    public static Task<MlResult<ILogger>> LogMlResultDebugAsync(this ILogger  logger,
                                                                     string   message)
        => logger.LogMlResultAsync(LogLevel.Debug, message);


    public static MlResult<T> LogMlResultDebug<T>(this MlResult<T>                   source, 
                                                       ILogger                       logger,
                                                       Func<T, string>               validBuilMessage = null!,
                                                       Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Debug, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);

    public static Task<MlResult<T>> LogMlResultDebugAsync<T>(this MlResult<T>                   source, 
                                                                  ILogger                       logger,
                                                                  Func<T, string>               validBuilMessage = null!,
                                                                  Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Debug, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage).ToAsync();


    public static async Task<MlResult<T>> LogMlResultDebugAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuilMessage = null!,
                                                                        Func<MlErrorsDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Debug, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);


    public static MlResult<T> LogMlResultDebug<T>(this MlResult<T> source, 
                                                       ILogger     logger,
                                                       string      validMessage = null!,
                                                       string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Debug, 
                                 validBuilMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultDebugAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      validMessage = null!,
                                                                        string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Debug, 
                                            validBuilMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultDebugAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                        ILogger           logger,
                                                                        string            validMessage = null!,
                                                                        string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Debug, 
                                                 validBuilMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);




    public static MlResult<T> LogMlResultDebug<T>(this MlResult<T> source, 
                                                       ILogger     logger,
                                                       string      message)
        => source.LogMlResult<T>(logger, LogLevel.Debug, message);

    public static Task<MlResult<T>> LogMlResultDebugAsync<T>(this MlResult<T> source, 
                                                                  ILogger     logger,
                                                                  string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Debug, message);


    public static async Task<MlResult<T>> LogMlResultDebugAsync<T>(this Task<MlResult<T>> source, 
                                                                        ILogger           logger,
                                                                        string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Debug, message);



    #endregion


    #region LogMlResultDebugIfValid


    public static MlResult<T> LogMlResultDebugIfValid<T>(this MlResult<T> source,
                                                              ILogger     logger,
                                                              string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Debug, validMessage);

    public static Task<MlResult<T>> LogMlResultDebugIfValidAsync<T>(this MlResult<T> source,
                                                                         ILogger     logger,
                                                                         string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Debug, validMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultDebugIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                               ILogger           logger,
                                                                               string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Debug, validMessage);
    public static MlResult<T> LogMlResultDebugIfValid<T>(this MlResult<T>     source,
                                                              ILogger         logger,
                                                              Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Debug, validBuildMessage);

    public static Task<MlResult<T>> LogMlResultDebugIfValidAsync<T>(this MlResult<T>     source,
                                                                         ILogger         logger,
                                                                         Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Debug, validBuildMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultDebugIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                               ILogger           logger,
                                                                               Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Debug, validBuildMessage);


    #endregion


    #region LogMlResultDebugIfFail

    public static MlResult<T> LogMlResultDebugIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Debug, errorMessage);

    public static Task<MlResult<T>> LogMlResultDebugIfFailAsync<T>(this MlResult<T> source,
                                                                        ILogger     logger,
                                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Debug, errorMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultDebugIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                              ILogger           logger,
                                                                              string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Debug, failMessage);

    public static MlResult<T> LogMlResultDebugIfFail<T>(this MlResult<T>                   source,
                                                             ILogger                       logger,
                                                             Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Debug, failBuildMessage);

    public static Task<MlResult<T>> LogMlResultDebugIfFailAsync<T>(this MlResult<T>                   source,
                                                                        ILogger                       logger,
                                                                        Func<MlErrorsDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Debug, failBuildMessage).ToAsync();

    public async static Task<MlResult<T>> LogMlResultDebugIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorsDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Debug, failBuildMessage);


    #endregion


    #region LogMlResultDebugIfFailWithValue


    public static MlResult<T> LogMlResultDebugIfFailWithValue<T>(this MlResult<T>                   source,
                                                                      ILogger                       logger,
                                                                      Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Debug, failBuildMessage);

    public static MlResult<T> LogMlResultDebugIfFailWithValue<T>(this MlResult<T> source,
                                                                      ILogger     logger,
                                                                      string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Debug, failMessage);




    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                       ILogger                       logger,
                                                                                       Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                       ILogger     logger,
                                                                                       string      faildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, faildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                       ILogger                       logger,
                                                                                       Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                       ILogger           logger,
                                                                                       string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, faildMessage);






    public static MlResult<T> LogMlResultDebugIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                                    ILogger                               logger,
                                                                                    Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Debug, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                               ILogger                               logger,
                                                                                               Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                               ILogger                               logger,
                                                                                               Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Debug, failBuildMessage);



    #endregion


    #region LogMlResultDebugIfFailWithException


    public static MlResult<T> LogMlResultDebugIfFailWithException<T>(this MlResult<T>                   source,
                                                                          ILogger                       logger,
                                                                          Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Debug, failBuildMessage);

    public static MlResult<T> LogMlResultDebugIfFailWithException<T>(this MlResult<T> source,
                                                                          ILogger     logger,
                                                                          string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Debug, failMessage);




    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                           ILogger                       logger,
                                                                                           Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                           ILogger     logger,
                                                                                           string      faildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, faildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                           ILogger                       logger,
                                                                                           Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                           ILogger           logger,
                                                                                           string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, faildMessage);






    public static MlResult<T> LogMlResultDebugIfFailWithException<T>(this MlResult<T>                              source,
                                                                          ILogger                                  logger,
                                                                          Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Debug, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                           ILogger                                  logger,
                                                                                           Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                           ILogger                                  logger,
                                                                                           Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Debug, failBuildMessage);




    #endregion


    #region LogMlResultDebugIfFailWithoutException


    public static MlResult<T> LogMlResultDebugIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                            ILogger                       logger,
                                                                            Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Debug, failBuildMessage);

    public static MlResult<T> LogMlResultDebugIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                             ILogger                       logger,
                                                                             string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Debug, failMessage);


    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                              ILogger                       logger,
                                                                                              Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                              ILogger     logger,
                                                                                              string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Debug, failMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                              ILogger                       logger,
                                                                                              Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Debug, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultDebugIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                              ILogger           logger,
                                                                                              string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Debug, failMessage);


    #endregion

























    #region old

    //public static MlResult<ILogger> LogMlResulttDebug(this ILogger  logger,
    //                                                    string   message)
    //    => logger.LogMlResult(LogLevel.Debug, message);

    //public static Task<MlResult<ILogger>> LogMlResulttDebugAsync(this ILogger  logger,
    //                                                               string   message)
    //    => logger.LogMlResultAsync(LogLevel.Debug, message);


    //public static MlResult<T> LogMlResulttDebug<T>(this MlResult<T>                   value, 
    //                                                 ILogger                       logger,
    //                                                 Func<T, string>               validBuildMessage = null!,
    //                                                 Func<MlErrorsDetails, string> failBuildMessage  = null!)
    //    => value.LogMlResult<T>(logger, LogLevel.Debug, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);



    //public static MlResult<T> LogMlResulttDebug<T>(this MlResult<T> value, 
    //                                                 ILogger     logger,
    //                                                 string      message)
    //=> value.LogMlResult<T>(logger, LogLevel.Debug, message);

    //public static Task<MlResult<T>> LogMlResulttDebugAsync<T>(this MlResult<T> value, 
    //                                                            ILogger     logger, 
    //                                                            string      message)
    //    => value.LogMlResultAsync<T>(logger, LogLevel.Debug, message);


    //public static async Task<MlResult<T>> LogMlResulttDebugAsync<T>(this Task<MlResult<T>> value, 
    //                                                                  ILogger           logger, 
    //                                                                  string            message)
    //    => await value.LogMlResultAsync<T>(logger, LogLevel.Debug, message);


    //public static MlResult<T> LogMlResulttIfValidDebug<T>(this MlResult<T>     value,
    //                                                        ILogger         logger,
    //                                                        Func<T, string> validBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Debug, validBuilMessage: validBuildMessage);

    //public static MlResult<T> LogMlResultIfFailDebug<T>(this MlResult<T>                   value,
    //                                                        ILogger                       logger,
    //                                                        Func<MlErrorsDetails, string> errorBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Debug, failBuildMessage: errorBuildMessage);


    //public static MlResult<T> LogMlResulttDebug<T>(this MlResult<T> value, 
    //                                                 ILogger     logger, 
    //                                                 string      validMessage = null!,
    //                                                 string      failMessage   = null!)
    //    => value.LogMlResult<T>(logger, 
    //                          LogLevel.Debug, 
    //                          validBuilMessage: x      => validMessage, 
    //                          failBuildMessage: errors => failMessage);

    //public static MlResult<T> LogMlResulttIfValidDebug<T>(this MlResult<T> value,
    //                                                        ILogger     logger,
    //                                                        string      validMessage)
    //    => value.LogMlResult<T>(logger, 
    //                          LogLevel.Debug, 
    //                          validBuilMessage: x => validMessage);


    //public static MlResult<T> LogMlResultIfFailDebug<T>(this MlResult<T> value,
    //                                                        ILogger     logger,
    //                                                        string      failMessage)
    //    => value.LogMlResult<T>(logger, 
    //                          LogLevel.Debug, 
    //                          failBuildMessage: errors => failMessage);











    //public static async Task<MlResult<T>> LogMlResulttDebugAsync<T>(this Task<MlResult<T>>      valueAsync, 
    //                                                           ILogger                       logger, 
    //                                                           Func<T, string>               validBuildMessage = null!,
    //                                                           Func<MlErrorsDetails, string> failBuildMessage  = null!)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Debug, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    //public static async Task<MlResult<T>> LogMlResulttIfValidDebugAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                         ILogger           logger,
    //                                                                         Func<T, string>   validBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Debug, validBuilMessage: validBuildMessage);

    //public static async Task<MlResult<T>> LogMlResultIfFailDebugAsync<T>(this Task<MlResult<T>>             valueAsync,
    //                                                                         ILogger                       logger,
    //                                                                         Func<MlErrorsDetails, string> errorBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Debug, failBuildMessage: errorBuildMessage);


    //public static async Task<MlResult<T>> LogMlResulttDebugAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                  ILogger           logger, 
    //                                                                  string            vallidMessage = null!,
    //                                                                  string            failMessage   = null!)
    //    => (await valueAsync).LogMlResult<T>(logger, 
    //                                        LogLevel.Debug, 
    //                                        validBuilMessage: x      => vallidMessage, 
    //                                        failBuildMessage: errors => failMessage);

    //public static async Task<MlResult<T>> LogMlResulttIfValidDebugAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                         ILogger           logger,
    //                                                                         string            successMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, 
    //                                       LogLevel.Debug, 
    //                                       validBuilMessage: x => successMessage);


    //public static async Task<MlResult<T>> LogMlResultIfFailDebugAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                         ILogger           logger,
    //                                                                         string            errorMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, 
    //                                       LogLevel.Debug, 
    //                                       failBuildMessage: errors => errorMessage);






    #endregion








}
