namespace MoralesLarios.OOFP.Shared.Pagination;

/// <summary>
/// Representa <c>PageRequest</c>.
/// </summary>
/// <param name="PageNumber">Valor de <c>PageNumber</c>.</param>
/// <param name="PageSize">Valor de <c>PageSize</c>.</param>
public record PageRequest([property: Range(0, int.MinValue)] int PageNumber, 
                             [property: Range(0, int.MinValue)] int PageSize)
{
    private const int MaxPageSize = 1000;

    /// <summary>
    /// Obtiene o establece el valor de <c>PageNumber</c>.
    /// </summary>
    public int PageNumber { get; init; } = Math.Max(1, PageNumber);
    /// <summary>
    /// Obtiene o establece el valor de <c>PageSize</c>.
    /// </summary>
    public int PageSize   { get; init; } = Math.Clamp(PageSize, 1, MaxPageSize);

     /// <summary>
     /// Realiza la conversión implícita a <c>PageRequest</c>.
     /// </summary>
     /// <param name="value">Valor sobre el que se opera.</param>
     /// <returns>Valor de tipo <c>PageRequest</c> obtenido por la operación.</returns>
     public static implicit operator PageRequest((int pageNumber, int pageSize) value)
        => new PageRequest(value.pageNumber, value.pageSize);

    /// <summary>
    /// Realiza la conversión implícita a <c>PageRequest</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>PageRequest</c> obtenido por la operación.</returns>
    public static implicit operator PageRequest((IntNonNegative pageNumber, IntNonNegative pageSize) value)
        => new PageRequest(value.pageNumber, value.pageSize);

}
