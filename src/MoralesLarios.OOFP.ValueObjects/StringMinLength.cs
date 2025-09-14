namespace MoralesLarios.OOFP.ValueObjects;

public class StringMinLength : ValueObject<NotEmptyString>
{
    protected StringMinLength(NotEmptyString value, int length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, length));
    }


    public static string BuildErrorMessage(string value, int lenght) => $"{value} cannot be less than {lenght} characters";
    public static bool IsValid(string value, int length) => value.Length >= length;

    public static StringMinLength FromStringLenght(string value, int lenght) => new StringMinLength(value, lenght);

    public static MlResult<StringMinLength> ByStringLength(string value, int lenght)
        => NotEmptyString.ByString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value, lenght), BuildErrorMessage(value, lenght)))
                            .Map ( _ => new StringMinLength(value, lenght));

    public static implicit operator string         (StringMinLength valueObject) => valueObject.Value;
    public static implicit operator StringMinLength((string value, int length) tupleValues) => new StringMinLength(tupleValues.value, tupleValues.length);
}
