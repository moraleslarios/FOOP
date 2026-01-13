

using MoralesLarios.OOFP.HttpClients.ParamsInfo;

namespace MoralesLarios.OOFP.HttpClients;

public interface IHttpClientFactoryManager
{
    Task<MlResult<T>> DeleteAsync<T>(Key httpClientFactoryKey, T itemBody, string url = null!, CancellationToken ct = default);
    Task<MlResult<T>> DeleteByIdAsync<T>(Key httpClientFactoryKey, NotEmptyString url, CancellationToken ct = default);
    Task<MlResult<IEnumerable<T>>> GetAsync<T>(Key httpClientFactoryKey, string url = "", CancellationToken ct = default);
    Task<MlResult<T>> PostAsync<T>(Key httpClientFactoryKey, T itemBody, string url = null!, CancellationToken ct = default);
    Task<MlResult<TResponse?>> PostGetAsync<TRequest, TResponse>(CallRequestParamsInfo<TRequest> parameters);
    Task<MlResult<PaginationResultInfo<TEnumrableResponse>>> PostGetPaginationAsync<TRequest, TEnumrableResponse>(CallRequestPaginationParamsInfo<TRequest> parameters, CancellationToken ct = default);
    Task<MlResult<T>> PutAsync<T>(Key httpClientFactoryKey, T itemBody, string url = null!, CancellationToken ct = default);
    MlResult<HttpClient> CreateHttpClient(Key httpClientFactoryKey);
}