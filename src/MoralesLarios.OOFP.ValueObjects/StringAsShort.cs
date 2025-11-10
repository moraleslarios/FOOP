namespace MoralesLarios.OOFP.ValueObjects;

public class StringAsShort : StringAsNumeric<short>
{
    protected StringAsShort(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = short.Parse(value);
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid short";
    public static bool IsValid(string value) => short.TryParse(value, out _);

    public static StringAsShort FromString(string value) => new StringAsShort(value);

    public static MlResult<StringAsShort> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsShort(value));

    public static implicit operator short(StringAsShort ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsShort(string value) => new StringAsShort(value);

    public static implicit operator string(StringAsShort valueObject) => valueObject.Value.ToString()!;
}
