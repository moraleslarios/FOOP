// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.WebControllers.Cache.Policies;

public sealed class PerControllerOutputCachePolicy : IOutputCachePolicy
{
    public static readonly PerControllerOutputCachePolicy Instance = new();

    private PerControllerOutputCachePolicy()
    {
    }

    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = true;
        context.AllowCacheStorage = true;
        context.AllowLocking = true;

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
        => ValueTask.CompletedTask;

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        if (context.HttpContext.Response.StatusCode == Microsoft.AspNetCore.Http.StatusCodes.Status200OK)
        {
            context.Tags.Add(GetControllerTag(context.HttpContext));
        }

        return ValueTask.CompletedTask;
    }

    public static string GetControllerTag(Microsoft.AspNetCore.Http.HttpContext httpContext)
    {
        var controller = httpContext.Request.RouteValues.TryGetValue("controller", out var controllerValue)
                        ? controllerValue?.ToString()
                        : null;

        return $"oc:{controller ?? "unknown-controller"}";
    }
}
