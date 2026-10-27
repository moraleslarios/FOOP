// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
public class EfPagedRepository<T, TContext>(TContext dbContext) : EfRepository(dbContext), IEfPagedRepository<T>
    where T : class
    where TContext : DbContext
{
    private readonly IEfPagedReadRepository<T> _repoReaderPaginatorFp = RegisterServices.GetRepository<IEfPagedReadRepository<T>>();
    private readonly IEfWriteRepository<T>           _repoWriterFp          = RegisterServices.GetRepository<IEfWriteRepository          <T>>();



    public virtual MlResult<T> TryFind(params object[] keyValues) => _repoReaderPaginatorFp.TryFind(keyValues);

    public virtual async Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] keyValues) => await _repoReaderPaginatorFp.TryFindAsync(token, keyValues);

    public virtual MlResult<T> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoReaderPaginatorFp.TryFind(notFoundErrorDetails, keyValues);

    public virtual async Task<MlResult<T>> TryFindAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoReaderPaginatorFp.TryFindAsync(notFoundErrorDetails, token, keyValues);

    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter) => _repoReaderPaginatorFp.TryFirst(filter);

    public virtual async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryFirstAsync(filter, token);

    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails) => _repoReaderPaginatorFp.TryFirst(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderPaginatorFp.TryFirstAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<T> TryGetLast(Expression<Func<T, bool>> filter) => _repoReaderPaginatorFp.TryGetLast(filter);

    public virtual async Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryGetLastAsync(filter, token);

    public virtual MlResult<T> TryGetLast(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails) => _repoReaderPaginatorFp.TryGetLast(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderPaginatorFp.TryGetLastAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter) => _repoReaderPaginatorFp.TryGetData(filter);

    public virtual async Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryGetDataAsync(filter, token);

    public virtual MlResult<IEnumerable<T>> TryAll() => _repoReaderPaginatorFp.TryAll();

    public virtual async Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default!) => await _repoReaderPaginatorFp.TryAllAsync(token);



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








    public virtual MlResult<PagedResult<T>> TryAllPagination(PageRequest              paginationInfo, 
                                                                      SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                      Expression<Func<T, object>> orderByField = null!)
         => _repoReaderPaginatorFp.TryAllPagination(paginationInfo, orderBy, orderByField);

    public virtual Task<MlResult<PagedResult<T>>> TryAllPaginationAsync(PageRequest              paginationInfo, 
                                                                                 SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                                 Expression<Func<T, object>> orderByField = null!,
                                                                                 CancellationToken ct                     = default!)
         => _repoReaderPaginatorFp.TryAllPaginationAsync(paginationInfo, orderBy, orderByField, ct);

    public virtual MlResult<PagedResult<T>> TryGetDataPagination(PageRequest              paginationInfo, 
                                                                          SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                          Expression<Func<T, object>> orderByField = null, 
                                                                          Expression<Func<T, bool>>   filter       = null!)
         => _repoReaderPaginatorFp.TryGetDataPagination(paginationInfo, orderBy, orderByField, filter);

    public virtual Task<MlResult<PagedResult<T>>> TryGetDataPaginationAsync(PageRequest paginationInfo, 
                                                                                     SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                                     Expression<Func<T, object>> orderByField = null!, 
                                                                                     Expression<Func<T, bool>>   filter       = null!,
                                                                                     CancellationToken ct                     = default!)
         => _repoReaderPaginatorFp.TryGetDataPaginationAsync(paginationInfo, orderBy, orderByField, filter, ct);
}

