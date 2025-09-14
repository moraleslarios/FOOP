using Microsoft.Extensions.DependencyInjection;

namespace MoralesLarios.OOFP.HttpClients;

public static class RegisterServices
{

    public static IServiceCollection AddHttpClientsFp(this IServiceCollection services)
    {
        services.AddTransient<IHttpClientFactoryManager, HttpClientFactoryManager>();

        return services;
    }


}
