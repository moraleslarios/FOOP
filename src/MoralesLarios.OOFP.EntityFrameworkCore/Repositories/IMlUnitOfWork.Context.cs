// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>Unidad de trabajo asociada a un tipo concreto de <see cref="DbContext"/>.</summary>
/// <typeparam name="TContext">Tipo del contexto cuyas operaciones confirma.</typeparam>
public interface IMlUnitOfWork<TContext> : IMlUnitOfWork
    where TContext : DbContext
{
}