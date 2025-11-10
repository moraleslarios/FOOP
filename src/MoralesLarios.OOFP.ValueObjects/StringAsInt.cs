namespace MoralesLarios.OOFP.ValueObjects;



public class StringAsInt : StringAsNumeric<int>
{

    protected StringAsInt(string value) : base(value)
    {
        if( ! IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = int.Parse(value);
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid int";
    public static bool IsValid(string value) => int.TryParse(value, out _);

    public static StringAsInt FromString(string value) => new StringAsInt(value);

    public static MlResult<StringAsInt> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsInt(value));


    public static implicit operator int(StringAsInt ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsInt(string value) => new StringAsInt(value);

    public static implicit operator string(StringAsInt valueObject) => valueObject.Value.ToString()!;

}
