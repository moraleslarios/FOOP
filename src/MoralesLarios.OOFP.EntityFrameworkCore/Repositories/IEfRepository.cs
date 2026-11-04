// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Contrato de <c>IEfRepository</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
public interface IEfRepository<T> : IEfReadRepository<T>, IEfWriteRepository<T>
    where T : class
{
    /// <summary>
    /// Obtiene la unidad de trabajo asociada al mismo <see cref="DbContext"/> que usa este repositorio.
    /// </summary>
    IMlUnitOfWork UnitOfWork { get; }
}
