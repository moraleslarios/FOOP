using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoralesLarios.OOFP.Utilities.Config;

namespace MoralesLarios.OOFP.Utilities;
public static class RegisterServices
{


    public static IServiceCollection AddMlUtilitiesConfig(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddTransient<IMlConfigManager>(x => new MlConfigManager(configuration));

        return services;
    }


}
