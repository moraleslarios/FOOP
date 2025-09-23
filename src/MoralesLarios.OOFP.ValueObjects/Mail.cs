namespace MoralesLarios.OOFP.ValueObjects;

public class Mail : RegexValue
{
    public const string EndpointPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

    protected Mail(NotEmptyString value) : base(value, EndpointPattern) { }

    public static string BuildErrorMessage(string value) => $"{value} is not a valid mail";
    public static bool IsValid(string value) => RegexValue.IsValid(value, EndpointPattern);

    public static Mail FromString(string value) => new Mail(value);

    public static MlResult<Mail> ByString(string value)
        => NotEmptyString.ByString(value)
                            .Bind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)))
                            .Map (_ => new Mail(value));

    public static implicit operator string(Mail valueObject) => valueObject.Value;
    public static implicit operator Mail(string value) => new Mail(value);
}