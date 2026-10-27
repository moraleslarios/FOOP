// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoralesLarios.OOFP.Configuration.Config;

namespace MoralesLarios.OOFP.Configuration;
public static partial class RegisterServices
{


    public static IServiceCollection AddMlConfigurationReader(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddTransient<IMlConfigurationReader>(x => new MlConfigurationReader(configuration));

        return services;
    }


}

