

using MoralesLarios.OOFP.HttpClients.ParamsInfo;

namespace MoralesLarios.OOFP.HttpClients;

public interface IHttpClientFactoryManager
{
    Task<MlResult<T>> DeleteAsync<T>(Name httpClientFactoryKey, T itemBody, string url = null!, CancellationToken ct = default);
    Task<MlResult<T>> DeleteByIdAsync<T>(Name httpClientFactoryKey, NotEmptyString url, CancellationToken ct = default);
    Task<MlResult<T>> GetAsync<T>(Name httpClientFactoryKey, string url, CancellationToken ct = default);
    Task<MlResult<T>> PostAsync<T>(Name httpClientFactoryKey, T itemBody, string url = null!, CancellationToken ct = default);
    Task<MlResult<TResponse?>> PostGetAsync<TRequest, TResponse>(CallRequestParamsInfo<TRequest> parameters);
    Task<MlResult<PaginationResultInfo<TEnumrableResponse>>> PostGetPaginationAsync<TRequest, TEnumrableResponse>(CallRequestPaginationParamsInfo<TRequest> parameters, CancellationToken ct = default);
    Task<MlResult<T>> PutAsync<T>(Name httpClientFactoryKey, T itemBody, string url = null!, CancellationToken ct = default);
}