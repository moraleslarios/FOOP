namespace MoralesLarios.OOFP.ValueObjects;

public class DecimalLessThan : ValueObject<decimal>
{
    protected DecimalLessThan(decimal value, decimal length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, length));
    }

    public static string BuildErrorMessage(decimal value, decimal length) => $"{value} must be less than {length}";
    public static bool IsValid(decimal value, decimal length) => value < length;

    public static DecimalLessThan FromDecimalLength(decimal value, decimal length) => new DecimalLessThan(value, length);
    public static MlResult<DecimalLessThan> ByDecimalLength(decimal value, decimal length, MlErrorsDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? BuildErrorMessage(value, length)))
                   .Map ( _ => new DecimalLessThan(value, length));

    public static implicit operator decimal(DecimalLessThan valueObject) => valueObject.Value;
    public static implicit operator DecimalLessThan((decimal value, decimal length) tupleValues) => new DecimalLessThan(tupleValues.value, tupleValues.length);
}
