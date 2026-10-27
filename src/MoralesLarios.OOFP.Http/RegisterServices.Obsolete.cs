// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using Microsoft.Extensions.DependencyInjection;

namespace MoralesLarios.OOFP.Http;
public static partial class RegisterServices
{
    [global::System.Obsolete("Renamed to AddRestClient. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddGenClientFp<TService, TImplementation>(this IServiceCollection services, Func<ServiceKey> configureHttpClientKey = null !, Action<HttpClient> configureClient = null !)
        where TService : class where TImplementation : class, TService => AddRestClient<TService, TImplementation>(services, configureHttpClientKey, configureClient);
    [global::System.Obsolete("Renamed to AddCompositeKeyRestClient. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddGenClientComplexFp<TService, TImplementation, TDto>(this IServiceCollection services, Func<ServiceKey> configureHttpClientKey = null !, Action<HttpClient> configureClient = null !)
        where TService : class where TImplementation : class, TService where TDto : class => AddCompositeKeyRestClient<TService, TImplementation, TDto>(services, configureHttpClientKey, configureClient);
    [global::System.Obsolete("Renamed to AddCompositeKeyRestClient. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddGenClientDuplexComplexFp<TService, TImplementation, TRequest, TResponse>(this IServiceCollection services, Func<ServiceKey> configureHttpClientKey = null !, Action<HttpClient> configureClient = null !)
        where TService : class where TImplementation : class, TService where TRequest : class where TResponse : class => AddCompositeKeyRestClient<TService, TImplementation, TRequest, TResponse>(services, configureHttpClientKey, configureClient);
    [global::System.Obsolete("Renamed to AddMlHttp. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddHttpClientsFp(this IServiceCollection services) => AddMlHttp(services);
}