// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EFCore.OopRepos;

internal interface IEFRepoReaderPagination<T>
        where T : class
{
    IEnumerable<T> GetData(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null!);
    Task<IEnumerable<T>> GetDataAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null!, CancellationToken token = default);
    IEnumerable<T> GetDataOrderby<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> filter = null!);
    Task<IEnumerable<T>> GetDataOrderbyAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> filter = null!, CancellationToken token = default);
    IEnumerable<T> GetDataOrderbyDescending<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderByDescending, Expression<Func<T, bool>> filter = null!);
    Task<IEnumerable<T>> GetDataOrderbyDescendingAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderByDescending, Expression<Func<T, bool>> filter = null!, CancellationToken token = default);
}

