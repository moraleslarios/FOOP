namespace MoralesLarios.OOFP.ValueObjects;

public class StringAsFloat : StringAsNumeric<float>
{
    protected StringAsFloat(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = float.Parse(value);
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid float";
    public static bool IsValid(string value) => float.TryParse(value, out _);

    public static StringAsFloat FromString(string value) => new StringAsFloat(value);

    public static MlResult<StringAsFloat> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsFloat(value));

    public static implicit operator float(StringAsFloat ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsFloat(string value) => new StringAsFloat(value);

    public static implicit operator string(StringAsFloat valueObject) => valueObject.Value.ToString()!;
}
