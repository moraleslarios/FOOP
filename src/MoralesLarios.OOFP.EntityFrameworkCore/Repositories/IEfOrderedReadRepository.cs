// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>Read repository with an explicitly ordered, server-limited last-item operation.</summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public interface IEfOrderedReadRepository<TEntity> : IEfReadRepository<TEntity>
    where TEntity : class
{
    /// <summary>Gets the last matching item according to the supplied sort order.</summary>
    /// <param name="filter">Filter applied to the entity set.</param>
    /// <param name="orderByField">Required sort selector.</param>
    /// <param name="orderBy">Ascending selects the maximum; descending selects the minimum.</param>
    /// <returns>The selected item, or a failed result if none matches.</returns>
    MlResult<TEntity> TryGetLastBy(Expression<Func<TEntity, bool>> filter,
                             Expression<Func<TEntity, object>> orderByField,
                             SortDirection orderBy = SortDirection.Ascending);

    /// <summary>Gets the last matching item using custom not-found details.</summary>
    /// <param name="filter">Filter applied to the entity set.</param>
    /// <param name="orderByField">Required sort selector.</param>
    /// <param name="notFoundErrorDetails">Error returned when no item matches.</param>
    /// <param name="orderBy">Ascending selects the maximum; descending selects the minimum.</param>
    /// <returns>The selected item, or a failed result if none matches.</returns>
    MlResult<TEntity> TryGetLastBy(Expression<Func<TEntity, bool>> filter,
                             Expression<Func<TEntity, object>> orderByField,
                             MlErrorDetails notFoundErrorDetails,
                             SortDirection orderBy = SortDirection.Ascending);

    /// <summary>Asynchronously gets the last matching item according to the supplied sort order.</summary>
    /// <param name="filter">Filter applied to the entity set.</param>
    /// <param name="orderByField">Required sort selector.</param>
    /// <param name="orderBy">Ascending selects the maximum; descending selects the minimum.</param>
    /// <param name="token">Token used to cancel the query.</param>
    /// <returns>A task containing the selected item, or a failed result if none matches.</returns>
    Task<MlResult<TEntity>> TryGetLastByAsync(Expression<Func<TEntity, bool>> filter,
                                        Expression<Func<TEntity, object>> orderByField,
                                        SortDirection orderBy = SortDirection.Ascending,
                                        CancellationToken token = default);

    /// <summary>Asynchronously gets the last matching item using custom not-found details.</summary>
    /// <param name="filter">Filter applied to the entity set.</param>
    /// <param name="orderByField">Required sort selector.</param>
    /// <param name="notFoundErrorDetails">Error returned when no item matches.</param>
    /// <param name="orderBy">Ascending selects the maximum; descending selects the minimum.</param>
    /// <param name="token">Token used to cancel the query.</param>
    /// <returns>A task containing the selected item, or a failed result if none matches.</returns>
    Task<MlResult<TEntity>> TryGetLastByAsync(Expression<Func<TEntity, bool>> filter,
                                        Expression<Func<TEntity, object>> orderByField,
                                        MlErrorDetails notFoundErrorDetails,
                                        SortDirection orderBy = SortDirection.Ascending,
                                        CancellationToken token = default);
}