// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

public class EfWriteRepository<T, TContext>(TContext dbContext) : EfRepository(dbContext), IEfWriteRepository<T>
    where T : class
    where TContext : DbContext
{
    internal readonly IEfCreateRepository<T>   _repoAdderFp   = RegisterServices.GetRepository<IEfCreateRepository  <T>>();
    internal readonly IEfDeleteRepository<T> _repoDeleterFp = RegisterServices.GetRepository<IEfDeleteRepository<T>>();
    internal readonly IEfUpdateRepository<T> _repoUpdaterFp = RegisterServices.GetRepository<IEfUpdateRepository<T>>();




    public virtual MlResult<T> TryAdd(T item) => _repoAdderFp.TryAdd(item);
    public virtual async Task<MlResult<T>> TryAddAsync(T item, CancellationToken token = default) => await _repoAdderFp.TryAddAsync(item, token);
    public virtual MlResult<IEnumerable<T>> TryAddRange(IEnumerable<T> items) => _repoAdderFp.TryAddRange(items);
    public virtual async Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoAdderFp.TryAddRangeAsync(items, token);
    public virtual MlResult<T> TryRemove(T item) => _repoDeleterFp.TryRemove(item);
    public virtual MlResult<T> TryRemove(params object[] keyValues) => _repoDeleterFp.TryRemove(keyValues);
    public virtual MlResult<T> TryRemove(MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoDeleterFp.TryRemove(notFoundErrorDetails, keyValues);
    public virtual async Task<MlResult<T>> TryRemoveAsync(T item, CancellationToken token = default) => await _repoDeleterFp.TryRemoveAsync(item, token);
    public virtual async Task<MlResult<T>> TryRemoveAsync(CancellationToken token = default, params object[] keyValues) => await _repoDeleterFp.TryRemoveAsync(token, keyValues);
    public virtual async Task<MlResult<T>> TryRemoveAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoDeleterFp.TryRemoveAsync(notFoundErrorDetails, token, keyValues);
    public virtual MlResult<IEnumerable<T>> TryRemoveRange(IEnumerable<T> items) => _repoDeleterFp.TryRemoveRange(items);
    public virtual async Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoDeleterFp.TryRemoveRangeAsync(items, token);
    public virtual MlResult<T> TryUpdate(T item) => _repoUpdaterFp.TryUpdate(item);
    public virtual MlResult<T> TryUpdate(T item, params object[] keyValues) => _repoUpdaterFp.TryUpdate(item, keyValues);
    public virtual MlResult<T> TryUpdate(T item, MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoUpdaterFp.TryUpdate(item, notFoundErrorDetails, keyValues);
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default) => await _repoUpdaterFp.TryUpdateAsync(item, token);
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] keyValues) => await _repoUpdaterFp.TryUpdateAsync(item, token, keyValues);
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoUpdaterFp.TryUpdateAsync(item, notFoundErrorDetails, token, keyValues);
    public virtual MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items) => _repoUpdaterFp.TryUpdateRange(items);
    public virtual async Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoUpdaterFp.TryUpdateRangeAsync(items, token);



}

