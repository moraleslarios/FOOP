// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types;
public partial record MlResult<T>
{
    [global::System.Obsolete("Renamed to IsSuccess. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public bool IsValid => IsSuccess;

    [global::System.Obsolete("Renamed to IsFailure. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public bool IsFail => IsFailure;
}