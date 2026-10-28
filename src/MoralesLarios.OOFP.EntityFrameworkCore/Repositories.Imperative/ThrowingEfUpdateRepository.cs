// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

/// <summary>
/// Implementación imperativa del repositorio de modificaciones sobre Entity Framework Core.
/// Da soporte a la actualización individual y por lotes de la entidad <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// Lanza excepciones ante errores en lugar de devolver <c>MlResult</c>. Para la variante
/// funcional utilice <c>EfUpdateRepository</c>.
/// </remarks>
/// <typeparam name="T">Tipo de la entidad actualizada por el repositorio.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de Entity Framework Core utilizado.</typeparam>
internal class ThrowingEfUpdateRepository<T, TContext> : ThrowingEfRepository, IThrowingEfUpdateRepository<T>
    where T : class
    where TContext : DbContext
{
    public ThrowingEfUpdateRepository(TContext dbContext) : base(dbContext)
    {
    }

    public T Update(T item)
    {
        internalDbContext.Update(item);

        internalDbContext.SaveChanges();

        return item;
    }

    public async Task<T> UpdateAsync(T item, CancellationToken token = default)
    {
        internalDbContext.Update(item);

        await internalDbContext.SaveChangesAsync(token);

        return item;
    }

    public IEnumerable<T> UpdateRange(IEnumerable<T> items)
    {
        internalDbContext.UpdateRange(items);

        internalDbContext.SaveChanges();

        return items;
    }

    public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        internalDbContext.UpdateRange(items);

        await internalDbContext.SaveChangesAsync(token);

        return items;
    }
}

