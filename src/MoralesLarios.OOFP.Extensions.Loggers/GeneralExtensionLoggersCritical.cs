using System.Net.Http.Headers;

namespace MoralesLarios.OOFP.Extensions.Loggers;
public static class GeneralExtensionLoggersCritical
{




    #region LogMlResultCritical


    public static MlResult<ILogger> LogMlResultCritical(this ILogger  logger,
                                                                string   message)
        => logger.LogMlResult(LogLevel.Critical, message);

    public static Task<MlResult<ILogger>> LogMlResultCriticalAsync(this ILogger  logger,
                                                                           string   message)
        => logger.LogMlResultAsync(LogLevel.Critical, message);


    public static MlResult<T> LogMlResultCritical<T>(this MlResult<T>                   source, 
                                                             ILogger                       logger,
                                                             Func<T, string>               validBuilMessage = null!,
                                                             Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Critical, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);

    public static Task<MlResult<T>> LogMlResultCriticalAsync<T>(this MlResult<T>                   source, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuilMessage = null!,
                                                                        Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Critical, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage).ToAsync();


    public static async Task<MlResult<T>> LogMlResultCriticalAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                              ILogger                       logger,
                                                                              Func<T, string>               validBuilMessage = null!,
                                                                              Func<MlErrorsDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Critical, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);


    public static MlResult<T> LogMlResultCritical<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      validMessage = null!,
                                                             string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Critical, 
                                 validBuilMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalAsync<T>(this MlResult<T> source, 
                                                                              ILogger     logger,
                                                                              string      validMessage = null!,
                                                                              string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Critical, 
                                            validBuilMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                              ILogger           logger,
                                                                              string            validMessage = null!,
                                                                              string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Critical, 
                                                 validBuilMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);




    public static MlResult<T> LogMlResultCritical<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      message)
        => source.LogMlResult<T>(logger, LogLevel.Critical, message);

    public static Task<MlResult<T>> LogMlResultCriticalAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Critical, message);


    public static async Task<MlResult<T>> LogMlResultCriticalAsync<T>(this Task<MlResult<T>> source, 
                                                                              ILogger           logger,
                                                                              string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Critical, message);



    #endregion


    #region LogMlResultCriticalIfValid


    public static MlResult<T> LogMlResultCriticalIfValid<T>(this MlResult<T> source,
                                                                    ILogger     logger,
                                                                    string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Critical, validMessage);

    public static Task<MlResult<T>> LogMlResultCriticalIfValidAsync<T>(this MlResult<T> source,
                                                                               ILogger     logger,
                                                                               string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Critical, validMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultCriticalIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Critical, validMessage);
    public static MlResult<T> LogMlResultCriticalIfValid<T>(this MlResult<T>     source,
                                                                    ILogger         logger,
                                                                    Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Critical, validBuildMessage);

    public static Task<MlResult<T>> LogMlResultCriticalIfValidAsync<T>(this MlResult<T>     source,
                                                                               ILogger         logger,
                                                                               Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Critical, validBuildMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultCriticalIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Critical, validBuildMessage);


    #endregion


    #region LogMlResultCriticalIfFail

    public static MlResult<T> LogMlResultCriticalIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Critical, errorMessage);

    public static Task<MlResult<T>> LogMlResultCriticalIfFailAsync<T>(this MlResult<T> source,
                                                                              ILogger     logger,
                                                                              string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Critical, errorMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Critical, failMessage);

    public static MlResult<T> LogMlResultCriticalIfFail<T>(this MlResult<T>                   source,
                                                                   ILogger                       logger,
                                                                   Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Critical, failBuildMessage);

    public static Task<MlResult<T>> LogMlResultCriticalIfFailAsync<T>(this MlResult<T>                   source,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorsDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Critical, failBuildMessage).ToAsync();

    public async static Task<MlResult<T>> LogMlResultCriticalIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                    ILogger                       logger,
                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Critical, failBuildMessage);


    #endregion


    #region LogMlResultCriticalIfFailWithValue


    public static MlResult<T> LogMlResultCriticalIfFailWithValue<T>(this MlResult<T>                   source,
                                                                            ILogger                       logger,
                                                                            Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Critical, failBuildMessage);

    public static MlResult<T> LogMlResultCriticalIfFailWithValue<T>(this MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Critical, failMessage);




    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                             ILogger     logger,
                                                                                             string      faildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, faildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                             ILogger           logger,
                                                                                             string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, faildMessage);






    public static MlResult<T> LogMlResultCriticalIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                                    ILogger                               logger,
                                                                                    Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Critical, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Critical, failBuildMessage);



    #endregion


    #region LogMlResultCriticalIfFailWithException


    public static MlResult<T> LogMlResultCriticalIfFailWithException<T>(this MlResult<T>                   source,
                                                                                ILogger                       logger,
                                                                                Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Critical, failBuildMessage);

    public static MlResult<T> LogMlResultCriticalIfFailWithException<T>(this MlResult<T> source,
                                                                                ILogger     logger,
                                                                                string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Critical, failMessage);




    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                                 ILogger     logger,
                                                                                                 string      faildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, faildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                                 ILogger           logger,
                                                                                                 string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, faildMessage);






    public static MlResult<T> LogMlResultCriticalIfFailWithException<T>(this MlResult<T>                              source,
                                                                                ILogger                                  logger,
                                                                                Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Critical, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Critical, failBuildMessage);




    #endregion


    #region LogMlResultCriticalIfFailWithoutException


    public static MlResult<T> LogMlResultCriticalIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Critical, failBuildMessage);

    public static MlResult<T> LogMlResultCriticalIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Critical, failMessage);


    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                                    ILogger     logger,
                                                                                                    string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Critical, failMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Critical, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultCriticalIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                                    ILogger           logger,
                                                                                                    string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Critical, failMessage);


    #endregion




    #region old


    //public static MlResult<ILogger>LogMlResultCritical(this ILogger  logger,
    //                                                       string   message)
    //    => logger.LogMlResult(LogLevel.Critical, message);

    //public static Task<MlResult<ILogger>>LogMlResultCriticalAsync(this ILogger  logger,
    //                                                                  string   message)
    //    => logger.LogMlResultAsync(LogLevel.Critical, message);


    //public static MlResult<T>LogMlResultCritical<T>(this MlResult<T>                   value, 
    //                                                    ILogger                       logger,
    //                                                    Func<T, string>               validBuildMessage = null!,
    //                                                    Func<MlErrorsDetails, string> failBuildMessage  = null!)
    //    => value.LogMlResult<T>(logger, LogLevel.Critical, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);



    //public static MlResult<T>LogMlResultCritical<T>(this MlResult<T> value, 
    //                                                    ILogger     logger,
    //                                                    string      message)
    //=> value.LogMlResult<T>(logger, LogLevel.Critical, message);

    //public static Task<MlResult<T>>LogMlResultCriticalAsync<T>(this MlResult<T> value, 
    //                                                                ILogger     logger, 
    //                                                                string      message)
    //    => value.LogMlResultAsync<T>(logger, LogLevel.Critical, message);


    //public static async Task<MlResult<T>>LogMlResultCriticalAsync<T>(this Task<MlResult<T>> value, 
    //                                                                     ILogger           logger, 
    //                                                                     string            message)
    //    => await value.LogMlResultAsync<T>(logger, LogLevel.Critical, message);


    //public static MlResult<T>LogMlResultIfValidCritical<T>(this MlResult<T>     value,
    //                                                           ILogger         logger,
    //                                                           Func<T, string> validBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Critical, validBuilMessage: validBuildMessage);

    //public static MlResult<T>LogMlResultIfFailCritical<T>(this MlResult<T>                   value,
    //                                                        ILogger                       logger,
    //                                                        Func<MlErrorsDetails, string> errorBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Critical, failBuildMessage: errorBuildMessage);


    //public static MlResult<T>LogMlResultCritical<T>(this MlResult<T> value, 
    //                                                    ILogger     logger, 
    //                                                    string      validMessage = null!,
    //                                                    string      failMessage   = null!)
    //    => value.LogMlResult<T>(logger, 
    //                          LogLevel.Critical, 
    //                          validBuilMessage: x      => validMessage, 
    //                          failBuildMessage: errors => failMessage);

    //public static MlResult<T>LogMlResultIfValidCritical<T>(this MlResult<T> value,
    //                                                           ILogger     logger,
    //                                                           string      validMessage)
    //    => value.LogMlResult<T>(logger, 
    //                          LogLevel.Critical, 
    //                          validBuilMessage: x => validMessage);


    //public static MlResult<T>LogMlResultIfFailCritical<T>(this MlResult<T> value,
    //                                                           ILogger     logger,
    //                                                           string      failMessage)
    //    => value.LogMlResult<T>(logger, 
    //                          LogLevel.Critical, 
    //                          failBuildMessage: errors => failMessage);











    //public static async Task<MlResult<T>>LogMlResultCriticalAsync<T>(this Task<MlResult<T>>      valueAsync, 
    //                                                                     ILogger                       logger, 
    //                                                                     Func<T, string>               validBuildMessage = null!,
    //                                                                     Func<MlErrorsDetails, string> failBuildMessage  = null!)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Critical, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    //public static async Task<MlResult<T>>LogMlResultIfValidCriticalAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                            ILogger           logger,
    //                                                                            Func<T, string>   validBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Critical, validBuilMessage: validBuildMessage);

    //public static async Task<MlResult<T>>LogMlResultIfFailCriticalAsync<T>(this Task<MlResult<T>>             valueAsync,
    //                                                                            ILogger                       logger,
    //                                                                            Func<MlErrorsDetails, string> errorBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Critical, failBuildMessage: errorBuildMessage);


    //public static async Task<MlResult<T>>LogMlResultCriticalAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                     ILogger           logger, 
    //                                                                     string            vallidMessage = null!,
    //                                                                     string            failMessage   = null!)
    //    => (await valueAsync).LogMlResult<T>(logger, 
    //                                       LogLevel.Critical, 
    //                                       validBuilMessage: x      => vallidMessage, 
    //                                       failBuildMessage: errors => failMessage);

    //public static async Task<MlResult<T>>LogMlResultIfValidCriticalAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                            ILogger           logger,
    //                                                                            string            successMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, 
    //                                       LogLevel.Critical, 
    //                                       validBuilMessage: x => successMessage);


    //public static async Task<MlResult<T>>LogMlResultIfFailCriticalAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                            ILogger           logger,
    //                                                                            string            errorMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, 
    //                                       LogLevel.Critical, 
    //                                       failBuildMessage: errors => errorMessage);







    #endregion




}
