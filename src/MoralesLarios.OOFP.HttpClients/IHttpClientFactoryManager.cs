

using MoralesLarios.OOFP.HttpClients.ParamsInfo;

namespace MoralesLarios.OOFP.HttpClients;

public interface IHttpClientFactoryManager
{
    Task<MlResult<TResponse?>> PostGetAsync<TRequest, TResponse>(CallRequestParamsInfo<TRequest> parameters);
    Task<MlResult<PaginationResultInfo<TEnumrableResponse>>> PostGetPaginationAsync<TRequest, TEnumrableResponse>(CallRequestPaginationParamsInfo<TRequest> parameters, CancellationToken ct = default);
    MlResult<HttpClient> CreateHttpClient(Key httpClientFactoryKey);
    Task<MlResult<T>> GetAsync<T>(Key httpClientFactoryKey, string url = "", Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<T>> PostAsync<T>(Key httpClientFactoryKey, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> PutAsync<T>(Key httpClientFactoryKey, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteAsync<T>(Key httpClientFactoryKey, T itemBody, string url = null, Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteByIdAsync<T>(Key httpClientFactoryKey, NotEmptyString url, Dictionary<string, string> headers = null, CancellationToken ct = default);
}