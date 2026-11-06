// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Defines entity-update operations for an EF Core repository.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IEfUpdateRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Marks an entity for update in the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <returns>A successful result containing the entity, or a failed result.</returns>
    MlResult<TEntity> TryUpdate(TEntity item);
    /// <summary>
    /// Finds an entity by its primary-key values and marks it for update.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A successful result containing the entity, or a failed result.</returns>
    MlResult<TEntity> TryUpdate(TEntity item, params object[] keyValues);
    /// <summary>
    /// Finds an entity by its primary-key values and marks it for update, using the supplied not-found error details.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <param name="notFoundErrorDetails">The error details returned when the entity is not found.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A successful result containing the entity, or a failed result.</returns>
    MlResult<TEntity> TryUpdate(TEntity item, MlErrorDetails notFoundErrorDetails, params object[] keyValues);
    /// <summary>
    /// Asynchronously marks an entity for update in the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the entity or a failed result.</returns>
    Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, CancellationToken token = default);
    /// <summary>
    /// Asynchronously finds an entity by its primary-key values and marks it for update.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A task containing the entity or a failed result.</returns>
    Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, CancellationToken token = default, params object[] keyValues);
    /// <summary>
    /// Asynchronously finds an entity by its primary-key values and marks it for update, using the supplied not-found error details.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <param name="notFoundErrorDetails">The error details returned when the entity is not found.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A task containing the entity or a failed result.</returns>
    Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues);
    /// <summary>
    /// Marks a collection of entities for update. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to update.</param>
    /// <returns>A successful result containing the entities, or a failed result.</returns>
    MlResult<IEnumerable<TEntity>> TryUpdateRange(IEnumerable<TEntity> items);
    /// <summary>
    /// Asynchronously marks a collection of entities for update. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to update.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the entities or a failed result.</returns>
    Task<MlResult<IEnumerable<TEntity>>> TryUpdateRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default);
}
