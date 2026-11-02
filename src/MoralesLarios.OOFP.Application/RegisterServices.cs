// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Application;

/// <summary>
/// Registro de los servicios CRUD funcionales en el contenedor de dependencias.
/// </summary>
/// <remarks>
/// Todos los servicios de la librería se registran con ciclo de vida <c>Transient</c>. Es el único
/// ciclo de vida admitido: un servicio CRUD no guarda estado propio y delega en repositorios, así
/// que crear una instancia nueva por resolución es la opción correcta y además la única compatible
/// con un <c>DbContext</c> registrado como <c>Scoped</c>.
/// </remarks>
public static partial class RegisterServices
{

    /// <summary>
    /// Registra, con ciclo de vida <c>Transient</c>, el servicio CRUD funcional de dos parámetros
    /// genéricos, sin registrar los repositorios que necesita.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>La misma colección de servicios, para permitir el encadenado.</returns>
    public static IServiceCollection AddCrudServicesFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(ICrudServiceFp<, >), typeof(CrudServiceFp<, >));

        return services;
    }

    /// <summary>
    /// Registra, con ciclo de vida <c>Transient</c>, el servicio CRUD funcional de tres parámetros
    /// genéricos (dúplex), sin registrar los repositorios que necesita.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>La misma colección de servicios, para permitir el encadenado.</returns>
    public static IServiceCollection AddCrudServicesDuplexFpWithoutRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(ICrudServiceFp<,,>), typeof(CrudServiceFp<,,>));

        return services;
    }

}
