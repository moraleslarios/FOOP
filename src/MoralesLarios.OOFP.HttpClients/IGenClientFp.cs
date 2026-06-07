

namespace MoralesLarios.OOFP.HttpClients;

public interface IGenClientFp<TDto>
{
    Task<MlResult<Empty>> DeleteAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TDto>> GetByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    IHttpClientFactoryManager GetIHttpClientFactoryManager();
    Task<MlResult<TDto>> PostAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> PutAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> PutByIdAsync(NotEmptyString idStr, TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
}



public interface IGenClientFp<TRequest, TResponse>
{
    Task<MlResult<Empty>> DeleteAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<IEnumerable<TResponse>>> GetAllAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TResponse>> GetByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    IHttpClientFactoryManager GetIHttpClientFactoryManager();
    Task<MlResult<TResponse>> PostAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> PutAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> PutByIdAsync(NotEmptyString idStr, TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
}