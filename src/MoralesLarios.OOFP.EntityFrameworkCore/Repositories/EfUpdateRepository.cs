// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Implements entity-update operations for an EF Core context.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TContext">The EF Core context type.</typeparam>
public class EfUpdateRepository<TEntity, TContext>(TContext dbContext) : EfRepository(dbContext), IEfUpdateRepository<TEntity>
        where TEntity        : class
        where TContext : DbContext
{

    private static readonly string typeName = typeof(TEntity).Name;




    /// <summary>
    /// Marks an entity for update in the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <returns>A successful result containing the entity, or a failed result.</returns>
    public virtual MlResult<TEntity> TryUpdate(TEntity item)
    {
        var result = EnsureFp.NotNull(item, "The entity item to update cannot be null")
                            .TryMap(x =>
                            {
                                GetContext().Update(item);
                                return item;
                            });
        return result;
    }


    /// <summary>
    /// Finds an entity by its primary-key values and marks it for update.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A successful result containing the entity, or a failed result.</returns>
    public virtual MlResult<TEntity> TryUpdate(TEntity item, params object[] keyValues) => TryUpdate(item, null!, keyValues);

    /// <summary>
    /// Finds an entity by its primary-key values and marks it for update, using the supplied not-found error details.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <param name="notFoundErrorDetails">The error details returned when the entity is not found.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A successful result containing the entity, or a failed result.</returns>
    public virtual MlResult<TEntity> TryUpdate(TEntity item, MlErrorDetails notFoundErrorDetails, params object[] keyValues)
    {
        var errorDetails = notFoundErrorDetails ?? (keyValues is null
            ? "The object array pk cannot be null"
            : $"{keyValues.FormatCompositeKey()} values not found data in {typeName}");

        var result = EnsureFp.NotNull  (item, "The entity item to update cannot be null")
                                .Bind  (_  => EnsureFp.NotNull(keyValues, "The object array pk cannot be null"))
                                .Bind  (validKeys => EnsureFp.NotEmptyCollection<object[], object>(validKeys!, "The object array pk cannot be empty"))
                                .TryMap ( validKeys =>
                                {
                                    var found = GetContext().Set<TEntity>().Find(validKeys);
                                    return found!;
                                })
                                .TryBind(x  => x.FailIfNull(errorDetails))
                                .TryMap( x  =>
                                {
                                    // Find ya trackea 'x' con la misma clave que 'item'. Si son instancias
                                    // distintas, EF Core no permite trackear 'item' sin antes desvincular 'x'.
                                    if (!ReferenceEquals(x, item))
                                    {
                                        GetContext().Entry(x).State = EntityState.Detached;
                                    }

                                    GetContext().Update(item);
                                    return item;
                                });
        return result;
    }


    /// <summary>
    /// Asynchronously marks an entity for update in the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the entity or a failed result.</returns>
    public virtual async Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(item, "The entity item to update cannot be null")
                            .TryMapAsync(async x =>
                            {
                                GetContext().Update(item);
                                return item;
                            });
        return result;
    }


    /// <summary>
    /// Asynchronously finds an entity by its primary-key values and marks it for update.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A task containing the entity or a failed result.</returns>
    public virtual Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, CancellationToken token = default, params object[] keyValues)
        => TryUpdateAsync(item, null!, token, keyValues);

    /// <summary>
    /// Asynchronously finds an entity by its primary-key values and marks it for update, using the supplied not-found error details.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <param name="notFoundErrorDetails">The error details returned when the entity is not found.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A task containing the entity or a failed result.</returns>
    public virtual async Task<MlResult<TEntity>> TryUpdateAsync(TEntity item, MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues)
    {
        var errorDetails = notFoundErrorDetails ?? (keyValues is null
            ? "The object array pk cannot be null"
            : $"{keyValues.FormatCompositeKey()} values not found data in {typeName}");

        var result = await EnsureFp.NotNullAsync( item, "The entity item to update cannot be null")
                                    .BindAsync  ( _  => EnsureFp.NotNullAsync(keyValues, "The object array pk cannot be null"))
                                    .BindAsync  (validKeys => EnsureFp.NotEmptyCollection<object[], object>(validKeys!, "The object array pk cannot be empty"))
                                    .TryMapAsync( async validKeys =>
                                    {
                                        var found = await GetContext().Set<TEntity>().FindAsync(validKeys, token);
                                        return found!;
                                    })
                                    .TryBindAsync( x => x.FailIfNullAsync(errorDetails))
                                    .TryMapAsync( async x =>
                                    {
                                        // FindAsync ya trackea 'x' con la misma clave que 'item'. Si son
                                        // instancias distintas, EF Core no permite trackear 'item' sin antes
                                        // desvincular 'x'.
                                        if (!ReferenceEquals(x, item))
                                        {
                                            GetContext().Entry(x).State = EntityState.Detached;
                                        }

                                        GetContext().Update(item);
                                        return item;
                                    });
        return result;
    }

    /// <summary>
    /// Marks a collection of entities for update. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to update.</param>
    /// <returns>A successful result containing the entities, or a failed result.</returns>
    public virtual MlResult<IEnumerable<TEntity>> TryUpdateRange(IEnumerable<TEntity> items)
    {
        var result = EnsureFp.NotNull(items, "The entity items to update cannot be null")
                            .TryMap(x =>
                            {
                                GetContext().UpdateRange(items);
                                return items;
                            });
        return result;
    }

    /// <summary>
    /// Asynchronously marks a collection of entities for update. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to update.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the entities or a failed result.</returns>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryUpdateRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(items, "The entity items to update cannot be null")
                            .TryMapAsync(async x =>
                            {
                                GetContext().UpdateRange(items);
                                return items;
                            });
        return result;
    }



}


