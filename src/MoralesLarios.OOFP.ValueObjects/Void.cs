namespace MoralesLarios.OOFP.ValueObjects;

public class Void : ValueObject<string>
{
    protected Void(string value) : base(string.Empty)
    {
    }


    public static Void Create() => new Void(string.Empty);

}
