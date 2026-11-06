// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Commits or discards changes tracked by a repository's <see cref="DbContext"/> and runs
/// operations within a transaction.
/// </summary>
/// <remarks>
/// Direct repository writes only prepare changes. They are not persisted until <see cref="TryCommit"/>
/// or <see cref="TryCommitAsync(CancellationToken)"/> is called. Use
/// <see cref="TryExecuteInTransactionAsync{TResult}"/> to commit multiple writes atomically.
/// </remarks>
public interface IMlUnitOfWork
{
    /// <summary>Gets whether the <c>ChangeTracker</c> contains pending changes.</summary>
    bool HasPendingChanges { get; }

    /// <summary>Commits pending changes and returns the number of affected rows.</summary>
    /// <returns>The number of affected rows, or a failed result if the commit fails.</returns>
    MlResult<int> TryCommit();

    /// <summary>Asynchronously commits pending changes and returns the number of affected rows.</summary>
    /// <param name="token">A token that can cancel the commit.</param>
    /// <returns>A task containing the number of affected rows, or a failed result.</returns>
    Task<MlResult<int>> TryCommitAsync(CancellationToken token = default);

    /// <summary>Discards tracked changes and returns the number of changed entries discarded.</summary>
    /// <returns>The number of discarded entries, or a failed result.</returns>
    MlResult<int> DiscardChanges();

    /// <summary>
    /// Runs operations within a transaction. Commits when they return a successful result; rolls back
    /// when they return a failed result or throw an exception.
    /// </summary>
    /// <typeparam name="TResult">The result type produced by the operations.</typeparam>
    /// <param name="operations">The operations that prepare changes in the context.</param>
    /// <returns>The operation result, or a failed result if execution or commit fails.</returns>
    MlResult<TResult> TryExecuteInTransaction<TResult>(Func<MlResult<TResult>> operations);

    /// <summary>
    /// Runs operations within a transaction. Commits when they return a successful result; rolls back
    /// when they return a failed result, throw an exception, or are canceled.
    /// </summary>
    /// <typeparam name="TResult">The result type produced by the operations.</typeparam>
    /// <param name="operations">The operations that prepare changes in the context.</param>
    /// <param name="token">A token that can cancel the transaction.</param>
    /// <returns>The operation result, or a failed result if execution or commit fails.</returns>
    Task<MlResult<TResult>> TryExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<MlResult<TResult>>> operations,
        CancellationToken token = default);
}