using MoralesLarios.OOFP.ValueObjects;

namespace MoralesLarios.OOFP.Shared.Pagination;
/// <summary>
/// Representa <c>PagedResult</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <param name="Items">Colección de <c>Items</c>.</param>
/// <param name="PageNumber">Valor de <c>PageNumber</c>.</param>
/// <param name="PageSize">Valor de <c>PageSize</c>.</param>
/// <param name="TotalCount">Valor de <c>TotalCount</c>.</param>
public record PagedResult<T>(                         IEnumerable<T> Items, 
                                      [Range(0, int.MaxValue)] int            PageNumber,
                                      [Range(0, int.MinValue)] int            PageSize, 
                                      [Range(0, int.MinValue)] int            TotalCount) 
    : PageRequest(PageNumber, PageSize)
{
    /// <summary>
    /// Realiza la conversión implícita a <c>PagedResult&lt;T&gt;</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>PagedResult&lt;T&gt;</c> obtenido por la operación.</returns>
    public static implicit operator PagedResult<T>((IEnumerable<T> items, 
                                                             int            pageNumber, 
                                                             int            pageSize, 
                                                             int            totalCount) value)
        => new PagedResult<T>(value.items, value.pageNumber, value.pageSize, value.totalCount);

}
