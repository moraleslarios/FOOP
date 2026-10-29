// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Contrato de <c>IEfWriteRepository</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
public interface IEfWriteRepository<T> : IEfCreateRepository<T>, IEfUpdateRepository<T>, IEfDeleteRepository<T>
    where T : class
{
}

