// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

internal class ThrowingEfRepository : IDisposable, IGetContextable
{
    protected internal DbContext internalDbContext { get; set; }


    public ThrowingEfRepository(DbContext dbContext)
    {
        internalDbContext = dbContext;
    }






    public virtual void Dispose()
    {
        internalDbContext?.Dispose();
    }

    public DbContext GetContext() => internalDbContext;
}

