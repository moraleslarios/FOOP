namespace MoralesLarios.OOFP.Extensions.Loggers;
public static class GeneralExtensionLoggersInformation
{


    #region LogMlResultInformation


    public static MlResult<ILogger> LogMlResultInformation(this ILogger  logger,
                                                                string   message)
        => logger.LogMlResult(LogLevel.Information, message);

    public static Task<MlResult<ILogger>> LogMlResultInformationAsync(this ILogger  logger,
                                                                           string   message)
        => logger.LogMlResultAsync(LogLevel.Information, message);


    public static MlResult<T> LogMlResultInformation<T>(this MlResult<T>                   source, 
                                                             ILogger                       logger,
                                                             Func<T, string>               validBuilMessage = null!,
                                                             Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult<T>(logger, LogLevel.Information, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);

    public static Task<MlResult<T>> LogMlResultInformationAsync<T>(this MlResult<T>                   source, 
                                                                        ILogger                       logger,
                                                                        Func<T, string>               validBuilMessage = null!,
                                                                        Func<MlErrorsDetails, string> failBuildMessage = null!)
        => source.LogMlResult(logger, LogLevel.Information, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage).ToAsync();


    public static async Task<MlResult<T>> LogMlResultInformationAsync<T>(this Task<MlResult<T>>             sourceAsync, 
                                                                              ILogger                       logger,
                                                                              Func<T, string>               validBuilMessage = null!,
                                                                              Func<MlErrorsDetails, string> failBuildMessage = null!)
        => (await sourceAsync).LogMlResult(logger, LogLevel.Information, validBuilMessage: validBuilMessage, failBuildMessage: failBuildMessage);


    public static MlResult<T> LogMlResultInformation<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      validMessage = null!,
                                                             string      failMessage   = null!)
        => source.LogMlResult<T>(logger, 
                                 LogLevel.Information, 
                                 validBuilMessage: _ => validMessage, 
                                 failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultInformationAsync<T>(this MlResult<T> source, 
                                                                              ILogger     logger,
                                                                              string      validMessage = null!,
                                                                              string      failMessage  = null!)
        => await source.LogMlResultAsync<T>(logger, 
                                            LogLevel.Information, 
                                            validBuilMessage: _ => validMessage, 
                                            failBuildMessage: _ => failMessage);

    public static async Task<MlResult<T>> LogMlResultInformationAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                              ILogger           logger,
                                                                              string            validMessage = null!,
                                                                              string            failMessage  = null!)
        => await sourceAsync.LogMlResultAsync<T>(logger,
                                                 LogLevel.Information, 
                                                 validBuilMessage: _ => validMessage, 
                                                 failBuildMessage: _ => failMessage);




    public static MlResult<T> LogMlResultInformation<T>(this MlResult<T> source, 
                                                             ILogger     logger,
                                                             string      message)
        => source.LogMlResult<T>(logger, LogLevel.Information, message);

    public static Task<MlResult<T>> LogMlResultInformationAsync<T>(this MlResult<T> source, 
                                                                        ILogger     logger,
                                                                        string      message)
        => source.LogMlResultAsync<T>(logger, LogLevel.Information, message);


    public static async Task<MlResult<T>> LogMlResultInformationAsync<T>(this Task<MlResult<T>> source, 
                                                                              ILogger           logger,
                                                                              string            message)
        => await source.LogMlResultAsync<T>(logger, LogLevel.Information, message);



    #endregion


    #region LogMlResultInformationIfValid


    public static MlResult<T> LogMlResultInformationIfValid<T>(this MlResult<T> source,
                                                                    ILogger     logger,
                                                                    string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Information, validMessage);

    public static Task<MlResult<T>> LogMlResultInformationIfValidAsync<T>(this MlResult<T> source,
                                                                               ILogger     logger,
                                                                               string      validMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Information, validMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultInformationIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     string            validMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Information, validMessage);
    public static MlResult<T> LogMlResultInformationIfValid<T>(this MlResult<T>     source,
                                                                    ILogger         logger,
                                                                    Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Information, validBuildMessage);

    public static Task<MlResult<T>> LogMlResultInformationIfValidAsync<T>(this MlResult<T>     source,
                                                                               ILogger         logger,
                                                                               Func<T, string> validBuildMessage)
        => source.LogMlResultIfValid(logger, LogLevel.Information, validBuildMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultInformationIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                     ILogger           logger,
                                                                                     Func<T, string>   validBuildMessage)
        => (await sourceAsync).LogMlResultIfValid(logger, LogLevel.Information, validBuildMessage);


    #endregion


    #region LogMlResultInformationIfFail

    public static MlResult<T> LogMlResultInformationIfFail<T>(this MlResult<T> source,
                                                        ILogger     logger,
                                                        string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Information, errorMessage);

