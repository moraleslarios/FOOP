// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Combines entity reads with paged read operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IEfPagedReadRepository<TEntity> : IEfReadRepository<TEntity> , IEfPaginator<TEntity>
    where TEntity : class
{









}
