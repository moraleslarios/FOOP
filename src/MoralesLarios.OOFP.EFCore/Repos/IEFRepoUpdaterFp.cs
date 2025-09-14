using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EFCore.Repos;

public interface IEFRepoUpdaterFp<T>
    where T : class
{
    MlResult<T> TryUpdate(T item);
    MlResult<T> TryUpdate(T item, params object[] pk);
    MlResult<T> TryUpdate(T item, MlErrorsDetails notFoundErrorDetails, params object[] pk);
    Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default);
    Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] pk);
    Task<MlResult<T>> TryUpdateAsync(T item, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk);
    MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items);
    Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}