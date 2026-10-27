

namespace MoralesLarios.OOFP.Http;

public interface IGenComplexClientFp<TDto>
{
    Task<MlResult<EmptyValue>> DeleteAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> DeleteByIdAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default, params object[] pk);
    Task<MlResult<EmptyValue>> DeleteByIdAsync(object[] pk, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TDto>> GetByIdAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default, params object[] pk);
    Task<MlResult<TDto>> GetByIdAsync(object[] pk, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TDto>> PostAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> PutAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> PutByIdAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default, params object[] pk);
    Task<MlResult<EmptyValue>> PutByIdAsync(object[] pk, TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
}



/*********************************************************************************
 * 
 *                                      DUPLEX
 *                      
 * ********************************************************************************/



public interface IGenComplexClientFp<TRequest, TResponse>
{
    Task<MlResult<EmptyValue>> DeleteAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> DeleteByIdAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default, params object[] pk);
    Task<MlResult<EmptyValue>> DeleteByIdAsync(object[] pk, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<IEnumerable<TResponse>>> GetAllAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TResponse>> GetByIdAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default, params object[] pk);
    Task<MlResult<TResponse>> GetByIdAsync(object[] pk, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TResponse>> PostAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> PutAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<EmptyValue>> PutByIdAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default, params object[] pk);
    Task<MlResult<EmptyValue>> PutByIdAsync(object[] pk, TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
}