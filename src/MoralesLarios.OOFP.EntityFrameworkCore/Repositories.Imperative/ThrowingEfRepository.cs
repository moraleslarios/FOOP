// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;

/// <summary>
/// Clase base de los repositorios imperativos sobre Entity Framework Core.
/// Encapsula el <see cref="DbContext"/> compartido por las operaciones concretas y gestiona su
/// liberación.
/// </summary>
/// <remarks>
/// Las clases derivadas lanzan excepciones ante errores en lugar de devolver <c>MlResult</c>.
/// Para la variante funcional utilice los repositorios equivalentes del espacio de nombres
/// <c>MoralesLarios.OOFP.EntityFrameworkCore.Repositories</c>.
/// </remarks>
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

