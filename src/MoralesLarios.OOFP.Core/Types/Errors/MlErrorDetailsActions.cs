// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types.Errors;

public static partial class MlErrorDetailsActions
{

    public static Dictionary<string, object> AppendExDetails(this MlErrorDetails source, Exception ex) => source.Metadata.AppendExDetails(ex);



    public static MlErrorDetails AppendExDetailsToMlDetails(this MlErrorDetails source, Exception ex) => (source.Errors,  source.Metadata.AppendExDetails(ex));

    public static MlErrorDetails AppendExDetailsToMlDetails(this MlErrorDetails source, 
                                                                  string          message, 
                                                                  Exception       ex) 
        => (source.Errors.Append(message),  source.Metadata.AppendExDetails(ex));





    public static MlErrorDetails AppendExErrorDetail(this MlErrorDetails source, 
                                                           Exception       ex,
                                                           Func<Exception, string> errorMessageBuilder = null!)
        => source.AppendExDetailsToMlDetails(errorMessageBuilder != null ? errorMessageBuilder(ex) : DEFAULT_EX_ERROR_MESSAGE(ex), ex);

    public static MlErrorDetails AppendExErrorDetail(this MlErrorDetails source,
                                                           Exception ex,
                                                           string message = null!)
        => source.AppendExDetailsToMlDetails(message != null ? message : DEFAULT_EX_ERROR_MESSAGE(ex), ex);



    public static MlErrorDetails Merge(this MlErrorDetails source, 
                                             MlErrorDetails other)
    {
        var errors = source.Errors.Concat(other.Errors).ToList();

        var sourceDetails = source?.Metadata ?? new Dictionary<string, object>();
        var otherDetails  = other ?.Metadata ?? new Dictionary<string, object>();

        var principalDetailsWithEx = sourceDetails.Where(x => x.Value is Exception)
                                           .Concat(otherDetails.Where(x => x.Value is Exception))
                                           .Select((x, index) => ($"{ExceptionMetadataKey}{(index == 0 ? string.Empty : index.ToString())}", x.Value))
                                           .ToDictionary(x => x.Item1, x => x.Item2);

        //var printipalDetailsWitoutEx = source.Metadata.Where(x => x.Key != ExceptionMetadataKey).ToDictionary(x => x.Key, x => x.SuccessValue);
        //var otherDetailsWitoutEx     = other .Metadata.Where(x => x.Key != ExceptionMetadataKey).ToDictionary(x => x.Key, x => x.SuccessValue);

        var printipalDetailsWitoutEx = sourceDetails.Where(x => x.Value is not Exception).ToDictionary(x => x.Key, x => x.Value);
        var otherDetailsWitoutEx     = otherDetails .Where(x => x.Value is not Exception).ToDictionary(x => x.Key, x => x.Value);

        var details = printipalDetailsWitoutEx.Concat(otherDetailsWitoutEx)
                                              .Concat(principalDetailsWithEx).ToDictionary(x => x.Key, x => x.Value);

        var result = (errors, details);

        return result;
    }



    public static MlErrorDetails Merge(this MlErrorDetails              source,
                                             IEnumerable<MlErrorDetails> errorsDetails)
    {
        foreach (var errorDetails in errorsDetails)
        {
            source = source.Merge(errorDetails);
        }

        return source;
    }



    public static MlResult<TReturn> MergeErrorsDetails<T, TReturn>(this MlErrorDetails source,
                                                                        MlResult<T>     secondary)
        => secondary.Match(
                                fail : source.Merge,
                                valid: _ => source
                           );

    public static Task<MlResult<TReturn>> MergeErrorsDetailsAsync<T, TReturn>(this MlErrorDetails source,
                                                                                   MlResult<T>     secondary)
        => source.MergeErrorsDetails<T, TReturn>(secondary).ToAsync();

    public static async Task<MlResult<TReturn>> MergeErrorsDetailsAsync<T, TReturn>(this Task<MlErrorDetails> sourceAsync,
                                                                                         MlResult<T>           secondary)
        => await (await sourceAsync).MergeErrorsDetailsAsync<T, TReturn>(secondary);

    public static async Task<MlResult<TReturn>> MergeErrorsDetailsAsync<T, TReturn>(this MlErrorDetails   source,
                                                                                   Task<MlResult<T>> secondaryAsync)
        => source.MergeErrorsDetails<T, TReturn>(await secondaryAsync);

    public static async Task<MlResult<TReturn>> MergeErrorsDetailsAsync<T, TReturn>(this Task<MlErrorDetails> sourceAsync,
                                                                                         Task<MlResult<T>>     secondaryAsync)
        => await (await sourceAsync).MergeErrorsDetailsAsync<T, TReturn>(await secondaryAsync);




    public static MlErrorDetails AddError(this MlErrorDetails source, MlError error) => (source.Errors.Append(error), source.Metadata);
    public static MlErrorDetails AddErrorMessage(this MlErrorDetails source, string errorMessage) => (source.Errors.Append(errorMessage), source.Metadata);
    public static MlErrorDetails AddErrors(this MlErrorDetails source, IEnumerable<MlError> errors) 
        => (source.Errors.Concat(errors).ToList(), source.Metadata);

    public static MlErrorDetails AddErrors(this MlErrorDetails source, params MlError[] errors) 
        => (source.Errors.Concat(errors).ToList(), source.Metadata);
    public static MlErrorDetails AddErrorsMessages(this MlErrorDetails source, IEnumerable<string> errorMessages) 
        => (source.Errors.Concat(errorMessages.ToMlErrors()).ToList(), source.Metadata);

    public static MlErrorDetails AddErrorsMessages(this MlErrorDetails source, params string[] errorMessages) 
        => (source.Errors.Concat(errorMessages.ToMlErrors()).ToList(), source.Metadata);

    public static MlErrorDetails AddMetadataOrThrow<T>(this MlErrorDetails source, string key, T value)    
    {
        source.Metadata.Add(key, value!);

        var result = (source.Errors, source.Metadata);

        return result;
    }

    public static MlErrorDetails AddMetadataRange(this MlErrorDetails source, Dictionary<string, object> otherDetails)    
    {
        var details = source.Metadata.Concat(otherDetails).ToDictionary(x => x.Key, x => x.Value);

        var result = (source.Errors, source.Metadata);

        return result;
    }

    public static MlErrorDetails AddMetadataRange(this MlErrorDetails source, params (string key, object value)[] otherDetails)
    {
        var details = source.Metadata.Concat(otherDetails.ToDictionary(x => x.key, x => x.value));

        var result = (source.Errors, source.Metadata);

        return result;
    }





    public static MlErrorDetails AddValueMetadata<T>(this MlErrorDetails source, T value)
    {
        if (source.Metadata.ContainsKey(ValueMetadataKey)) source.Metadata.Remove(ValueMetadataKey);

        var result = source.AddMetadataOrThrow(ValueMetadataKey, value!);

        return result;
    }



    public static bool ContainsValueMetadata(this MlErrorDetails source) => source.Metadata.ContainsKey(ValueMetadataKey);

    public static Task<bool> ContainsValueMetadataAsync(this MlErrorDetails source) => source.ContainsValueMetadata().ToAsync();


    public static bool ContainsException(this MlErrorDetails source) => source.Metadata.ContainsKey(ExceptionMetadataKey);

    public static Task<bool> ContainsExceptionAsync(this MlErrorDetails source) => source.ContainsException().ToAsync();

    public static bool ContainsMetadata(this MlErrorDetails source, string key) => source.Metadata.ContainsKey(key);



}

