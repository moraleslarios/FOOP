// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EFCore.Repos;
public class EFRepoBaseFp(DbContext dbContext) : IGetContextable , IDisposable
{
    public DbContext GetContext() => dbContext;


    public virtual void Dispose()
    {
        GetContext()?.Dispose();
    }


}

