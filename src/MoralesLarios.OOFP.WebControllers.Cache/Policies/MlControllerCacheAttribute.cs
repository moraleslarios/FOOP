// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.WebControllers.Cache.Policies;

/// <summary>
/// Atributo que aplica la <see cref="PerControllerOutputCachePolicy"/> y permite
/// indicar opcionalmente la duración del cache. Si no se especifica duración se usa
/// la configurada por defecto en <c>AddOutputCache</c> mediante
/// <see cref="Microsoft.AspNetCore.OutputCaching.OutputCacheOptions.DefaultExpirationTimeSpan"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class MlControllerCacheAttribute : Attribute, IOutputCachePolicy
{
    private readonly TimeSpan? _expiration;

    public MlControllerCacheAttribute() { }

    /// <param name="durationSeconds">Tiempo en segundos. Si es menor o igual a 0 se ignora y se usa el por defecto.</param>
    public MlControllerCacheAttribute(int durationSeconds)
    {
        if (durationSeconds > 0)
            _expiration = TimeSpan.FromSeconds(durationSeconds);
    }

    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var inner = PerControllerOutputCachePolicy.Instance.CacheRequestAsync(context, cancellationToken);

        if (_expiration is { } exp && context.EnableOutputCaching)
            context.ResponseExpirationTimeSpan = exp;

        return inner;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
        => PerControllerOutputCachePolicy.Instance.ServeFromCacheAsync(context, cancellationToken);

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
        => PerControllerOutputCachePolicy.Instance.ServeResponseAsync(context, cancellationToken);
}
