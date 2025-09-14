namespace MoralesLarios.OOFP.ValueObjects;

public class Name : StringMinLength

{
    public static int MinLenght => 3;

    public Name(string value) : base(value, MinLenght) => StringMinLength.FromStringLenght(value, MinLenght);


    public static string BuildErrorMessage(string value) => StringMinLength.BuildErrorMessage(value, MinLenght);
    public static new bool IsValid(string value, int length) => StringMinLength.IsValid(value, MinLenght);

    public static Name FromString(string value) => new Name(value);

    public static MlResult<Name> ByString(string value)
        => StringMinLength.ByStringLength(value, MinLenght)  
                            .Map ( _ => new Name(value));

    public static implicit operator string(Name   valueObject) => valueObject.Value;
    public static implicit operator Name  (string value      ) => new Name(value);



}
