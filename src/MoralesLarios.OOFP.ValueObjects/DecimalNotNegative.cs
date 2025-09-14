namespace MoralesLarios.OOFP.ValueObjects;

public class DecimalNotNegative : DecimalMoreThan
{
    public static decimal limit { get; private set; } =  -1.0m;

    protected DecimalNotNegative(Decimal value) : base(value, limit)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));
    }

    public static string BuildErrorMessage(Decimal value) => $"{value} must be More than {limit}";
    public static bool IsValid(Decimal value) => value > limit;


    public static DecimalNotNegative FromDecimal(Decimal value) => new DecimalNotNegative(value);
    public static MlResult<DecimalNotNegative> ByDecimal(Decimal value)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)))
                    .Map ( _ => new DecimalNotNegative(value));

    public static implicit operator Decimal            (DecimalNotNegative valueObject) => valueObject.Value;
    public static implicit operator DecimalNotNegative (Decimal            value      ) => new DecimalNotNegative(value);

}
