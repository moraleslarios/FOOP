namespace MoralesLarios.OOFP.ValueObjects;

public class StringAsByte : StringAsNumeric<byte>
{
    protected StringAsByte(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = byte.Parse(value);
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid byte";
    public static bool IsValid(string value) => byte.TryParse(value, out _);

    public static StringAsByte FromString(string value) => new StringAsByte(value);

    public static MlResult<StringAsByte> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsByte(value));

    public static implicit operator byte(StringAsByte ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsByte(string value) => new StringAsByte(value);

    public static implicit operator string(StringAsByte valueObject) => valueObject.Value.ToString()!;
}
