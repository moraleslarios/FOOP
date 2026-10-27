

using MoralesLarios.OOFP.Http.Requests;

namespace MoralesLarios.OOFP.Http;

public interface IHttpClientFactoryManager
{
    //Task<MlResult<TResponse?>> PostGetAsync<TRequest, TResponse>(CallRequestParamsInfo<TRequest> parameters);
    Task<MlResult<TResult>> PostGetAsync<T, TResult>(ServiceKey                        httpClientFactoryKey,
                                                     T                          itemBody,
                                                     string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                     Dictionary<string, string> headers = null!,
                                                     CancellationToken          ct      = default);
    Task<MlResult<PagedResult<TItem>>> PostGetPaginationAsync<TRequest, TItem>(CallRequestPaginationParamsInfo<TRequest> parameters, CancellationToken ct = default);
    MlResult<HttpClient> CreateHttpClient(ServiceKey httpClientFactoryKey);
    Task<MlResult<T>> GetAsync<T>(ServiceKey httpClientFactoryKey, string url = "", Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<T>> PostAsync<T>(ServiceKey httpClientFactoryKey, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> PutAsync<T>(ServiceKey httpClientFactoryKey, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> DeleteAsync<T>(ServiceKey httpClientFactoryKey, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> DeleteByIdAsync<T>(ServiceKey httpClientFactoryKey, NotEmptyString url, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<K>> PostAsync<T, K>(ServiceKey httpClientFactoryKey, T itemBody, string url = null, Dictionary<string, string> headers = null, CancellationToken ct = default);
}