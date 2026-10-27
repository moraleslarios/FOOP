// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
public partial class DecimalBetween
{
    [global::System.Obsolete("Renamed to FromDecimalLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<DecimalBetween> ByDecimalLength(decimal value, decimal minLength, decimal maxLength, MlErrorDetails errorsDetails = null !) => FromDecimalLength(value, minLength, maxLength, errorsDetails);
}
