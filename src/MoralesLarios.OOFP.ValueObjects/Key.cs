namespace MoralesLarios.OOFP.ValueObjects;

public class Key : StringMinLength
{
    public static int MinLenght => 1;

    public Key(string value) : base(value, MinLenght) => StringMinLength.FromStringLenght(value, MinLenght);

    public static string BuildErrorMessage(string value) => StringMinLength.BuildErrorMessage(value, MinLenght);
    public static new bool IsValid(string value, int length) => StringMinLength.IsValid(value, MinLenght);

    public static Key FromString(string value) => new Key(value);

    public static MlResult<Key> ByString(string value)
        => StringMinLength.ByStringLength(value, MinLenght)
                            .Map(_ => new Key(value));

    public static implicit operator string(Key   valueObject) => valueObject.Value;
    public static implicit operator Key   (string value      ) => new Key(value);
}
