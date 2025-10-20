namespace MoralesLarios.OOFP.ValueObjects;

public class DoubleLessThan : ValueObject<double>
{
    protected DoubleLessThan(double value, double length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, length));
    }

    public static string BuildErrorMessage(double value, double length) => $"{value} must be less than {length}";
    public static bool IsValid(double value, double length) => value < length;

    public static DoubleLessThan FromDoubleLength(double value, double length) => new DoubleLessThan(value, length);
    public static MlResult<DoubleLessThan> ByDoubleLength(double value, double length, MlErrorsDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? BuildErrorMessage(value, length)))
                   .Map ( _ => new DoubleLessThan(value, length));

    public static implicit operator double(DoubleLessThan valueObject) => valueObject.Value;
    public static implicit operator DoubleLessThan((double value, double length) tupleValues) => new DoubleLessThan(tupleValues.value, tupleValues.length);
}
