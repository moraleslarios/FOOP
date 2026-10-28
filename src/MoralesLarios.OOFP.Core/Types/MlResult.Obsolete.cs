// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Representa <c>MlResult</c>.
/// </summary>
public partial record MlResult<T>
{
    /// <summary>
    /// Indica si valid.
    /// </summary>
    [global::System.Obsolete("Renamed to IsSuccess. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public bool IsValid => IsSuccess;

    /// <summary>
    /// Indica si fail.
    /// </summary>
    [global::System.Obsolete("Renamed to IsFailure. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public bool IsFail => IsFailure;
}
