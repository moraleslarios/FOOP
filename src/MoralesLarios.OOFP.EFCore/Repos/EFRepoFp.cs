// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EFCore.Repos;

public class EFRepoFp<T, TContext>(TContext dbContext): EFRepoBaseFp(dbContext), IEFRepoFp<T>
    where T        : class
    where TContext : DbContext
{
    internal readonly IEFRepoReaderFp<T> _repoReaderFp = RegisterServices.ResolveRepoFp<IEFRepoReaderFp<T>>();
    internal readonly IEFRepoWriterFp<T> _repoWriterFp = RegisterServices.ResolveRepoFp<IEFRepoWriterFp<T>>();



    public virtual MlResult<T> TryFind(params object[] pk) => _repoReaderFp.TryFind(pk);

    public virtual async Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] pk) => await _repoReaderFp.TryFindAsync(token, pk);

    public virtual MlResult<T> TryFind(MlErrorsDetails notFoundErrorDetails, params object[] pk) => _repoReaderFp.TryFind(notFoundErrorDetails, pk);

    public virtual async Task<MlResult<T>> TryFindAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk) => await _repoReaderFp.TryFindAsync(notFoundErrorDetails, token, pk);

    public virtual MlResult<T> TryFirstOrDefault(Expression<Func<T, bool>> filter) => _repoReaderFp.TryFirstOrDefault(filter);

    public virtual async Task<MlResult<T>> TryFirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryFirstOrDefaultAsync(filter, token);

    public virtual MlResult<T> TryFirstOrDefault(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails) => _repoReaderFp.TryFirstOrDefault(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryFirstOrDefaultAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderFp.TryFirstOrDefaultAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter) => _repoReaderFp.TryFirst(filter);

    public virtual async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryFirstAsync(filter, token);

    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails) => _repoReaderFp.TryFirst(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderFp.TryFirstAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<T> TryLastOrDefault(Expression<Func<T, bool>> filter) => _repoReaderFp.TryLastOrDefault(filter);

    public virtual async Task<MlResult<T>> TryLastOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryLastOrDefaultAsync(filter, token);

    public virtual MlResult<T> TryLastOrDefault(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails) => _repoReaderFp.TryLastOrDefault(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryLastOrDefaultAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderFp.TryLastOrDefaultAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<T> TryLast(Expression<Func<T, bool>> filter) => _repoReaderFp.TryLast(filter);

    public virtual async Task<MlResult<T>> TryLastAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryLastAsync(filter, token);

    public virtual MlResult<T> TryLast(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails) => _repoReaderFp.TryLast(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryLastAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderFp.TryLastAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter) => _repoReaderFp.TryGetData(filter);

    public virtual async Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryGetDataAsync(filter, token);

    public virtual MlResult<IEnumerable<T>> TryAll() => _repoReaderFp.TryAll();

    public virtual async Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default!) => await _repoReaderFp.TryAllAsync(token);



    public virtual MlResult<T> TryAdd(T item) => _repoWriterFp.TryAdd(item);
    public virtual async Task<MlResult<T>> TryAddAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryAddAsync(item, token);
    public virtual MlResult<IEnumerable<T>> TryAddRange(IEnumerable<T> items) => _repoWriterFp.TryAddRange(items);
    public virtual async Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryAddRangeAsync(items, token);
    public virtual MlResult<T> TryRemove(T item) => _repoWriterFp.TryRemove(item);
    public virtual MlResult<T> TryRemove(params object[] pk) => _repoWriterFp.TryRemove(pk);
    public virtual MlResult<T> TryRemove(MlErrorsDetails notFoundErrorDetails, params object[] pk) => _repoWriterFp.TryRemove(notFoundErrorDetails, pk);
    public virtual async Task<MlResult<T>> TryRemoveAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryRemoveAsync(item, token);
    public virtual async Task<MlResult<T>> TryRemoveAsync(CancellationToken token = default, params object[] pk) => await _repoWriterFp.TryRemoveAsync(token, pk);
    public virtual async Task<MlResult<T>> TryRemoveAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk) => await _repoWriterFp.TryRemoveAsync(notFoundErrorDetails, token, pk);
    public virtual MlResult<IEnumerable<T>> TryRemoveRange(IEnumerable<T> items) => _repoWriterFp.TryRemoveRange(items);
    public virtual async Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryRemoveRangeAsync(items, token);
    public virtual MlResult<T> TryUpdate(T item) => _repoWriterFp.TryUpdate(item);
    public virtual MlResult<T> TryUpdate(T item, params object[] pk) => _repoWriterFp.TryUpdate(item, pk);
    public virtual MlResult<T> TryUpdate(T item, MlErrorsDetails notFoundErrorDetails, params object[] pk) => _repoWriterFp.TryUpdate(item, notFoundErrorDetails, pk);
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryUpdateAsync(item, token);
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] pk) => await _repoWriterFp.TryUpdateAsync(item, token, pk);
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk) => await _repoWriterFp.TryUpdateAsync(item, notFoundErrorDetails, token, pk);
    public virtual MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items) => _repoWriterFp.TryUpdateRange(items);
    public virtual async Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryUpdateRangeAsync(items, token);

}
