using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EFCore.Repos;

public interface IEFRepoDeleterFp<T>
    where T : class
{
    MlResult<T> TryRemove(T item);
    MlResult<T> TryRemove(params object[] pk);
    MlResult<T> TryRemove(MlErrorsDetails notFoundErrorDetails, params object[] pk);
    Task<MlResult<T>> TryRemoveAsync(T item, CancellationToken token = default);
    Task<MlResult<T>> TryRemoveAsync(CancellationToken token = default, params object[] pk);
    Task<MlResult<T>> TryRemoveAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk);
    MlResult<IEnumerable<T>> TryRemoveRange(IEnumerable<T> items);
    Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}