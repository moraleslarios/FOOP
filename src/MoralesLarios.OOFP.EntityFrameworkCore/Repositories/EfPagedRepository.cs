// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
/// <summary>
/// Combines entity CRUD operations with paged reads.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TContext">The EF Core context type.</typeparam>
public class EfPagedRepository<TEntity, TContext> : EfRepository, IEfPagedRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    private readonly IEfPagedReadRepository<TEntity> _repoReaderPaginatorFp;
    private readonly IEfWriteRepository<TEntity> _repoWriterFp;
    private readonly IMlUnitOfWork<TContext> _unitOfWork;

    /// <summary>Gets the unit of work associated with this repository's context.</summary>
    public IMlUnitOfWork UnitOfWork => _unitOfWork;

    /// <summary>Initializes a paged CRUD repository for the specified context.</summary>
    /// <param name="dbContext">The context used by this repository.</param>
    public EfPagedRepository(TContext dbContext) : this(dbContext, new MlUnitOfWork<TContext>(dbContext))
    {
    }

    internal EfPagedRepository(TContext dbContext, IMlUnitOfWork<TContext> unitOfWork) : base(dbContext)
    {
        _unitOfWork = unitOfWork;
        _repoReaderPaginatorFp = CreateReaderPaginator(dbContext);
        _repoWriterFp          = CreateWriter(dbContext);
    }

    /// <summary>
    /// Creates the paged-read collaborator using the aggregate's context.
    /// </summary>
    /// <param name="context">The context shared with the aggregate.</param>
    /// <returns>The paged-read collaborator.</returns>
    protected virtual IEfPagedReadRepository<TEntity> CreateReaderPaginator(TContext context) => new EfPagedReadRepository<TEntity, TContext>(context);

    /// <summary>
    /// Creates the write collaborator using the aggregate's context.
    /// </summary>
    /// <param name="context">The context shared with the aggregate.</param>
    /// <returns>The write collaborator.</returns>
    protected virtual IEfWriteRepository<TEntity> CreateWriter(TContext context) => new EfWriteRepository<TEntity, TContext>(context);



    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFind(params object[] keyValues) => _repoReaderPaginatorFp.TryFind(keyValues);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryFindAsync(CancellationToken token = default, params object[] keyValues) => await _repoReaderPaginatorFp.TryFindAsync(token, keyValues);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoReaderPaginatorFp.TryFind(notFoundErrorDetails, keyValues);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryFindAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoReaderPaginatorFp.TryFindAsync(notFoundErrorDetails, token, keyValues);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFirst(Expression<Func<TEntity, bool>> filter) => _repoReaderPaginatorFp.TryFirst(filter);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryFirstAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryFirstAsync(filter, token);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFirst(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails) => _repoReaderPaginatorFp.TryFirst(filter, notFoundErrorDetails);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryFirstAsync(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderPaginatorFp.TryFirstAsync(filter, notFoundErrorDetails, token);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryGetLast(Expression<Func<TEntity, bool>> filter) => _repoReaderPaginatorFp.TryGetLast(filter);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryGetLastAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryGetLastAsync(filter, token);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryGetLast(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails) => _repoReaderPaginatorFp.TryGetLast(filter, notFoundErrorDetails);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryGetLastAsync(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderPaginatorFp.TryGetLastAsync(filter, notFoundErrorDetails, token);

    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryGetData(Expression<Func<TEntity, bool>> filter) => _repoReaderPaginatorFp.TryGetData(filter);

    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryGetDataAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryGetDataAsync(filter, token);

    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryAll() => _repoReaderPaginatorFp.TryAll();

    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryAllAsync(CancellationToken token = default!) => await _repoReaderPaginatorFp.TryAllAsync(token);



    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryAdd(TEntity item) => _repoWriterFp.TryAdd(item);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryAddAsync(TEntity item, CancellationToken token = default) => await _repoWriterFp.TryAddAsync(item, token);
    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryAddRange(IEnumerable<TEntity> items) => _repoWriterFp.TryAddRange(items);
    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryAddRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default) => await _repoWriterFp.TryAddRangeAsync(items, token);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryRemove(TEntity item) => _repoWriterFp.TryRemove(item);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryRemove(params object[] keyValues) => _repoWriterFp.TryRemove(keyValues);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryRemove(MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoWriterFp.TryRemove(notFoundErrorDetails, keyValues);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryRemoveAsync(TEntity item, CancellationToken token = default) => await _repoWriterFp.TryRemoveAsync(item, token);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryRemoveAsync(CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryRemoveAsync(token, keyValues);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryRemoveAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryRemoveAsync(notFoundErrorDetails, token, keyValues);
    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryRemoveRange(IEnumerable<TEntity> items) => _repoWriterFp.TryRemoveRange(items);
    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryRemoveRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default) => await _repoWriterFp.TryRemoveRangeAsync(items, token);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryUpdate(TEntity item) => _repoWriterFp.TryUpdate(item);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryUpdate(TEntity item, params object[] keyValues) => _repoWriterFp.TryUpdate(item, keyValues);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryUpdate(TEntity item, MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoWriterFp.TryUpdate(item, notFoundErrorDetails, keyValues);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, CancellationToken token = default) => await _repoWriterFp.TryUpdateAsync(item, token);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryUpdateAsync(item, token, keyValues);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryUpdateAsync(item, notFoundErrorDetails, token, keyValues);
    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryUpdateRange(IEnumerable<TEntity> items) => _repoWriterFp.TryUpdateRange(items);
    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryUpdateRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default) => await _repoWriterFp.TryUpdateRangeAsync(items, token);








    /// <inheritdoc/>
    public virtual MlResult<PagedResult<TEntity>> TryAllPagination(PageRequest              paginationInfo,
                                                                      SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                      Expression<Func<TEntity, object>>? orderByField = null)
         => _repoReaderPaginatorFp.TryAllPagination(paginationInfo, orderBy, orderByField);

    /// <inheritdoc/>
    public virtual Task<MlResult<PagedResult<TEntity>>> TryAllPaginationAsync(PageRequest              paginationInfo,
                                                                                 SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                                 Expression<Func<TEntity, object>>? orderByField = null,
                                                                                 CancellationToken ct                     = default!)
         => _repoReaderPaginatorFp.TryAllPaginationAsync(paginationInfo, orderBy, orderByField, ct);

    /// <inheritdoc/>
    public virtual MlResult<PagedResult<TEntity>> TryGetDataPagination(PageRequest              paginationInfo,
                                                                          SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                          Expression<Func<TEntity, object>>? orderByField = null,
                                                                          Expression<Func<TEntity, bool>>?   filter       = null)
         => _repoReaderPaginatorFp.TryGetDataPagination(paginationInfo, orderBy, orderByField, filter);

    /// <inheritdoc/>
    public virtual Task<MlResult<PagedResult<TEntity>>> TryGetDataPaginationAsync(PageRequest paginationInfo,
                                                                                     SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                                     Expression<Func<TEntity, object>>? orderByField = null,
                                                                                     Expression<Func<TEntity, bool>>?   filter       = null,
                                                                                     CancellationToken ct                     = default!)
         => _repoReaderPaginatorFp.TryGetDataPaginationAsync(paginationInfo, orderBy, orderByField, filter, ct);
}

