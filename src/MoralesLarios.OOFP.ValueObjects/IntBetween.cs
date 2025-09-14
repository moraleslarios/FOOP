namespace MoralesLarios.OOFP.ValueObjects;

public class IntBetween : ValueObject<int>
{
    protected IntBetween(int value, int minLenght, int maxLenght) : base(value)
    {
        if ( ! IsValid(value, minLenght, maxLenght)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, minLenght, maxLenght));
    }

    public static string BuildErrorMessage(int value, int minLenght, int maxLenght) => $"{value} must be between {minLenght} and {maxLenght}";
    public static bool IsValid(int value, int minLenght, int maxLenght) => value > minLenght && value < maxLenght;


    public static IntBetween FromIntLenght(int value, int minLenght, int maxLenght) => new IntBetween(value, minLenght, maxLenght);

    public static MlResult<IntBetween> ByIntLength(int value, int minLenght, int maxLenght)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value, minLenght, maxLenght), BuildErrorMessage(value, minLenght, maxLenght)))
                    .Map ( _ => new IntBetween(value, minLenght, maxLenght));

    public static implicit operator int (IntBetween valueObject) => valueObject.Value;
    public static implicit operator IntBetween ((int value, int minLenght, int maxLenght) tupleValues) => new IntBetween(tupleValues.value, 
                                                                                                                         tupleValues.minLenght, 
                                                                                                                         tupleValues.maxLenght);



}