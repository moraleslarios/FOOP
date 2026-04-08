// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.EFCore.OopRepos;

namespace MoralesLarios.OOFP.EFCore.Repos;

public interface IEFRepoPaginationFp<T> : IEFRepoFp<T>, IEFPaginatorFp<T>
    where T : class
{
}
