// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;

namespace MoralesLarios.OOFP.Http;

/// <summary>
/// Representa <c>RegisterServices</c>.
/// </summary>
public static partial class RegisterServices
{

    /// <summary>
    /// Añade el elemento indicado de <c>MlHttp</c>.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddMlHttp(this IServiceCollection services)
    {
        services.AddTransient<IHttpApiInvoker, HttpApiInvoker>();

        return services;
    }



    /// <summary>
    /// Añade el elemento indicado de <c>RestClient</c>.
    /// </summary>
    /// <typeparam name="TService">Tipo del servicio.</typeparam>
    /// <typeparam name="TImplementation">Tipo genérico TImplementation.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <param name="clientNameFactory">Función que crea la instancia solicitada.</param>
    /// <param name="configureClient">Acción a ejecutar.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
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

        services.AddTransient<TService>(sp => CreateClient<TImplementation>(sp, clientName));

        return services;
    }


    /// <summary>
    /// Añade el elemento indicado de <c>CompositeKeyRestClient</c>.
    /// </summary>
    /// <typeparam name="TService">Tipo del servicio.</typeparam>
    /// <typeparam name="TImplementation">Tipo genérico TImplementation.</typeparam>
    /// <typeparam name="TDto">Tipo del objeto de transferencia de datos.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <param name="clientNameFactory">Función que crea la instancia solicitada.</param>
    /// <param name="configureClient">Acción a ejecutar.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
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


        services.AddTransient<IRestClientFp<TDto>>(sp => CreateClient<RestClientFp<TDto>>(sp, clientName));
        services.AddTransient<TService           >(sp => CreateClient<TImplementation   >(sp, clientName));

        return services;
    }


    /// <summary>
    /// Añade el elemento indicado de <c>CompositeKeyRestClient</c>.
    /// </summary>
    /// <typeparam name="TService">Tipo del servicio.</typeparam>
    /// <typeparam name="TImplementation">Tipo genérico TImplementation.</typeparam>
    /// <typeparam name="TRequest">Tipo de la petición.</typeparam>
    /// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <param name="clientNameFactory">Función que crea la instancia solicitada.</param>
    /// <param name="configureClient">Acción a ejecutar.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
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


        services.AddTransient<IRestClientFp<TRequest, TResponse>>(sp => CreateClient<RestClientFp<TRequest, TResponse>>(sp, clientName));
        services.AddTransient<TService                          >(sp => CreateClient<TImplementation                  >(sp, clientName));

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


    /// <summary>
    /// Crea la instancia del cliente resolviendo sus dependencias desde el contenedor.
    /// Únicamente se le pasa <paramref name="clientName"/> cuando alguno de sus constructores
    /// públicos declara un parámetro de tipo <see cref="ServiceKey"/>; de lo contrario
    /// <c>ActivatorUtilities</c> rechazaría el argumento como sobrante.
    /// </summary>
    /// <typeparam name="T">Tipo del cliente a construir.</typeparam>
    /// <param name="serviceProvider">Proveedor de servicios desde el que se resuelven las dependencias.</param>
    /// <param name="clientName">Clave del servicio que identifica el cliente HTTP a utilizar.</param>
    /// <returns>Instancia de <typeparamref name="T"/> con sus dependencias resueltas.</returns>
    private static T CreateClient<T>(IServiceProvider serviceProvider, ServiceKey clientName)
        where T : class
        => AcceptsServiceKey(typeof(T))
                ? ActivatorUtilities.CreateInstance<T>(serviceProvider, clientName)
                : ActivatorUtilities.CreateInstance<T>(serviceProvider);


    /// <summary>
    /// Indica si el tipo declara algún constructor público que acepte un <see cref="ServiceKey"/>.
    /// </summary>
    /// <param name="type">Tipo a inspeccionar.</param>
    /// <returns><c>true</c> si algún constructor público admite un <see cref="ServiceKey"/>; en caso contrario, <c>false</c>.</returns>
    private static bool AcceptsServiceKey(Type type)
        => type.GetConstructors()
               .Any(ctor => ctor.GetParameters()
                                .Any(parameter => parameter.ParameterType == typeof(ServiceKey)));

}

