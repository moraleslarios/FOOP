namespace MoralesLarios.OOFP.ValueObjects;

public class StringAsDouble : StringAsNumeric<double>
{
    protected StringAsDouble(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = double.Parse(value);
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid double";
    public static bool IsValid(string value) => double.TryParse(value, out _);

    public static StringAsDouble FromString(string value) => new StringAsDouble(value);

    public static MlResult<StringAsDouble> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsDouble(value));

    public static implicit operator double(StringAsDouble ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsDouble(string value) => new StringAsDouble(value);

    public static implicit operator string(StringAsDouble valueObject) => valueObject.Value.ToString()!;
}
