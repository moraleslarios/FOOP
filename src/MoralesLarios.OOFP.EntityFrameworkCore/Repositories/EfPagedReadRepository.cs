// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
public class EfPagedReadRepository<T, TContext>(TContext dbContext) : EfReadRepository<T, TContext>(dbContext), IEfPagedReadRepository<T>
    where T        : class
    where TContext : DbContext
{


    public virtual MlResult<PagedResult<T>> TryAllPagination(PageRequest              paginationInfo,
                                                                      SortDirection                     orderBy           = SortDirection.Ascending,
                                                                      Expression<Func<T, object>> orderByField = null!)
    {
        var result = EnsureFp.NotNull(paginationInfo, nameof(paginationInfo))
                                .Bind(paginationInfo => TryQueryPage(paginationInfo.PageNumber, 
                                                                           paginationInfo.PageSize,
                                                                           orderBy,
                                                                           filter: null!,
                                                                           orderByField));
        return result;
    }


    public virtual async Task<MlResult<PagedResult<T>>> TryAllPaginationAsync(PageRequest              paginationInfo,
                                                                                      SortDirection                     orderBy      = SortDirection.Ascending,
                                                                                      Expression<Func<T, object>> orderByField = null!,
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



    public virtual MlResult<PagedResult<T>> TryGetDataPagination(PageRequest              paginationInfo,
                                                                          SortDirection                     orderBy           = SortDirection.Ascending,
                                                                          Expression<Func<T, object>> orderByField    = null!,
                                                                          Expression<Func<T, bool  >> filter          = null!)
    {
        var result = EnsureFp.NotNull(paginationInfo, nameof(paginationInfo))
                                .Bind(paginationInfo => TryQueryPage(paginationInfo.PageNumber,
                                                                           paginationInfo.PageSize,
                                                                           orderBy,
                                                                           filter,
                                                                           orderByField));
        return result;
    }

    public virtual async Task<MlResult<PagedResult<T>>> TryGetDataPaginationAsync(PageRequest              paginationInfo,
                                                                                          SortDirection                     orderBy      = SortDirection.Ascending,
                                                                                          Expression<Func<T, object>> orderByField = null!,
                                                                                          Expression<Func<T, bool  >> filter       = null!,
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












    protected virtual MlResult<PagedResult<T>> TryQueryPage(int pageNumber, 
                                                                           int pageSize,
                                                                           SortDirection                     orderBy      = SortDirection.Ascending,
                                                                           Expression<Func<T, bool  >> filter       = null!,
                                                                           Expression<Func<T, object>> orderByField = null!)
    {
        var result = (filter ?? (x => true)).ToMlResultValid()
                        .TryMap(filter => GetContext().Set<T>()
                                            .AsNoTracking()
                                            .Where(filter)
                                            .PrivateOrderBy(orderBy, orderByField)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize).ToList())
                        .TryMap(items => new {
                                                Count = GetContext().Set<T>().Count(), 
                                                Items = items 
                                             })
                        .Map   (pagInfo => new PagedResult<T>(pagInfo.Items, pageNumber, pageSize, pagInfo.Count));
        return result;
    }

    protected virtual async Task<MlResult<PagedResult<T>>> TryGetInternalDataAsync(int                         pageNumber, 
                                                                                            int                         pageSize,
                                                                                            SortDirection                     orderBy      = SortDirection.Ascending,
                                                                                            Expression<Func<T, bool  >> filter       = null!,
                                                                                            Expression<Func<T, object>> orderByField = null!,
                                                                                            CancellationToken ct                     = default!)
    {
        var result = await (filter ?? (x => true)).ToMlResultValidAsync()
                        .TryMapAsync(filter => GetContext().Set<T>()
                                                .AsNoTracking()
                                                .Where(filter)
                                                .PrivateOrderBy(orderBy, orderByField)
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync(ct))
                        .TryMapAsync(items => new {
                                                      Count = GetContext().Set<T>().Where(filter!).Count(), 
                                                      Items = items 
                                                  })
                        .MapAsync   (pagInfo => new PagedResult<T>(pagInfo.Items, pageNumber, pageSize, pagInfo.Count));
        return result;
    }





}

