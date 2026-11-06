// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0


namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Implements entity-creation operations for an EF Core context.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TContext">The EF Core context type.</typeparam>
public class EfCreateRepository<TEntity, TContext>(TContext dbContext) : EfRepository(dbContext), IEfCreateRepository<TEntity>
    where TEntity        : class
    where TContext : DbContext
{

    /// <summary>
    /// Adds an entity to the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to add.</param>
    /// <returns>A successful result containing the added entity, or a failed result.</returns>
    public virtual MlResult<TEntity> TryAdd(TEntity item)
    {
        var result = EnsureFp.NotNull(item, "The entity item to add cannot be null")
                            .TryMap(x =>
                            {
                                GetContext().Set<TEntity>().Add(item);

                                return item;
                            });

        return result;
    }

    /// <summary>
    /// Asynchronously adds an entity to the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to add.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the added entity or a failed result.</returns>
    public virtual async Task<MlResult<TEntity>> TryAddAsync(TEntity item, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(item, "The entity item to add cannot be null")
                            .TryMapAsync(async x =>
                            {
                                await GetContext().Set<TEntity>().AddAsync(item, token);

                                return item;
                            });

        return result;
    }


    /// <summary>
    /// Adds a collection of entities to the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to add.</param>
    /// <returns>A successful result containing the added entities, or a failed result.</returns>
    public virtual MlResult<IEnumerable<TEntity>> TryAddRange(IEnumerable<TEntity> items)
    {
        var result = EnsureFp.NotNull(items, "The entity items to addRange cannot be null")
                            .TryMap(x =>
                            {
                                GetContext().Set<TEntity>().AddRange(items);

                                return items;
                            });

        return result;
    }

    /// <summary>
    /// Asynchronously adds a collection of entities to the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to add.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the added entities or a failed result.</returns>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryAddRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(items, "The entity items to addRange cannot be null")
                            .TryMapAsync(async x =>
                            {
                                await GetContext().Set<TEntity>().AddRangeAsync(items, token);

                                return items;
                            });

        return result;
    }



}


