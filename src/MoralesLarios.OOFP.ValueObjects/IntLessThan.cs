namespace MoralesLarios.OOFP.ValueObjects;

public class IntLessThan : ValueObject<int>
{
    protected IntLessThan(int value, int length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, length));
    }

    public static string BuildErrorMessage(int value, int lenght) => $"{value} must be less than {lenght}";
    public static bool IsValid(int value, int length) => value < length;

    public static IntLessThan FromIntLenght(int value, int lenght) => new IntLessThan(value, lenght);
    public static MlResult<IntLessThan> ByIntLength(int value, int lenght)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value, lenght), BuildErrorMessage(value, lenght)))
                    .Map ( _ => new IntLessThan(value, lenght));

    public static implicit operator int (IntLessThan valueObject) => valueObject.Value;
    public static implicit operator IntLessThan ((int value, int length) tupleValues) => new IntLessThan(tupleValues.value, tupleValues.length);



}
