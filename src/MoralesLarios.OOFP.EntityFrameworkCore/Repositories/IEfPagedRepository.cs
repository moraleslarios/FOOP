// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

public interface IEfPagedRepository<T> : IEfRepository<T>, IEfPaginator<T>
    where T : class
{
}
