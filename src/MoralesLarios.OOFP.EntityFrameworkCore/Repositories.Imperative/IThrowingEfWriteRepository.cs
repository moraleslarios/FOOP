// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

/// <summary>
/// Contrato imperativo de repositorio de solo escritura sobre Entity Framework Core.
/// Expone las operaciones de alta, modificación y borrado de la entidad
/// <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// Las implementaciones lanzan excepciones ante errores en lugar de devolver <c>MlResult</c>.
/// Para la variante funcional utilice los repositorios equivalentes del espacio de nombres
/// <c>MoralesLarios.OOFP.EntityFrameworkCore.Repositories</c>.
/// </remarks>
/// <typeparam name="T">Tipo de la entidad persistida por el repositorio.</typeparam>
internal interface IThrowingEfWriteRepository<T> where T : class
{
    T Add(T item);
    Task<T> AddAsync(T item, CancellationToken token = default);
    IEnumerable<T> AddRange(IEnumerable<T> items);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
    T Remove(T item);
    Task<T> RemoveAsync(T item, CancellationToken token = default);
    IEnumerable<T> RemoveRange(IEnumerable<T> items);
    Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default);
    T Update(T item);
    Task<T> UpdateAsync(T item, CancellationToken token = default);
    IEnumerable<T> UpdateRange(IEnumerable<T> items);
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}
