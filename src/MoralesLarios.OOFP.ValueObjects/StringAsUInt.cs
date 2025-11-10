namespace MoralesLarios.OOFP.ValueObjects;

public class StringAsUInt : StringAsNumeric<uint>
{
    protected StringAsUInt(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = uint.Parse(value);
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid uint";
    public static bool IsValid(string value) => uint.TryParse(value, out _);

    public static StringAsUInt FromString(string value) => new StringAsUInt(value);

    public static MlResult<StringAsUInt> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsUInt(value));

    public static implicit operator uint(StringAsUInt ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsUInt(string value) => new StringAsUInt(value);

    public static implicit operator string(StringAsUInt valueObject) => valueObject.Value.ToString()!;
}
