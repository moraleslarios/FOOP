// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
public partial class DoubleLessThan
{
    [global::System.Obsolete("Renamed to FromDoubleLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<DoubleLessThan> ByDoubleLength(double value, double length, MlErrorDetails errorsDetails = null !) => FromDoubleLength(value, length, errorsDetails);
}
