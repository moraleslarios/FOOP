// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0




namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

public interface IEFPaginatorFp<T> where T : class
{
    MlResult<PagedResult<T>>       TryAllPagination         (PageRequest paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!);
    Task<MlResult<PagedResult<T>>> TryAllPaginationAsync    (PageRequest paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!                                            , CancellationToken ct = default!);
    MlResult<PagedResult<T>>       TryGetDataPagination     (PageRequest paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!, Expression<Func<T, bool>> filter = null!);
    Task<MlResult<PagedResult<T>>> TryGetDataPaginationAsync(PageRequest paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!, Expression<Func<T, bool>> filter = null!  , CancellationToken ct = default!);
}
