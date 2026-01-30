namespace MoralesLarios.OOFP.ValueObjects;

public class Empty : ValueObject<string>
{
    protected Empty(string value) : base(string.Empty)
    {
    }


    public static Empty Create() => new Empty(string.Empty);

}
