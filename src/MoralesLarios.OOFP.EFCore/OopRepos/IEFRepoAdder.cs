// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.EFCore.OopRepos;

public interface IEFRepoAdder<T> where T : class
{
    T Add(T item);
    Task<T> AddAsync(T item, CancellationToken token = default);
    IEnumerable<T> AddRange(IEnumerable<T> items);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}
