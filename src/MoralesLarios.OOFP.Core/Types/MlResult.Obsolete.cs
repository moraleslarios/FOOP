// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Miembros obsoletos de <see cref="MlResult{T}"/>, conservados como alias de compatibilidad con la versión 2.0.
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

/// <summary>
/// Miembros obsoletos de <see cref="MlResult"/>, conservados como alias de compatibilidad con la versión 2.0.
/// </summary>
public partial record MlResult
{
    /// <summary>
    /// Obtiene una instancia de descarte, utilizada cuando el valor transportado es irrelevante.
    /// </summary>
    [global::System.Obsolete("Renamed to Discard. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult _ => Discard;
}
