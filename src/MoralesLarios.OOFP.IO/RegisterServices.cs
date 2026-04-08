// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

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

