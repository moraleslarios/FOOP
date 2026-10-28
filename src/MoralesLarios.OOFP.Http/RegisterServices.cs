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
        var clientName = ResolveClientName(clientNameFactory, typeof(TImplementation).Name!);

        services.AddHttpClient(clientName, client =>
        {
            if (configureClient is not null) configureClient(client);
        });

        services.AddTransient<TService>(sp => ActivatorUtilities.CreateInstance<TImplementation>(sp, clientName));

        return services;
    }


    public static IServiceCollection AddCompositeKeyRestClient<TService, TImplementation, TDto>(this IServiceCollection services,
                                                                                                 Func<ServiceKey>      clientNameFactory = null!,
                                                                                                 Action<HttpClient> configureClient    = null!)
        where TService        : class
        where TImplementation : class, TService
        where TDto            : class
    {

        var clientName = ResolveClientName(clientNameFactory, typeof(TImplementation).Name!);

        services.AddHttpClient(clientName, client =>
        {
            if (configureClient is not null) configureClient(client);
        });


        services.AddTransient<IRestClientFp<TDto>>(sp => ActivatorUtilities.CreateInstance<RestClientFp<TDto>>(sp, clientName));
        services.AddTransient<TService          >(sp => ActivatorUtilities.CreateInstance<TImplementation  >(sp, clientName));

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

        var clientName = ResolveClientName(clientNameFactory, typeof(TImplementation).Name!);

        services.AddHttpClient(clientName, client =>
        {
            if (configureClient is not null) configureClient(client);
        });


        services.AddTransient<IRestClientFp<TRequest, TResponse>>(sp => ActivatorUtilities.CreateInstance<RestClientFp<TRequest, TResponse>>(sp, clientName));
        services.AddTransient<TService                         >(sp => ActivatorUtilities.CreateInstance<TImplementation                 >(sp, clientName));

        return services;
    }


    private static ServiceKey ResolveClientName(Func<ServiceKey> clientNameFactory, string defaultKeyName)
    {
        var clientName = clientNameFactory is not null
            ? clientNameFactory()
            : ServiceKey.CreateFromString(defaultKeyName);

        if (clientName is null)
            throw new InvalidOperationException("clientName no puede ser null");

        return clientName;
    }



}

