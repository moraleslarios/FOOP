

using MoralesLarios.OOFP.HttpClients.ParamsInfo;

namespace MoralesLarios.OOFP.HttpClients;

public interface IHttpClientFactoryManager
{
    //Task<MlResult<TResponse?>> PostGetAsync<TRequest, TResponse>(CallRequestParamsInfo<TRequest> parameters);
    Task<MlResult<TResult>> PostGetAsync<T, TResult>(Key                        httpClientFactoryKey,
                                                     T                          itemBody,
                                                     string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                     Dictionary<string, string> headers = null!,
                                                     CancellationToken          ct      = default);
    Task<MlResult<PaginationResultInfo<TEnumrableResponse>>> PostGetPaginationAsync<TRequest, TEnumrableResponse>(CallRequestPaginationParamsInfo<TRequest> parameters, CancellationToken ct = default);
    MlResult<HttpClient> CreateHttpClient(Key httpClientFactoryKey);
    Task<MlResult<T>> GetAsync<T>(Key httpClientFactoryKey, string url = "", Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<T>> PostAsync<T>(Key httpClientFactoryKey, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> PutAsync<T>(Key httpClientFactoryKey, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteAsync<T>(Key httpClientFactoryKey, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteByIdAsync<T>(Key httpClientFactoryKey, NotEmptyString url, Dictionary<string, string> headers = null!, CancellationToken ct = default);
}