// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
/// <summary>
/// Representa <c>EfPagedRepository</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
public class EfPagedRepository<T, TContext>(TContext dbContext) : EfRepository(dbContext), IEfPagedRepository<T>
    where T : class
    where TContext : DbContext
{
    private readonly IEfPagedReadRepository<T> _repoReaderPaginatorFp = RegisterServices.GetRepository<IEfPagedReadRepository<T>>();
    private readonly IEfWriteRepository<T>           _repoWriterFp          = RegisterServices.GetRepository<IEfWriteRepository          <T>>();



    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryFind(params object[] keyValues) => _repoReaderPaginatorFp.TryFind(keyValues);

    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] keyValues) => await _repoReaderPaginatorFp.TryFindAsync(token, keyValues);

    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoReaderPaginatorFp.TryFind(notFoundErrorDetails, keyValues);

    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryFindAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoReaderPaginatorFp.TryFindAsync(notFoundErrorDetails, token, keyValues);

    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter) => _repoReaderPaginatorFp.TryFirst(filter);

    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryFirstAsync(filter, token);

    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails) => _repoReaderPaginatorFp.TryFirst(filter, notFoundErrorDetails);

    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderPaginatorFp.TryFirstAsync(filter, notFoundErrorDetails, token);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryGetLast(Expression<Func<T, bool>> filter) => _repoReaderPaginatorFp.TryGetLast(filter);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryGetLastAsync(filter, token);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryGetLast(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails) => _repoReaderPaginatorFp.TryGetLast(filter, notFoundErrorDetails);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default) => await _repoReaderPaginatorFp.TryGetLastAsync(filter, notFoundErrorDetails, token);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Data</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter) => _repoReaderPaginatorFp.TryGetData(filter);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Data</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!) => await _repoReaderPaginatorFp.TryGetDataAsync(filter, token);

    /// <summary>
    /// Indica si todos los elementos cumplen la condición. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<IEnumerable<T>> TryAll() => _repoReaderPaginatorFp.TryAll();

    /// <summary>
    /// Indica si todos los elementos cumplen la condición. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default!) => await _repoReaderPaginatorFp.TryAllAsync(token);



    /// <summary>
    /// Añade el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryAdd(T item) => _repoWriterFp.TryAdd(item);
    /// <summary>
    /// Añade el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryAddAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryAddAsync(item, token);
    /// <summary>
    /// Añade el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte en
    /// un resultado fallido.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<IEnumerable<T>> TryAddRange(IEnumerable<T> items) => _repoWriterFp.TryAddRange(items);
    /// <summary>
    /// Añade el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte en
    /// un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryAddRangeAsync(items, token);
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryRemove(T item) => _repoWriterFp.TryRemove(item);
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryRemove(params object[] keyValues) => _repoWriterFp.TryRemove(keyValues);
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryRemove(MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoWriterFp.TryRemove(notFoundErrorDetails, keyValues);
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryRemoveAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryRemoveAsync(item, token);
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryRemoveAsync(CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryRemoveAsync(token, keyValues);
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryRemoveAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryRemoveAsync(notFoundErrorDetails, token, keyValues);
    /// <summary>
    /// Elimina el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<IEnumerable<T>> TryRemoveRange(IEnumerable<T> items) => _repoWriterFp.TryRemoveRange(items);
    /// <summary>
    /// Elimina el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryRemoveRangeAsync(items, token);
    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryUpdate(T item) => _repoWriterFp.TryUpdate(item);
    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryUpdate(T item, params object[] keyValues) => _repoWriterFp.TryUpdate(item, keyValues);
    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryUpdate(T item, MlErrorDetails notFoundErrorDetails, params object[] keyValues) => _repoWriterFp.TryUpdate(item, notFoundErrorDetails, keyValues);
    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default) => await _repoWriterFp.TryUpdateAsync(item, token);
    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryUpdateAsync(item, token, keyValues);
    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues) => await _repoWriterFp.TryUpdateAsync(item, notFoundErrorDetails, token, keyValues);
    /// <summary>
    /// Actualiza el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items) => _repoWriterFp.TryUpdateRange(items);
    /// <summary>
    /// Actualiza el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoWriterFp.TryUpdateRangeAsync(items, token);








    /// <summary>
    /// Indica si todos los elementos cumplen la condición de <c>Pagination</c>. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido.
    /// </summary>
    /// <param name="paginationInfo">Valor de <c>paginationInfo</c>.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<PagedResult<T>> TryAllPagination(PageRequest              paginationInfo, 
                                                                      SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                      Expression<Func<T, object>> orderByField = null!)
         => _repoReaderPaginatorFp.TryAllPagination(paginationInfo, orderBy, orderByField);

    /// <summary>
    /// Indica si todos los elementos cumplen la condición de <c>Pagination</c>. Captura cualquier excepción
    /// producida y la convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="paginationInfo">Valor de <c>paginationInfo</c>.</param>
    /// <param name="orderBy">Expresión de ordenación aplicada a la consulta.</param>
    /// <param name="orderByField">Valor de <c>orderByField</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<PagedResult<T>>> TryAllPaginationAsync(PageRequest              paginationInfo, 
                                                                                 SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                                 Expression<Func<T, object>> orderByField = null!,
                                                                                 CancellationToken ct                     = default!)
         => _repoReaderPaginatorFp.TryAllPaginationAsync(paginationInfo, orderBy, orderByField, ct);

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
                                                                          SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                          Expression<Func<T, object>> orderByField = null, 
                                                                          Expression<Func<T, bool>>   filter       = null!)
         => _repoReaderPaginatorFp.TryGetDataPagination(paginationInfo, orderBy, orderByField, filter);

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
    public virtual Task<MlResult<PagedResult<T>>> TryGetDataPaginationAsync(PageRequest paginationInfo, 
                                                                                     SortDirection                     orderBy      = SortDirection.Ascending, 
                                                                                     Expression<Func<T, object>> orderByField = null!, 
                                                                                     Expression<Func<T, bool>>   filter       = null!,
                                                                                     CancellationToken ct                     = default!)
         => _repoReaderPaginatorFp.TryGetDataPaginationAsync(paginationInfo, orderBy, orderByField, filter, ct);
}

