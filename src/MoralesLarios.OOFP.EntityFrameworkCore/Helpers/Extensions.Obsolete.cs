// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.EntityFrameworkCore.Helpers;
public static partial class Extensions
{
    [global::System.Obsolete("Renamed to FormatCompositeKey. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string GetPkValues(this object[] source) => FormatCompositeKey(source);
}
