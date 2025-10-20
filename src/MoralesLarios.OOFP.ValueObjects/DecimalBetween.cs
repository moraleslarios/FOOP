namespace MoralesLarios.OOFP.ValueObjects;

public class DecimalBetween : ValueObject<decimal>
{
    protected DecimalBetween(decimal value, decimal minLength, decimal maxLength) : base(value)
    {
        if ( ! IsValid(value, minLength, maxLength)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, minLength, maxLength));
    }

    public static string BuildErrorMessage(decimal value, decimal minLength, decimal maxLength) => $"{value} must be between {minLength} and {maxLength}";
    public static bool IsValid(decimal value, decimal minLength, decimal maxLength) => value > minLength && value < maxLength;

    public static DecimalBetween FromDecimalLength(decimal value, decimal minLength, decimal maxLength) => new DecimalBetween(value, minLength, maxLength);

    public static MlResult<DecimalBetween> ByDecimalLength(decimal value, decimal minLength, decimal maxLength, MlErrorsDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, minLength, maxLength), errorsDetails ?? BuildErrorMessage(value, minLength, maxLength)))
                   .Map ( _ => new DecimalBetween(value, minLength, maxLength));

    public static implicit operator decimal(DecimalBetween valueObject) => valueObject.Value;
    public static implicit operator DecimalBetween((decimal value, decimal minLength, decimal maxLength) tupleValues)
        => new DecimalBetween(tupleValues.value, tupleValues.minLength, tupleValues.maxLength);
}
