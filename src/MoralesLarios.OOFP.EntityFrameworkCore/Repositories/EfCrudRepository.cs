// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Combines entity reads and writes in a single repository.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TContext">The EF Core context type.</typeparam>
public class EfCrudRepository<TEntity, TContext> : EfRepository, IEfRepository<TEntity>
    where TEntity        : class
    where TContext : DbContext
{
    internal readonly IEfReadRepository<TEntity> _repoReaderFp;
    internal readonly IEfWriteRepository<TEntity> _repoWriterFp;
    private readonly IMlUnitOfWork<TContext> _unitOfWork;

    /// <summary>Gets the unit of work associated with this repository's context.</summary>
    public IMlUnitOfWork UnitOfWork => _unitOfWork;

    /// <summary>Initializes a CRUD repository for the specified context.</summary>
    /// <param name="dbContext">The context used by this repository.</param>
    public EfCrudRepository(TContext dbContext) : this(dbContext, new MlUnitOfWork<TContext>(dbContext))
    {
    }

    internal EfCrudRepository(TContext dbContext, IMlUnitOfWork<TContext> unitOfWork) : base(dbContext)
    {
        _unitOfWork = unitOfWork;
        _repoReaderFp = CreateReader(dbContext);
        _repoWriterFp = CreateWriter(dbContext);
    }

    /// <summary>
    /// Creates the read collaborator using the aggregate's context.
    /// </summary>
    /// <param name="context">The context shared with the aggregate.</param>
    /// <returns>The read collaborator.</returns>
    protected virtual IEfReadRepository<TEntity> CreateReader(TContext context) => new EfReadRepository<TEntity, TContext>(context);

    /// <summary>
    /// Creates the write collaborator using the aggregate's context.
    /// </summary>
    /// <param name="context">The context shared with the aggregate.</param>
    /// <returns>The write collaborator.</returns>
    protected virtual IEfWriteRepository<TEntity> CreateWriter(TContext context) => new EfWriteRepository<TEntity, TContext>(context);



    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFind(params object[] keyValues) => _repoReaderFp.TryFind(keyValues);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryFindAsync(CancellationToken token = default, params object[] keyValues) => await _repoReaderFp.TryFindAsync(token, keyValues);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoReaderFp.TryFind(notFoundErrorDetails, keyValues);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryFindAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoReaderFp.TryFindAsync(notFoundErrorDetails, token, keyValues);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFirst(Expression<Func<TEntity, bool>> filter) => _repoReaderFp.TryFirst(filter);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryFirstAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryFirstAsync(filter, token);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFirst(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails) => _repoReaderFp.TryFirst(filter, notFoundErrorDetails);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryFirstAsync(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderFp.TryFirstAsync(filter, notFoundErrorDetails, token);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryGetLast(Expression<Func<TEntity, bool>> filter) => _repoReaderFp.TryGetLast(filter);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryGetLastAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryGetLastAsync(filter, token);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryGetLast(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails) => _repoReaderFp.TryGetLast(filter, notFoundErrorDetails);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryGetLastAsync(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderFp.TryGetLastAsync(filter, notFoundErrorDetails, token);

    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryGetData(Expression<Func<TEntity, bool>> filter) => _repoReaderFp.TryGetData(filter);

    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryGetDataAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default!) => await _repoReaderFp.TryGetDataAsync(filter, token);

    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryAll() => _repoReaderFp.TryAll();

    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryAllAsync(CancellationToken token = default!) => await _repoReaderFp.TryAllAsync(token);



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

}
