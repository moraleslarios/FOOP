namespace MoralesLarios.OOFP.EFCore.Repos;

public interface IEFRepoFp<T> : IEFRepoReaderFp<T>, IEFRepoWriterFp<T>
    where T : class
{
    //MlResult<T> TryFind(params object[] pk);
    //Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] pk);
    //MlResult<T> TryFirstOrDefault(Expression<Func<T, bool>> filter);
    //Task<MlResult<T>> TryFirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!);
    //MlResult<T> TryFirst(Expression<Func<T, bool>> filter);
    //Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!);
    //MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter);
    //Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!);
    //MlResult<IEnumerable<T>> TryAll();
    //Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default!);
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
    //Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default);
    //MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items);
    //Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default);
    //Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] pk);
    //MlResult<T> TryUpdate(T item, params object[] pk);
}