// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

/// <summary>
/// Implementación imperativa del repositorio de altas sobre Entity Framework Core.
/// Da soporte a la inserción individual y por lotes de la entidad <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// Lanza excepciones ante errores en lugar de devolver <c>MlResult</c>. Para la variante
/// funcional utilice <c>EfCreateRepository</c>.
/// </remarks>
/// <typeparam name="T">Tipo de la entidad persistida por el repositorio.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de Entity Framework Core utilizado.</typeparam>
internal class ThrowingEfCreateRepository<T, TContext> : ThrowingEfRepository, IThrowingEfCreateRepository<T>
    where T : class
    where TContext : DbContext
{
    public ThrowingEfCreateRepository(TContext dbContext) : base(dbContext)
    {
    }

    public T Add(T item)
    {
        internalDbContext.Set<T>().Add(item);

        internalDbContext.SaveChanges();

        return item;
    }

    public async Task<T> AddAsync(T item, CancellationToken token = default)
    {
        await internalDbContext.Set<T>().AddAsync(item, token);

        await internalDbContext.SaveChangesAsync(token);

        return item;
    }

    public IEnumerable<T> AddRange(IEnumerable<T> items)
    {
        internalDbContext.Set<T>().AddRange(items);

        internalDbContext.SaveChanges();

        return items;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        await internalDbContext.Set<T>().AddRangeAsync(items, token);

        await internalDbContext.SaveChangesAsync(token);

        return items;
    }
}


