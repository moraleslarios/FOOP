// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.EFCore.OopRepos;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EFCore.Repos;
public class EFRepoPaginationFp<T, TContext>(TContext dbContext) : EFRepoBaseFp(dbContext), IEFRepoPaginationFp<T>
    where T : class
    where TContext : DbContext
{
    private readonly IEFRepoReaderPaginationFp<T> _repoReaderPaginatorFp = RegisterServices.ResolveRepoFp<IEFRepoReaderPaginationFp<T>>();
    private readonly IEFRepoWriterFp<T>           _repoWriterFp          = RegisterServices.ResolveRepoFp<IEFRepoWriterFp          <T>>();



    public virtual MlResult<T> TryFind(params object[] pk) => _repoReaderPaginatorFp.TryFind(pk);

    public virtual async Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] pk) => await _repoReaderPaginatorFp.TryFindAsync(token, pk);

    public virtual MlResult<T> TryFind(MlErrorsDetails notFoundErrorDetails, params object[] pk) => _repoReaderPaginatorFp.TryFind(notFoundErrorDetails, pk);

    public virtual async Task<MlResult<T>> TryFindAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk) => await _repoReaderPaginatorFp.TryFindAsync(notFoundErrorDetails, token, pk);

    public virtual MlResult<T> TryFirstOrDefault(Expression<Func<T, bool>> filter) => _repoReaderPaginatorFp.TryFirstOrDefault(filter);

    public virtual async Task<MlResult<T>> TryFirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryFirstOrDefaultAsync(filter, token);

    public virtual MlResult<T> TryFirstOrDefault(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails) => _repoReaderPaginatorFp.TryFirstOrDefault(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryFirstOrDefaultAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderPaginatorFp.TryFirstOrDefaultAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter) => _repoReaderPaginatorFp.TryFirst(filter);

    public virtual async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryFirstAsync(filter, token);

    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails) => _repoReaderPaginatorFp.TryFirst(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderPaginatorFp.TryFirstAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<T> TryLastOrDefault(Expression<Func<T, bool>> filter) => _repoReaderPaginatorFp.TryLastOrDefault(filter);

    public virtual async Task<MlResult<T>> TryLastOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryLastOrDefaultAsync(filter, token);

    public virtual MlResult<T> TryLastOrDefault(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails) => _repoReaderPaginatorFp.TryLastOrDefault(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryLastOrDefaultAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderPaginatorFp.TryLastOrDefaultAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<T> TryLast(Expression<Func<T, bool>> filter) => _repoReaderPaginatorFp.TryLast(filter);

    public virtual async Task<MlResult<T>> TryLastAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryLastAsync(filter, token);

    public virtual MlResult<T> TryLast(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails) => _repoReaderPaginatorFp.TryLast(filter, notFoundErrorDetails);

    public virtual async Task<MlResult<T>> TryLastAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderPaginatorFp.TryLastAsync(filter, notFoundErrorDetails, token);

    public virtual MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter) => _repoReaderPaginatorFp.TryGetData(filter);

    public virtual async Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryGetDataAsync(filter, token);

    public virtual MlResult<IEnumerable<T>> TryAll() => _repoReaderPaginatorFp.TryAll();

    public virtual async Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default!) => await _repoReaderPaginatorFp.TryAllAsync(token);



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








    public virtual MlResult<PaginationResultInfo<T>> TryAllPagination(PaginationInfo              paginationInfo, 
                                                                      OrderBy                     orderBy      = OrderBy.Ascending, 
                                                                      Expression<Func<T, object>> orderByField = null!)
         => _repoReaderPaginatorFp.TryAllPagination(paginationInfo, orderBy, orderByField);

    public virtual Task<MlResult<PaginationResultInfo<T>>> TryAllPaginationAsync(PaginationInfo              paginationInfo, 
                                                                                 OrderBy                     orderBy      = OrderBy.Ascending, 
                                                                                 Expression<Func<T, object>> orderByField = null!,
                                                                                 CancellationToken ct                     = default!)
         => _repoReaderPaginatorFp.TryAllPaginationAsync(paginationInfo, orderBy, orderByField, ct);

    public virtual MlResult<PaginationResultInfo<T>> TryGetDataPagination(PaginationInfo              paginationInfo, 
                                                                          OrderBy                     orderBy      = OrderBy.Ascending, 
                                                                          Expression<Func<T, object>> orderByField = null, 
                                                                          Expression<Func<T, bool>>   filter       = null!)
         => _repoReaderPaginatorFp.TryGetDataPagination(paginationInfo, orderBy, orderByField, filter);

    public virtual Task<MlResult<PaginationResultInfo<T>>> TryGetDataPaginationAsync(PaginationInfo paginationInfo, 
                                                                                     OrderBy                     orderBy      = OrderBy.Ascending, 
                                                                                     Expression<Func<T, object>> orderByField = null!, 
                                                                                     Expression<Func<T, bool>>   filter       = null!,
                                                                                     CancellationToken ct                     = default!)
         => _repoReaderPaginatorFp.TryGetDataPaginationAsync(paginationInfo, orderBy, orderByField, filter, ct);
}

