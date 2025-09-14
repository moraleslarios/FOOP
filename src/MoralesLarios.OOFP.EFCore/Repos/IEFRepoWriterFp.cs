using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EFCore.Repos;

public interface IEFRepoWriterFp<T> : IEFRepoAdderFp<T>, IEFRepoUpdaterFp<T>, IEFRepoDeleterFp<T>
    where T : class
{
    //MlResult<T> TryAdd(T item);
    //Task<MlResult<T>> TryAddAsync(T item, CancellationToken token = default);
    //MlResult<IEnumerable<T>> TryAddRange(IEnumerable<T> items);
    //Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
    //MlResult<T> TryRemove(T item);
    //MlResult<T> TryRemove(params object[] pk);
    //Task<MlResult<T>> TryRemoveAsync(T item, CancellationToken token = default);
    //Task<MlResult<T>> TryRemoveAsync(CancellationToken token = default, params object[] pk);
    //MlResult<IEnumerable<T>> TryRemoveRange(IEnumerable<T> items);
    //Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default);
    //MlResult<T> TryUpdate(T item);
    //MlResult<T> TryUpdate(T item, params object[] pk);
    //MlResult<T> TryUpdate(T item, MlErrorsDetails notFoundErrorDetails, params object[] pk);
    //Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default);
    //Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] pk);
    //Task<MlResult<T>> TryUpdateAsync(T item, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk);
    //MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items);
    //Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}
