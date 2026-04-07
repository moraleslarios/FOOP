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
                                                                                    Action<Key>        configureHttpClientKey = null!,
                                                                                    Action<HttpClient> configureClient        = null!)
    where TService        : class
    where TImplementation : class, TService
    {
        Key httpClientFactoryKey = null!;

        if (configureHttpClientKey is not null) configureHttpClientKey(httpClientFactoryKey);
        else httpClientFactoryKey = Key.FromString(typeof(TImplementation).Name!);

        services.AddHttpClient(httpClientFactoryKey, client =>
        {
            if (configureClient is not null) configureClient(client);
        });

        services.AddTransient<TService>(sp => ActivatorUtilities.CreateInstance<TImplementation>(sp, httpClientFactoryKey));

        return services;
    }



}
