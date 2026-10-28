// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Representa <c>EfUpdateRepository</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
public class EfUpdateRepository<T, TContext>(TContext dbContext) : EfRepository(dbContext), IEfUpdateRepository<T>
        where T        : class
        where TContext : DbContext
{

    internal readonly IThrowingEfUpdateRepository<T>?  _internalRepoUpdater = RegisterServices.GetRepository<IThrowingEfUpdateRepository <T>>();
    internal readonly IEfReadRepository<T>? _repoReaderFp        = RegisterServices.GetRepository<IEfReadRepository<T>>();





    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<T> TryUpdate(T item)
    {
        var result = EnsureFp.NotNull(item, "The entity item to update cannot be null")
                            .TryMap(x => _internalRepoUpdater!.Update(item));
        return result;
    }


    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryUpdate(T item, params object[] keyValues) => TryUpdate(item, null!, keyValues);

    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<T> TryUpdate(T item, MlErrorDetails notFoundErrorDetails, params object[] keyValues)
    {
        var result = EnsureFp.NotNull  (item, "The entity item to update cannot be null")
                                .Bind  ( _  => EnsureFp.NotEmpty(keyValues, "The object array pk cannot be empty"))
                                .Bind  ( pk => notFoundErrorDetails is null 
                                                ? _repoReaderFp!.TryFind(pk) 
                                                : _repoReaderFp!.TryFind(notFoundErrorDetails, pk))
                                .TryMap( x  => _internalRepoUpdater!.Update(item));
        return result;
    }


    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(item, "The entity item to update cannot be null")
                            .TryMapAsync(x => _internalRepoUpdater!.UpdateAsync(item, token));
        return result;
    }


    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] keyValues)
    {
        var result = await EnsureFp.NotNullAsync( item, "The entity item to update cannot be null")
                                    .BindAsync  ( _  => EnsureFp.NotEmptyAsync(keyValues, "The object array pk cannot be empty"))
                                    .BindAsync  ( _ => _repoReaderFp!.TryFindAsync(token, keyValues))
                                    .TryMapAsync( x  => _internalRepoUpdater!.UpdateAsync(item, token));
        return result;
    }

    /// <summary>
    /// Actualiza el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryUpdateAsync(T item, MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues)
    {
        var result = await EnsureFp.NotNullAsync( item, "The entity item to update cannot be null")
                                    .BindAsync  ( _  => EnsureFp.NotEmptyAsync(keyValues, "The object array pk cannot be empty"))
                                    .BindAsync  ( _ => notFoundErrorDetails is null 
                                                        ? _repoReaderFp!.TryFindAsync(token, keyValues) 
                                                        : _repoReaderFp!.TryFindAsync(notFoundErrorDetails, token, keyValues))
                                    .TryMapAsync( x  => _internalRepoUpdater!.UpdateAsync(item, token));
        return result;
    }


    /// <summary>
    /// Actualiza el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items)
    {
        var result = EnsureFp.NotNull(items, "The entity items to update cannot be null")
                            .TryMap(x => _internalRepoUpdater!.UpdateRange(items));
        return result;
    }

    /// <summary>
    /// Actualiza el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(items, "The entity items to update cannot be null")
                            .TryMapAsync(x => _internalRepoUpdater!.UpdateRangeAsync(items, token));
        return result;
    }



}


