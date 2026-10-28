// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
/// <summary>
/// Representa <c>EfPagedReadRepository</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
public class EfPagedReadRepository<T, TContext>(TContext dbContext) : EfReadRepository<T, TContext>(dbContext), IEfPagedReadRepository<T>
    where T        : class
    where TContext : DbContext
{


    /// <summary>
    /// Indica si todos los elementos cumplen la condición de <c>Pagination</c>. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido.
    /// </summary>
    /// <param name="paginationInfo">Valor de <c>paginationInfo</c>.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
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


    /// <summary>
    /// Indica si todos los elementos cumplen la condición de <c>Pagination</c>. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="paginationInfo">Valor de <c>paginationInfo</c>.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
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



    /// <summary>
    /// Obtiene el elemento solicitado de <c>DataPagination</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <param name="paginationInfo">Valor de <c>paginationInfo</c>.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
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












    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por query page.
    /// </summary>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    protected virtual MlResult<PagedResult<T>> TryQueryPage(int pageNumber, 
                                                                           int pageSize,
                                                                           SortDirection                     orderBy      = SortDirection.Ascending,
                                                                           Expression<Func<T, bool  >> filter       = null!,
                                                                           Expression<Func<T, object>> orderByField = null!)
    {
        var effectiveFilter = filter ?? (x => true);

        var result = effectiveFilter.ToMlResultValid()
                        .TryMap(filter => GetContext().Set<T>()
                                            .AsNoTracking()
                                            .Where(filter)
                                            .PrivateOrderBy(orderBy, orderByField)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize).ToList())
                        .TryMap(items => new {
                                                // Punto 36: el total tiene que respetar el mismo filtro que la pagina,
                                                // si no la paginacion que recibe el cliente es incoherente.
                                                Count = GetContext().Set<T>().Where(effectiveFilter).Count(), 
                                                Items = items 
                                             })
                        .Map   (pagInfo => new PagedResult<T>(pagInfo.Items, pageNumber, pageSize, pagInfo.Count));
        return result;
    }

    /// <summary>
    /// Obtiene el elemento solicitado de <c>InternalData</c>. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    protected virtual async Task<MlResult<PagedResult<T>>> TryGetInternalDataAsync(int                         pageNumber, 
                                                                                            int                         pageSize,
                                                                                            SortDirection                     orderBy      = SortDirection.Ascending,
                                                                                            Expression<Func<T, bool  >> filter       = null!,
                                                                                            Expression<Func<T, object>> orderByField = null!,
                                                                                            CancellationToken ct                     = default!)
    {
        var effectiveFilter = filter ?? (x => true);

        var result = await effectiveFilter.ToMlResultValidAsync()
                        .TryMapAsync(filter => GetContext().Set<T>()
                                                .AsNoTracking()
                                                .Where(filter)
                                                .PrivateOrderBy(orderBy, orderByField)
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync(ct))
                        .TryMapAsync(items => new {
                                                      Count = GetContext().Set<T>().Where(effectiveFilter).Count(), 
                                                      Items = items 
                                                  })
                        .MapAsync   (pagInfo => new PagedResult<T>(pagInfo.Items, pageNumber, pageSize, pagInfo.Count));
        return result;
    }

}

