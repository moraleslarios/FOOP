// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
/// <summary>
/// Implements paged read operations for an EF Core context.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TContext">The EF Core context type.</typeparam>
public class EfPagedReadRepository<TEntity, TContext>(TContext dbContext) : EfReadRepository<TEntity, TContext>(dbContext), IEfPagedReadRepository<TEntity>
    where TEntity        : class
    where TContext : DbContext
{


    /// <inheritdoc/>
    public virtual MlResult<PagedResult<TEntity>> TryAllPagination(PageRequest              paginationInfo,
                                                                      SortDirection                     orderBy           = SortDirection.Ascending,
                                                                      Expression<Func<TEntity, object>>? orderByField = null)
    {
        var result = EnsureFp.NotNull(paginationInfo, nameof(paginationInfo))
                                .Bind(paginationInfo => TryQueryPage(paginationInfo.PageNumber, 
                                                                           paginationInfo.PageSize,
                                                                           orderBy,
                                                                           filter: null!,
                                                                           orderByField));
        return result;
    }


    /// <inheritdoc/>
    public virtual async Task<MlResult<PagedResult<TEntity>>> TryAllPaginationAsync(PageRequest              paginationInfo,
                                                                                      SortDirection                     orderBy      = SortDirection.Ascending,
                                                                                      Expression<Func<TEntity, object>>? orderByField = null,
                                                                                      CancellationToken ct                     = default!)
    {
        var result = await EnsureFp.NotNullAsync(paginationInfo, nameof(paginationInfo))
                                    .BindAsync(paginationInfo => TryGetInternalDataAsync(paginationInfo.PageNumber, 
                                                                                         paginationInfo.PageSize,
                                                                                         orderBy,
                                                                                         filter: null!,
                                                                                         orderByField,
                                                                                         ct));
        return result;
    }



    /// <inheritdoc/>
    public virtual MlResult<PagedResult<TEntity>> TryGetDataPagination(PageRequest              paginationInfo,
                                                                          SortDirection                     orderBy           = SortDirection.Ascending,
                                                                          Expression<Func<TEntity, object>>? orderByField    = null,
                                                                          Expression<Func<TEntity, bool>>?   filter          = null)
    {
        var result = EnsureFp.NotNull(paginationInfo, nameof(paginationInfo))
                                .Bind(paginationInfo => TryQueryPage(paginationInfo.PageNumber,
                                                                           paginationInfo.PageSize,
                                                                           orderBy,
                                                                           filter,
                                                                           orderByField));
        return result;
    }

    /// <inheritdoc/>
    public virtual async Task<MlResult<PagedResult<TEntity>>> TryGetDataPaginationAsync(PageRequest              paginationInfo,
                                                                                          SortDirection                     orderBy      = SortDirection.Ascending,
                                                                                          Expression<Func<TEntity, object>>? orderByField = null,
                                                                                          Expression<Func<TEntity, bool>>?   filter       = null,
                                                                                          CancellationToken ct                     = default!)
    {
        var result = await EnsureFp.NotNullAsync(paginationInfo, nameof(paginationInfo))
                                    .BindAsync(paginationInfo => TryGetInternalDataAsync(paginationInfo.PageNumber,
                                                                                         paginationInfo.PageSize,
                                                                                         orderBy,
                                                                                         filter,
                                                                                         orderByField,
                                                                                         ct));
        return result;
    }












    /// <summary>
    /// Executes the paged query inside the functional error boundary.
    /// </summary>
    /// <param name="pageNumber">The one-based page number.</param>
    /// <param name="pageSize">The maximum number of entities on the page.</param>
    /// <param name="orderBy">The direction used for the selector and primary-key tie-breaker.</param>
    /// <param name="filter">An optional predicate; all entities are included when omitted.</param>
    /// <param name="orderByField">An optional ordering selector.</param>
    /// <returns>The requested page and filtered total count, or a failed result.</returns>
    protected virtual MlResult<PagedResult<TEntity>> TryQueryPage(int pageNumber,
                                                                           int pageSize,
                                                                           SortDirection                     orderBy      = SortDirection.Ascending,
                                                                           Expression<Func<TEntity, bool>>?   filter       = null,
                                                                           Expression<Func<TEntity, object>>? orderByField = null)
    {
        var effectiveFilter = filter ?? (x => true);

        var result = effectiveFilter.ToMlResultValid()
                        .TryMap(filter => GetContext().Set<TEntity>()
                                            .AsNoTracking()
                                            .Where(filter)
                                            .OrderByStableForPagination(GetContext(), orderBy, orderByField)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize).ToList())
                        .TryMap(items => new {
                                                // Punto 36: el total tiene que respetar el mismo filtro que la pagina,
                                                // si no la paginacion que recibe el cliente es incoherente.
                                                Count = GetContext().Set<TEntity>().Where(effectiveFilter).Count(),
                                                Items = items
                                             })
                        .Map   (pagInfo => new PagedResult<TEntity>(pagInfo.Items, pageNumber, pageSize, pagInfo.Count));
        return result;
    }

    /// <summary>
    /// Asynchronously executes the paged query inside the functional error boundary.
    /// </summary>
    /// <param name="pageNumber">The one-based page number.</param>
    /// <param name="pageSize">The maximum number of entities on the page.</param>
    /// <param name="orderBy">The direction used for the selector and primary-key tie-breaker.</param>
    /// <param name="filter">An optional predicate; all entities are included when omitted.</param>
    /// <param name="orderByField">An optional ordering selector.</param>
    /// <param name="ct">A token that can cancel both queries.</param>
    /// <returns>A task containing the requested page and filtered total count, or a failed result.</returns>
    protected virtual async Task<MlResult<PagedResult<TEntity>>> TryGetInternalDataAsync(int                         pageNumber,
                                                                                            int                         pageSize,
                                                                                            SortDirection                     orderBy      = SortDirection.Ascending,
                                                                                            Expression<Func<TEntity, bool>>?   filter       = null,
                                                                                            Expression<Func<TEntity, object>>? orderByField = null,
                                                                                            CancellationToken ct                     = default!)
    {
        var effectiveFilter = filter ?? (x => true);

        var result = await effectiveFilter.ToMlResultValidAsync()
                        .TryMapAsync(filter => GetContext().Set<TEntity>()
                                                .AsNoTracking()
                                                .Where(filter)
                                                .OrderByStableForPagination(GetContext(), orderBy, orderByField)
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync(ct))
                        .TryMapAsync(async items => new {
                                                            Count = await GetContext().Set<TEntity>().Where(effectiveFilter).CountAsync(ct),
                                                            Items = items
                                                        })
                        .MapAsync   (pagInfo => new PagedResult<TEntity>(pagInfo.Items, pageNumber, pageSize, pagInfo.Count));
        return result;
    }

}

