namespace MoralesLarios.OOFP.ValueObjects;

public class Endpoint : RegexValue
{
    public const string EndpointPattern = @"^https://[A-Za-z0-9.-]+(?::[1-9][0-9]{0,4})?$"; // host + optional port

    protected Endpoint(NotEmptyString value) : base(value, EndpointPattern) { }

    public static string BuildErrorMessage(string value) => $"{value} is not a valid endpoint";
    public static bool   IsValid(string value)          => RegexValue.IsValid(value, EndpointPattern);

    public static Endpoint FromString(string value) => new Endpoint(value);

    public static MlResult<Endpoint> ByString(string value, MlErrorsDetails errorsDetails = null!)
        => NotEmptyString.ByString(value)
                            .Bind(_ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value)))
                            .Map (_ => new Endpoint(value));

    public static implicit operator string  (Endpoint valueObject) => valueObject.Value;
    public static implicit operator Endpoint(string   value      ) => new Endpoint(value);
}
        