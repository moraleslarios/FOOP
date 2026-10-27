// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using System.Text.RegularExpressions;

namespace MoralesLarios.OOFP.ValueObjects;
public partial class RegexValue
{
    [global::System.Obsolete("Renamed to FromRegex. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<RegexValue> ByRegex(string value, string pattern, MlErrorDetails errorsDetails = null !) => FromRegex(value, pattern, errorsDetails);
}
