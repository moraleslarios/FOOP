namespace MoralesLarios.OOFP.ValueObjects;

public class StringAsULong : StringAsNumeric<ulong>
{
    protected StringAsULong(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = ulong.Parse(value);
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid ulong";
    public static bool IsValid(string value) => ulong.TryParse(value, out _);

    public static StringAsULong FromString(string value) => new StringAsULong(value);

    public static MlResult<StringAsULong> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsULong(value));

    public static implicit operator ulong(StringAsULong ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsULong(string value) => new StringAsULong(value);

    public static implicit operator string(StringAsULong valueObject) => valueObject.Value.ToString()!;
}
