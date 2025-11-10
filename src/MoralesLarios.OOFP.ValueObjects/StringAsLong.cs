namespace MoralesLarios.OOFP.ValueObjects;

public class StringAsLong : StringAsNumeric<long>
{
    protected StringAsLong(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = long.Parse(value);
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid long";
    public static bool IsValid(string value) => long.TryParse(value, out _);

    public static StringAsLong FromString(string value) => new StringAsLong(value);

    public static MlResult<StringAsLong> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsLong(value));

    public static implicit operator long(StringAsLong ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsLong(string value) => new StringAsLong(value);

    public static implicit operator string(StringAsLong valueObject) => valueObject.Value.ToString()!;
}
