// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Application;

/// <summary>
/// Registro de los servicios CRUD funcionales en el contenedor de dependencias.
/// </summary>
/// <remarks>
/// Todos los servicios de la librería se registran con ciclo de vida <c>Scoped</c>. Es el único
/// ciclo de vida admitido: un servicio CRUD no guarda estado propio, pero delega en repositorios
/// que viven al lado del <c>DbContext</c>, y ese contexto es <c>Scoped</c>. Alinear los tres hace
/// que servicio, repositorios y contexto compartan ámbito y unidad de trabajo.
/// </remarks>
public static partial class RegisterServices
{

    /// <summary>
    /// Registra, con ciclo de vida <c>Scoped</c>, el servicio CRUD funcional de dos parámetros
    /// genéricos, sin registrar los repositorios que necesita.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>La misma colección de servicios, para permitir el encadenado.</returns>
    public static IServiceCollection AddCrudServicesFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(ICrudServiceFp<, >), typeof(CrudServiceFp<, >));

        return services;
    }

    /// <summary>
    /// Registra, con ciclo de vida <c>Scoped</c>, el servicio CRUD funcional de tres parámetros
    /// genéricos (dúplex), sin registrar los repositorios que necesita.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>La misma colección de servicios, para permitir el encadenado.</returns>
    public static IServiceCollection AddCrudServicesDuplexFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(ICrudServiceFp<,,>), typeof(CrudServiceFp<,,>));

        return services;
    }

}
