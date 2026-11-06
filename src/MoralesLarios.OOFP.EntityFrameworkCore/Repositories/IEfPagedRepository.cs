// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Combines entity reads and writes with paged read operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IEfPagedRepository<TEntity> : IEfRepository<TEntity>, IEfPaginator<TEntity>
    where TEntity : class
{
}
