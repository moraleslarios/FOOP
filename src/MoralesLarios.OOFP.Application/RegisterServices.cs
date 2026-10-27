// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.Application;



public static partial class RegisterServices
{
    //public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration )
    //{
        


    //    return services;
    //}



    public static IServiceCollection AddTransientCrudServicesFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(ICrudServiceFp<, >), typeof(CrudServiceFp<, >));

        return services;
    }

    public static IServiceCollection AddScopedCrudServicesFpWithoutRepositories(this IServiceCollection services)
    {

        services.AddScoped(typeof(ICrudServiceFp<, >), typeof(CrudServiceFp<, >));

        return services;
    }


    public static IServiceCollection AddSingletonCrudServicesFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ICrudServiceFp<, >), typeof(CrudServiceFp<, >));

        return services;
    }


    public static IServiceCollection AddTransientCrudServicesDuplexFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(IGenServiceFp<,,>), typeof(GenServiceFp<,,>));

        return services;
    }

    public static IServiceCollection AddScopedCrudServicesDuplexFpWithoutRepositories(this IServiceCollection services)
    {

        services.AddScoped(typeof(IGenServiceFp<,,>), typeof(GenServiceFp<,,>));

        return services;
    }


    public static IServiceCollection AddSingletonCrudServicesDuplexFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IGenServiceFp<,,>), typeof(GenServiceFp<,,>));

        return services;
    }



}


