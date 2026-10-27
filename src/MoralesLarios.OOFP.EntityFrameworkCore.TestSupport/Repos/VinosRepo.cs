// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.TestSupport.Repos;

public class VinosRepo(TestDbContext dbContext) : EfCrudRepository<Vino, TestDbContext>(dbContext), IVinosRepo
{





}

