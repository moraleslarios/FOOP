// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
public partial class DoubleBetween
{
    [global::System.Obsolete("Renamed to FromDoubleLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<DoubleBetween> ByDoubleLength(double value, double minLength, double maxLength, MlErrorDetails errorsDetails = null !) => FromDoubleLength(value, minLength, maxLength, errorsDetails);
}
