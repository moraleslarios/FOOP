// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Policies;

/// <summary>
/// Atributo que aplica la <see cref="ControllerScopedOutputCachePolicy"/> y permite
/// indicar opcionalmente la duraci�n del cache. Si no se especifica duraci�n se usa
/// la configurada por defecto en <c>AddOutputCache</c> mediante
/// <see cref="Microsoft.AspNetCore.OutputCaching.OutputCacheOptions.DefaultExpirationTimeSpan"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class MlControllerCacheAttribute : Attribute, IOutputCachePolicy
{
    private readonly TimeSpan? _expiration;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="MlControllerCacheAttribute"/>.
    /// </summary>
    public MlControllerCacheAttribute() { }

    /// <param name="durationSeconds">Tiempo en segundos. Si es menor o igual a 0 se ignora y se usa el por defecto.</param>
    public MlControllerCacheAttribute(int durationSeconds)
    {
        if (durationSeconds > 0)
            _expiration = TimeSpan.FromSeconds(durationSeconds);
    }

    /// <summary>
    /// Ejecuta <c>CacheRequest</c>. Versión asíncrona.
    /// </summary>
    /// <param name="context">Contexto de ejecucion.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var inner = ControllerScopedOutputCachePolicy.Instance.CacheRequestAsync(context, cancellationToken);

        if (_expiration is { } exp && context.EnableOutputCaching)
            context.ResponseExpirationTimeSpan = exp;

        return inner;
    }

    /// <summary>
    /// Ejecuta <c>ServeFromCache</c>. Versión asíncrona.
    /// </summary>
    /// <param name="context">Contexto de ejecucion.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
        => ControllerScopedOutputCachePolicy.Instance.ServeFromCacheAsync(context, cancellationToken);

    /// <summary>
    /// Ejecuta <c>ServeResponse</c>. Versión asíncrona.
    /// </summary>
    /// <param name="context">Contexto de ejecucion.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
        => ControllerScopedOutputCachePolicy.Instance.ServeResponseAsync(context, cancellationToken);
}
