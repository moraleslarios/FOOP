using System.Text.RegularExpressions;

namespace MoralesLarios.OOFP.ValueObjects;

public class RegexValue : ValueObject<NotEmptyString>
{
    private readonly string _pattern;

    protected RegexValue(NotEmptyString value, string pattern) : base(value)
    {
        if ( ! IsValid(value, pattern)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, pattern));

        _pattern = pattern;
    }

    public string Pattern => _pattern;

    public static string BuildErrorMessage(string value, string pattern)
        => $"{value} does not match regex pattern '{pattern}'";

    public static bool IsValid(string value, string pattern) => Regex.IsMatch(value, pattern);

    public static RegexValue FromRegex(string value, string pattern) => new RegexValue(NotEmptyString.FromString(value), pattern);

    public static MlResult<RegexValue> ByRegex(string value, string pattern, MlErrorsDetails errorsDetails = null!)
        => NotEmptyString.ByString(value)
            .Bind( _ => EnsureFp.That(value, IsValid(value, pattern), errorsDetails ?? BuildErrorMessage(value, pattern)))
            .Map ( _ => new RegexValue(value, pattern));

    public static implicit operator string(RegexValue valueObject) => valueObject.Value;
    public static implicit operator RegexValue((string value, string pattern) tupleValues)
        => new RegexValue(tupleValues.value, tupleValues.pattern);
}
