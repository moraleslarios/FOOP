namespace MoralesLarios.OOFP.ValueObjects;

public class StringAsDecimal : StringAsNumeric<decimal>
{
    protected StringAsDecimal(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = decimal.Parse(value);
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid decimal";
    public static bool IsValid(string value) => decimal.TryParse(value, out _);

    public static StringAsDecimal FromString(string value) => new StringAsDecimal(value);

    public static MlResult<StringAsDecimal> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsDecimal(value));

    public static implicit operator decimal(StringAsDecimal ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsDecimal(string value) => new StringAsDecimal(value);

    public static implicit operator string(StringAsDecimal valueObject) => valueObject.Value.ToString()!;
}
