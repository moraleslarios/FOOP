namespace MoralesLarios.OOFP.ValueObjects;

public class StringAsNumeric<TValue> : ValueObject
    where TValue : struct
{

    protected TValue Value;

    protected StringAsNumeric(string value)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));

        Value = (TValue)Convert.ChangeType(value, typeof(TValue))!;
    }



    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }


    public static string BuildErrorMessage(string value) => $"{nameof(value)} should be a valid number";
    public static bool IsValid(string value) => Convert.ChangeType(value, typeof(TValue)) != null;

    public static StringAsNumeric<TValue> FromString(string value) => new StringAsNumeric<TValue>(value);

    public static MlResult<StringAsNumeric<TValue>> ByString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)),
                                            $"{BuildErrorMessage(value)}")
                    .Map(_ => new StringAsNumeric<TValue>(value));










    public static implicit operator TValue(StringAsNumeric<TValue> ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsNumeric<TValue>(string value) => new StringAsNumeric<TValue>(value);

    public static implicit operator string(StringAsNumeric<TValue> valueObject) => valueObject.Value.ToString()!;



    public override string? ToString() => Value.ToString();


}
