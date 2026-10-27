// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
public partial class Age
{
    [global::System.Obsolete("Renamed to FromInt. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<Age> ByInt(int value, MlErrorDetails errorsDetails = null !) => FromInt(value, errorsDetails);
}
