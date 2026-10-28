// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Http;
public partial class RestClientFp<TDto>
{
    [global::System.Obsolete("Renamed to GetClientName. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public ServiceKey GetHttpClientFactoryKey() => GetClientName();
    [global::System.Obsolete("Renamed to GetApiInvoker. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public IHttpApiInvoker GetIHttpClientFactoryManager() => GetApiInvoker();
}
public partial class RestClientFp<TRequest, TResponse>
{
    [global::System.Obsolete("Renamed to GetClientName. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public ServiceKey GetHttpClientFactoryKey() => GetClientName();
    [global::System.Obsolete("Renamed to GetApiInvoker. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public IHttpApiInvoker GetIHttpClientFactoryManager() => GetApiInvoker();
}
