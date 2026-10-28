// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

/// <summary>
/// Contrato imperativo de repositorio paginado de solo lectura sobre Entity Framework Core.
/// Expone las consultas que devuelven páginas de la entidad <typeparamref name="T"/>, con
/// filtrado y ordenación opcionales.
/// </summary>
/// <remarks>
/// Las implementaciones lanzan excepciones ante errores en lugar de devolver <c>MlResult</c>.
/// La ordenación se aplica siempre antes de la paginación, de modo que el resultado sea
/// estable entre llamadas consecutivas.
/// </remarks>
/// <typeparam name="T">Tipo de la entidad consultada por el repositorio.</typeparam>
internal interface IThrowingEfPagedReadRepository<T>
        where T : class
{
    IEnumerable<T> GetData(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null!);
    Task<IEnumerable<T>> GetDataAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null!, CancellationToken token = default);
    IEnumerable<T> GetDataOrderby<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> filter = null!);
    Task<IEnumerable<T>> GetDataOrderbyAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> filter = null!, CancellationToken token = default);
    IEnumerable<T> GetDataOrderbyDescending<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderByDescending, Expression<Func<T, bool>> filter = null!);
    Task<IEnumerable<T>> GetDataOrderbyDescendingAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderByDescending, Expression<Func<T, bool>> filter = null!, CancellationToken token = default);
}

