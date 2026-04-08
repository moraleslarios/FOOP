// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

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

