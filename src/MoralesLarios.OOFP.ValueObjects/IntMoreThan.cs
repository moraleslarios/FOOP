namespace MoralesLarios.OOFP.ValueObjects;

public class IntMoreThan : ValueObject<int>
{
    protected IntMoreThan(int value, int length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, length));
    }

    public static string BuildErrorMessage(int value, int lenght) => $"{value} must be More than {lenght}";
    public static bool IsValid(int value, int length) => value > length;


    public static IntMoreThan FromIntLenght(int value, int lenght) => new IntMoreThan(value, lenght);
    public static MlResult<IntMoreThan> ByIntLength(int value, int lenght, MlErrorsDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value, lenght), errorsDetails ?? BuildErrorMessage(value, lenght)))
                    .Map ( _ => new IntMoreThan(value, lenght));

    public static implicit operator int (IntMoreThan valueObject) => valueObject.Value;
    public static implicit operator IntMoreThan ((int value, int length) tupleValues) => new IntMoreThan(tupleValues.value, tupleValues.length);



}
