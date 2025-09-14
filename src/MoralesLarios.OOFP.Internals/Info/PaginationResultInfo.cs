using MoralesLarios.OOFP.ValueObjects;

namespace MoralesLarios.OOFP.Internals.Info;
public record PaginationResultInfo<T>(                         IEnumerable<T> Items, 
                                      [Range(0, int.MaxValue)] int            PageNumber,
                                      [Range(0, int.MinValue)] int            PageSize, 
                                      [Range(0, int.MinValue)] int            TotalCount) 
    : PaginationInfo(PageNumber, PageSize)
{
    public static implicit operator PaginationResultInfo<T>((IEnumerable<T> items, 
                                                             int            pageNumber, 
                                                             int            pageSize, 
                                                             int            totalCount) value)
        => new PaginationResultInfo<T>(value.items, value.pageNumber, value.pageSize, value.totalCount);

}
