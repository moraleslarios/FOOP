// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Defines entity-removal operations for an EF Core repository.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IEfDeleteRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Removes an entity from the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to remove.</param>
    /// <returns>A successful result containing the removed entity, or a failed result.</returns>
    MlResult<TEntity> TryRemove(TEntity item);
    /// <summary>
    /// Finds and removes an entity by its primary-key values.
    /// </summary>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>The removed entity or a failed result.</returns>
    MlResult<TEntity> TryRemove(params object[] keyValues);
    /// <summary>
    /// Finds and removes an entity by its primary-key values using the supplied not-found error details.
    /// </summary>
    /// <param name="notFoundErrorDetails">The error details returned when no entity is found.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>The removed entity or a failed result.</returns>
    MlResult<TEntity> TryRemove(MlErrorDetails notFoundErrorDetails, params object[] keyValues);
    /// <summary>
    /// Asynchronously removes an entity from the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to remove.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the removed entity or a failed result.</returns>
    Task<MlResult<TEntity>> TryRemoveAsync(TEntity item, CancellationToken token = default);
    /// <summary>
    /// Asynchronously finds and removes an entity by its primary-key values.
    /// </summary>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A task containing the removed entity or a failed result.</returns>
    Task<MlResult<TEntity>> TryRemoveAsync(CancellationToken token = default, params object[] keyValues);
    /// <summary>
    /// Asynchronously finds and removes an entity by its primary-key values using the supplied not-found error details.
    /// </summary>
    /// <param name="notFoundErrorDetails">The error details returned when no entity is found.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A task containing the removed entity or a failed result.</returns>
    Task<MlResult<TEntity>> TryRemoveAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues);
    /// <summary>
    /// Removes a collection of entities from the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to remove.</param>
    /// <returns>A successful result containing the removed entities, or a failed result.</returns>
    MlResult<IEnumerable<TEntity>> TryRemoveRange(IEnumerable<TEntity> items);
    /// <summary>
    /// Asynchronously removes a collection of entities from the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to remove.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the removed entities or a failed result.</returns>
    Task<MlResult<IEnumerable<TEntity>>> TryRemoveRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default);
}
