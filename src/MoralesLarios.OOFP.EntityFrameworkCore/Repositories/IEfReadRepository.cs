// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Contrato de <c>IEfReadRepository</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
public interface IEfReadRepository<T> where T : class
{
    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<T> TryFind(params object[] keyValues);
    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] keyValues);
    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<T> TryFirst(Expression<Func<T, bool>> filter);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<T> TryGetLast(Expression<Func<T, bool>> filter);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Data</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Data</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    /// <summary>
    /// Indica si todos los elementos cumplen la condición. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<IEnumerable<T>> TryAll();
    /// <summary>
    /// Indica si todos los elementos cumplen la condición. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default);
    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<T> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues);
    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<T>> TryFindAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues);
    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<T> TryFirst(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails);
    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<T> TryGetLast(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default);
}
