


namespace MoralesLarios.OOFP.EFCore.Repos;

public interface IEFPaginatorFp<T> where T : class
{
    MlResult<PaginationResultInfo<T>>       TryAllPagination         (PaginationInfo paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!);
    Task<MlResult<PaginationResultInfo<T>>> TryAllPaginationAsync    (PaginationInfo paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!                                            , CancellationToken ct = default!);
    MlResult<PaginationResultInfo<T>>       TryGetDataPagination     (PaginationInfo paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!, Expression<Func<T, bool>> filter = null!);
    Task<MlResult<PaginationResultInfo<T>>> TryGetDataPaginationAsync(PaginationInfo paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!, Expression<Func<T, bool>> filter = null!  , CancellationToken ct = default!);
}