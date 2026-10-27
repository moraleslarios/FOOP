namespace MoralesLarios.OOFP.Shared.Pagination;

public record PageRequest([property: Range(0, int.MinValue)] int PageNumber, 
                             [property: Range(0, int.MinValue)] int PageSize)
{
    private const int MaxPageSize = 1000;

    public int PageNumber { get; init; } = Math.Max(1, PageNumber);
    public int PageSize   { get; init; } = Math.Clamp(PageSize, 1, MaxPageSize);

     public static implicit operator PageRequest((int pageNumber, int pageSize) value)
        => new PageRequest(value.pageNumber, value.pageSize);

    public static implicit operator PageRequest((IntNonNegative pageNumber, IntNonNegative pageSize) value)
        => new PageRequest(value.pageNumber, value.pageSize);

}
