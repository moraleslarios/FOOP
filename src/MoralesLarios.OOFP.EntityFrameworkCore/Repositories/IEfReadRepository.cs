// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

public interface IEfReadRepository<T> where T : class
{
    MlResult<T> TryFind(params object[] keyValues);
    Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] keyValues);
    Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    MlResult<T> TryFirst(Expression<Func<T, bool>> filter);
    Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    MlResult<T> TryGetLast(Expression<Func<T, bool>> filter);
    MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter);
    Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    MlResult<IEnumerable<T>> TryAll();
    Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default);
    MlResult<T> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues);
    Task<MlResult<T>> TryFindAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues);
    MlResult<T> TryFirst(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails);
    Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default);
    MlResult<T> TryGetLast(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails);
    Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default);
}
