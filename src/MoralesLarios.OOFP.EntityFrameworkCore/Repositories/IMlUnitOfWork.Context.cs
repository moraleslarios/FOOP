// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>A unit of work associated with a specific <see cref="DbContext"/> type.</summary>
/// <typeparam name="TContext">The context type whose changes are committed.</typeparam>
public interface IMlUnitOfWork<TContext> : IMlUnitOfWork
    where TContext : DbContext
{
}