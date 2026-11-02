// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
/// <summary>
/// Clase base de los repositorios funcionales sobre Entity Framework Core.
/// </summary>
/// <remarks>
/// El repositorio <b>no es dueño</b> del <see cref="DbContext"/> que recibe: lo gestiona el
/// contenedor de dependencias, que lo libera al cerrar el ámbito. Por eso esta clase no implementa
/// <c>IDisposable</c>.
/// </remarks>
public class EfRepository(DbContext dbContext) : IGetContextable
{
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Context</c>.
    /// </summary>
    /// <returns>Valor de tipo <c>DbContext</c> obtenido por la operación.</returns>
    public DbContext GetContext() => dbContext;

}

