// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.EntityFrameworkCore;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

internal class ThrowingEfPagedRepository<T, TContext> : ThrowingEfCrudRepository<T, TContext>, IThrowingEfPagedRepository<T>
    where T : class
    where TContext : DbContext
{


    private readonly IThrowingEfPagedReadRepository<T>? _repoReaderPagination;

    public ThrowingEfPagedRepository(TContext dbContext) : base(dbContext)
    {
        _repoReaderPagination = RegisterServices.ServiceProvider.GetService<IThrowingEfPagedReadRepository<T>>();
    }

    public IEnumerable<T> GetData(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null!)
        => _repoReaderPagination!.GetData(pageNumber, pageSize, filter);

    public Task<IEnumerable<T>> GetDataAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null!, CancellationToken token = default)
        => _repoReaderPagination!.GetDataAsync(pageNumber, pageSize, filter, token);

    public IEnumerable<T> GetDataOrderby<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> filter = null!)
        => _repoReaderPagination!.GetDataOrderby(pageNumber, pageSize, orderBy, filter);

    public Task<IEnumerable<T>> GetDataOrderbyAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> filter = null!, CancellationToken token = default)
        => _repoReaderPagination!.GetDataOrderbyAsync(pageNumber, pageSize, orderBy, filter, token);

    public IEnumerable<T> GetDataOrderbyDescending<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderByDescending, Expression<Func<T, bool>> filter = null!)
        => _repoReaderPagination!.GetDataOrderbyDescending(pageNumber, pageSize, orderByDescending, filter);

    public Task<IEnumerable<T>> GetDataOrderbyDescendingAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderByDescending, Expression<Func<T, bool>> filter = null!, CancellationToken token = default)
        => _repoReaderPagination!.GetDataOrderbyDescendingAsync(pageNumber, pageSize, orderByDescending, filter, token);
}

