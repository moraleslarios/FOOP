// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

public interface IEfUpdateRepository<T>
    where T : class
{
    MlResult<T> TryUpdate(T item);
    MlResult<T> TryUpdate(T item, params object[] keyValues);
    MlResult<T> TryUpdate(T item, MlErrorDetails notFoundErrorDetails, params object[] keyValues);
    Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default);
    Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] keyValues);
    Task<MlResult<T>> TryUpdateAsync(T item, MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues);
    MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items);
    Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}
