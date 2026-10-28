// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

/// <summary>
/// Implementación imperativa del repositorio de solo escritura sobre Entity Framework Core.
/// Agrupa las operaciones de alta, modificación y borrado de la entidad
/// <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// Lanza excepciones ante errores en lugar de devolver <c>MlResult</c>. Para la variante
/// funcional utilice <c>EfWriteRepository</c>.
/// </remarks>
/// <typeparam name="T">Tipo de la entidad persistida por el repositorio.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de Entity Framework Core utilizado.</typeparam>
internal class ThrowingEfWriteRepository<T, TContext> : ThrowingEfRepository, IThrowingEfWriteRepository<T>
    where T        : class
    where TContext : DbContext
{
    private readonly IThrowingEfCreateRepository<T>?   _repoAdder;
    private readonly IThrowingEfDeleteRepository<T>? _repoDeleter;
    private readonly IThrowingEfUpdateRepository<T>? _repoUpdater;

    public ThrowingEfWriteRepository(TContext dbContext) : base(dbContext)
    {
        _repoAdder   = RegisterServices.ServiceProvider.GetService<IThrowingEfCreateRepository  <T>>();
        _repoDeleter = RegisterServices.ServiceProvider.GetService<IThrowingEfDeleteRepository<T>>();
        _repoUpdater = RegisterServices.ServiceProvider.GetService<IThrowingEfUpdateRepository<T>>();
    }

    public T Add(T item) => _repoAdder!.Add(item);
    public async Task<T> AddAsync(T item, CancellationToken token = default) => await _repoAdder!.AddAsync(item, token);

    public IEnumerable<T> AddRange(IEnumerable<T> items) => _repoAdder!.AddRange(items);

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoAdder!.AddRangeAsync(items, token);

    public T Remove(T item) => _repoDeleter!.Remove(item);

    public async Task<T> RemoveAsync(T item, CancellationToken token = default) => await _repoDeleter!.RemoveAsync(item, token);

    public IEnumerable<T> RemoveRange(IEnumerable<T> items) => _repoDeleter!.RemoveRange(items);

    public async Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoDeleter!.RemoveRangeAsync(items, token);

    public T Update(T item) => _repoUpdater!.Update(item);

    public async Task<T> UpdateAsync(T item, CancellationToken token = default) => await _repoUpdater!.UpdateAsync(item, token);

    public IEnumerable<T> UpdateRange(IEnumerable<T> items) => _repoUpdater!.UpdateRange(items);

    public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoUpdater!.UpdateRangeAsync(items, token);
}

