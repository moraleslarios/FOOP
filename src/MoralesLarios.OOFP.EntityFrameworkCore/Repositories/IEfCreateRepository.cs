// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Defines entity-creation operations for an EF Core repository.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IEfCreateRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Adds an entity to the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to add.</param>
    /// <returns>A successful result containing the added entity, or a failed result.</returns>
    MlResult<TEntity> TryAdd(TEntity item);
    /// <summary>
    /// Asynchronously adds an entity to the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to add.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the added entity or a failed result.</returns>
    Task<MlResult<TEntity>> TryAddAsync(TEntity item, CancellationToken token = default);
    /// <summary>
    /// Adds a collection of entities to the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to add.</param>
    /// <returns>A successful result containing the added entities, or a failed result.</returns>
    MlResult<IEnumerable<TEntity>> TryAddRange(IEnumerable<TEntity> items);
    /// <summary>
    /// Asynchronously adds a collection of entities to the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to add.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the added entities or a failed result.</returns>
    Task<MlResult<IEnumerable<TEntity>>> TryAddRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default);
}
