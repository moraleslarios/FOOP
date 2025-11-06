namespace MoralesLarios.OOFP.Types.Errors;



public class MlErrorsDetails(IEnumerable<MlError>       Errors  = null!,
                             Dictionary<string, object> Details = null!)
{
    public IEnumerable<MlError>       Errors  { get; init; } = Errors  ?? new List<MlError>();
    public Dictionary<string, object> Details { get; init; } = Details ?? new();

    protected MlErrorsDetails(IEnumerable<string> errorMessages, 
                              Dictionary<string, object> details) : this()
    {
        Errors  = errorMessages.Select(x => MlError.FromErrorMessage(x));
        Details = details ?? new Dictionary<string, object>();
    }


    public static MlErrorsDetails FromErrorsMessagesDetails  (IEnumerable<string> errorsMessages, Dictionary<string, object> details = null!) => new(errorsMessages, details);
    public static MlErrorsDetails FromErrorMessageDetails    (string              errorMessage  , Dictionary<string, object> details = null!) => new(new List<MlError>{ MlError.FromErrorMessage(errorMessage) }, details);
    public static MlErrorsDetails FromEnumerableErrors       (IEnumerable<MlError> errors                                                   ) => new(errors);
    public static MlErrorsDetails FromEnumerableErrors       (IEnumerable<string>  errors                                                   ) => new(errors, null!);
    public static MlErrorsDetails FromEnumerableErrorsDetails(IEnumerable<MlError> errors       , Dictionary<string, object> details = null!) => new(errors, details);
    public static MlErrorsDetails FromEnumerableStrings      (IEnumerable<string> errorsMessages                                            ) => FromErrorsMessagesDetails(errorsMessages);
    public static MlErrorsDetails FromError                  (MlError             error                                                     ) => new(new List<MlError> { error });
    public static MlErrorsDetails FromErrorDetails           (MlError             error         , Dictionary<string, object> details = null!) => new(new List<MlError> { error }, details);
    public static MlErrorsDetails FromErrorDetails           (string              errorMessage  , string key, object detail                 ) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { key, detail} }); 
    public static MlErrorsDetails FromErrorMessage           (string              errorMessage                                              ) => FromErrorMessageDetails(errorMessage);

    public static MlErrorsDetails FromErrorMessageWithException(string errorMessage, Exception exception) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { EX_DESC_KEY, exception} });
    public static MlErrorsDetails FromErrorMessageWithValue    (string errorMessage, object    value    ) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { VALUE_KEY , value     } });
    public static MlErrorsDetails FromErrorMessageWithValue<T> (string errorMessage, T         value    ) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { VALUE_KEY , value!    } });


    public static implicit operator MlErrorsDetails(List<MlError> errors        ) => new(errors);
    public static implicit operator MlErrorsDetails(MlError[]     errors        ) => new(errors);
    public static implicit operator MlErrorsDetails(List<string>  errorsMessages) => FromErrorsMessagesDetails(errorsMessages);
    public static implicit operator MlErrorsDetails(string[]      errorsMessages) => FromErrorsMessagesDetails(errorsMessages);
    public static implicit operator MlErrorsDetails(MlError       error         ) => new(new List<MlError> { error });
    public static implicit operator MlErrorsDetails(string        errorMessage  ) => FromErrorMessageDetails(errorMessage);

    public static implicit operator MlErrorsDetails((IEnumerable<MlError>, Dictionary<string, object>) errorDetails) => new(errorDetails.Item1, errorDetails.Item2);
    public static implicit operator MlErrorsDetails((IEnumerable<string >, Dictionary<string, object>) errorDetails) => new(errorDetails.Item1, errorDetails.Item2);
    public static implicit operator MlErrorsDetails((MlError             , Dictionary<string, object>) errorDetails) => new(new List<MlError> { errorDetails.Item1 }, errorDetails.Item2);
    public static implicit operator MlErrorsDetails((string              , Dictionary<string, object>) errorDetails) => FromErrorMessageDetails(errorDetails.Item1, errorDetails.Item2);
    public static implicit operator MlErrorsDetails((string              , string, object)             errorDetail ) => FromErrorMessageDetails(errorDetail.Item1, new Dictionary<string, object>() { { errorDetail.Item2, errorDetail.Item3} });



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

        if (Details.Any())
        {
            sb.AppendLine("Details:");
            foreach (var detail in Details)
            {
                sb.AppendLine($"     {detail.Key}: {detail.Value}");
            }
        }

        var result = sb.ToString().TrimEnd();

        return result;
    }


}


public static class MlErrorDetailsExtensions
{


    public static MlErrorsDetails ToMlErrorsDetails(this IEnumerable<MlError> errors) => new(errors);
    public static MlErrorsDetails ToMlErrorsDetails(this (IEnumerable<MlError> errors, Dictionary<string, object> details) source) => new(source.errors, source.details);
    public static MlErrorsDetails ToMlErrorsDetails(this MlError error) => new(error.ToMlErrors());
    public static MlErrorsDetails ToMlErrorsDetails(this (MlError error, Dictionary<string, object> details) source) => new(source.error.ToMlErrors());
    public static MlErrorsDetails ToMlErrorsDetails(this IEnumerable<string> errorMessages) => new(errorMessages.ToMlErrors());
    public static MlErrorsDetails ToMlErrorsDetails(this (IEnumerable<string> errorsMessages, Dictionary<string, object> details) source) => new(source.errorsMessages.ToMlErrors(), source.details);
    public static MlErrorsDetails ToMlErrorsDetails(this string errorMessage) => new(errorMessage.ToMlErrors());
    public static MlErrorsDetails ToMlErrorsDetails(this (string errorMessage, Dictionary<string, object> details) source) => new(source.errorMessage.ToMlErrors(), source.details);


    public static IEnumerable<string> ToErrorsMessages(this MlErrorsDetails source) => source.Errors.Select(x => x.Message);



    public static string ToDescription(this IEnumerable<string> source) 
        => source.Count() > 1 
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, source)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, source);

    public static string ToErrorsDescription(this IEnumerable<MlError> source) 
        => source.Count() > 1 
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, source)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, source);

    public static string ToErrorsDescription(this MlErrorsDetails source) 
        => source.Errors.Count() > 1 
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, source.Errors)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, source.Errors);

    public static string ToDetailsDescription(this MlErrorsDetails source)
    {
        var errorData = source.Details.Select(x => $"{x.Key} : {x.Value}");
        return errorData.Count() > 1
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, errorData)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, errorData);
    }

    public static string ToErrorsDetailsDescription(this MlErrorsDetails source) => source.ToString();






}