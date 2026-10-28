// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Policies;
/// <summary>
/// Representa <c>ControllerScopedOutputCachePolicy</c>.
/// </summary>
public sealed partial class ControllerScopedOutputCachePolicy
{
    /// <summary>
    /// Constante con bypass header.
    /// </summary>
    [global::System.Obsolete("Renamed to CacheBypassHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public const string BypassHeader = CacheBypassHeader;
}
