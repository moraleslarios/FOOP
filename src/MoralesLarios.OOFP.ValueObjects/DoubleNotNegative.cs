namespace MoralesLarios.OOFP.ValueObjects;

public class DoubleNotNegative : DoubleMoreThan
{
    public static double limit { get; private set; } =  -1.0;

    protected DoubleNotNegative(double value) : base(value, limit)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));
    }

    public static string BuildErrorMessage(double value) => $"{value} must be More than {limit}";
    public static bool IsValid(double value) => value > limit;


    public static DoubleNotNegative FromDouble(double value) => new DoubleNotNegative(value);
    public static MlResult<DoubleNotNegative> ByDouble(double value, MlErrorsDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value)))
                    .Map ( _ => new DoubleNotNegative(value));

    public static implicit operator double            (DoubleNotNegative valueObject) => valueObject.Value;
    public static implicit operator DoubleNotNegative (double            value      ) => new DoubleNotNegative(value);

}