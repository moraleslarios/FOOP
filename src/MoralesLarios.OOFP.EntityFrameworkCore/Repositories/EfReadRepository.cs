// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;


/// <summary>
/// Implements entity-read operations for an EF Core context.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TContext">The EF Core context type.</typeparam>
public class EfReadRepository<TEntity, TContext>(TContext dbContext) : EfRepository(dbContext), IEfOrderedReadRepository<TEntity>
    where TEntity        : class
    where TContext : DbContext
{

    private static readonly string typeName = typeof(TEntity).Name;


    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues)
    {
        var result = EnsureFp.NotNull(keyValues, "The object array pk cannot be null")
                                .TryMap (x => EnsureFp.NotEmpty(keyValues, "The object array pk cannot be empty"))
                                .TryMap (x => GetContext().Set<TEntity>().Find(keyValues)!)
                                .TryBind(x => x.FailIfNull(notFoundErrorDetails));
        return result;
    }

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFind(params object[] keyValues)
        => EnsureFp.NotNull(keyValues, "The object array pk cannot be null")
                   .Bind(validKeys => TryFind(
                       $"{validKeys.FormatCompositeKey()} values not found data in {typeName}", validKeys));


    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryFindAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues)
    {
        var result = await EnsureFp.NotNullAsync(keyValues, "The object array pk cannot be null")
                                    .TryMapAsync (x => EnsureFp.NotEmptyAsync(keyValues, "The object array pk cannot be empty"))
                                    .TryMapAsync (async x => (await GetContext().Set<TEntity>().FindAsync(keyValues, token))!)
                                    .TryBindAsync(x => x.FailIfNullAsync(notFoundErrorDetails));
        return result;
    }

    /// <inheritdoc/>
    public virtual Task<MlResult<TEntity>> TryFindAsync(CancellationToken token = default, params object[] keyValues)
        => EnsureFp.NotNullAsync(keyValues, "The object array pk cannot be null")
                   .BindAsync(validKeys => TryFindAsync(
                       $"{validKeys.FormatCompositeKey()} values not found data in {typeName}", token, validKeys));


    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFirst(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails)
    {
        var result = EnsureFp.NotNull(filter, "The filter cannot be null")
                                .TryMap (x => GetContext().Set<TEntity>().FirstOrDefault(filter)!)
                                .TryBind(x => x.FailIfNull(notFoundErrorDetails));
        return result;
    }


    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryFirst(Expression<Func<TEntity, bool>> filter)
        => TryFirst(filter, $"The query did not return any elements");

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryFirstAsync(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default!)
    {
        var result = await EnsureFp.NotNullAsync(filter, "The filter cannot be null")
                                    .TryMapAsync (async x => (await GetContext().Set<TEntity>().FirstOrDefaultAsync(filter, token))!)
                                    .TryBindAsync(x => x.FailIfNullAsync(notFoundErrorDetails));
        return result;
    }

    /// <inheritdoc/>
    public virtual Task<MlResult<TEntity>> TryFirstAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default!)
        => TryFirstAsync(filter, $"The query did not return any elements", token);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryGetLast(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails)
    {
        var result = EnsureFp.NotNull(filter, "The filter cannot be null")
                                .TryMap (x => GetContext().Set<TEntity>().AsNoTracking().Where(filter).AsEnumerable().LastOrDefault()!)
                                .TryBind(x => x.FailIfNull(notFoundErrorDetails));

        return result;
    }


    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryGetLast(Expression<Func<TEntity, bool>> filter)
        => TryGetLast(filter, $"The query did not return any elements");

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryGetLastAsync(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default!)
    {
        var result = await EnsureFp.NotNullAsync(filter, "The filter cannot be null")
                                    .TryMapAsync (async x => (await GetContext().Set<TEntity>().AsNoTracking().Where(filter).ToListAsync(token)).LastOrDefault()!)
                                    .TryBindAsync(x => x.FailIfNullAsync(notFoundErrorDetails));

        return result;
    }

    /// <inheritdoc/>
    public virtual Task<MlResult<TEntity>> TryGetLastAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default!)
        => TryGetLastAsync(filter, $"The query did not return any elements", token);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryGetLastBy(Expression<Func<TEntity, bool>> filter,
                                             Expression<Func<TEntity, object>> orderByField,
                                             SortDirection orderBy = SortDirection.Ascending)
        => TryGetLastBy(filter, orderByField, "The query did not return any elements", orderBy);

    /// <inheritdoc/>
    public virtual MlResult<TEntity> TryGetLastBy(Expression<Func<TEntity, bool>> filter,
                                             Expression<Func<TEntity, object>> orderByField,
                                             MlErrorDetails notFoundErrorDetails,
                                             SortDirection orderBy = SortDirection.Ascending)
    {
        var result = EnsureFp.NotNull(filter, "The filter cannot be null")
            .TryMap(validFilter =>
            {
                if (orderByField is null)
                    throw new ArgumentNullException(nameof(orderByField));

                return GetContext().Set<TEntity>()
                .AsNoTracking()
                .Where(validFilter)
                .OrderByStableForPagination(GetContext(), Reverse(orderBy), orderByField)
                .FirstOrDefault()!;
            })
            .TryBind(item => item.FailIfNull(notFoundErrorDetails));

        return result;
    }

    /// <inheritdoc/>
    public virtual Task<MlResult<TEntity>> TryGetLastByAsync(Expression<Func<TEntity, bool>> filter,
                                                        Expression<Func<TEntity, object>> orderByField,
                                                        SortDirection orderBy = SortDirection.Ascending,
                                                        CancellationToken token = default)
        => TryGetLastByAsync(filter, orderByField, "The query did not return any elements", orderBy, token);

    /// <inheritdoc/>
    public virtual async Task<MlResult<TEntity>> TryGetLastByAsync(Expression<Func<TEntity, bool>> filter,
                                                              Expression<Func<TEntity, object>> orderByField,
                                                              MlErrorDetails notFoundErrorDetails,
                                                              SortDirection orderBy = SortDirection.Ascending,
                                                              CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(filter, "The filter cannot be null")
            .TryMapAsync(async validFilter =>
            {
                if (orderByField is null)
                    throw new ArgumentNullException(nameof(orderByField));

                return (await GetContext().Set<TEntity>()
                .AsNoTracking()
                .Where(validFilter)
                .OrderByStableForPagination(GetContext(), Reverse(orderBy), orderByField)
                .FirstOrDefaultAsync(token))!;
            })
            .TryBindAsync(item => item.FailIfNullAsync(notFoundErrorDetails));

        return result;
    }

    private static SortDirection Reverse(SortDirection direction)
        => direction == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;


    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryGetData(Expression<Func<TEntity, bool>> filter)
    {
        var result = EnsureFp.NotNull(filter, "The filter cannot be null")
                                .TryMap(x => (IEnumerable<TEntity>)GetContext().Set<TEntity>().AsNoTracking().Where(filter).ToList());

        return result;
    }


    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryGetDataAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default!)
    {
        var result = await EnsureFp.NotNullAsync(filter, "The filter cannot be null")
                                    .TryMapAsync(async x => (IEnumerable<TEntity>)await GetContext().Set<TEntity>().AsNoTracking().Where(filter).ToListAsync(token));

        return result;
    }


    /// <inheritdoc/>
    public virtual MlResult<IEnumerable<TEntity>> TryAll()
    {
        var result = MlResult<IEnumerable<TEntity>>._
                            .TryMap(x => (IEnumerable<TEntity>)GetContext().Set<TEntity>().AsNoTracking().ToList());

        return result;
    }


    /// <inheritdoc/>
    public virtual async Task<MlResult<IEnumerable<TEntity>>> TryAllAsync(CancellationToken token = default!)
    {
        var result = await MlResult<IEnumerable<TEntity>>._
                                .TryMapAsync(async x => (IEnumerable<TEntity>)await GetContext().Set<TEntity>().AsNoTracking().ToListAsync(token));

        return result;
    }


}



