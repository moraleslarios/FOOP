namespace MoralesLarios.OOFP.ValueObjects;

public class StringMaxLength : ValueObject<NotEmptyString>
{
    protected StringMaxLength(NotEmptyString value, int length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, length));
    }


    public static string BuildErrorMessage(string value, int lenght) => $"{value} cannot be longer than {lenght} characters";
    public static bool IsValid(string value, int length) => value.Length < length;

    public static StringMaxLength FromStringLenght(string value, int lenght) => new StringMaxLength(value, lenght);

    public static MlResult<StringMaxLength> ByStringLength(string value, int lenght)
        => NotEmptyString.ByString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value, lenght), BuildErrorMessage(value, lenght)))
                            .Map ( _ => new StringMaxLength(value, lenght));

    public static implicit operator string         (StringMaxLength valueObject) => valueObject.Value;
    public static implicit operator StringMaxLength((string value, int length) tupleValues) => new StringMaxLength(tupleValues.value, tupleValues.length);
}
