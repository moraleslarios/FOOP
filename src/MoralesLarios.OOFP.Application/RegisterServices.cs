// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.Application;



/// <summary>
/// Representa <c>RegisterServices</c>.
/// </summary>
public static partial class RegisterServices
{
    //public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration )
    //{
        


    //    return services;
    //}



    /// <summary>
    /// Añade el elemento indicado de <c>TransientCrudServicesFpWithoutRepositories</c>.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddTransientCrudServicesFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(ICrudServiceFp<, >), typeof(CrudServiceFp<, >));

        return services;
    }

    /// <summary>
    /// Añade el elemento indicado de <c>ScopedCrudServicesFpWithoutRepositories</c>.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddScopedCrudServicesFpWithoutRepositories(this IServiceCollection services)
    {

        services.AddScoped(typeof(ICrudServiceFp<, >), typeof(CrudServiceFp<, >));

        return services;
    }


    /// <summary>
    /// Añade el elemento indicado de <c>SingletonCrudServicesFpWithoutRepositories</c>.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddSingletonCrudServicesFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ICrudServiceFp<, >), typeof(CrudServiceFp<, >));

        return services;
    }


    /// <summary>
    /// Añade el elemento indicado de <c>TransientCrudServicesDuplexFpWithoutRepositories</c>.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddTransientCrudServicesDuplexFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(ICrudServiceFp<,,>), typeof(CrudServiceFp<,,>));

        return services;
    }

    /// <summary>
    /// Añade el elemento indicado de <c>ScopedCrudServicesDuplexFpWithoutRepositories</c>.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddScopedCrudServicesDuplexFpWithoutRepositories(this IServiceCollection services)
    {

        services.AddScoped(typeof(ICrudServiceFp<,,>), typeof(CrudServiceFp<,,>));

        return services;
    }


    /// <summary>
    /// Añade el elemento indicado de <c>SingletonCrudServicesDuplexFpWithoutRepositories</c>.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddSingletonCrudServicesDuplexFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ICrudServiceFp<,,>), typeof(CrudServiceFp<,,>));

        return services;
    }



}