    public static Task<MlResult<T>> LogMlResultInformationIfFailAsync<T>(this MlResult<T> source,
                                                                              ILogger     logger,
                                                                              string      errorMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Information, errorMessage).ToAsync();

    public static async Task<MlResult<T>> LogMlResultInformationIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                    ILogger           logger,
                                                                                    string            failMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Information, failMessage);

    public static MlResult<T> LogMlResultInformationIfFail<T>(this MlResult<T>                   source,
                                                                   ILogger                       logger,
                                                                   Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFail(logger, LogLevel.Information, failBuildMessage);

    public static Task<MlResult<T>> LogMlResultInformationIfFailAsync<T>(this MlResult<T>                   source,
                                                                              ILogger                       logger,
                                                                              Func<MlErrorsDetails, string> failBuildMessage)

        => source.LogMlResultIfFail(logger, LogLevel.Information, failBuildMessage).ToAsync();

    public async static Task<MlResult<T>> LogMlResultInformationIfFailAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                    ILogger                       logger,
                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => (await sourceAsync).LogMlResultIfFail(logger, LogLevel.Information, failBuildMessage);


    #endregion


    #region LogMlResultInformationIfFailWithValue


    public static MlResult<T> LogMlResultInformationIfFailWithValue<T>(this MlResult<T>                   source,
                                                                            ILogger                       logger,
                                                                            Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Information, failBuildMessage);

    public static MlResult<T> LogMlResultInformationIfFailWithValue<T>(this MlResult<T> source,
                                                                            ILogger     logger,
                                                                            string      failMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Information, failMessage);




    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithValueAsync<T>(this MlResult<T>                   source,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Information, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithValueAsync<T>(this MlResult<T> source,
                                                                                             ILogger     logger,
                                                                                             string      faildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Information, faildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithValueAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                             ILogger                       logger,
                                                                                             Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Information, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                             ILogger           logger,
                                                                                             string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Information, faildMessage);






    public static MlResult<T> LogMlResultInformationIfFailWithValue<T, TValue>(this MlResult<T>                           source,
                                                                                    ILogger                               logger,
                                                                                    Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => source.LogMlResultIfFailWithValue(logger, LogLevel.Information, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithValueAsync<T, TValue>(this MlResult<T>                           source,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await source.LogMlResultIfFailWithValueAsync(logger, LogLevel.Information, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     ILogger                               logger,
                                                                                                     Func<MlErrorsDetails, TValue, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithValueAsync(logger, LogLevel.Information, failBuildMessage);



    #endregion


    #region LogMlResultInformationIfFailWithException


    public static MlResult<T> LogMlResultInformationIfFailWithException<T>(this MlResult<T>                   source,
                                                                                ILogger                       logger,
                                                                                Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Information, failBuildMessage);

    public static MlResult<T> LogMlResultInformationIfFailWithException<T>(this MlResult<T> source,
                                                                                ILogger     logger,
                                                                                string      failMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Information, failMessage);




    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Information, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithExceptionAsync<T>(this MlResult<T> source,
                                                                                                 ILogger     logger,
                                                                                                 string      faildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Information, faildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                 ILogger                       logger,
                                                                                                 Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Information, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                                 ILogger           logger,
                                                                                                 string            faildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Information, faildMessage);






    public static MlResult<T> LogMlResultInformationIfFailWithException<T>(this MlResult<T>                              source,
                                                                                ILogger                                  logger,
                                                                                Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => source.LogMlResultIfFailWithException(logger, LogLevel.Information, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithExceptionAsync<T>(this MlResult<T>                              source,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await source.LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Information, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 ILogger                                  logger,
                                                                                                 Func<MlErrorsDetails, Exception, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithExceptionAsync(logger, LogLevel.Information, failBuildMessage);




    #endregion


    #region LogMlResultInformationIfFailWithoutException


    public static MlResult<T> LogMlResultInformationIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   Func<MlErrorsDetails, string> failBuildMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Information, failBuildMessage);

    public static MlResult<T> LogMlResultInformationIfFailWithoutException<T>(this MlResult<T>                   source,
                                                                                   ILogger                       logger,
                                                                                   string                        failMessage)
        => source.LogMlResultIfFailWithoutException(logger, LogLevel.Information, failMessage);


    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithoutExceptionAsync<T>(this MlResult<T>                   source,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Information, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithoutExceptionAsync<T>(this MlResult<T> source,
                                                                                                    ILogger     logger,
                                                                                                    string      failMessage)
        => await source.LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Information, failMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>             sourceAsync,
                                                                                                    ILogger                       logger,
                                                                                                    Func<MlErrorsDetails, string> failBuildMessage)
        => await (await sourceAsync).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Information, failBuildMessage);

    public static async Task<MlResult<T>> LogMlResultInformationIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>> source,
                                                                                                    ILogger           logger,
                                                                                                    string            failMessage)
        => await (await source).LogMlResultIfFailWithoutExceptionAsync(logger, LogLevel.Information, failMessage);


    #endregion




    #region old


    //public static MlResult<ILogger> LogMlResultInformation(this ILogger logger,
    //                                                          string message)
    //    => logger.LogMlResult(LogLevel.Information, message);

    //public static Task<MlResult<ILogger>> LogMlResultInformationAsync(this ILogger logger,
    //                                                                     string message)
    //    => logger.LogMlResultAsync(LogLevel.Information, message);


    //public static MlResult<T> LogMlResultInformation<T>(this MlResult<T> value,
    //                                                       ILogger logger,
    //                                                       Func<T, string> validBuildMessage = null!,
    //                                                       Func<MlErrorsDetails, string> failBuildMessage = null!)
    //    => value.LogMlResult<T>(logger, LogLevel.Information, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);



    //public static MlResult<T> LogMlResultInformation<T>(this MlResult<T> value,
    //                                                       ILogger logger,
    //                                                       string message)
    //=> value.LogMlResult<T>(logger, LogLevel.Information, message);

    //public static Task<MlResult<T>> LogMlResultInformationAsync<T>(this MlResult<T> value,
    //                                                                  ILogger logger,
    //                                                                  string message)
    //    => value.LogMlResultAsync<T>(logger, LogLevel.Information, message);


    //public static async Task<MlResult<T>> LogMlResultInformationAsync<T>(this Task<MlResult<T>> value,
    //                                                                        ILogger logger,
    //                                                                        string message)
    //    => await value.LogMlResultAsync<T>(logger, LogLevel.Information, message);


    //public static MlResult<T> LogMlResultIfValidInformation<T>(this MlResult<T> value,
    //                                                              ILogger logger,
    //                                                              Func<T, string> validBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Information, validBuilMessage: validBuildMessage);

    //public static MlResult<T> LogMlResultIfFailInformation<T>(this MlResult<T> value,
    //                                                              ILogger logger,
    //                                                              Func<MlErrorsDetails, string> errorBuildMessage)
    //    => value.LogMlResult<T>(logger, LogLevel.Information, failBuildMessage: errorBuildMessage);


    //public static MlResult<T> LogMlResultInformation<T>(this MlResult<T> value,
    //                                                       ILogger logger,
    //                                                       string validMessage = null!,
    //                                                       string failMessage = null!)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Information,
    //                          validBuilMessage: x => validMessage,
    //                          failBuildMessage: errors => failMessage);

    //public static MlResult<T> LogMlResultIfValidInformation<T>(this MlResult<T> value,
    //                                                              ILogger logger,
    //                                                              string validMessage)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Information,
    //                          validBuilMessage: x => validMessage);


    //public static MlResult<T> LogMlResultIfFailInformation<T>(this MlResult<T> value,
    //                                                              ILogger logger,
    //                                                              string failMessage)
    //    => value.LogMlResult<T>(logger,
    //                          LogLevel.Information,
    //                          failBuildMessage: errors => failMessage);











    //public static async Task<MlResult<T>> LogMlResultInformationAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                        ILogger logger,
    //                                                                        Func<T, string> validBuildMessage = null!,
    //                                                                        Func<MlErrorsDetails, string> failBuildMessage = null!)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Information, validBuilMessage: validBuildMessage, failBuildMessage: failBuildMessage);

    //public static async Task<MlResult<T>> LogMlResultIfValidInformationAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                               ILogger logger,
    //                                                                               Func<T, string> validBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Information, validBuilMessage: validBuildMessage);

    //public static async Task<MlResult<T>> LogMlResultIfFailInformationAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                               ILogger logger,
    //                                                                               Func<MlErrorsDetails, string> errorBuildMessage)
    //    => (await valueAsync).LogMlResult<T>(logger, LogLevel.Information, failBuildMessage: errorBuildMessage);


    //public static async Task<MlResult<T>> LogMlResultInformationAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                        ILogger logger,
    //                                                                        string vallidMessage = null!,
    //                                                                        string failMessage = null!)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                        LogLevel.Information,
    //                                        validBuilMessage: x => vallidMessage,
    //                                        failBuildMessage: errors => failMessage);

    //public static async Task<MlResult<T>> LogMlResultIfValidInformationAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                               ILogger logger,
    //                                                                               string successMessage)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                       LogLevel.Information,
    //                                       validBuilMessage: x => successMessage);


    //public static async Task<MlResult<T>> LogMlResultIfFailAsync<T>(this Task<MlResult<T>> valueAsync,
    //                                                                    ILogger logger,
    //                                                                    LogLevel logLevel,
    //                                                                    string errorMessage)
    //    => (await valueAsync).LogMlResult<T>(logger,
    //                                       LogLevel.Information,
    //                                       failBuildMessage: errors => errorMessage);





    #endregion





}
