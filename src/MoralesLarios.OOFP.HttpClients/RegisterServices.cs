// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;

namespace MoralesLarios.OOFP.HttpClients;

public static class RegisterServices
{

    public static IServiceCollection AddHttpClientsFp(this IServiceCollection services)
    {
        services.AddTransient<IHttpClientFactoryManager, HttpClientFactoryManager>();

        return services;
    }



    public static IServiceCollection AddGenClientFp<TService, TImplementation>(this IServiceCollection services,
                                                                                    Func<Key>      configureHttpClientKey = null!,
                                                                                    Action<HttpClient> configureClient    = null!)
        where TService        : class
        where TImplementation : class, TService
    {
        var httpClientFactoryKey = ResolveHttpClientFactoryKey(configureHttpClientKey, typeof(TImplementation).Name!);

        services.AddHttpClient(httpClientFactoryKey, client =>
        {
            if (configureClient is not null) configureClient(client);
        });

        services.AddTransient<TService>(sp => ActivatorUtilities.CreateInstance<TImplementation>(sp, httpClientFactoryKey));

        return services;
    }


    public static IServiceCollection AddGenClientComplexFp<TService, TImplementation, TDto>(this IServiceCollection services,
                                                                                                 Func<Key>      configureHttpClientKey = null!,
                                                                                                 Action<HttpClient> configureClient    = null!)
        where TService        : class
        where TImplementation : class, TService
        where TDto            : class
    {

        var httpClientFactoryKey = ResolveHttpClientFactoryKey(configureHttpClientKey, typeof(TImplementation).Name!);

        services.AddHttpClient(httpClientFactoryKey, client =>
        {
            if (configureClient is not null) configureClient(client);
        });


        services.AddTransient<IGenClientFp<TDto>>(sp => ActivatorUtilities.CreateInstance<GenClientFp<TDto>>(sp, httpClientFactoryKey));
        services.AddTransient<TService          >(sp => ActivatorUtilities.CreateInstance<TImplementation  >(sp, httpClientFactoryKey));

        return services;
    }


    public static IServiceCollection AddGenClientDuplexComplexFp<TService, TImplementation, TRequest, TResponse>(this IServiceCollection services,
                                                                                                                      Func<Key>      configureHttpClientKey = null!,
                                                                                                                      Action<HttpClient> configureClient    = null!)
        where TService        : class
        where TImplementation : class, TService
        where TRequest        : class
        where TResponse       : class
    {

        var httpClientFactoryKey = ResolveHttpClientFactoryKey(configureHttpClientKey, typeof(TImplementation).Name!);

        services.AddHttpClient(httpClientFactoryKey, client =>
        {
            if (configureClient is not null) configureClient(client);
        });


        services.AddTransient<IGenClientFp<TRequest, TResponse>>(sp => ActivatorUtilities.CreateInstance<GenClientFp<TRequest, TResponse>>(sp, httpClientFactoryKey));
        services.AddTransient<TService                         >(sp => ActivatorUtilities.CreateInstance<TImplementation                 >(sp, httpClientFactoryKey));

        return services;
    }


    private static Key ResolveHttpClientFactoryKey(Func<Key> configureHttpClientKey, string defaultKeyName)
    {
        var httpClientFactoryKey = configureHttpClientKey is not null
            ? configureHttpClientKey()
            : Key.FromString(defaultKeyName);

        if (httpClientFactoryKey is null)
            throw new InvalidOperationException("httpClientFactoryKey no puede ser null");

        return httpClientFactoryKey;
    }



}

