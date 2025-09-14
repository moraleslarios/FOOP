namespace MoralesLarios.OOFP.ValueObjects;

public class DoubleNotNegative : DoubleMoreThan
{
    public static double limit { get; private set; } =  -1.0;

    protected DoubleNotNegative(Double value) : base(value, limit)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));
    }

    public static string BuildErrorMessage(Double value) => $"{value} must be More than {limit}";
    public static bool IsValid(Double value) => value > limit;


    public static DoubleNotNegative FromDouble(Double value) => new DoubleNotNegative(value);
    public static MlResult<DoubleNotNegative> ByDouble(Double value)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)))
                    .Map ( _ => new DoubleNotNegative(value));

    public static implicit operator Double            (DoubleNotNegative valueObject) => valueObject.Value;
    public static implicit operator DoubleNotNegative (Double            value      ) => new DoubleNotNegative(value);

}