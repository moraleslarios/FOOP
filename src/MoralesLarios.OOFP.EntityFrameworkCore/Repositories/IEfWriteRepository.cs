// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Combines entity creation, update, and removal operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IEfWriteRepository<TEntity> : IEfCreateRepository<TEntity>, IEfUpdateRepository<TEntity>, IEfDeleteRepository<TEntity>
    where TEntity : class
{
}

