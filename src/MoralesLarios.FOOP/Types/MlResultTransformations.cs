
using System.Reflection;

namespace MoralesLarios.OOFP.Types;
public static class MlResultTransformations
{


    public static MlResult<TReturn> ToMlResult<T, TReturn>(this Func<T, TReturn> source, 
                                                                T                value)
    {
        TReturn partialResult = source(value);

        MlResult<TReturn> result = partialResult;

        return result;
    }

    public static MlResult<TReturn> TryToMlResult<T, TReturn>(this Func<T, TReturn> source, 
                                                                   T                value,
                                                                   string           exceptionAditionalMessage = null!)
        => source.TryToInternalMlResult<T, TReturn>(value, exceptionAditionalMessage);

    public static MlResult<TReturn> TryToMlResult<T, TReturn>(this Func<T, TReturn>         source, 
                                                                   T                        value,
                                                                   Func<Exception, string>  errorMessageBuilder)
        => source.TryToInternalMlResult<T, TReturn>(value, errorMessageBuilder);


    public static MlResult<TReturn> TryToMlResult<T, TReturn>(this Func<T, MlResult<TReturn>> source, 
                                                                   T                          value,
                                                                   string                     exceptionAditionalMessage = null!)
        => source.TryToInternalMlResult<T, TReturn>(value, exceptionAditionalMessage);

    public static MlResult<TReturn> TryToMlResult<T, TReturn>(this Func<T, MlResult<TReturn>> source, 
                                                                   T                          value,
                                                                   Func<Exception, string>    errorMessageBuilder)
        => source.TryToInternalMlResult<T, TReturn>(value, errorMessageBuilder);



    public static async Task<MlResult<TReturn>> ToMlResultAsync<T, TReturn>(this Func<T, Task<TReturn>> sourceAsync, 
                                                                                 T                      value)
    {
        TReturn partialResult = await sourceAsync(value);

        MlResult<TReturn> result = partialResult;

        return result;
    }



