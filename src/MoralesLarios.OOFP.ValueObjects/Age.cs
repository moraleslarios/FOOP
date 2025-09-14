namespace MoralesLarios.OOFP.ValueObjects;

public class Age : IntMoreThan
{
    private static int Limit => -1;

    public Age(int value) : base(value, Limit)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), BuildErrorMessage(value));
    }
    public static string BuildErrorMessage(int value) => $"Age cannot be negative.";
    public static bool IsValid(int value) => Age.IsValid(value, Limit);

    public static Age FromInt(int value) => new Age(value);
    public static MlResult<Age> ByInt(int value)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), BuildErrorMessage(value)))
                    .Map ( _ => new Age(value));

    public static implicit operator int (Age valueObject) => valueObject.Value;
    public static implicit operator Age (int value) => new Age(value);


}
