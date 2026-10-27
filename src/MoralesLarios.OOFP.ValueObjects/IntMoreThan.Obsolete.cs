// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
public partial class IntMoreThan
{
    [global::System.Obsolete("Renamed to FromIntLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IntMoreThan FromIntLenght(int value, int lenght) => CreateFromIntLength(value, lenght);
    [global::System.Obsolete("Renamed to FromIntLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IntMoreThan> ByIntLength(int value, int length, MlErrorDetails errorsDetails = null !) => FromIntLength(value, length, errorsDetails);
}