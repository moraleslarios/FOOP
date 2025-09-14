using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EFCore.Repos;

public class EFRepoWriterFp<T, TContext>(TContext dbContext) : EFRepoBaseFp(dbContext), IEFRepoWriterFp<T>
    where T : class
    where TContext : DbContext
{
    internal readonly IEFRepoAdderFp<T>   _repoAdderFp   = RegisterServices.ResolveRepoFp<IEFRepoAdderFp  <T>>();
    internal readonly IEFRepoDeleterFp<T> _repoDeleterFp = RegisterServices.ResolveRepoFp<IEFRepoDeleterFp<T>>();
    internal readonly IEFRepoUpdaterFp<T> _repoUpdaterFp = RegisterServices.ResolveRepoFp<IEFRepoUpdaterFp<T>>();




    public MlResult<T> TryAdd(T item) => _repoAdderFp.TryAdd(item);
    public async Task<MlResult<T>> TryAddAsync(T item, CancellationToken token = default) => await _repoAdderFp.TryAddAsync(item, token);
    public MlResult<IEnumerable<T>> TryAddRange(IEnumerable<T> items) => _repoAdderFp.TryAddRange(items);
    public async Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoAdderFp.TryAddRangeAsync(items, token);
    public MlResult<T> TryRemove(T item) => _repoDeleterFp.TryRemove(item);
    public MlResult<T> TryRemove(params object[] pk) => _repoDeleterFp.TryRemove(pk);
    public MlResult<T> TryRemove(MlErrorsDetails notFoundErrorDetails, params object[] pk) => _repoDeleterFp.TryRemove(notFoundErrorDetails, pk);
    public async Task<MlResult<T>> TryRemoveAsync(T item, CancellationToken token = default) => await _repoDeleterFp.TryRemoveAsync(item, token);
    public async Task<MlResult<T>> TryRemoveAsync(CancellationToken token = default, params object[] pk) => await _repoDeleterFp.TryRemoveAsync(token, pk);
    public async Task<MlResult<T>> TryRemoveAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk) => await _repoDeleterFp.TryRemoveAsync(notFoundErrorDetails, token, pk);
    public MlResult<IEnumerable<T>> TryRemoveRange(IEnumerable<T> items) => _repoDeleterFp.TryRemoveRange(items);
    public async Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoDeleterFp.TryRemoveRangeAsync(items, token);
    public MlResult<T> TryUpdate(T item) => _repoUpdaterFp.TryUpdate(item);
    public MlResult<T> TryUpdate(T item, params object[] pk) => _repoUpdaterFp.TryUpdate(item, pk);
    public MlResult<T> TryUpdate(T item, MlErrorsDetails notFoundErrorDetails, params object[] pk) => _repoUpdaterFp.TryUpdate(item, notFoundErrorDetails, pk);
    public async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default) => await _repoUpdaterFp.TryUpdateAsync(item, token);
    public async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] pk) => await _repoUpdaterFp.TryUpdateAsync(item, token, pk);
    public async Task<MlResult<T>> TryUpdateAsync(T item, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk) => await _repoUpdaterFp.TryUpdateAsync(item, notFoundErrorDetails, token, pk);
    public MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items) => _repoUpdaterFp.TryUpdateRange(items);
    public async Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoUpdaterFp.TryUpdateRangeAsync(items, token);



}
