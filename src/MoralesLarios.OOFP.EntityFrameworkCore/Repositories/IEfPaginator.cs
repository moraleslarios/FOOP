// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0




namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Defines paged read operations for an EF Core repository.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IEfPaginator<TEntity> where TEntity : class
{
    /// <summary>
    /// Returns a page containing all entities, ordered by the primary key when no selector is supplied.
    /// </summary>
    /// <param name="paginationInfo">The requested page number and size.</param>
    /// <param name="orderBy">The direction used for the selector and primary-key tie-breaker.</param>
    /// <param name="orderByField">An optional selector; the primary key is used when omitted.</param>
    /// <returns>The requested page and total count, or a failed result.</returns>
    MlResult<PagedResult<TEntity>>       TryAllPagination         (PageRequest paginationInfo, SortDirection orderBy = SortDirection.Ascending, Expression<Func<TEntity, object>>? orderByField = null);
    /// <summary>
    /// Asynchronously returns a page containing all entities, ordered by the primary key when no selector is supplied.
    /// </summary>
    /// <param name="paginationInfo">The requested page number and size.</param>
    /// <param name="orderBy">The direction used for the selector and primary-key tie-breaker.</param>
    /// <param name="orderByField">An optional selector; the primary key is used when omitted.</param>
    /// <param name="ct">A token that can cancel the queries.</param>
    /// <returns>A task containing the requested page and total count, or a failed result.</returns>
    Task<MlResult<PagedResult<TEntity>>> TryAllPaginationAsync    (PageRequest paginationInfo, SortDirection orderBy = SortDirection.Ascending, Expression<Func<TEntity, object>>? orderByField = null, CancellationToken ct = default!);
    /// <summary>
    /// Returns a filtered page. The primary key provides a default order and breaks selector ties.
    /// </summary>
    /// <param name="paginationInfo">The requested page number and size.</param>
    /// <param name="orderBy">The direction used for the selector and primary-key tie-breaker.</param>
    /// <param name="orderByField">An optional selector; the primary key is used when omitted.</param>
    /// <param name="filter">An optional predicate; all entities are included when omitted.</param>
    /// <returns>The requested page and filtered total count, or a failed result.</returns>
    MlResult<PagedResult<TEntity>>       TryGetDataPagination     (PageRequest paginationInfo, SortDirection orderBy = SortDirection.Ascending, Expression<Func<TEntity, object>>? orderByField = null, Expression<Func<TEntity, bool>>? filter = null);
    /// <summary>
    /// Asynchronously returns a filtered page. The primary key provides a default order and breaks selector ties.
    /// </summary>
    /// <param name="paginationInfo">The requested page number and size.</param>
    /// <param name="orderBy">The direction used for the selector and primary-key tie-breaker.</param>
    /// <param name="orderByField">An optional selector; the primary key is used when omitted.</param>
    /// <param name="filter">An optional predicate; all entities are included when omitted.</param>
    /// <param name="ct">A token that can cancel the queries.</param>
    /// <returns>A task containing the requested page and filtered total count, or a failed result.</returns>
    Task<MlResult<PagedResult<TEntity>>> TryGetDataPaginationAsync(PageRequest paginationInfo, SortDirection orderBy = SortDirection.Ascending, Expression<Func<TEntity, object>>? orderByField = null, Expression<Func<TEntity, bool>>? filter = null, CancellationToken ct = default!);
}
