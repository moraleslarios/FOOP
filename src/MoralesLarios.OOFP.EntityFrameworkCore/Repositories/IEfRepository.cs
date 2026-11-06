// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Combines entity reads and writes with the unit of work associated with the same context.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IEfRepository<TEntity> : IEfReadRepository<TEntity>, IEfWriteRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Gets the unit of work associated with the <see cref="DbContext"/> used by this repository.
    /// </summary>
    IMlUnitOfWork UnitOfWork { get; }
}
