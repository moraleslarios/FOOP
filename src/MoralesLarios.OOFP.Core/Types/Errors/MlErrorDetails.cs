// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types.Errors;



public partial class MlErrorDetails(IEnumerable<MlError>       Errors  = null!,
                             Dictionary<string, object> Details = null!)
{
    public IEnumerable<MlError>       Errors  { get; init; } = Errors  ?? new List<MlError>();
    public Dictionary<string, object> Metadata { get; init; } = Details ?? new();

    protected MlErrorDetails(IEnumerable<string> errorMessages, 
                              Dictionary<string, object> details) : this()
    {
        Errors  = errorMessages.Select(x => MlError.FromErrorMessage(x));
        Metadata = details ?? new Dictionary<string, object>();
    }


    public static MlErrorDetails FromErrorsMessagesDetails  (IEnumerable<string> errorsMessages, Dictionary<string, object> details = null!) => new(errorsMessages, details);
    public static MlErrorDetails FromErrorMessageDetails    (string              errorMessage  , Dictionary<string, object> details = null!) => new(new List<MlError>{ MlError.FromErrorMessage(errorMessage) }, details);
    public static MlErrorDetails FromEnumerableErrors       (IEnumerable<MlError> errors                                                   ) => new(errors);
    public static MlErrorDetails FromEnumerableErrors       (IEnumerable<string>  errors                                                   ) => new(errors, null!);
    public static MlErrorDetails FromEnumerableErrorsDetails(IEnumerable<MlError> errors       , Dictionary<string, object> details = null!) => new(errors, details);
    public static MlErrorDetails FromEnumerableStrings      (IEnumerable<string> errorsMessages                                            ) => FromErrorsMessagesDetails(errorsMessages);
    public static MlErrorDetails FromError                  (MlError             error                                                     ) => new(new List<MlError> { error });
    public static MlErrorDetails FromErrorDetails           (MlError             error         , Dictionary<string, object> details = null!) => new(new List<MlError> { error }, details);
    public static MlErrorDetails FromErrorDetails           (string              errorMessage  , string key, object detail                 ) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { key, detail} }); 
    public static MlErrorDetails FromErrorMessage           (string              errorMessage                                              ) => FromErrorMessageDetails(errorMessage);

    public static MlErrorDetails FromErrorMessageWithException(string errorMessage, Exception exception) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { ExceptionMetadataKey, exception} });
    public static MlErrorDetails FromErrorMessageWithValue    (string errorMessage, object    value    ) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { ValueMetadataKey , value     } });
    public static MlErrorDetails FromErrorMessageWithValue<T> (string errorMessage, T         value    ) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { ValueMetadataKey , value!    } });


    public static implicit operator MlErrorDetails(List<MlError> errors        ) => new(errors);
    public static implicit operator MlErrorDetails(MlError[]     errors        ) => new(errors);
    public static implicit operator MlErrorDetails(List<string>  errorsMessages) => FromErrorsMessagesDetails(errorsMessages);
    public static implicit operator MlErrorDetails(string[]      errorsMessages) => FromErrorsMessagesDetails(errorsMessages);
    public static implicit operator MlErrorDetails(MlError       error         ) => new(new List<MlError> { error });
    public static implicit operator MlErrorDetails(string        errorMessage  ) => FromErrorMessageDetails(errorMessage);

    public static implicit operator MlErrorDetails((IEnumerable<MlError>, Dictionary<string, object>) errorDetails) => new(errorDetails.Item1, errorDetails.Item2);
    public static implicit operator MlErrorDetails((IEnumerable<string >, Dictionary<string, object>) errorDetails) => new(errorDetails.Item1, errorDetails.Item2);
    public static implicit operator MlErrorDetails((MlError             , Dictionary<string, object>) errorDetails) => new(new List<MlError> { errorDetails.Item1 }, errorDetails.Item2);
    public static implicit operator MlErrorDetails((string              , Dictionary<string, object>) errorDetails) => FromErrorMessageDetails(errorDetails.Item1, errorDetails.Item2);
    public static implicit operator MlErrorDetails((string              , string, object)             errorDetail ) => FromErrorMessageDetails(errorDetail.Item1, new Dictionary<string, object>() { { errorDetail.Item2, errorDetail.Item3} });



    public override string ToString()
    {
        StringBuilder sb = new();

        if (Errors.Any())
        {
            sb.AppendLine("MlError:");
            foreach (var error in Errors)
            {
                sb.AppendLine($"     {error.Message}");
            }
        }

        if (Metadata.Any())
        {
            sb.AppendLine("Details:");
            foreach (var detail in Metadata)
            {
                sb.AppendLine($"     {detail.Key}: {detail.Value}");
            }
        }

        var result = sb.ToString().TrimEnd();

        return result;
    }


}


public static partial class MlErrorDetailsExtensions
{


    public static MlErrorDetails ToMlErrorsDetails(this IEnumerable<MlError> errors) => new(errors);
    public static MlErrorDetails ToMlErrorsDetails(this (IEnumerable<MlError> errors, Dictionary<string, object> details) source) => new(source.errors, source.details);
    public static MlErrorDetails ToMlErrorsDetails(this MlError error) => new(error.ToMlErrors());
    public static MlErrorDetails ToMlErrorsDetails(this (MlError error, Dictionary<string, object> details) source) => new(source.error.ToMlErrors());
    public static MlErrorDetails ToMlErrorsDetails(this IEnumerable<string> errorMessages) => new(errorMessages.ToMlErrors());
    public static MlErrorDetails ToMlErrorsDetails(this (IEnumerable<string> errorsMessages, Dictionary<string, object> details) source) => new(source.errorsMessages.ToMlErrors(), source.details);
    public static MlErrorDetails ToMlErrorsDetails(this string errorMessage) => new(errorMessage.ToMlErrors());
    public static MlErrorDetails ToMlErrorsDetails(this (string errorMessage, Dictionary<string, object> details) source) => new(source.errorMessage.ToMlErrors(), source.details);


    public static IEnumerable<string> GetErrorMessages(this MlErrorDetails source) => source.Errors.Select(x => x.Message);



    public static string ToDescription(this IEnumerable<string> source) 
        => source.Count() > 1 
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, source)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, source);

    public static string GetErrorsSummary(this IEnumerable<MlError> source) 
        => source.Count() > 1 
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, source)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, source);

    public static string GetErrorsSummary(this MlErrorDetails source) 
        => source.Errors.Count() > 1 
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, source.Errors)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, source.Errors);

    public static string GetMetadataSummary(this MlErrorDetails source)
    {
        var errorData = source.Metadata.Select(x => $"{x.Key} : {x.Value}");
        return errorData.Count() > 1
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, errorData)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, errorData);
    }

    public static string ToErrorsDetailsDescription(this MlErrorDetails source) => source.ToString();






}
