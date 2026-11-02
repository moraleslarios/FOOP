// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;

namespace MoralesLarios.OOFP.FileSystem;

/// <summary>
/// Representa <c>RegisterServices</c>.
/// </summary>
public static class RegisterServices
{

    /// <summary>
    /// Registra, con ciclo de vida <c>Transient</c>, el servicio de acceso al sistema de archivos.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>La misma colección de servicios, para permitir el encadenado.</returns>
    public static IServiceCollection AddOOFPIO(this IServiceCollection services)
    {
        services.AddTransient<IMlFileSystem, MlFileSystem>();

        return services;
    }



}

