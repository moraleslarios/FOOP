namespace MoralesLarios.OOFP.ValueObjects;

public class StringBetweenLength : ValueObject<NotEmptyString>
{
    protected StringBetweenLength(NotEmptyString value, int minLenght, int maxLenght) : base(value)
    {
        if ( ! IsValid(value, minLenght, maxLenght)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, minLenght, maxLenght));
    }


    public static string BuildErrorMessage(string value, int minLenght, int maxLenght) => $"{value} must be between {maxLenght} and {minLenght}";
    public static bool IsValid(string value, int minLenght, int maxLenght) => value.Length > minLenght && value.Length < maxLenght;

    public static StringBetweenLength FromStringLenght(string value, int minLenght, int maxLenght) => new StringBetweenLength(value, minLenght, maxLenght);

    public static MlResult<StringBetweenLength> ByStringLength(string value, int minLenght, int maxLenght)
        => NotEmptyString.ByString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value, minLenght, maxLenght), BuildErrorMessage(value, minLenght, maxLenght)))
                            .Map ( _ => new StringBetweenLength(value, minLenght, maxLenght));

    public static implicit operator string         (StringBetweenLength valueObject) => valueObject.Value;
    public static implicit operator StringBetweenLength((string value, int minLenght, int maxLenght) tupleValues) => new StringBetweenLength(tupleValues.value, 
                                                                                                                                             tupleValues.minLenght, 
                                                                                                                                             tupleValues.maxLenght);
}
