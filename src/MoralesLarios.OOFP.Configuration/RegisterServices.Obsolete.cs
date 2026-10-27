// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoralesLarios.OOFP.Configuration.Config;

namespace MoralesLarios.OOFP.Configuration;
public static partial class RegisterServices
{
    [global::System.Obsolete("Renamed to AddMlConfigurationReader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddMlUtilitiesConfig(this IServiceCollection services, IConfiguration configuration) => AddMlConfigurationReader(services, configuration);
}
