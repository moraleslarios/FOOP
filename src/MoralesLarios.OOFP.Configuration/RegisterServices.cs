// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoralesLarios.OOFP.Configuration.Config;

namespace MoralesLarios.OOFP.Configuration;
/// <summary>
/// Representa <c>RegisterServices</c>.
/// </summary>
public static partial class RegisterServices
{


    /// <summary>
    /// Añade el elemento indicado de <c>MlConfigurationReader</c>.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <param name="configuration">Configuración de la aplicacion.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddMlConfigurationReader(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddTransient<IMlConfigurationReader>(x => new MlConfigurationReader(configuration));

        return services;
    }


}

