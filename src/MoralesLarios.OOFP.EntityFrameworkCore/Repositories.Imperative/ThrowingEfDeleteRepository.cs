// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

/// <summary>
/// Implementación imperativa del repositorio de borrados sobre Entity Framework Core.
/// Da soporte a la eliminación individual y por lotes de la entidad <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// Lanza excepciones ante errores en lugar de devolver <c>MlResult</c>. Para la variante
/// funcional utilice <c>EfDeleteRepository</c>.
/// </remarks>
/// <typeparam name="T">Tipo de la entidad eliminada por el repositorio.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de Entity Framework Core utilizado.</typeparam>
internal class ThrowingEfDeleteRepository<T, TContext> : ThrowingEfRepository, IThrowingEfDeleteRepository<T>
    where T : class
    where TContext : DbContext
{
    public ThrowingEfDeleteRepository(TContext dbContext) : base(dbContext)
    {
    }

    public T Remove(T item)
    {
        internalDbContext.Set<T>().Remove(item);

        internalDbContext.SaveChanges();

        return item;
    }

    public async Task<T> RemoveAsync(T item, CancellationToken token = default)
    {
        internalDbContext.Set<T>().Remove(item);

        await internalDbContext.SaveChangesAsync(token);

        return item;
    }

    public IEnumerable<T> RemoveRange(IEnumerable<T> items)
    {
        internalDbContext.Set<T>().RemoveRange(items);

        internalDbContext.SaveChanges();

        return items;
    }

    public async Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        internalDbContext.Set<T>().RemoveRange(items);

        await internalDbContext.SaveChangesAsync(token);

        return items;
    }
}



