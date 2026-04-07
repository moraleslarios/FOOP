

namespace MoralesLarios.OOFP.HttpClients;

public interface IGenClientFp<TDto>
{
    Task<MlResult<Empty>> DeleteAsync(TDto itemBody, Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<TDto>> GetByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<TDto>> PostAsync(TDto itemBody, Dictionary<string, string> headers = null, CancellationToken ct = default);
    Task<MlResult<Empty>> PutAsync(TDto itemBody, Dictionary<string, string> headers = null, CancellationToken ct = default);
}