// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
public partial class IntBetween
{
    [global::System.Obsolete("Renamed to FromIntLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IntBetween FromIntLenght(int value, int minLenght, int maxLenght) => CreateFromIntLength(value, minLenght, maxLenght);
    [global::System.Obsolete("Renamed to FromIntLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IntBetween> ByIntLength(int value, int minLength, int maxLength, MlErrorDetails errorsDetails = null !) => FromIntLength(value, minLength, maxLength, errorsDetails);
}