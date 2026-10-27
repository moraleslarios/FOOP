// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types.Errors;
public partial class MlErrorDetails
{
    [global::System.Obsolete("Renamed to Metadata. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public Dictionary<string, object> Details => Metadata;
}