    public static async Task<MlResult<TReturn>> TryToMlResultAsync<T, TReturn>(this Func<T, Task<TReturn>> sourceAsync, 
                                                                                    T                      value,
                                                                                    string                 exceptionAditionalMessage = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T, TReturn>(value, exceptionAditionalMessage);

    public static async Task<MlResult<TReturn>> TryToMlResultAsync<T, TReturn>(this Func<T, Task<TReturn>>  sourceAsync, 
                                                                                    T                       value,
                                                                                    Func<Exception, string> errorMessageBuilder)
        => await sourceAsync.TryToInternalMlResultAsync<T, TReturn>(value, errorMessageBuilder);


    public static async Task<MlResult<TReturn>> TryToMlResultAsync<T, TReturn>(this Func<T, Task<MlResult<TReturn>>> sourceAsync, 
                                                                                    T                                value,
                                                                                    string                           exceptionAditionalMessage = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T, TReturn>(value, exceptionAditionalMessage);


    public static async Task<MlResult<TReturn>> TryToMlResultAsync<T, TReturn>(this Func<T, Task<MlResult<TReturn>>> sourceAsync, 
                                                                                    T                                value,
                                                                                    Func<Exception, string>          errorMessageBuilder)
        => await sourceAsync.TryToInternalMlResultAsync<T, TReturn>(value, errorMessageBuilder);






    //public static MlResult<T> TryToMlResult<T>(this Func<T>                 source, 
    //                                                Func<Exception, string> errorMessageBuilder = null!)
    //{

    //    /// ********** Cuando ees una MlResult en Fail y con errores, no se añaden los nuevos errores

    //    try
    //    {
    //        var result = source();

    //        return result.ToMlResultValid();
    //    }
    //    catch (Exception ex)
    //    {
    //        string message = BuildErrorMessage(errorMessageBuilder, ex);

    //        var errorDetails = new MlErrorsDetails(Errors : new List<MlError> { new MlError(message) },
    //                                               Details: new Dictionary<string, object> { { EX_DESC_KEY, ex } });

    //        return errorDetails.ToMlResultFail<T>();
    //    }
    //}


    public static MlResult<T> TryToMlResult<T>(this Func<T>                 source, 
                                                    Func<Exception, string> errorMessageBuilder = null!)
        => source.TryToInternalMlResult<T>(errorMessageBuilder);

    public static MlResult<T> TryToMlResult<T>(this Func<MlResult<T>>       source, 
                                                    Func<Exception, string> errorMessageBuilder = null!)
        => source.TryToInternalMlResult<T>(errorMessageBuilder);


    private static MlResult<T> TryToInternalMlResult<T>(this object                  source,
                                                             Func<Exception, string> errorMessageBuilder)
    {
        MlResult<T> result = default!;

        try
        {
            result = source switch
            {
                Func<T>           mySource => mySource(),
                Func<MlResult<T>> mySource => mySource(),
                _ => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
            };

        }
        catch (Exception ex)
        {
            string message = BuildErrorMessage(errorMessageBuilder, ex);

            var errorDetails = new MlErrorsDetails(Errors : new List<MlError> { new MlError(message) },
                                                   Details: new Dictionary<string, object> { { EX_DESC_KEY, ex } });

            result = errorDetails.ToMlResultFail<T>();
        }

        return result;
    }




    public static MlResult<T> TryToMlResult<T>(this Action<T>               source, 
                                                    T                       value,
                                                    Func<Exception, string> messageBuilder = null!)
    {
        /// ********** Cuando ees una MlResult en Fail y con errores, no se añaden los nuevos errores

        try
        {
            source(value);

            return value.ToMlResultValid();
        }
        catch (Exception ex)
        {
            string message = BuildErrorMessage(messageBuilder, ex);

            var errorDetails = new MlErrorsDetails(Errors : new List<MlError> { new MlError(message) },
                                                   Details: new Dictionary<string, object> { { EX_DESC_KEY, ex } });

            return errorDetails.ToMlResultFail<T>();
        }
    }





    //public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<Task<T>>           sourceAsync, 
    //                                                                 Func<Exception, string> errorMessageBuilder = null!)
    //{
    //    try
    //    {
    //        var result = await sourceAsync();

    //        return result.ToMlResultValid();
    //    }
    //    catch (Exception ex)
    //    {
    //        string message = BuildErrorMessage(errorMessageBuilder, ex);

    //        var errorDetails = new MlErrorsDetails(Errors : new List<MlError> { new MlError(message) },
    //                                    Details: new Dictionary<string, object> { { EX_DESC_KEY, ex } });

    //        return errorDetails.ToMlResultFail<T>();
    //    }
    //}


    public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<Task<T>>           sourceAsync, 
                                                                     Func<Exception, string> errorMessageBuilder = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T>(errorMessageBuilder);

    public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<Task<MlResult<T>>> sourceAsync, 
                                                                     Func<Exception, string> errorMessageBuilder = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T>(errorMessageBuilder);

    private static async Task<MlResult<T>> TryToInternalMlResultAsync<T>(this object                  source,
                                                                              Func<Exception, string> errorMessageBuilder)
    {
        MlResult<T> result = default!;

        try
        {
            result = source switch
            {
                Func<Task<T>>           mySource => await mySource(),
                Func<Task<MlResult<T>>> mySource => await mySource(),
                _ => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
            };
        }
        catch (Exception ex)
        {
            string message = BuildErrorMessage(errorMessageBuilder, ex);

            var errorDetails = new MlErrorsDetails(Errors : new List<MlError> { new MlError(message) },
                                        Details: new Dictionary<string, object> { { EX_DESC_KEY, ex } });

            result = errorDetails.ToMlResultFail<T>();
        }

        return result;
    }






    public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<T, Task>           sourceAsync,
                                                                     T                       returnValue,
                                                                     Func<Exception, string> messageBuilder = null!)
    {
        try
        {
            await sourceAsync(returnValue);

            return returnValue.ToMlResultValid();
        }
        catch (Exception ex)
        {
            string message = BuildErrorMessage(messageBuilder, ex);

            var errorDetails = new MlErrorsDetails(Errors : new List<MlError> { new MlError(message) },
                                        Details: new Dictionary<string, object> { { EX_DESC_KEY, ex } });

            return errorDetails.ToMlResultFail<T>();
        }
    }


    public static MlResult<T> TryToMlResultErrors<T>(this Action<MlErrorsDetails> source,
                                                          MlErrorsDetails         errorsDetails,
                                                          Func<Exception, string> errorMessageBuilder = null!)
    {
        MlResult<T> result = default!;

        try
        {
            source(errorsDetails);

            result = errorsDetails.ToMlResultFail<T>();
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }



    public static async Task<MlResult<T>> TryToMlResultErrorsAsync<T>(this Func<MlErrorsDetails, Task> sourceAsync,
                                                                           MlErrorsDetails             errorsDetails,
                                                                           Func<Exception, string>     errorMessageBuilder = null!)
    {
        MlResult<T> result = default!;

        try
        {
            await sourceAsync(errorsDetails);

            result = errorsDetails.ToMlResultFail<T>();
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }


    public static async Task<MlResult<TReturn>> ToMlTaskResult<T, TReturn>(this Func<T, Task<TReturn>> sourceAsync,
                                                                                T value)
    {
        TReturn partialResult = await sourceAsync(value);

        MlResult<TReturn> result = partialResult;

        return result;
    }










    //private static MlResult<TReturn> ToInternalMlResultEx<T, TReturn>(this object source, 
    //                                                                       T      value,
    //                                                                       string exceptionAditionalMessage = null!)
    //    => source.ToInternalMlResultEx<T, TReturn>(value, ex => exceptionAditionalMessage ?? DEFAULT_ERROR_MESSAGE);

    //private static MlResult<TReturn> ToInternalMlResultEx<T, TReturn>(this object                  source, 
    //                                                                       T                       value,
    //                                                                       Func<Exception, string> errorMessageBuilder)
    //{
    //    MlResult<TReturn> result = default!;

    //    try
    //    {
    //        result = source switch
    //        {
    //            Func<T, TReturn>           mySource => mySource.ToMlResult(value),
    //            Func<T, MlResult<TReturn>> mySource => mySource(value),
    //            _                                   => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        string msg = BuildErrorMessage(errorMessageBuilder, ex);

    //        result = new MlErrorsDetails(Errors : new List<MlError> { new MlError(msg) },
    //                                     Details: new Dictionary<string, object> { { EX_DESC_KEY, ex } });

    //    }

    //    return result;
    //}




    public static MlResult<T> TryToMlResult<T>(this Func<MlErrorsDetails,T> source,
                                                    MlErrorsDetails         errorsDetails,
                                                    Func<Exception, string> errorMessageBuilder)
        => TryToInternalMlResult<T>(source, errorsDetails, errorMessageBuilder);


    public static MlResult<T> TryToMlResult<T>(this Func<MlErrorsDetails, MlResult<T>> source,
                                                    MlErrorsDetails                    errorsDetails,
                                                    Func<Exception, string>            errorMessageBuilder)
        => TryToInternalMlResult<T>(source, errorsDetails, errorMessageBuilder);


    public static Task<MlResult<T>> TryToMlResultAsync<T>(this Func<MlErrorsDetails,Task<T>> sourceAsync,
                                                               MlErrorsDetails               errorsDetails,
                                                               Func<Exception, string>       errorMessageBuilder)
        => TryToInternalMlResultAsync<T>(sourceAsync, errorsDetails, errorMessageBuilder);

    public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<MlErrorsDetails, Task<MlResult<T>>> sourceAsync,
                                                                     MlErrorsDetails                          errorsDetails,
                                                                     Func<Exception, string>                  errorMessageBuilder)
        => await TryToInternalMlResultAsync<T>(sourceAsync, errorsDetails, errorMessageBuilder);

    public static MlResult<T> TryToMlResult<T>(this Func<T>                 source,
                                                    MlErrorsDetails         errorsDetails,
                                                    Func<Exception, string> errorMessageBuilder)
        => TryToInternalMlResult<T>(source, errorsDetails, errorMessageBuilder);

    public static Task<MlResult<T>> TryToMlResultAsync<T>(this Func<Task<T>>           sourceAsync,
                                                               MlErrorsDetails         errorsDetails,
                                                               Func<Exception, string> errorMessageBuilder)
        => TryToInternalMlResultAsync<T>(sourceAsync, errorsDetails, errorMessageBuilder);


    private static MlResult<T> TryToInternalMlResult<T>(this object                  source,
                                                             MlErrorsDetails         errorsDetails,
                                                             Func<Exception, string> errorMessageBuilder)
    {
        MlResult<T> result = default!;

        try
        {
            result = source switch
            {
                Func<T>                            mySource => mySource(),
                Func<MlErrorsDetails, T>           mySource => mySource(errorsDetails),
                Func<MlErrorsDetails, MlResult<T>> mySource => mySource(errorsDetails),
                _ => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
            };
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }


    private static async Task<MlResult<T>> TryToInternalMlResultAsync<T>(this object                  source,
                                                                              MlErrorsDetails         errorsDetails,
                                                                              Func<Exception, string> errorMessageBuilder)
    {
        MlResult<T> result = default!;

        try
        {
            result = source switch
            {
                Func<Task<T>>                            mySource => await mySource(),
                Func<MlErrorsDetails, Task<T>>           mySource => await mySource(errorsDetails),
                Func<MlErrorsDetails, Task<MlResult<T>>> mySource => await mySource(errorsDetails),
                _ => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
            };
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }




        

    public static string BuildErrorMessage(string errorMessage, Exception ex)
        => string.IsNullOrWhiteSpace(errorMessage) ? errorMessage : DEFAULT_EX_ERROR_MESSAGE(ex);

    public static string BuildErrorMessage(Func<Exception, string> messageBuilder, Exception ex)
        => messageBuilder != null ? messageBuilder(ex) : DEFAULT_EX_ERROR_MESSAGE(ex);



    public static MlResult<T> ToMlResultValid<T>(this T                    source) => source;
    public static MlResult<T> ToMlResultFail <T>(this MlErrorsDetails      source) => source;
    public static MlResult<T> ToMlResultFail <T>(this MlError              source) => source;
    public static MlResult<T> ToMlResultFail <T>(this string               source) => MlError.FromErrorMessage(source).ToMlResultFail<T>();
    public static MlResult<T> ToMlResultFail <T>(this List<MlError>        source) => source;
    public static MlResult<T> ToMlResultFail <T>(this List<string>         source) => MlErrorsDetails.FromErrorsMessagesDetails(source);
    public static MlResult<T> ToMlResultFail <T>(this MlError[]            source) => source;
    public static MlResult<T> ToMlResultFail <T>(this string[]             source) => MlErrorsDetails.FromErrorsMessagesDetails(source);
    public static MlResult<T> ToMlResultFail <T>(this IEnumerable<MlError> source) => new MlErrorsDetails(source);
    public static MlResult<T> ToMlResultFail <T>(this IEnumerable<string>  source) => MlErrorsDetails.FromErrorsMessagesDetails(source);
    public static MlResult<T> ToMlResultFail <T>(this (IEnumerable<MlError>, Dictionary<string, object>) source) => new MlErrorsDetails(source.Item1, source.Item2);
    public static MlResult<T> ToMlResultFail <T>(this (IEnumerable<string> , Dictionary<string, object>) source) => MlErrorsDetails.FromErrorsMessagesDetails(source.Item1, source.Item2);
    public static MlResult<T> ToMlResultFail <T>(this (MlError             , Dictionary<string, object>) source) => new MlErrorsDetails(new List<MlError> { source.Item1 }, source.Item2);
    public static MlResult<T> ToMlResultFail <T>(this (string              , Dictionary<string, object>) source) => MlErrorsDetails.FromErrorMessageDetails(source.Item1, source.Item2);

    public static Task<MlResult<T>> ToMlResultValidAsync<T>(this T                    source) => source.ToMlResultValid().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this MlErrorsDetails      source) => source.ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this MlError              source) => source.ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this string               source) => MlError.FromErrorMessage(source).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this List<MlError>        source) => source.ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this List<string>         source) => MlErrorsDetails.FromErrorsMessagesDetails(source).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this MlError[]            source) => source.ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this string[]             source) => MlErrorsDetails.FromErrorsMessagesDetails(source).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this IEnumerable<MlError> source) => new MlErrorsDetails(source).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this IEnumerable<string>  source) => MlErrorsDetails.FromErrorsMessagesDetails(source).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (IEnumerable<MlError>, Dictionary<string, object>) source) => new MlErrorsDetails(source.Item1, source.Item2).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (IEnumerable<string> , Dictionary<string, object>) source) => MlErrorsDetails.FromErrorsMessagesDetails(source.Item1, source.Item2).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (MlError             , Dictionary<string, object>) source) => new MlErrorsDetails(new List<MlError> { source.Item1 }, source.Item2).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (string              , Dictionary<string, object>) source) => MlErrorsDetails.FromErrorMessageDetails(source.Item1, source.Item2).ToMlResultFail<T>().ToAsync();




    public static MlResult<TReturn> ToMlResultFail<T, TReturn>(this MlResult<T> source)
        => source.Match(
                            fail: errorDetails => errorDetails,
                            valid: _           => MlResult<TReturn>.Fail("Don't change MlResult Fail of valid source.")
                        );

    public static Task<MlResult<TReturn>> ToMlResultFailAsync<T, TReturn>(this MlResult<T> source)
        => source.ToMlResultFail<T, TReturn>().ToAsync();

    public static async Task<MlResult<TReturn>> ToMlResultFailAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync)
        => await (await sourceAsync).ToMlResultFailAsync<T, TReturn>();



    public static object SecureGetValueFromMlResultBoxed(this object source)
    {
        var properties = source.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        var partialResult = MlResult.Empty()
                                        .Map      ( _              => properties.FirstOrDefault(p => p.Name == "IsValid" && p.PropertyType == typeof(bool)))
                                        .MapEnsure(isValidProperty => isValidProperty is not null,
                                                    _              => "The encapsulated MlResult object does not have a property 'IsValid' of type bool. The type isn't MlResult.")
                                        .Map      (isValidProperty => isValidProperty!.GetValue(source))
                                        .MapEnsure(isValidObj      => isValidObj is bool,
                                                    _              => "The 'IsValid' property is not of type bool")
                                        .MapEnsure(isValid         => (bool)isValid!,
                                                    _              => "The encapsulated MlResult object in obj, from which you want to extract the 'Value' property, is in a Fail state, so this information cannot be retrieved.")
                                        .Map      ( _              => properties.FirstOrDefault(p => p.Name == "Value"))
                                        .MapEnsure(valueProperty   => valueProperty is not null,
                                                    _              => "The source object does not have a property 'Value'")
                                        .Map      ( valueProperty  => valueProperty!.GetValue(source));

        var result = partialResult.IsValid ? partialResult.Value : throw new ArgumentException(partialResult.ErrorsDetails.ToString());

        return result!;
    }

    public static Task<object> SecureGetValueFromMlResultBoxedAsync(this object source)
        => source.SecureGetValueFromMlResultBoxed().ToAsync();

    public static async Task<object> SecureGetValueFromMlResultBoxedAsync(this Task<object> sourceAsync)
        => await (await sourceAsync).SecureGetValueFromMlResultBoxedAsync();


    public static MlResult<object> ToMlResultObject<T>(this MlResult<T> source)
    {
        var result = source.Match(
                            fail: errorDetails => errorDetails.ToMlResultFail<object>(),
                            valid: value => ((object)value).ToMlResultValid<object>()
                        );
        return result;
    }
    public static Task<MlResult<object>> ToMlResultObjectAsync<T>(this MlResult<T> source)
        => source.ToMlResultObject<T>().ToAsync();

    public static async Task<MlResult<object>> ToMlResultObjectAsync<T>(this Task<MlResult<T>> sourceAsync)
        => await (await sourceAsync).ToMlResultObjectAsync<T>();





    private static MlResult<TReturn> TryToInternalMlResult<T, TReturn>(this object source, 
                                                                            T      value,
                                                                            string exceptionAditionalMessage = null!)
        => source.TryToInternalMlResult<T, TReturn>(value, ex => exceptionAditionalMessage ?? DEFAULT_ERROR_MESSAGE);

    private static MlResult<TReturn> TryToInternalMlResult<T, TReturn>(this object                  source, 
                                                                            T                       value,
                                                                            Func<Exception, string> errorMessageBuilder)
    {
        MlResult<TReturn> result = default!;

        try
        {
            result = source switch
            {
                Func<T, TReturn>           mySource => mySource.ToMlResult(value),
                Func<T, MlResult<TReturn>> mySource => mySource(value),
                _                                   => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
            };
        }
        catch (Exception ex)
        {
            string msg = BuildErrorMessage(errorMessageBuilder, ex);

            result = new MlErrorsDetails(Errors : new List<MlError> { new MlError(msg) },
                                         Details: new Dictionary<string, object> { { EX_DESC_KEY, ex } });
            
        }

        return result;
    }






    private static async Task<MlResult<TReturn>> TryToInternalMlResultAsync<T, TReturn>(this object sourceAsync, 
                                                                                             T      value,
                                                                                             string exceptionAditionalMessage = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T, TReturn>(value, ex => exceptionAditionalMessage ?? DEFAULT_ERROR_MESSAGE);


    private static async Task<MlResult<TReturn>> TryToInternalMlResultAsync<T, TReturn>(this object                  sourceAsync, 
                                                                                             T                       value,
                                                                                             Func<Exception, string> errorMessageBuilder)
    {
        MlResult<TReturn> result = default!;

        try
        {
            result = sourceAsync switch
            {
                Func<T, Task<TReturn>>           mySource => await mySource.ToMlResultAsync(value),
                Func<T, Task<MlResult<TReturn>>> mySource => await mySource(value),
                _                                         => throw new ArgumentException($"The type {sourceAsync.GetType()} is not a valid type")
            };
        }
        catch (Exception ex)
        {


            string msg = BuildErrorMessage(errorMessageBuilder, ex);

            result = new MlErrorsDetails(Errors : new List<MlError> { new MlError(msg) },
                                         Details: new Dictionary<string, object> { { "Ex", ex } });
            
        }

        return result;
    }






}
