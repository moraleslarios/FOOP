namespace MoralesLarios.OOFP.ValueObjects;

public class DecimalNotNegative : DecimalMoreThan
{
    public static decimal limit { get; private set; } =  -1.0m;

    protected DecimalNotNegative(decimal value) : base(value, limit)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));
    }

    public static string BuildErrorMessage(decimal value) => $"{value} must be More than {limit}";
    public static bool IsValid(decimal value) => value > limit;


    public static DecimalNotNegative FromDecimal(decimal value) => new DecimalNotNegative(value);
    public static MlResult<DecimalNotNegative> ByDecimal(decimal value, MlErrorsDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value)))
                    .Map ( _ => new DecimalNotNegative(value));

    public static implicit operator decimal            (DecimalNotNegative valueObject) => valueObject.Value;
    public static implicit operator DecimalNotNegative (decimal            value      ) => new DecimalNotNegative(value);

}
