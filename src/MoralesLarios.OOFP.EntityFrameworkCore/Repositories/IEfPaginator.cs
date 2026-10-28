// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0




namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Contrato de <c>IEfPaginator</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
public interface IEfPaginator<T> where T : class
{
    /// <summary>
    /// Indica si todos los elementos cumplen la condición de <c>Pagination</c>. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido.
    /// </summary>
    /// <param name="paginationInfo">Valor de <c>paginationInfo</c>.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<PagedResult<T>>       TryAllPagination         (PageRequest paginationInfo, SortDirection orderBy = SortDirection.Ascending, Expression<Func<T, object>> orderByField = null!);
    /// <summary>
    /// Indica si todos los elementos cumplen la condición de <c>Pagination</c>. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="paginationInfo">Valor de <c>paginationInfo</c>.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<PagedResult<T>>> TryAllPaginationAsync    (PageRequest paginationInfo, SortDirection orderBy = SortDirection.Ascending, Expression<Func<T, object>> orderByField = null!                                            , CancellationToken ct = default!);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>DataPagination</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <param name="paginationInfo">Valor de <c>paginationInfo</c>.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<PagedResult<T>>       TryGetDataPagination     (PageRequest paginationInfo, SortDirection orderBy = SortDirection.Ascending, Expression<Func<T, object>> orderByField = null!, Expression<Func<T, bool>> filter = null!);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>DataPagination</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="paginationInfo">Valor de <c>paginationInfo</c>.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<PagedResult<T>>> TryGetDataPaginationAsync(PageRequest paginationInfo, SortDirection orderBy = SortDirection.Ascending, Expression<Func<T, object>> orderByField = null!, Expression<Func<T, bool>> filter = null!  , CancellationToken ct = default!);
}
