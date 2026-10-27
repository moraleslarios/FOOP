

using MoralesLarios.OOFP.Http.Requests;

namespace MoralesLarios.OOFP.Http;

public interface IHttpApiInvoker
{
    //Task<MlResult<TResponse?>> PostAndReadAsync<TRequest, TResponse>(ApiCallOptions<TRequest> parameters);
    Task<MlResult<TResult>> PostAndReadAsync<T, TResult>(ServiceKey                        clientName,
                                                     T                          itemBody,
                                                     string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                     Dictionary<string, string> headers = null!,
                                                     CancellationToken          ct      = default);
    Task<MlResult<PagedResult<TItem>>> PostAndReadPagedAsync<TRequest, TItem>(PagedApiCallOptions<TRequest> parameters, CancellationToken ct = default);
    MlResult<HttpClient> CreateHttpClient(ServiceKey clientName);
    Task<MlResult<T>> GetAsync<T>(ServiceKey clientName, string url = "", Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<T>> PostAsync<T>(ServiceKey clientName, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> PutAsync<T>(ServiceKey clientName, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> DeleteAsync<T>(ServiceKey clientName, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> DeleteByIdAsync<T>(ServiceKey clientName, NotEmptyString url, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TResponse>> PostAsync<T, TResponse>(ServiceKey clientName, T itemBody, string url = null, Dictionary<string, string> headers = null, CancellationToken ct = default);
}