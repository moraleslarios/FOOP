namespace MoralesLarios.OOFP.Types.Errors;

public static class MlErrorsDetailsActions
{

    public static Dictionary<string, object> AppendExDetails(this MlErrorsDetails source, Exception ex) => source.Details.AppendExDetails(ex);



    public static MlErrorsDetails AppendExDetailsToMlDetails(this MlErrorsDetails source, Exception ex) => (source.Errors,  source.Details.AppendExDetails(ex));

    public static MlErrorsDetails AppendExDetailsToMlDetails(this MlErrorsDetails source, 
                                                                  string          message, 
                                                                  Exception       ex) 
        => (source.Errors.Append(message),  source.Details.AppendExDetails(ex));





    public static MlErrorsDetails AppendExErrorDetail(this MlErrorsDetails source, 
                                                           Exception       ex,
                                                           Func<Exception, string> errorMessageBuilder = null!)
        => source.AppendExDetailsToMlDetails(errorMessageBuilder != null ? errorMessageBuilder(ex) : DEFAULT_EX_ERROR_MESSAGE(ex), ex);

    public static MlErrorsDetails AppendExErrorDetail(this MlErrorsDetails source,
                                                           Exception ex,
                                                           string message = null!)
        => source.AppendExDetailsToMlDetails(message != null ? message : DEFAULT_EX_ERROR_MESSAGE(ex), ex);



    public static MlErrorsDetails Merge(this MlErrorsDetails source, 
                                             MlErrorsDetails other)
    {
        var errors = source.Errors.Concat(other.Errors).ToList();

        var sourceDetails = source?.Details ?? new Dictionary<string, object>();
        var otherDetails  = other ?.Details ?? new Dictionary<string, object>();

        var principalDetailsWithEx = sourceDetails.Where(x => x.Value is Exception)
                                           .Concat(otherDetails.Where(x => x.Value is Exception))
                                           .Select((x, index) => ($"{EX_DESC_KEY}{(index == 0 ? string.Empty : index.ToString())}", x.Value))
                                           .ToDictionary(x => x.Item1, x => x.Item2);

        //var printipalDetailsWitoutEx = source.Details.Where(x => x.Key != EX_DESC_KEY).ToDictionary(x => x.Key, x => x.Value);
        //var otherDetailsWitoutEx     = other .Details.Where(x => x.Key != EX_DESC_KEY).ToDictionary(x => x.Key, x => x.Value);

        var printipalDetailsWitoutEx = sourceDetails.Where(x => x.Value is not Exception).ToDictionary(x => x.Key, x => x.Value);
        var otherDetailsWitoutEx     = otherDetails .Where(x => x.Value is not Exception).ToDictionary(x => x.Key, x => x.Value);

        var details = printipalDetailsWitoutEx.Concat(otherDetailsWitoutEx)
                                              .Concat(principalDetailsWithEx).ToDictionary(x => x.Key, x => x.Value);

        var result = (errors, details);

        return result;
    }



    public static MlErrorsDetails Merge(this MlErrorsDetails              source,
                                             IEnumerable<MlErrorsDetails> errorsDetails)
    {
        foreach (var errorDetails in errorsDetails)
        {
            source = source.Merge(errorDetails);
        }

        return source;
    }



    public static MlResult<TReturn> MergeErrorsDetails<T, TReturn>(this MlErrorsDetails source,
                                                                        MlResult<T>     secondary)
        => secondary.Match(
                                fail : source.Merge,
                                valid: _ => source
                           );

    public static Task<MlResult<TReturn>> MergeErrorsDetailsAsync<T, TReturn>(this MlErrorsDetails source,
                                                                                   MlResult<T>     secondary)
        => source.MergeErrorsDetails<T, TReturn>(secondary).ToAsync();

    public static async Task<MlResult<TReturn>> MergeErrorsDetailsAsync<T, TReturn>(this Task<MlErrorsDetails> sourceAsync,
                                                                                         MlResult<T>           secondary)
        => await (await sourceAsync).MergeErrorsDetailsAsync<T, TReturn>(secondary);

    public static async Task<MlResult<TReturn>> MergeErrorsDetailsAsync<T, TReturn>(this MlErrorsDetails   source,
                                                                                   Task<MlResult<T>> secondaryAsync)
        => source.MergeErrorsDetails<T, TReturn>(await secondaryAsync);

    public static async Task<MlResult<TReturn>> MergeErrorsDetailsAsync<T, TReturn>(this Task<MlErrorsDetails> sourceAsync,
                                                                                         Task<MlResult<T>>     secondaryAsync)
        => await (await sourceAsync).MergeErrorsDetailsAsync<T, TReturn>(await secondaryAsync);




    public static MlErrorsDetails AddError(this MlErrorsDetails source, MlError error) => (source.Errors.Append(error), source.Details);
    public static MlErrorsDetails AddErrorMessage(this MlErrorsDetails source, string errorMessage) => (source.Errors.Append(errorMessage), source.Details);
    public static MlErrorsDetails AddErrors(this MlErrorsDetails source, IEnumerable<MlError> errors) 
        => (source.Errors.Concat(errors).ToList(), source.Details);

    public static MlErrorsDetails AddErrors(this MlErrorsDetails source, params MlError[] errors) 
        => (source.Errors.Concat(errors).ToList(), source.Details);
    public static MlErrorsDetails AddErrorsMessages(this MlErrorsDetails source, IEnumerable<string> errorMessages) 
        => (source.Errors.Concat(errorMessages.ToMlErrors()).ToList(), source.Details);

    public static MlErrorsDetails AddErrorsMessages(this MlErrorsDetails source, params string[] errorMessages) 
        => (source.Errors.Concat(errorMessages.ToMlErrors()).ToList(), source.Details);

