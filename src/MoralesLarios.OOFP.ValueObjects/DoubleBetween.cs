namespace MoralesLarios.OOFP.ValueObjects;

public class DoubleBetween : ValueObject<double>
{
    protected DoubleBetween(double value, double minLength, double maxLength) : base(value)
    {
        if (!IsValid(value, minLength, maxLength)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, minLength, maxLength));
    }

    public static string BuildErrorMessage(double value, double minLength, double maxLength) => $"{value} must be between {minLength} and {maxLength}";
    public static bool IsValid(double value, double minLength, double maxLength) => value > minLength && value < maxLength;

    public static DoubleBetween FromDoubleLength(double value, double minLength, double maxLength) => new DoubleBetween(value, minLength, maxLength);

    public static MlResult<DoubleBetween> ByDoubleLength(double value, double minLength, double maxLength)
        => MlResult.Empty()
                   .Bind(_ => EnsureFp.That(value, IsValid(value, minLength, maxLength), BuildErrorMessage(value, minLength, maxLength)))
                   .Map(_ => new DoubleBetween(value, minLength, maxLength));

    public static implicit operator double(DoubleBetween valueObject) => valueObject.Value;
    public static implicit operator DoubleBetween((double value, double minLength, double maxLength) tupleValues)
        => new DoubleBetween(tupleValues.value, tupleValues.minLength, tupleValues.maxLength);
}
