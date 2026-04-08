// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Repos;

public class VinosCatasRepo(JfCatasDbContext dbContext) : EFRepoFp<VinosCata, JfCatasDbContext>(dbContext), IVinosCatasRepo
{
}
