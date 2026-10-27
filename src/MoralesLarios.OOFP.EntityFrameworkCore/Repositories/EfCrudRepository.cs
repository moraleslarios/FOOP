// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

public class EfCrudRepository<T, TContext>(TContext dbContext): EfRepository(dbContext), IEfRepository<T>
    where T        : class
    where TContext : DbContext
{
    internal readonly IEfReadRepository<T> _repoReaderFp = RegisterServices.GetRepository<IEfReadRepository<T>>();
    internal readonly IEfWriteRepository<T> _repoWriterFp = RegisterServices.GetRepository<IEfWriteRepository<T>>();



    public virtual MlResult<T> TryFind(params object[] keyValues) => _repoReaderFp.TryFind(keyValues);

    public virtual async Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] keyValues) => await _repoReaderFp.TryFindAsync(token, keyValues);

    public virtual MlResult<T> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoReaderFp.TryFind(notFoundErrorDetails, keyValues);

    public virtual async Task<MlResult<T>> TryFindAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoReaderFp.TryFindAsync(notFoundErrorDetails, token, keyValues);

    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter) => _repoReaderFp.TryFirst(filter);

    public virtual async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryFirstAsync(filter, token);

    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails) => _repoReaderFp.TryFirst(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderFp.TryFirstAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<T> TryGetLast(Expression<Func<T, bool>> filter) => _repoReaderFp.TryGetLast(filter);

    public virtual async Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryGetLastAsync(filter, token);

    public virtual MlResult<T> TryGetLast(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails) => _repoReaderFp.TryGetLast(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderFp.TryGetLastAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter) => _repoReaderFp.TryGetData(filter);

    public virtual async Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryGetDataAsync(filter, token);

    public virtual MlResult<IEnumerable<T>> TryAll() => _repoReaderFp.TryAll();

    public virtual async Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default!) => await _repoReaderFp.TryAllAsync(token);



    public virtual MlResult<T> TryAdd(T item) => _repoWriterFp.TryAdd(item);
    public virtual async Task<MlResult<T>> TryAddAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryAddAsync(item, token);
    public virtual MlResult<IEnumerable<T>> TryAddRange(IEnumerable<T> items) => _repoWriterFp.TryAddRange(items);
    public virtual async Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryAddRangeAsync(items, token);
    public virtual MlResult<T> TryRemove(T item) => _repoWriterFp.TryRemove(item);
    public virtual MlResult<T> TryRemove(params object[] keyValues) => _repoWriterFp.TryRemove(keyValues);
    public virtual MlResult<T> TryRemove(MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoWriterFp.TryRemove(notFoundErrorDetails, keyValues);
    public virtual async Task<MlResult<T>> TryRemoveAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryRemoveAsync(item, token);
    public virtual async Task<MlResult<T>> TryRemoveAsync(CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryRemoveAsync(token, keyValues);
    public virtual async Task<MlResult<T>> TryRemoveAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryRemoveAsync(notFoundErrorDetails, token, keyValues);
    public virtual MlResult<IEnumerable<T>> TryRemoveRange(IEnumerable<T> items) => _repoWriterFp.TryRemoveRange(items);
    public virtual async Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryRemoveRangeAsync(items, token);
    public virtual MlResult<T> TryUpdate(T item) => _repoWriterFp.TryUpdate(item);
    public virtual MlResult<T> TryUpdate(T item, params object[] keyValues) => _repoWriterFp.TryUpdate(item, keyValues);
    public virtual MlResult<T> TryUpdate(T item, MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoWriterFp.TryUpdate(item, notFoundErrorDetails, keyValues);
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryUpdateAsync(item, token);
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryUpdateAsync(item, token, keyValues);
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryUpdateAsync(item, notFoundErrorDetails, token, keyValues);
    public virtual MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items) => _repoWriterFp.TryUpdateRange(items);
    public virtual async Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryUpdateRangeAsync(items, token);

}
