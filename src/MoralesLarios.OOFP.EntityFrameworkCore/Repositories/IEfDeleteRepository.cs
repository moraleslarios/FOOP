// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Contrato de <c>IEfDeleteRepository</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
public interface IEfDeleteRepository<T>
    where T : class
{
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<T> TryRemove(T item);
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<T> TryRemove(params object[] keyValues);
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<T> TryRemove(MlErrorDetails notFoundErrorDetails, params object[] keyValues);
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<T>> TryRemoveAsync(T item, CancellationToken token = default);
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<T>> TryRemoveAsync(CancellationToken token = default, params object[] keyValues);
    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<T>> TryRemoveAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues);
    /// <summary>
    /// Elimina el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<IEnumerable<T>> TryRemoveRange(IEnumerable<T> items);
    /// <summary>
    /// Elimina el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}
