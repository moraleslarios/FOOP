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
    /// Añade el elemento indicado de <c>OOFPIO</c>.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddOOFPIO(this IServiceCollection services)
    {
        services.AddSingleton<IMlFileSystem, MlFileSystem>();

        return services;
    }



}

