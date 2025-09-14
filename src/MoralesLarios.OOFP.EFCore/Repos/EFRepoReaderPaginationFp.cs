





namespace MoralesLarios.OOFP.EFCore.Repos;
public class EFRepoReaderPaginationFp<T, TContext>(TContext dbContext) : EFRepoReaderFp<T, TContext>(dbContext), IEFRepoReaderPaginationFp<T>
    where T        : class
    where TContext : DbContext
{


    public MlResult<PaginationResultInfo<T>> TryAllPagination(PaginationInfo              paginationInfo,
                                                              OrderBy                     orderBy           = OrderBy.Ascending,
                                                              Expression<Func<T, object>> orderByField = null!)
    {
        var result = EnsureFp.NotNull(paginationInfo, nameof(paginationInfo))
                                .Bind(paginationInfo => TryGetInternalData(paginationInfo.PageNumber, 
                                                                           paginationInfo.PageSize,
                                                                           orderBy,
                                                                           filter: null!,
                                                                           orderByField));
        return result;
    }


    public async Task<MlResult<PaginationResultInfo<T>>> TryAllPaginationAsync(PaginationInfo              paginationInfo,
                                                                               OrderBy                     orderBy      = OrderBy.Ascending,
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



    public MlResult<PaginationResultInfo<T>> TryGetDataPagination(PaginationInfo              paginationInfo,
                                                                  OrderBy                     orderBy           = OrderBy.Ascending,
                                                                  Expression<Func<T, object>> orderByField    = null!,
                                                                  Expression<Func<T, bool  >> filter          = null!)
    {
        var result = EnsureFp.NotNull(paginationInfo, nameof(paginationInfo))
                                .Bind(paginationInfo => TryGetInternalData(paginationInfo.PageNumber,
                                                                           paginationInfo.PageSize,
                                                                           orderBy,
                                                                           filter,
                                                                           orderByField));
        return result;
    }
    
    public async Task<MlResult<PaginationResultInfo<T>>> TryGetDataPaginationAsync(PaginationInfo              paginationInfo,
                                                                                   OrderBy                     orderBy      = OrderBy.Ascending,
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












    protected MlResult<PaginationResultInfo<T>> TryGetInternalData(int pageNumber, 
                                                                   int pageSize,
                                                                   OrderBy                     orderBy      = OrderBy.Ascending,
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
                        .Map   (pagInfo => new PaginationResultInfo<T>(pagInfo.Items, pageNumber, pageSize, pagInfo.Count));
        return result;
    }

    protected async Task<MlResult<PaginationResultInfo<T>>> TryGetInternalDataAsync(int                         pageNumber, 
                                                                                    int                         pageSize,
                                                                                    OrderBy                     orderBy      = OrderBy.Ascending,
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
                        .MapAsync   (pagInfo => new PaginationResultInfo<T>(pagInfo.Items, pageNumber, pageSize, pagInfo.Count));
        return result;
    }





}
