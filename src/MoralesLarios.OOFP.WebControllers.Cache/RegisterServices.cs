// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.WebControllers.Cache;

public static class RegisterServices
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, Action<OutputCacheOptions> options = null!)
    {
        services.AddOutputCache(outputCacheOptions =>
        {
            options?.Invoke(outputCacheOptions);

            //outputCacheOptions.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);

            outputCacheOptions.AddPolicy("PerControllerTag", Policies.PerControllerOutputCachePolicy.Instance);
        });

        return services;
    }
}

