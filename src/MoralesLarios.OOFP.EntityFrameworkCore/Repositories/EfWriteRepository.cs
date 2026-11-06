// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Delegates entity-write operations to focused repository collaborators.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TContext">The EF Core context type.</typeparam>
public class EfWriteRepository<TEntity, TContext> : EfRepository, IEfWriteRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    internal readonly IEfCreateRepository<TEntity> _repoAdderFp;
    internal readonly IEfDeleteRepository<TEntity> _repoDeleterFp;
    internal readonly IEfUpdateRepository<TEntity> _repoUpdaterFp;

    /// <summary>Initializes a write repository for the specified context.</summary>
    /// <param name="dbContext">The context shared by the repository collaborators.</param>
    public EfWriteRepository(TContext dbContext) : base(dbContext)
    {
        _repoAdderFp   = CreateAdder(dbContext);
        _repoDeleterFp = CreateDeleter(dbContext);
        _repoUpdaterFp = CreateUpdater(dbContext);
    }

    /// <summary>
    /// Creates the entity-creation collaborator using the aggregate's context.
    /// </summary>
    /// <param name="context">The context shared with the aggregate.</param>
    /// <returns>The creation collaborator.</returns>
    protected virtual IEfCreateRepository<TEntity> CreateAdder(TContext context) => new EfCreateRepository<TEntity, TContext>(context);

    /// <summary>
    /// Creates the entity-removal collaborator using the aggregate's context.
    /// </summary>
    /// <param name="context">The context shared with the aggregate.</param>
    /// <returns>The removal collaborator.</returns>
    protected virtual IEfDeleteRepository<TEntity> CreateDeleter(TContext context) => new EfDeleteRepository<TEntity, TContext>(context);

    /// <summary>
    /// Creates the entity-update collaborator using the aggregate's context.
    /// </summary>
    /// <param name="context">The context shared with the aggregate.</param>
    /// <returns>The update collaborator.</returns>
    protected virtual IEfUpdateRepository<TEntity> CreateUpdater(TContext context) => new EfUpdateRepository<TEntity, TContext>(context);




    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryAdd(TEntity item) => _repoAdderFp.TryAdd(item);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryAddAsync(TEntity item, CancellationToken token = default) => await _repoAdderFp.TryAddAsync(item, token);
    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryAddRange(IEnumerable<TEntity> items) => _repoAdderFp.TryAddRange(items);
    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryAddRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default) => await _repoAdderFp.TryAddRangeAsync(items, token);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryRemove(TEntity item) => _repoDeleterFp.TryRemove(item);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryRemove(params object[] keyValues) => _repoDeleterFp.TryRemove(keyValues);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryRemove(MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoDeleterFp.TryRemove(notFoundErrorDetails, keyValues);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryRemoveAsync(TEntity item, CancellationToken token = default) => await _repoDeleterFp.TryRemoveAsync(item, token);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryRemoveAsync(CancellationToken token = default, params object[] keyValues) => await _repoDeleterFp.TryRemoveAsync(token, keyValues);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryRemoveAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoDeleterFp.TryRemoveAsync(notFoundErrorDetails, token, keyValues);
    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryRemoveRange(IEnumerable<TEntity> items) => _repoDeleterFp.TryRemoveRange(items);
    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryRemoveRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default) => await _repoDeleterFp.TryRemoveRangeAsync(items, token);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryUpdate(TEntity item) => _repoUpdaterFp.TryUpdate(item);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryUpdate(TEntity item, params object[] keyValues) => _repoUpdaterFp.TryUpdate(item, keyValues);
    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryUpdate(TEntity item, MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoUpdaterFp.TryUpdate(item, notFoundErrorDetails, keyValues);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, CancellationToken token = default) => await _repoUpdaterFp.TryUpdateAsync(item, token);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, CancellationToken token = default, params object[] keyValues) => await _repoUpdaterFp.TryUpdateAsync(item, token, keyValues);
    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoUpdaterFp.TryUpdateAsync(item, notFoundErrorDetails, token, keyValues);
    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryUpdateRange(IEnumerable<TEntity> items) => _repoUpdaterFp.TryUpdateRange(items);
    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryUpdateRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default) => await _repoUpdaterFp.TryUpdateRangeAsync(items, token);
}

