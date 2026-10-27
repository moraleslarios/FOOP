// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using System.Reflection;
using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Types;
public static partial class MlResultConversions
{
    [global::System.Obsolete("Renamed to FormatErrorMessage. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string BuildErrorMessage(string errorMessage, Exception ex) => FormatErrorMessage(errorMessage, ex);
    [global::System.Obsolete("Renamed to FormatErrorMessage. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string BuildErrorMessage(Func<Exception, string> messageBuilder, Exception ex) => FormatErrorMessage(messageBuilder, ex);
}
