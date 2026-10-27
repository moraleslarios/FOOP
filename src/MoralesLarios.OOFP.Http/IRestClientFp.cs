

namespace MoralesLarios.OOFP.Http;

public interface IRestClientFp<TDto>
{
    Task<MlResult<EmptyValue>> DeleteAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> DeleteByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TDto>> GetByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    IHttpApiInvoker GetIHttpClientFactoryManager();
    Task<MlResult<TDto>> PostAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> PutAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> PutByIdAsync(NotEmptyString idStr, TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
}



public interface IRestClientFp<TRequest, TResponse>
{
    Task<MlResult<EmptyValue>> DeleteAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> DeleteByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<IEnumerable<TResponse>>> GetAllAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TResponse>> GetByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    IHttpApiInvoker GetIHttpClientFactoryManager();
    Task<MlResult<TResponse>> PostAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> PutAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> PutByIdAsync(NotEmptyString idStr, TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
}