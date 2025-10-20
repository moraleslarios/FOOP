namespace MoralesLarios.OOFP.ValueObjects;

public class DoubleMoreThan : ValueObject<double>
{
    protected DoubleMoreThan(double value, double length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, length));
    }

    public static string BuildErrorMessage(double value, double length) => $"{value} must be more than {length}";
    public static bool IsValid(double value, double length) => value > length;

    public static DoubleMoreThan FromDoubleLength(double value, double length) => new DoubleMoreThan(value, length);
    public static MlResult<DoubleMoreThan> ByDoubleLength(double value, double length, MlErrorsDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? BuildErrorMessage(value, length)))
                   .Map ( _ => new DoubleMoreThan(value, length));

    public static implicit operator double(DoubleMoreThan valueObject) => valueObject.Value;
    public static implicit operator DoubleMoreThan((double value, double length) tupleValues) => new DoubleMoreThan(tupleValues.value, tupleValues.length);
}
