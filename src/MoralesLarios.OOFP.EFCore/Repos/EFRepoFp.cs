
namespace MoralesLarios.OOFP.EFCore.Repos;

public class EFRepoFp<T, TContext>(TContext dbContext): EFRepoBaseFp(dbContext), IEFRepoFp<T>
    where T        : class
    where TContext : DbContext
{
    internal readonly IEFRepoReaderFp<T> _repoReaderFp = RegisterServices.ResolveRepoFp<IEFRepoReaderFp<T>>();
    internal readonly IEFRepoWriterFp<T> _repoWriterFp = RegisterServices.ResolveRepoFp<IEFRepoWriterFp<T>>();



    public MlResult<T> TryFind(params object[] pk) => _repoReaderFp.TryFind(pk);

    public async Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] pk) => await _repoReaderFp.TryFindAsync(token, pk);

    public MlResult<T> TryFind(MlErrorsDetails notFoundErrorDetails, params object[] pk) => _repoReaderFp.TryFind(notFoundErrorDetails, pk);

    public async Task<MlResult<T>> TryFindAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk) => await _repoReaderFp.TryFindAsync(notFoundErrorDetails, token, pk);

    public MlResult<T> TryFirstOrDefault(Expression<Func<T, bool>> filter) => _repoReaderFp.TryFirstOrDefault(filter);

    public async Task<MlResult<T>> TryFirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryFirstOrDefaultAsync(filter, token);

    public MlResult<T> TryFirstOrDefault(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails) => _repoReaderFp.TryFirstOrDefault(filter, notFoundErrorDetails);

    public async Task<MlResult<T>> TryFirstOrDefaultAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderFp.TryFirstOrDefaultAsync(filter, notFoundErrorDetails, token);

    public MlResult<T> TryFirst(Expression<Func<T, bool>> filter) => _repoReaderFp.TryFirst(filter);

    public async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryFirstAsync(filter, token);

    public MlResult<T> TryFirst(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails) => _repoReaderFp.TryFirst(filter, notFoundErrorDetails);

    public async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderFp.TryFirstAsync(filter, notFoundErrorDetails, token);

    public MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter) => _repoReaderFp.TryGetData(filter);

    public async Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryGetDataAsync(filter, token);

    public MlResult<IEnumerable<T>> TryAll() => _repoReaderFp.TryAll();

    public async Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default!) => await _repoReaderFp.TryAllAsync(token);



    public MlResult<T> TryAdd(T item) => _repoWriterFp.TryAdd(item);
    public async Task<MlResult<T>> TryAddAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryAddAsync(item, token);
    public MlResult<IEnumerable<T>> TryAddRange(IEnumerable<T> items) => _repoWriterFp.TryAddRange(items);
    public async Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryAddRangeAsync(items, token);
    public MlResult<T> TryRemove(T item) => _repoWriterFp.TryRemove(item);
    public MlResult<T> TryRemove(params object[] pk) => _repoWriterFp.TryRemove(pk);
    public MlResult<T> TryRemove(MlErrorsDetails notFoundErrorDetails, params object[] pk) => _repoWriterFp.TryRemove(notFoundErrorDetails, pk);
    public async Task<MlResult<T>> TryRemoveAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryRemoveAsync(item, token);
    public async Task<MlResult<T>> TryRemoveAsync(CancellationToken token = default, params object[] pk) => await _repoWriterFp.TryRemoveAsync(token, pk);
    public async Task<MlResult<T>> TryRemoveAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk) => await _repoWriterFp.TryRemoveAsync(notFoundErrorDetails, token, pk);
    public MlResult<IEnumerable<T>> TryRemoveRange(IEnumerable<T> items) => _repoWriterFp.TryRemoveRange(items);
    public async Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryRemoveRangeAsync(items, token);
    public MlResult<T> TryUpdate(T item) => _repoWriterFp.TryUpdate(item);
    public MlResult<T> TryUpdate(T item, params object[] pk) => _repoWriterFp.TryUpdate(item, pk);
    public MlResult<T> TryUpdate(T item, MlErrorsDetails notFoundErrorDetails, params object[] pk) => _repoWriterFp.TryUpdate(item, notFoundErrorDetails, pk);
    public async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryUpdateAsync(item, token);
    public async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] pk) => await _repoWriterFp.TryUpdateAsync(item, token, pk);
    public async Task<MlResult<T>> TryUpdateAsync(T item, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk) => await _repoWriterFp.TryUpdateAsync(item, notFoundErrorDetails, token, pk);
    public MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items) => _repoWriterFp.TryUpdateRange(items);
    public async Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryUpdateRangeAsync(items, token);

}