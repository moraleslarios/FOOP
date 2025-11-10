namespace MoralesLarios.OOFP.ValueObjects;

public class StringAsSByte : StringAsNumeric<sbyte>
{
    protected StringAsSByte(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = sbyte.Parse(value);
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid sbyte";
    public static bool IsValid(string value) => sbyte.TryParse(value, out _);

    public static StringAsSByte FromString(string value) => new StringAsSByte(value);

    public static MlResult<StringAsSByte> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsSByte(value));

    public static implicit operator sbyte(StringAsSByte ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsSByte(string value) => new StringAsSByte(value);

    public static implicit operator string(StringAsSByte valueObject) => valueObject.Value.ToString()!;
}
