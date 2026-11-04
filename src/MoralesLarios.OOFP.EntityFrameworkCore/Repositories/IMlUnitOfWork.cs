// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Confirma o descarta los cambios rastreados por el <c>DbContext</c> de un repositorio y ejecuta
/// operaciones dentro de una transacción.
/// </summary>
/// <remarks>
/// Las escrituras directas de los repositorios solo preparan cambios y no persisten hasta llamar a
/// <see cref="TryCommit"/> o <see cref="TryCommitAsync(CancellationToken)"/>. Para agrupar varias
/// escrituras de forma atómica, usa <see cref="TryExecuteInTransactionAsync{TResult}"/>.
/// </remarks>
public interface IMlUnitOfWork
{
    /// <summary>Indica si el <c>ChangeTracker</c> contiene cambios pendientes.</summary>
    bool HasPendingChanges { get; }

    /// <summary>Confirma los cambios pendientes y devuelve el número de filas afectadas.</summary>
    MlResult<int> TryCommit();

    /// <summary>Confirma los cambios pendientes y devuelve el número de filas afectadas.</summary>
    Task<MlResult<int>> TryCommitAsync(CancellationToken token = default);

    /// <summary>Descarta los cambios rastreados y devuelve cuántas entradas modificadas descartó.</summary>
    MlResult<int> DiscardChanges();

    /// <summary>
    /// Ejecuta operaciones dentro de una transacción. Confirma los cambios si el resultado es válido;
    /// ante un resultado fallido o una excepción revierte la transacción.
    /// </summary>
    /// <typeparam name="TResult">Tipo del valor producido por las operaciones.</typeparam>
    /// <param name="operations">Operaciones que preparan los cambios del contexto.</param>
    /// <returns>Resultado de las operaciones o fallo de ejecución/confirmación.</returns>
    MlResult<TResult> TryExecuteInTransaction<TResult>(Func<MlResult<TResult>> operations);

    /// <summary>
    /// Ejecuta operaciones dentro de una transacción. Confirma los cambios si el resultado es válido;
    /// ante un resultado fallido, una excepción o cancelación revierte la transacción.
    /// </summary>
    /// <typeparam name="TResult">Tipo del valor producido por las operaciones.</typeparam>
    /// <param name="operations">Operaciones que preparan los cambios del contexto.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Resultado de las operaciones o fallo de ejecución/confirmación.</returns>
    Task<MlResult<TResult>> TryExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<MlResult<TResult>>> operations,
        CancellationToken token = default);
}