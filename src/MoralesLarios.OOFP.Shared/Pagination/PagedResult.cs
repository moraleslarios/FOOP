using MoralesLarios.OOFP.ValueObjects;

namespace MoralesLarios.OOFP.Shared.Pagination;
public record PagedResult<T>(                         IEnumerable<T> Items, 
                                      [Range(0, int.MaxValue)] int            PageNumber,
                                      [Range(0, int.MinValue)] int            PageSize, 
                                      [Range(0, int.MinValue)] int            TotalCount) 
    : PageRequest(PageNumber, PageSize)
{
    public static implicit operator PagedResult<T>((IEnumerable<T> items, 
                                                             int            pageNumber, 
                                                             int            pageSize, 
                                                             int            totalCount) value)
        => new PagedResult<T>(value.items, value.pageNumber, value.pageSize, value.totalCount);

}
