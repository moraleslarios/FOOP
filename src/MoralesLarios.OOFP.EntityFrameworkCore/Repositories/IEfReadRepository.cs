// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Defines read operations for entities stored by Entity Framework Core.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IEfReadRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Finds an entity by its primary-key values. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A successful result containing the entity, or a failed result when it cannot be found or the query fails.</returns>
    MlResult<TEntity> TryFind(params object[] keyValues);
    /// <summary>
    /// Asynchronously finds an entity by its primary-key values. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="token">A token that can cancel the query.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A task containing the entity or a failed result.</returns>
    Task<MlResult<TEntity>> TryFindAsync(CancellationToken token = default, params object[] keyValues);
    /// <summary>
    /// Asynchronously returns the first entity that matches the filter. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="filter">The predicate used to filter entities.</param>
    /// <param name="token">A token that can cancel the query.</param>
    /// <returns>A task containing the first matching entity or a failed result.</returns>
    Task<MlResult<TEntity>> TryFirstAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default);
    /// <summary>
    /// Returns the first entity that matches the filter. Exceptions are returned as a failed result.
    /// </summary>
    /// <param name="filter">The predicate used to filter entities.</param>
    /// <returns>The first matching entity or a failed result.</returns>
    MlResult<TEntity> TryFirst(Expression<Func<TEntity, bool>> filter);
    /// <summary>
    /// Asynchronously returns the last entity yielded by the filtered query. No explicit ordering is applied.
    /// </summary>
    /// <param name="filter">The predicate used to filter entities.</param>
    /// <param name="token">A token that can cancel the query.</param>
    /// <returns>A task containing the last entity yielded by the query or a failed result.</returns>
    Task<MlResult<TEntity>> TryGetLastAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default);
    /// <summary>
    /// Returns the last entity yielded by the filtered query. No explicit ordering is applied.
    /// </summary>
    /// <param name="filter">The predicate used to filter entities.</param>
    /// <returns>The last entity yielded by the query or a failed result.</returns>
    MlResult<TEntity> TryGetLast(Expression<Func<TEntity, bool>> filter);
    /// <summary>
    /// Returns all entities that match the filter. The query is materialized before this method returns.
    /// </summary>
    /// <param name="filter">The predicate used to filter entities.</param>
    /// <returns>A materialized sequence of matching entities or a failed result.</returns>
    MlResult<IEnumerable<TEntity>> TryGetData(Expression<Func<TEntity, bool>> filter);
    /// <summary>
    /// Asynchronously returns all entities that match the filter. The query is materialized before this method returns.
    /// </summary>
    /// <param name="filter">The predicate used to filter entities.</param>
    /// <param name="token">A token that can cancel the query.</param>
    /// <returns>A task containing a materialized sequence of matching entities or a failed result.</returns>
    Task<MlResult<IEnumerable<TEntity>>> TryGetDataAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default);
    /// <summary>
    /// Returns all entities. The query is materialized before this method returns.
    /// </summary>
    /// <returns>A materialized sequence of entities or a failed result.</returns>
    MlResult<IEnumerable<TEntity>> TryAll();
    /// <summary>
    /// Asynchronously returns all entities. The query is materialized before this method returns.
    /// </summary>
    /// <param name="token">A token that can cancel the query.</param>
    /// <returns>A task containing a materialized sequence of entities or a failed result.</returns>
    Task<MlResult<IEnumerable<TEntity>>> TryAllAsync(CancellationToken token = default);
    /// <summary>
    /// Finds an entity by its primary-key values using the supplied not-found error details.
    /// </summary>
    /// <param name="notFoundErrorDetails">The error details returned when no entity is found.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>The matching entity or a failed result.</returns>
    MlResult<TEntity> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues);
    /// <summary>
    /// Asynchronously finds an entity by its primary-key values using the supplied not-found error details.
    /// </summary>
    /// <param name="notFoundErrorDetails">The error details returned when no entity is found.</param>
    /// <param name="token">A token that can cancel the query.</param>
    /// <param name="keyValues">The primary-key values, in the order defined by the EF model.</param>
    /// <returns>A task containing the matching entity or a failed result.</returns>
    Task<MlResult<TEntity>> TryFindAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues);
    /// <summary>
    /// Returns the first entity that matches the filter, using the supplied not-found error details.
    /// </summary>
    /// <param name="filter">The predicate used to filter entities.</param>
    /// <param name="notFoundErrorDetails">The error details returned when no entity matches.</param>
    /// <returns>The first matching entity or a failed result.</returns>
    MlResult<TEntity> TryFirst(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails);
    /// <summary>
    /// Asynchronously returns the first entity that matches the filter, using the supplied not-found error details.
    /// </summary>
    /// <param name="filter">The predicate used to filter entities.</param>
    /// <param name="notFoundErrorDetails">The error details returned when no entity matches.</param>
    /// <param name="token">A token that can cancel the query.</param>
    /// <returns>A task containing the first matching entity or a failed result.</returns>
    Task<MlResult<TEntity>> TryFirstAsync(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default);
    /// <summary>
    /// Returns the last entity yielded by the filtered query, using the supplied not-found error details. No explicit ordering is applied.
    /// </summary>
    /// <param name="filter">The predicate used to filter entities.</param>
    /// <param name="notFoundErrorDetails">The error details returned when no entity matches.</param>
    /// <returns>The last entity yielded by the query or a failed result.</returns>
    MlResult<TEntity> TryGetLast(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails);
    /// <summary>
    /// Asynchronously returns the last entity yielded by the filtered query, using the supplied not-found error details. No explicit ordering is applied.
    /// </summary>
    /// <param name="filter">The predicate used to filter entities.</param>
    /// <param name="notFoundErrorDetails">The error details returned when no entity matches.</param>
    /// <param name="token">A token that can cancel the query.</param>
    /// <returns>A task containing the last entity yielded by the query or a failed result.</returns>
    Task<MlResult<TEntity>> TryGetLastAsync(Expression<Func<TEntity, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default);
}
