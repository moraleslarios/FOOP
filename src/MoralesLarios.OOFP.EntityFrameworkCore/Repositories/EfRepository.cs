// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
/// <summary>
/// Representa <c>EfRepository</c>.
/// </summary>
public class EfRepository(DbContext dbContext) : IGetContextable , IDisposable
{
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Context</c>.
    /// </summary>
    /// <returns>Valor de tipo <c>DbContext</c> obtenido por la operación.</returns>
    public DbContext GetContext() => dbContext;


    /// <summary>
    /// Ejecuta <c>Dispose</c>.
    /// </summary>
    public virtual void Dispose()
    {
        GetContext()?.Dispose();
    }


}

