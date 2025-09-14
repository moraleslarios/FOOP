namespace MoralesLarios.OOFP.ValueObjects;

public class IntNotNegative : IntMoreThan
{
    public static int limit = -1;

    protected IntNotNegative(int value) : base(value, limit)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));
    }

    public static string BuildErrorMessage(int value) => $"{value} must be More than {limit}";
    public static bool IsValid(int value) => value > limit;


    public static IntNotNegative FromInt(int value) => new IntNotNegative(value);
    public static MlResult<IntNotNegative> ByInt(int value)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)))
                    .Map ( _ => new IntNotNegative(value));

    public static implicit operator int            (IntNotNegative valueObject) => valueObject.Value;
    public static implicit operator IntNotNegative (int            value      ) => new IntNotNegative(value);

}
