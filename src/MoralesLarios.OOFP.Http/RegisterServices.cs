// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;

namespace MoralesLarios.OOFP.Http;

public static partial class RegisterServices
{

    public static IServiceCollection AddMlHttp(this IServiceCollection services)
    {
        services.AddTransient<IHttpApiInvoker, HttpApiInvoker>();

        return services;
    }



    public static IServiceCollection AddRestClient<TService, TImplementation>(this IServiceCollection services,
                                                                                    Func<ServiceKey>      clientNameFactory = null!,
                                                                                    Action<HttpClient> configureClient    = null!)
        where TService        : class
        where TImplementation : class, TService
    {
        var httpClientFactoryKey = ResolveHttpClientFactoryKey(clientNameFactory, typeof(TImplementation).Name!);

        services.AddHttpClient(httpClientFactoryKey, client =>
        {
            if (configureClient is not null) configureClient(client);
        });

        services.AddTransient<TService>(sp => ActivatorUtilities.CreateInstance<TImplementation>(sp, httpClientFactoryKey));

        return services;
    }


    public static IServiceCollection AddCompositeKeyRestClient<TService, TImplementation, TDto>(this IServiceCollection services,
                                                                                                 Func<ServiceKey>      clientNameFactory = null!,
                                                                                                 Action<HttpClient> configureClient    = null!)
        where TService        : class
        where TImplementation : class, TService
        where TDto            : class
    {

        var httpClientFactoryKey = ResolveHttpClientFactoryKey(clientNameFactory, typeof(TImplementation).Name!);

        services.AddHttpClient(httpClientFactoryKey, client =>
        {
            if (configureClient is not null) configureClient(client);
        });


        services.AddTransient<IRestClientFp<TDto>>(sp => ActivatorUtilities.CreateInstance<RestClientFp<TDto>>(sp, httpClientFactoryKey));
        services.AddTransient<TService          >(sp => ActivatorUtilities.CreateInstance<TImplementation  >(sp, httpClientFactoryKey));

        return services;
    }


    public static IServiceCollection AddCompositeKeyRestClient<TService, TImplementation, TRequest, TResponse>(this IServiceCollection services,
                                                                                                                      Func<ServiceKey>      clientNameFactory = null!,
                                                                                                                      Action<HttpClient> configureClient    = null!)
        where TService        : class
        where TImplementation : class, TService
        where TRequest        : class
        where TResponse       : class
    {

        var httpClientFactoryKey = ResolveHttpClientFactoryKey(clientNameFactory, typeof(TImplementation).Name!);

        services.AddHttpClient(httpClientFactoryKey, client =>
        {
            if (configureClient is not null) configureClient(client);
        });


        services.AddTransient<IRestClientFp<TRequest, TResponse>>(sp => ActivatorUtilities.CreateInstance<RestClientFp<TRequest, TResponse>>(sp, httpClientFactoryKey));
        services.AddTransient<TService                         >(sp => ActivatorUtilities.CreateInstance<TImplementation                 >(sp, httpClientFactoryKey));

        return services;
    }


    private static ServiceKey ResolveHttpClientFactoryKey(Func<ServiceKey> clientNameFactory, string defaultKeyName)
    {
        var httpClientFactoryKey = clientNameFactory is not null
            ? clientNameFactory()
            : ServiceKey.CreateFromString(defaultKeyName);

        if (httpClientFactoryKey is null)
            throw new InvalidOperationException("httpClientFactoryKey no puede ser null");

        return httpClientFactoryKey;
    }



}

