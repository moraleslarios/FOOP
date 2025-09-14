namespace MoralesLarios.OOFP.Types.Errors;


public record MlError
{
    public string Message { get; init; }

    public MlError(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) Message = DEFAULT_ERROR_MESSAGE;
        else                                    Message = message;
    }

    public override string ToString() => Message;

    public static MlError FromErrorMessage(string Message) => new MlError(Message);

    public static implicit operator MlError(string message) => new(message);

}


public static class MlErrorExtensions
{

    public static MlError ToMlError(this string message) => new(message);
    public static IEnumerable<MlError> ToMlErrors(this MlError error) => new List<MlError> { error };

    public static IEnumerable<MlError> ToMlErrors(this IEnumerable<string> messages) => messages.Select(x => x.ToMlError());
    public static IEnumerable<MlError> ToMlErrors(this string message) => new List<MlError> { message.ToMlError() };

}

