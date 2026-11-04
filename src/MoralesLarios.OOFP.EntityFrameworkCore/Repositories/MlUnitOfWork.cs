// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoralesLarios.OOFP.EntityFrameworkCore;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

internal sealed class MlUnitOfWork<TContext> : IMlUnitOfWork<TContext>, IDisposable
    where TContext : DbContext
{
    private readonly TContext _context;
    private readonly ILogger<MlUnitOfWork<TContext>>? _logger;
    private readonly bool _warnOnUncommittedChanges;
    private bool _disposed;

    internal MlUnitOfWork(TContext context)
    {
        _context = context;
    }

    internal MlUnitOfWork(TContext context,
                          ILogger<MlUnitOfWork<TContext>> logger,
                          IOptions<MlUnitOfWorkDiagnostics> diagnostics)
    {
        _context = context;
        _logger = logger;
        _warnOnUncommittedChanges = diagnostics.Value.WarnOnUncommittedChanges;
    }

    public bool HasPendingChanges => _context.ChangeTracker.HasChanges();

    public MlResult<int> TryCommit()
    {
        try
        {
            return _context.SaveChanges().ToMlResultValid();
        }
        catch (Exception exception)
        {
            return BuildCommitFailure<int>(exception);
        }
    }

    public async Task<MlResult<int>> TryCommitAsync(CancellationToken token = default)
    {
        try
        {
            return (await _context.SaveChangesAsync(token)).ToMlResultValid();
        }
        catch (Exception exception)
        {
            return BuildCommitFailure<int>(exception);
        }
    }

    public MlResult<int> DiscardChanges()
    {
        try
        {
            var count = _context.ChangeTracker.Entries()
                                .Count(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);
            _context.ChangeTracker.Clear();
            return count.ToMlResultValid();
        }
        catch (Exception exception)
        {
            return MlErrorDetails.FromErrorMessageWithException(
                "No se pudieron descartar los cambios pendientes de la unidad de trabajo.", exception)
                .ToMlResultFail<int>();
        }
    }

    public MlResult<TResult> TryExecuteInTransaction<TResult>(Func<MlResult<TResult>> operations)
    {
        if (operations is null)
            return "El delegado de la transacción no puede ser nulo.".ToMlResultFail<TResult>();

        var existingTransaction = _context.Database.CurrentTransaction;
        var ownsTransaction = existingTransaction is null;
        IDbContextTransaction? transaction = existingTransaction;

        try
        {
            transaction ??= ownsTransaction ? _context.Database.BeginTransaction() : null;
            var result = operations();

            if (result.IsFailure)
            {
                if (ownsTransaction)
                {
                    transaction!.Rollback();
                    _context.ChangeTracker.Clear();
                }

                return result;
            }

            var commit = TryCommit();
            if (commit.IsFailure)
            {
                if (ownsTransaction)
                {
                    SafeRollback(transaction!);
                    _context.ChangeTracker.Clear();
                }

                return commit.GetFailureDetailsOrThrow().ToMlResultFail<TResult>();
            }

            if (ownsTransaction)
                transaction!.Commit();

            return result;
        }
        catch (Exception exception)
        {
            if (ownsTransaction && transaction is not null)
            {
                SafeRollback(transaction);
                _context.ChangeTracker.Clear();
            }

            return MlErrorDetails.FromErrorMessageWithException(
                "No se pudo ejecutar o confirmar la transacción.", exception)
                .ToMlResultFail<TResult>();
        }
        finally
        {
            if (ownsTransaction)
                transaction?.Dispose();
        }
    }

    public async Task<MlResult<TResult>> TryExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<MlResult<TResult>>> operations,
        CancellationToken token = default)
    {
        if (operations is null)
            return "El delegado de la transacción no puede ser nulo.".ToMlResultFail<TResult>();

        var existingTransaction = _context.Database.CurrentTransaction;
        var ownsTransaction = existingTransaction is null;
        IDbContextTransaction? transaction = existingTransaction;

        try
        {
            transaction ??= ownsTransaction
                ? await _context.Database.BeginTransactionAsync(token)
                : null;
            var result = await operations(token);

            if (result.IsFailure)
            {
                if (ownsTransaction)
                {
                    await transaction!.RollbackAsync(CancellationToken.None);
                    _context.ChangeTracker.Clear();
                }

                return result;
            }

            var commit = await TryCommitAsync(token);
            if (commit.IsFailure)
            {
                if (ownsTransaction)
                {
                    await SafeRollbackAsync(transaction!);
                    _context.ChangeTracker.Clear();
                }

                return commit.GetFailureDetailsOrThrow().ToMlResultFail<TResult>();
            }

            if (ownsTransaction)
                await transaction!.CommitAsync(token);

            return result;
        }
        catch (Exception exception)
        {
            if (ownsTransaction && transaction is not null)
            {
                await SafeRollbackAsync(transaction);
                _context.ChangeTracker.Clear();
            }

            return MlErrorDetails.FromErrorMessageWithException(
                "No se pudo ejecutar o confirmar la transacción.", exception)
                .ToMlResultFail<TResult>();
        }
        finally
        {
            if (ownsTransaction && transaction is not null)
                await transaction.DisposeAsync();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        if (_warnOnUncommittedChanges && HasPendingChanges)
            _logger?.LogWarning("La unidad de trabajo de {ContextType} se cerró con cambios sin confirmar.",
                                typeof(TContext).Name);
    }

    private static MlResult<T> BuildCommitFailure<T>(Exception exception)
    {
        var message = exception switch
        {
            DbUpdateConcurrencyException => "Conflicto de concurrencia al confirmar la unidad de trabajo.",
            DbUpdateException => $"Error al confirmar los cambios en la base de datos. {exception.InnerException?.Message ?? exception.Message}",
            _ => $"No se pudieron confirmar los cambios de la unidad de trabajo. {exception.Message}"
        };

        return MlErrorDetails.FromErrorMessageWithException(message, exception).ToMlResultFail<T>();
    }

    private void SafeRollback(IDbContextTransaction transaction)
    {
        try
        {
            transaction.Rollback();
        }
        catch (Exception exception)
        {
            _logger?.LogWarning(exception, "Falló el rollback de la transacción de {ContextType}.",
                                typeof(TContext).Name);
        }
    }

    private async Task SafeRollbackAsync(IDbContextTransaction transaction)
    {
        try
        {
            await transaction.RollbackAsync(CancellationToken.None);
        }
        catch (Exception exception)
        {
            _logger?.LogWarning(exception, "Falló el rollback de la transacción de {ContextType}.",
                                typeof(TContext).Name);
        }
    }
}