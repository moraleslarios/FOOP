namespace MoralesLarios.OOFP.ValueObjects;

public class StringAsUShort : StringAsNumeric<ushort>
{
    protected StringAsUShort(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = ushort.Parse(value);
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid ushort";
    public static bool IsValid(string value) => ushort.TryParse(value, out _);

    public static StringAsUShort FromString(string value) => new StringAsUShort(value);

    public static MlResult<StringAsUShort> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsUShort(value));

    public static implicit operator ushort(StringAsUShort ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsUShort(string value) => new StringAsUShort(value);

    public static implicit operator string(StringAsUShort valueObject) => valueObject.Value.ToString()!;
}
