namespace MoralesLarios.OOFP.ValueObjects;

public class IdLite : IntMoreThan
{
    private static int Limit => 0;

    public IdLite(int value) : base(value, Limit)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), BuildErrorMessIdLite(value));
    }
    public static string BuildErrorMessIdLite(int value) => $"IdLite can be mayor 0.";
    public static bool IsValid(int value) => IdLite.IsValid(value, Limit);

    public static IdLite FromInt(int value) => new IdLite(value);
    public static MlResult<IdLite> ByInt(int value)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), BuildErrorMessIdLite(value)))
                    .Map ( _ => new IdLite(value));

    public static implicit operator int   (IdLite valueObject) => valueObject.Value;
    public static implicit operator IdLite(int    value      ) => new IdLite(value);


}
