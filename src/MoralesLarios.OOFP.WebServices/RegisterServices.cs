

namespace MoralesLarios.OOFP.WebServices;



public static class RegisterServices
{
    //public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration )
    //{
        


    //    return services;
    //}



    public static IServiceCollection AddTransientGenServicesFpWithoutReposGeneral(this IServiceCollection services)
    {
        services.AddTransient(typeof(IGenServiceFp<, >), typeof(GenServiceFp<, >));

        return services;
    }

    public static IServiceCollection AddScopedtGenServicesFpWithoutReposGeneral(this IServiceCollection services)
    {

        services.AddScoped(typeof(IGenServiceFp<, >), typeof(GenServiceFp<, >));

        return services;
    }


    public static IServiceCollection AddSingletonGenServicesFpWithoutReposGeneral(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IGenServiceFp<, >), typeof(GenServiceFp<, >));

        return services;
    }

}
