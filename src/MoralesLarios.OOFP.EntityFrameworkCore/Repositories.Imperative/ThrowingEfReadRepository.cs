// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;


/// <summary>
/// Implementación imperativa del repositorio de solo lectura sobre Entity Framework Core.
/// Da soporte a las consultas de búsqueda y recuperación de la entidad
/// <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// Lanza excepciones ante errores en lugar de devolver <c>MlResult</c>. Para la variante
/// funcional utilice <c>EfReadRepository</c>.
/// </remarks>
/// <typeparam name="T">Tipo de la entidad consultada por el repositorio.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de Entity Framework Core utilizado.</typeparam>
internal class ThrowingEfReadRepository<T, TContext> : ThrowingEfRepository, IThrowingEfReadRepository<T>
    where T        : class
    where TContext : DbContext
{

    public ThrowingEfReadRepository(TContext dbContext) : base(dbContext)
    {

    }




    public T Find(params object[] keys) => internalDbContext.Set<T>().Find(keys)!;

    public async Task<T> FindAsync(CancellationToken token = default, params object[] keys) => (await internalDbContext.Set<T>().FindAsync(keys, token))!;

    public T FirstOrDefault(Expression<Func<T, bool>> filter) => internalDbContext.Set<T>().FirstOrDefault(filter)!;

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default) => (await internalDbContext.Set<T>().FirstOrDefaultAsync(filter, token))!;

    public T First(Expression<Func<T, bool>> filter) => internalDbContext.Set<T>().First(filter)!;

    public async Task<T> FirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default) => (await internalDbContext.Set<T>().FirstAsync(filter, token))!;

    public T LastOrDefault(Expression<Func<T, bool>> filter) => internalDbContext.Set<T>().AsNoTracking().Where(filter).AsEnumerable().LastOrDefault()!;

    public async Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default)
        => (await internalDbContext.Set<T>().AsNoTracking().Where(filter).ToListAsync(token)).LastOrDefault()!;

    public T Last(Expression<Func<T, bool>> filter) => internalDbContext.Set<T>().AsNoTracking().Where(filter).AsEnumerable().Last();

    public async Task<T> LastAsync(Expression<Func<T, bool>> filter, CancellationToken token = default)
        => (await internalDbContext.Set<T>().AsNoTracking().Where(filter).ToListAsync(token)).Last();


    public IEnumerable<T> All() => internalDbContext.Set<T>().AsNoTracking();

    public async Task<IEnumerable<T>> AllAsync(CancellationToken token = default) => await internalDbContext.Set<T>().AsNoTracking().ToListAsync(token);

    public IEnumerable<T> GetData(Expression<Func<T, bool>> filter) => internalDbContext.Set<T>().AsNoTracking().Where(filter);

    public async Task<IEnumerable<T>> GetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default)
        => await internalDbContext.Set<T>().AsNoTracking().Where(filter).ToListAsync(token);



}