    public static MlErrorsDetails AddDetail<T>(this MlErrorsDetails source, string key, T value)    
    {
        source.Details.Add(key, value!);

        var result = (source.Errors, source.Details);

        return result;
    }

    public static MlErrorsDetails AddDetails(this MlErrorsDetails source, Dictionary<string, object> otherDetails)    
    {
        var details = source.Details.Concat(otherDetails).ToDictionary(x => x.Key, x => x.Value);

        var result = (source.Errors, source.Details);

        return result;
    }

    public static MlErrorsDetails AddDetails(this MlErrorsDetails source, params (string key, object value)[] otherDetails)
    {
        var details = source.Details.Concat(otherDetails.ToDictionary(x => x.key, x => x.value));

        var result = (source.Errors, source.Details);

        return result;
    }


    public static MlResult<T> GetDetail<T>(this MlErrorsDetails source, string key)
    {
        if ( ! source.Details.ContainsKey(key)) return source.AddError($"The key {key} does not exist in the details");
        
        var result = source.Details[key] is T value 
                            ? MlResult<T>.Valid(value) 
                            : source.AddError($"The key {key} does not contain a value of type {typeof(T).Name}");

        return result;
    }



    public static MlResult<T> MergeErrorsDetailsIfFail<T>(this MlResult<T> source,
                                                               MlResult<T> secondary)
        => source.Match(
                            fail : errorDetails => secondary.Match(
                                                                        fail : errorDetails2 => errorDetails.Merge(errorDetails2),
                                                                        valid: _             => errorDetails
                                                                    ),
                            valid: _ => source
                        );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this MlResult<T> source,
                                                                                MlResult<T> secondary)
        => await source.MatchAsync(
                                    failAsync : errorDetails => secondary.MatchAsync(
                                                                                failAsync : errorDetails2 => errorDetails.Merge(errorDetails2).ToMlResultFailAsync<T>(),
                                                                                validAsync: _             => errorDetails.ToMlResultFailAsync<T>()
                                                                            ),
                                    validAsync: _ => source.ToAsync()
                                );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                MlResult<T>       secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFail(secondary);

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                Task<MlResult<T>> secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFail(await secondary);

    public static MlResult<T> MergeErrorsDetailsIfFailDiferentTypes<T,T2>(this MlResult<T > source,
                                                                               MlResult<T2> secondary)
        => source.Match(
                                    fail : errorDetails => secondary.Match(
                                                                                fail : errorDetails2 => errorDetails.Merge(errorDetails2),
                                                                                valid: _             => errorDetails
                                                                            ),
                                    valid: _ => source
                                );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T,T2>(this MlResult<T > source,
                                                                                                MlResult<T2> secondary)
        => await source.MatchAsync(
                                    failAsync : errorDetails => secondary.MatchAsync(
                                                                                        failAsync : errorDetails2 => errorDetails.Merge(errorDetails2).ToMlResultFailAsync<T>(),
                                                                                        validAsync: _             => errorDetails.ToMlResultFailAsync<T>()
                                                                            ),
                                    validAsync: _ => source.ToAsync()
                                );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T,T2>(this Task<MlResult<T>> sourceAsync,
                                                                                                MlResult<T2>      secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFailDiferentTypes(secondary);

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T,T2>(this Task<MlResult<T>>  sourceAsync,
                                                                                                Task<MlResult<T2>> secondaryAsync)
        => (await sourceAsync).MergeErrorsDetailsIfFailDiferentTypes(await secondaryAsync);


    public static MlResult<T> GetDetailValue<T>(this MlErrorsDetails source)
    {
        if ( ! source.Details.ContainsKey(VALUE_KEY)) return source.AddError($"The key {VALUE_KEY} does not exist in the details");  //MlResult<T>.Fail($"The key {VALUE_KEY} does not exist in the details");
        
        var result = source.Details[VALUE_KEY] is T value 
                            ? MlResult<T>.Valid(value) 
                            : source.AddError($"The key {VALUE_KEY} does not contain a value of type {typeof(T).Name}");

        return result;
    }


    public static Task<MlResult<T>> GetDetailValueAsync<T>(this MlErrorsDetails source) => source.GetDetailValue<T>().ToAsync();


    public static MlResult<T> GetDetailException<T>(this MlErrorsDetails source) where T : Exception => source.GetDetail<T>(EX_DESC_KEY);
    public static Task<MlResult<T>> GetDetailExceptionAsync<T>(this MlErrorsDetails source) where T : Exception => source.GetDetail<T>(EX_DESC_KEY).ToAsync();
    public static MlResult<Exception> GetDetailException(this MlErrorsDetails source) => source.GetDetail<Exception>(EX_DESC_KEY);
    public static Task<MlResult<Exception>> GetDetailExceptionAsync(this MlErrorsDetails source) => source.GetDetail<Exception>(EX_DESC_KEY).ToAsync();



    public static MlErrorsDetails AddDetailValue<T>(this MlErrorsDetails source, T value)
    {
        if (source.Details.ContainsKey(VALUE_KEY)) source.Details.Remove(VALUE_KEY);

        var result = source.AddDetail(VALUE_KEY, value!);

        return result;
    }



    public static bool HasValueDetails(this MlErrorsDetails source) => source.Details.ContainsKey(VALUE_KEY);

    public static Task<bool> HasValueDetailsAsync(this MlErrorsDetails source) => source.HasValueDetails().ToAsync();


    public static bool HasExceptionDetails(this MlErrorsDetails source) => source.Details.ContainsKey(EX_DESC_KEY);

    public static Task<bool> HasExceptionDetailsAsync(this MlErrorsDetails source) => source.HasExceptionDetails().ToAsync();

    public static bool HasKeyDetails(this MlErrorsDetails source, string key) => source.Details.ContainsKey(key);



}
