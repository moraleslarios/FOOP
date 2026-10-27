// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using Microsoft.Extensions.DependencyInjection;

namespace MoralesLarios.OOFP.AspNetCore.Controllers;
public static partial class RegisterServices
{
    [global::System.Obsolete("Renamed to AddMlControllers, which is itself a no-op. Remove the call. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddWebControllers(this IServiceCollection services) => services;
}
