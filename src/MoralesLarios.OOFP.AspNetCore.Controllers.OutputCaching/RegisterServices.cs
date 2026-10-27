// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching;

public static partial class RegisterServices
{
    public static IServiceCollection AddMlControllersOutputCaching(this IServiceCollection services, Action<OutputCacheOptions> options = null!)
    {
        services.AddOutputCache(outputCacheOptions =>
        {
            options?.Invoke(outputCacheOptions);

            //outputCacheOptions.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);

            outputCacheOptions.AddPolicy("PerControllerTag", Policies.ControllerScopedOutputCachePolicy.Instance);
        });

        return services;
    }





}

