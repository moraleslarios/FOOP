// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0


namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Implements entity-removal operations for an EF Core context.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TContext">The EF Core context type.</typeparam>
public class EfDeleteRepository<TEntity, TContext>(TContext dbContext) : EfRepository(dbContext), IEfDeleteRepository<TEntity>
    where TEntity        : class
    where TContext : DbContext
{

    private static readonly string typeName = typeof(TEntity).Name;


    /// <summary>
    /// Removes an entity from the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to remove.</param>
    /// <returns>A successful result containing the removed entity, or a failed result.</returns>
    public virtual MlResult<TEntity> TryRemove(TEntity item)
    {
        var result = EnsureFp.NotNull(item, "The entity item to remove cannot be null")
                            .TryMap(x =>
                            {
                                GetContext().Set<TEntity>().Remove(item);
                                return item;
                            });
        return result;
    }

    /// <summary>
    /// Finds and removes an entity by its primary-key values.
    /// </summary>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>The removed entity or a failed result.</returns>
    public virtual MlResult<TEntity> TryRemove(params object[] keyValues)
        => TryRemove(null!, keyValues);

    /// <summary>
    /// Finds and removes an entity by its primary-key values using the supplied not-found error details.
    /// </summary>
    /// <param name="notFoundErrorDetails">The error details returned when no entity is found.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>The removed entity or a failed result.</returns>
    public virtual MlResult<TEntity> TryRemove(MlErrorDetails notFoundErrorDetails, params object[] keyValues)
    {
        var errorDetails = notFoundErrorDetails ?? (keyValues is null
            ? "The object array pk cannot be null"
            : $"{keyValues.FormatCompositeKey()} values not found data in {typeName}");

        var result = EnsureFp.NotNull(keyValues, "The object array pk cannot be null")
                                .Bind(validKeys => EnsureFp.NotEmptyCollection<object[], object>(validKeys!, "The object array pk cannot be empty"))
                                .TryMap (validKeys => GetContext().Set<TEntity>().Find(validKeys)!)
                                .TryBind(bdItem  => bdItem.FailIfNull(errorDetails))
                                .TryMap( bdItem  =>
                                {
                                    GetContext().Set<TEntity>().Remove(bdItem);
                                    return bdItem;
                                });
        return result;
    }


    /// <summary>
    /// Asynchronously removes an entity from the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="item">The entity to remove.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the removed entity or a failed result.</returns>
    public virtual async Task<MlResult<TEntity>> TryRemoveAsync(TEntity item, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(item, "The entity item to remove cannot be null")
                            .TryMapAsync(async x =>
                            {
                                GetContext().Set<TEntity>().Remove(item);
                                return item;
                            });
        return result;
    }


    /// <summary>
    /// Asynchronously finds and removes an entity by its primary-key values.
    /// </summary>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A task containing the removed entity or a failed result.</returns>
    public virtual Task<MlResult<TEntity>> TryRemoveAsync(CancellationToken token = default, params object[] keyValues)
        => TryRemoveAsync(null!, token, keyValues);

    /// <summary>
    /// Asynchronously finds and removes an entity by its primary-key values using the supplied not-found error details.
    /// </summary>
    /// <param name="notFoundErrorDetails">The error details returned when no entity is found.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A task containing the removed entity or a failed result.</returns>
    public virtual async Task<MlResult<TEntity>> TryRemoveAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues)
    {
        var errorDetails = notFoundErrorDetails ?? (keyValues is null
            ? "The object array pk cannot be null"
            : $"{keyValues.FormatCompositeKey()} values not found data in {typeName}");

        var result = await EnsureFp.NotNullAsync(keyValues, "The object array pk cannot be null")
                                    .BindAsync(validKeys => EnsureFp.NotEmptyCollection<object[], object>(validKeys!, "The object array pk cannot be empty"))
                                    .TryMapAsync ( async validKeys =>
                                    {
                                        var found = await GetContext().Set<TEntity>().FindAsync(validKeys, token);
                                        return found!;
                                    })
                                    .TryBindAsync( bdItem        => bdItem.FailIfNullAsync(errorDetails))
                                    .TryMapAsync( async bdItem  =>
                                    {
                                        GetContext().Set<TEntity>().Remove(bdItem);
                                        return bdItem;
                                    });
        return result;
    }

    /// <summary>
    /// Removes a collection of entities from the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to remove.</param>
    /// <returns>A successful result containing the removed entities, or a failed result.</returns>
    public virtual MlResult<IEnumerable<TEntity>> TryRemoveRange(IEnumerable<TEntity> items)
    {
        var result = EnsureFp.NotNull(items, "The entity items to remove cannot be null")
                            .TryMap(x =>
                            {
                                GetContext().Set<TEntity>().RemoveRange(items);
                                return items;
                            });
        return result;
    }

    // Punto 35: el resto de metodos *Async del repositorio arrancan la cadena con la guarda
    // asincrona (EnsureFp.NotNullAsync) y esperan el resultado. Aqui se usaba la guarda sincrona
    // y se devolvia la Task sin await, con lo que el metodo no era homogeneo con sus hermanos
    // (ni respecto a la propagacion de excepciones ni al punto de captura de la pila).
    /// <summary>
    /// Asynchronously removes a collection of entities from the context. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="items">The entities to remove.</param>
    /// <param name="token">A token that can cancel the operation.</param>
    /// <returns>A task containing the removed entities or a failed result.</returns>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryRemoveRangeAsync(IEnumerable<TEntity> items, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(items, "The entity items to remove cannot be null")
                            .TryMapAsync(async x =>
                            {
                                GetContext().Set<TEntity>().RemoveRange(items);
                                return items;
                            });
        return result;
    }

}
