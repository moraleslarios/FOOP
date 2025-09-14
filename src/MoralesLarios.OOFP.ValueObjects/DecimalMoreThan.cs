namespace MoralesLarios.OOFP.ValueObjects;

public class DecimalMoreThan : ValueObject<decimal>
{
    protected DecimalMoreThan(decimal value, decimal length) : base(value)
    {
        if (!IsValid(value, length)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, length));
    }

    public static string BuildErrorMessage(decimal value, decimal length) => $"{value} must be more than {length}";
    public static bool IsValid(decimal value, decimal length) => value > length;

    public static DecimalMoreThan FromDecimalLength(decimal value, decimal length) => new DecimalMoreThan(value, length);
    public static MlResult<DecimalMoreThan> ByDecimalLength(decimal value, decimal length)
        => MlResult.Empty()
                   .Bind(_ => EnsureFp.That(value, IsValid(value, length), BuildErrorMessage(value, length)))
                   .Map(_ => new DecimalMoreThan(value, length));

    public static implicit operator decimal(DecimalMoreThan valueObject) => valueObject.Value;
    public static implicit operator DecimalMoreThan((decimal value, decimal length) tupleValues) => new DecimalMoreThan(tupleValues.value, tupleValues.length);
}
