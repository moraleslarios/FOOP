// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching;

/// <summary>
/// Representa <c>RegisterServices</c>.
/// </summary>
public static partial class RegisterServices
{
    /// <summary>
    /// Añade el elemento indicado de <c>MlControllersOutputCaching</c>.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <param name="options">Opciones que modifican el comportamiento de la operación.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    public static IServiceCollection AddMlControllersOutputCaching(this IServiceCollection services, Action<OutputCacheOptions> options = null!)
    {
        services.AddOutputCache(outputCacheOptions =>
        {
            options?.Invoke(outputCacheOptions);

            //outputCacheOptions.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);

            outputCacheOptions.AddPolicy("PerControllerTag", Policies.ControllerScopedOutputCachePolicy.Instance);
        });

        return services;
    }





}

