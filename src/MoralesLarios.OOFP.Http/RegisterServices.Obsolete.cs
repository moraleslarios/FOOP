// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using Microsoft.Extensions.DependencyInjection;

namespace MoralesLarios.OOFP.Http;
/// <summary>
/// Representa <c>RegisterServices</c>.
/// </summary>
public static partial class RegisterServices
{
    /// <summary>
    /// Añade el elemento indicado de <c>GenClientFp</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TService">Tipo del servicio.</typeparam>
    /// <typeparam name="TImplementation">Tipo genérico TImplementation.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <param name="configureHttpClientKey">Clave identificativa.</param>
    /// <param name="configureClient">Acción a ejecutar.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddRestClient. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddGenClientFp<TService, TImplementation>(this IServiceCollection services, Func<ServiceKey> configureHttpClientKey = null !, Action<HttpClient> configureClient = null !)
        where TService : class where TImplementation : class, TService => AddRestClient<TService, TImplementation>(services, configureHttpClientKey, configureClient);
    /// <summary>
    /// Añade el elemento indicado de <c>GenClientComplexFp</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TService">Tipo del servicio.</typeparam>
    /// <typeparam name="TImplementation">Tipo genérico TImplementation.</typeparam>
    /// <typeparam name="TDto">Tipo del objeto de transferencia de datos.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <param name="configureHttpClientKey">Clave identificativa.</param>
    /// <param name="configureClient">Acción a ejecutar.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddCompositeKeyRestClient. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddGenClientComplexFp<TService, TImplementation, TDto>(this IServiceCollection services, Func<ServiceKey> configureHttpClientKey = null !, Action<HttpClient> configureClient = null !)
        where TService : class where TImplementation : class, TService where TDto : class => AddCompositeKeyRestClient<TService, TImplementation, TDto>(services, configureHttpClientKey, configureClient);
    /// <summary>
    /// Añade el elemento indicado de <c>GenClientDuplexComplexFp</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TService">Tipo del servicio.</typeparam>
    /// <typeparam name="TImplementation">Tipo genérico TImplementation.</typeparam>
    /// <typeparam name="TRequest">Tipo de la petición.</typeparam>
    /// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <param name="configureHttpClientKey">Clave identificativa.</param>
    /// <param name="configureClient">Acción a ejecutar.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddCompositeKeyRestClient. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddGenClientDuplexComplexFp<TService, TImplementation, TRequest, TResponse>(this IServiceCollection services, Func<ServiceKey> configureHttpClientKey = null !, Action<HttpClient> configureClient = null !)
        where TService : class where TImplementation : class, TService where TRequest : class where TResponse : class => AddCompositeKeyRestClient<TService, TImplementation, TRequest, TResponse>(services, configureHttpClientKey, configureClient);
    /// <summary>
    /// Añade el elemento indicado de <c>HttpClientsFp</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddMlHttp. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddHttpClientsFp(this IServiceCollection services) => AddMlHttp(services);
}