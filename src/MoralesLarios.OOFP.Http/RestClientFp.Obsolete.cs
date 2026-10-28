// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Http;
/// <summary>
/// Representa <c>RestClientFp</c>.
/// </summary>
public partial class RestClientFp<TDto>
{
    /// <summary>
    /// Obtiene el elemento solicitado de <c>HttpClientFactoryKey</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <returns>Valor de tipo <c>ServiceKey</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to GetClientName. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public ServiceKey GetHttpClientFactoryKey() => GetClientName();
    /// <summary>
    /// Obtiene el elemento solicitado de <c>IHttpClientFactoryManager</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <returns>Valor de tipo <c>IHttpApiInvoker</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to GetApiInvoker. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public IHttpApiInvoker GetIHttpClientFactoryManager() => GetApiInvoker();
}
/// <summary>
/// Representa <c>RestClientFp</c>.
/// </summary>
public partial class RestClientFp<TRequest, TResponse>
{
    /// <summary>
    /// Obtiene el elemento solicitado de <c>HttpClientFactoryKey</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <returns>Valor de tipo <c>ServiceKey</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to GetClientName. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public ServiceKey GetHttpClientFactoryKey() => GetClientName();
    /// <summary>
    /// Obtiene el elemento solicitado de <c>IHttpClientFactoryManager</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <returns>Valor de tipo <c>IHttpApiInvoker</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to GetApiInvoker. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public IHttpApiInvoker GetIHttpClientFactoryManager() => GetApiInvoker();
}
