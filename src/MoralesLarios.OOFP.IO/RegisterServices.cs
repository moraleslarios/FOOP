using Microsoft.Extensions.DependencyInjection;

namespace MoralesLarios.OOFP.IO;

public static class RegisterServices
{

    public static IServiceCollection AddOOFPIO(this IServiceCollection services)
    {
        services.AddSingleton<IWrapperIO, WrapperIO>();

        return services;
    }



}
