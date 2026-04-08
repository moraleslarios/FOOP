// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;

namespace MoralesLarios.OOFP.WebServices.Tests.Unit.FakeData;
public class FakeDbContext : DbContext
{

    public DbSet<MyTable> MyTables { get; set; } = null!;


}

