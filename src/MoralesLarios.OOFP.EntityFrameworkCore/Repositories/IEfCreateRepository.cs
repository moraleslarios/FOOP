// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

public interface IEfCreateRepository<T>
    where T : class
{
    MlResult<T> TryAdd(T item);
    Task<MlResult<T>> TryAddAsync(T item, CancellationToken token = default);
    MlResult<IEnumerable<T>> TryAddRange(IEnumerable<T> items);
    Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}
