namespace MoralesLarios.OOFP.ValueObjects.IO;

public class MlDirectory : RegexValue
{
    public const string EndpointPattern = @"^(?:[a-zA-Z]:[\\\/]|\\\\[^\\\/]+[\\\/][^\\\/]+[\\\/]?|\.{1,2}[\\\/])?(?:[^<>:""/\\|?*\x00-\x1F]+[\\\/])*[^<>:""/\\|?*\x00-\x1F]*$";

    protected MlDirectory(NotEmptyString value) : base(value, EndpointPattern) { }

    public static string BuildErrorMessage(string value) => $"{value} is not a valid directory path";
    public static bool IsValid(string value) => RegexValue.IsValid(value, EndpointPattern);

    public static MlDirectory FromString(string value) => new MlDirectory(value);

    public static MlResult<MlDirectory> ByString(string value, MlErrorsDetails errorsDetails = null!)
        => NotEmptyString.ByString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value)))
                            .Map ( _ => new MlDirectory(value));

    public static implicit operator string     (MlDirectory valueObject) => valueObject.Value;
    public static implicit operator MlDirectory(string      value)       => new MlDirectory(value);
}
