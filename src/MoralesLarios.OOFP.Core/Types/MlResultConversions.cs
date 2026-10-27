// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0


using System.Reflection;
using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Types;
public static partial class MlResultConversions
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
                                                                   string           exceptionAdditionalMessage = null!)
        => source.TryToInternalMlResult<T, TReturn>(value, exceptionAdditionalMessage);

    public static MlResult<TReturn> TryToMlResult<T, TReturn>(this Func<T, TReturn>         source, 
                                                                   T                        value,
                                                                   Func<Exception, string>  errorMessageBuilder)
        => source.TryToInternalMlResult<T, TReturn>(value, errorMessageBuilder);


    public static MlResult<TReturn> TryToMlResult<T, TReturn>(this Func<T, MlResult<TReturn>> source, 
                                                                   T                          value,
                                                                   string                     exceptionAdditionalMessage = null!)
        => source.TryToInternalMlResult<T, TReturn>(value, exceptionAdditionalMessage);

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
                                                                                    string                 exceptionAdditionalMessage = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T, TReturn>(value, exceptionAdditionalMessage);

    public static async Task<MlResult<TReturn>> TryToMlResultAsync<T, TReturn>(this Func<T, Task<TReturn>>  sourceAsync, 
                                                                                    T                       value,
                                                                                    Func<Exception, string> errorMessageBuilder)
        => await sourceAsync.TryToInternalMlResultAsync<T, TReturn>(value, errorMessageBuilder);


    public static async Task<MlResult<TReturn>> TryToMlResultAsync<T, TReturn>(this Func<T, Task<MlResult<TReturn>>> sourceAsync, 
                                                                                    T                                value,
                                                                                    string                           exceptionAdditionalMessage = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T, TReturn>(value, exceptionAdditionalMessage);


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
    //        string message = FormatErrorMessage(errorMessageBuilder, ex);

    //        var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
    //                                               Metadata: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

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
            string message = FormatErrorMessage(errorMessageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                                   Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

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
            string message = FormatErrorMessage(messageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                                   Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

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
    //        string message = FormatErrorMessage(errorMessageBuilder, ex);

    //        var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
    //                                    Metadata: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

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
            string message = FormatErrorMessage(errorMessageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                        Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

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
            string message = FormatErrorMessage(messageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                        Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

            return errorDetails.ToMlResultFail<T>();
        }
    }


    public static MlResult<T> TryToMlResultErrors<T>(this Action<MlErrorDetails> source,
                                                          MlErrorDetails         errorsDetails,
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



    public static async Task<MlResult<T>> TryToMlResultErrorsAsync<T>(this Func<MlErrorDetails, Task> sourceAsync,
                                                                           MlErrorDetails             errorsDetails,
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
    //                                                                       string exceptionAdditionalMessage = null!)
    //    => source.ToInternalMlResultEx<T, TReturn>(value, ex => exceptionAdditionalMessage ?? DEFAULT_ERROR_MESSAGE);

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
    //        string msg = FormatErrorMessage(errorMessageBuilder, ex);

    //        result = new MlErrorDetails(Errors : new List<MlError> { new MlError(msg) },
    //                                     Metadata: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

    //    }

    //    return result;
    //}




    public static MlResult<T> TryToMlResult<T>(this Func<MlErrorDetails,T> source,
                                                    MlErrorDetails         errorsDetails,
                                                    Func<Exception, string> errorMessageBuilder)
        => TryToInternalMlResult<T>(source, errorsDetails, errorMessageBuilder);


    public static MlResult<T> TryToMlResult<T>(this Func<MlErrorDetails, MlResult<T>> source,
                                                    MlErrorDetails                    errorsDetails,
                                                    Func<Exception, string>            errorMessageBuilder)
        => TryToInternalMlResult<T>(source, errorsDetails, errorMessageBuilder);


    public static Task<MlResult<T>> TryToMlResultAsync<T>(this Func<MlErrorDetails,Task<T>> sourceAsync,
                                                               MlErrorDetails               errorsDetails,
                                                               Func<Exception, string>       errorMessageBuilder)
        => TryToInternalMlResultAsync<T>(sourceAsync, errorsDetails, errorMessageBuilder);

    public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<MlErrorDetails, Task<MlResult<T>>> sourceAsync,
                                                                     MlErrorDetails                          errorsDetails,
                                                                     Func<Exception, string>                  errorMessageBuilder)
        => await TryToInternalMlResultAsync<T>(sourceAsync, errorsDetails, errorMessageBuilder);

    public static MlResult<T> TryToMlResult<T>(this Func<T>                 source,
                                                    MlErrorDetails         errorsDetails,
                                                    Func<Exception, string> errorMessageBuilder)
        => TryToInternalMlResult<T>(source, errorsDetails, errorMessageBuilder);

    public static Task<MlResult<T>> TryToMlResultAsync<T>(this Func<Task<T>>           sourceAsync,
                                                               MlErrorDetails         errorsDetails,
                                                               Func<Exception, string> errorMessageBuilder)
        => TryToInternalMlResultAsync<T>(sourceAsync, errorsDetails, errorMessageBuilder);



    public static MlResult<T> TryToMlResult<T>(this Action                  source, 
                                                    T                       value,
                                                    Func<Exception, string> messageBuilder = null!)
    {
        /// ********** Cuando ees una MlResult en Fail y con errores, no se añaden los nuevos errores

        try
        {
            source();

            return value.ToMlResultValid();
        }
        catch (Exception ex)
        {
            string message = FormatErrorMessage(messageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                                   Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

            return errorDetails.ToMlResultFail<T>();
        }
    }

    public static MlResult<T> TryToMlResultErrors<T>(this Action                  source,
                                                          MlErrorDetails         errorsDetails,
                                                          Func<Exception, string> errorMessageBuilder = null!)
    {
        MlResult<T> result = default!;

        try
        {
            source();

            result = errorsDetails.ToMlResultFail<T>();
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }


    public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<Task>              sourceAsync, 
                                                                     T                       value,
                                                                     Func<Exception, string> messageBuilder = null!)
    {
        /// ********** Cuando ees una MlResult en Fail y con errores, no se añaden los nuevos errores

        try
        {
            await sourceAsync();

            return value.ToMlResultValid();
        }
        catch (Exception ex)
        {
            string message = FormatErrorMessage(messageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                                   Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

            return errorDetails.ToMlResultFail<T>();
        }
    }



    public static async Task<MlResult<T>> TryToMlResultErrorsAsync<T>(this Func<Task>              sourceAsync,
                                                                           MlErrorDetails         errorsDetails,
                                                                           Func<Exception, string> errorMessageBuilder = null!)
    {
        MlResult<T> result = default!;

        try
        {
            await sourceAsync();

            result = errorsDetails.ToMlResultFail<T>();
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }





    private static MlResult<T> TryToInternalMlResult<T>(this object                  source,
                                                             MlErrorDetails         errorsDetails,
                                                             Func<Exception, string> errorMessageBuilder)
    {
        MlResult<T> result = default!;

        try
        {
            result = source switch
            {
                Func<T>                            mySource => mySource(),
                Func<MlErrorDetails, T>           mySource => mySource(errorsDetails),
                Func<MlErrorDetails, MlResult<T>> mySource => mySource(errorsDetails),
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
                                                                              MlErrorDetails         errorsDetails,
                                                                              Func<Exception, string> errorMessageBuilder)
    {
        MlResult<T> result = default!;

        try
        {
            result = source switch
            {
                Func<Task<T>>                            mySource => await mySource(),
                Func<MlErrorDetails, Task<T>>           mySource => await mySource(errorsDetails),
                Func<MlErrorDetails, Task<MlResult<T>>> mySource => await mySource(errorsDetails),
                _ => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
            };
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }













        

    public static string FormatErrorMessage(string errorMessage, Exception ex)
        => string.IsNullOrWhiteSpace(errorMessage) ? DEFAULT_EX_ERROR_MESSAGE(ex) : errorMessage;

    public static string FormatErrorMessage(Func<Exception, string> messageBuilder, Exception ex)
        => messageBuilder != null ? messageBuilder(ex) : DEFAULT_EX_ERROR_MESSAGE(ex);



    public static MlResult<T> ToMlResultValid<T>(this T                    source) => source;
    public static MlResult<T> ToMlResultFail <T>(this MlErrorDetails      source) => source;
    public static MlResult<T> ToMlResultFail <T>(this MlError              source) => source;
    public static MlResult<T> ToMlResultFail <T>(this string               source) => MlError.FromErrorMessage(source).ToMlResultFail<T>();
    public static MlResult<T> ToMlResultFail <T>(this List<MlError>        source) => source;
    public static MlResult<T> ToMlResultFail <T>(this List<string>         source) => MlErrorDetails.FromErrorsMessagesDetails(source);
    public static MlResult<T> ToMlResultFail <T>(this MlError[]            source) => source;
    public static MlResult<T> ToMlResultFail <T>(this string[]             source) => MlErrorDetails.FromErrorsMessagesDetails(source);
    public static MlResult<T> ToMlResultFail <T>(this IEnumerable<MlError> source) => new MlErrorDetails(source);
    public static MlResult<T> ToMlResultFail <T>(this IEnumerable<string>  source) => MlErrorDetails.FromErrorsMessagesDetails(source);
    public static MlResult<T> ToMlResultFail <T>(this (IEnumerable<MlError>, Dictionary<string, object>) source) => new MlErrorDetails(source.Item1, source.Item2);
    public static MlResult<T> ToMlResultFail <T>(this (IEnumerable<string> , Dictionary<string, object>) source) => MlErrorDetails.FromErrorsMessagesDetails(source.Item1, source.Item2);
    public static MlResult<T> ToMlResultFail <T>(this (MlError             , Dictionary<string, object>) source) => new MlErrorDetails(new List<MlError> { source.Item1 }, source.Item2);
    public static MlResult<T> ToMlResultFail <T>(this (string              , Dictionary<string, object>) source) => MlErrorDetails.FromErrorMessageDetails(source.Item1, source.Item2);

    public static Task<MlResult<T>> ToMlResultValidAsync<T>(this T                    source) => source.ToMlResultValid().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this MlErrorDetails      source) => source.ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this MlError              source) => source.ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this string               source) => MlError.FromErrorMessage(source).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this List<MlError>        source) => source.ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this List<string>         source) => MlErrorDetails.FromErrorsMessagesDetails(source).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this MlError[]            source) => source.ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this string[]             source) => MlErrorDetails.FromErrorsMessagesDetails(source).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this IEnumerable<MlError> source) => new MlErrorDetails(source).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this IEnumerable<string>  source) => MlErrorDetails.FromErrorsMessagesDetails(source).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (IEnumerable<MlError>, Dictionary<string, object>) source) => new MlErrorDetails(source.Item1, source.Item2).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (IEnumerable<string> , Dictionary<string, object>) source) => MlErrorDetails.FromErrorsMessagesDetails(source.Item1, source.Item2).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (MlError             , Dictionary<string, object>) source) => new MlErrorDetails(new List<MlError> { source.Item1 }, source.Item2).ToMlResultFail<T>().ToAsync();
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (string              , Dictionary<string, object>) source) => MlErrorDetails.FromErrorMessageDetails(source.Item1, source.Item2).ToMlResultFail<T>().ToAsync();




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
                                        .Map      ( _              => properties.FirstOrDefault(p => p.Name == nameof(MlResult<object>.IsSuccess) && p.PropertyType == typeof(bool)))
                                        .MapEnsure(isValidProperty => isValidProperty is not null,
                                                    _              => $"The encapsulated MlResult object does not have a property '{nameof(MlResult<object>.IsSuccess)}' of type bool. The type isn't MlResult.")
                                        .Map      (isValidProperty => isValidProperty!.GetValue(source))
                                        .MapEnsure(isValidObj      => isValidObj is bool,
                                                    _              => $"The '{nameof(MlResult<object>.IsSuccess)}' property is not of type bool")
                                        .MapEnsure(isValid         => (bool)isValid!,
                                                    _              => $"The encapsulated MlResult object in obj, from which you want to extract the '{nameof(MlResult<object>.SuccessValue)}' property, is in a Fail state, so this information cannot be retrieved.")
                                        .Map      ( _              => properties.FirstOrDefault(p => p.Name == nameof(MlResult<object>.SuccessValue)))
                                        .MapEnsure(valueProperty   => valueProperty is not null,
                                                    _              => $"The source object does not have a property '{nameof(MlResult<object>.SuccessValue)}'")
                                        .Map      ( valueProperty  => valueProperty!.GetValue(source));

        var result = partialResult.IsSuccess ? partialResult.SuccessValue : throw new ArgumentException(partialResult.FailureDetails.ToString());

        return result!;
    }

    public static Task<object> SecureGetValueFromMlResultBoxedAsync(this object source)
        => source.SecureGetValueFromMlResultBoxed().ToAsync();

    public static async Task<object> SecureGetValueFromMlResultBoxedAsync(this Task<object> sourceAsync)
        => await (await sourceAsync).SecureGetValueFromMlResultBoxedAsync();


    public static MlResult<object> ToMlResultObject<T>(this MlResult<T> source)
    {
        var result = source.Match(
                            fail : errorDetails => errorDetails    .ToMlResultFail <object>(),
                            valid: value        => ((object)value!).ToMlResultValid()
                        );
        return result;
    }
    public static Task<MlResult<object>> ToMlResultObjectAsync<T>(this MlResult<T> source)
        => source.ToMlResultObject().ToAsync();

    public static async Task<MlResult<object>> ToMlResultObjectAsync<T>(this Task<MlResult<T>> sourceAsync)
        => await (await sourceAsync).ToMlResultObjectAsync();


    public static MlResult<object> ToMlResultObject<T>(this T source) => ((object)source!).ToMlResultValid();

    public static Task<MlResult<object>> ToMlResultObjectAsync<T>(this T source) => source.ToMlResultObject().ToAsync();

    public static async Task<MlResult<object>> ToMlResultObjectAsync<T>(this Task<T> sourceAsync) => await (await sourceAsync).ToMlResultObjectAsync();


    public static MlResult<T> FromMlResultObject<T>(this MlResult<object> source)
    {
        var result = source.Match(
                                    fail : errorDetails => errorDetails.ToMlResultFail<T>(),
                                    valid: value        => (value is T tValue) ? tValue.ToMlResultValid() : MlResult<T>.Fail($"The value '{value}' of type '{value?.GetType()}' cannot be cast to the requested type '{typeof(T)}'.")
                                );
        return result;
    }

    public static Task<MlResult<T>> FromMlResultObjectAsync<T>(this MlResult<object> source)
        => source.FromMlResultObject<T>().ToAsync();

    public static async Task<MlResult<T>> FromMlResultObjectAsync<T>(this Task<MlResult<object>> sourceAsync)
        => await (await sourceAsync).FromMlResultObjectAsync<T>();









    private static MlResult<TReturn> TryToInternalMlResult<T, TReturn>(this object source, 
                                                                            T      value,
                                                                            string exceptionAdditionalMessage = null!)
        => source.TryToInternalMlResult<T, TReturn>(value, ex => exceptionAdditionalMessage ?? DEFAULT_ERROR_MESSAGE);

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
            string msg = FormatErrorMessage(errorMessageBuilder, ex);

            result = new MlErrorDetails(Errors : new List<MlError> { new MlError(msg) },
                                         Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });
            
        }

        return result;
    }






    private static async Task<MlResult<TReturn>> TryToInternalMlResultAsync<T, TReturn>(this object sourceAsync, 
                                                                                             T      value,
                                                                                             string exceptionAdditionalMessage = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T, TReturn>(value, ex => exceptionAdditionalMessage ?? DEFAULT_ERROR_MESSAGE);


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


            string msg = FormatErrorMessage(errorMessageBuilder, ex);

            result = new MlErrorDetails(Errors : new List<MlError> { new MlError(msg) },
                                         Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });
            
        }

        return result;
    }






}

