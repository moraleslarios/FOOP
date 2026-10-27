// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching;
public static partial class RegisterServices
{
    [global::System.Obsolete("Renamed to AddMlControllersOutputCaching. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddWebControllersCache(this IServiceCollection services, Action<OutputCacheOptions> options = null !) => AddMlControllersOutputCaching(services, options);
}
