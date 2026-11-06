// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
/// <summary>
/// Base class for functional repositories that use Entity Framework Core.
/// </summary>
/// <remarks>
/// The repository does not own the <see cref="DbContext"/> it receives. The dependency-injection
/// container manages and disposes the context when its scope ends, so this class does not implement
/// <see cref="IDisposable"/>.
/// </remarks>
public class EfRepository(DbContext dbContext) : IGetContextable
{
    /// <inheritdoc/>
    public DbContext GetContext() => dbContext;

}

