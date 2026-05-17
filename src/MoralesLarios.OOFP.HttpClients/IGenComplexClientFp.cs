

namespace MoralesLarios.OOFP.HttpClients;

public interface IGenComplexClientFp<TDto>
{
    Task<MlResult<Empty>> DeleteAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteByIdAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default, params object[] pk);
    Task<MlResult<Empty>> DeleteByIdAsync(object[] pk, Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TDto>> GetByIdAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default, params object[] pk);
    Task<MlResult<TDto>> GetByIdAsync(object[] pk, Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<TDto>> PostAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> PutAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> PutByIdAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default, params object[] pk);
    Task<MlResult<Empty>> PutByIdAsync(object[] pk, TDto itemBody, Dictionary<string, string> headers = null, CancellationToken ct = default);
}