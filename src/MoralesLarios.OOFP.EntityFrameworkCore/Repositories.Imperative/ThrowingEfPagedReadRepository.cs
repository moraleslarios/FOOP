// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

/// <summary>
/// Implementación imperativa del repositorio paginado de solo lectura sobre Entity Framework
/// Core. Devuelve páginas de la entidad <typeparamref name="T"/> con filtrado y ordenación
/// opcionales.
/// </summary>
/// <remarks>
/// Lanza excepciones ante errores en lugar de devolver <c>MlResult</c>. La ordenación se aplica
/// siempre antes de <c>Skip</c>/<c>Take</c>, de modo que el resultado sea estable entre llamadas
/// consecutivas.
/// </remarks>
/// <typeparam name="T">Tipo de la entidad consultada por el repositorio.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de Entity Framework Core utilizado.</typeparam>
internal class ThrowingEfPagedReadRepository<T, TContext> : ThrowingEfRepository, IThrowingEfPagedReadRepository<T>
    where T : class
    where TContext : DbContext
{
    public ThrowingEfPagedReadRepository(TContext dbContext) : base(dbContext)
    {
    }

    public IEnumerable<T> GetData(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null!) => GetInternalData(pageNumber, pageSize, filter).ToList();

    public async Task<IEnumerable<T>> GetDataAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null!, CancellationToken token = default)
        => await GetInternalData(pageNumber, pageSize, filter).ToListAsync(token);

    public IEnumerable<T> GetDataOrderby<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> filter = null!)
        => GetInternalData(pageNumber, pageSize, filter, source => source.OrderBy(orderBy)).ToList();

    public async Task<IEnumerable<T>> GetDataOrderbyAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> filter = null!, CancellationToken token = default)
        => await GetInternalData(pageNumber, pageSize, filter, source => source.OrderBy(orderBy)).ToListAsync(token);

    public IEnumerable<T> GetDataOrderbyDescending<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderByDescending, Expression<Func<T, bool>> filter = null!)
        => GetInternalData(pageNumber, pageSize, filter, source => source.OrderByDescending(orderByDescending)).ToList();


    public async Task<IEnumerable<T>> GetDataOrderbyDescendingAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderByDescending, Expression<Func<T, bool>> filter = null!, CancellationToken token = default)
        => await GetInternalData(pageNumber, pageSize, filter, source => source.OrderByDescending(orderByDescending)).ToListAsync(token);

    private IQueryable<T> GetInternalData(int                                       pageNumber, 
                                          int                                       pageSize, 
                                          Expression<Func<T, bool>>                 filter  = null!,
                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderer = null!)
    {
        var internalFilter = filter ?? (x => true);

        var from = (pageNumber - 1) * pageSize;

        IQueryable<T> source = internalDbContext.Set<T>()
                                    .AsNoTracking()
                                    .Where(internalFilter);

        // Punto 37: la ordenacion debe aplicarse ANTES de Skip/Take. Si se aplica despues,
        // se ordena unicamente la pagina ya recortada y el resultado es no determinista.
        if (orderer is not null) source = orderer(source);

        var result = source.Skip(from)
                           .Take(pageSize);

        return result;
    }
}
