namespace MoralesLarios.OOFP.ValueObjects;

public class Empty : ValueObject<string>
{
    protected Empty(string value) : base(string.Empty)
    {
    }


    public static Empty Create() => new Empty(string.Empty);


    public static Task<Empty> CreateAsync() => Create().ToAsync();


}
