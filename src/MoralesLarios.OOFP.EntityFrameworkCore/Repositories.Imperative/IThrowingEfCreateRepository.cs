// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

/// <summary>
/// Contrato de <c>IThrowingEfCreateRepository</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
public interface IThrowingEfCreateRepository<T> where T : class
{
    /// <summary>
    /// Añade el elemento indicado.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>T</c> obtenido por la operación.</returns>
    T Add(T item);
    /// <summary>
    /// Añade el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>T</c> obtenido por la operación.</returns>
    Task<T> AddAsync(T item, CancellationToken token = default);
    /// <summary>
    /// Añade el elemento indicado de <c>Range</c>.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    IEnumerable<T> AddRange(IEnumerable<T> items);
    /// <summary>
    /// Añade el elemento indicado de <c>Range</c>. Versión asíncrona.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el colección con los elementos obtenidos.</returns>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}
