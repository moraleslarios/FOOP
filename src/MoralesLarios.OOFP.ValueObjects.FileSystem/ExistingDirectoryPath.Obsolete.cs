// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects.FileSystem;
public partial class ExistingDirectoryPath
{
    [global::System.Obsolete("Renamed to FromPath. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public new static MlResult<ExistingDirectoryPath> ByString(string directoryStr, MlErrorDetails errorsDetails = null !) => FromPath(directoryStr, errorsDetails);
}
