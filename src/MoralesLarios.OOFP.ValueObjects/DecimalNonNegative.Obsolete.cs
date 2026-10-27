// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
public partial class DecimalNonNegative
{
    [global::System.Obsolete("Renamed to FromDecimal. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<DecimalNonNegative> ByDecimal(decimal value, MlErrorDetails errorsDetails = null !) => FromDecimal(value, errorsDetails);
    [global::System.Obsolete("Renamed to MinAllowedValue. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static decimal limit => MinAllowedValue;
}