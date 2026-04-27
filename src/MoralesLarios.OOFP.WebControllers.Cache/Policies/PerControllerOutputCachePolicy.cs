// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.WebControllers.Cache.Policies;

public sealed class PerControllerOutputCachePolicy : IOutputCachePolicy
{
    public const string BypassHeader = "X-Bypass-Cache";

    public static readonly PerControllerOutputCachePolicy Instance = new();

    private PerControllerOutputCachePolicy()
    {
    }

    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        if (ShouldBypassCache(context.HttpContext))
        {
            context.EnableOutputCaching = false;
            context.AllowCacheLookup    = false;
            context.AllowCacheStorage   = false;
            context.AllowLocking        = false;

            return ValueTask.CompletedTask;
        }

        context.EnableOutputCaching = true;
        context.AllowCacheLookup = true;
        context.AllowCacheStorage = true;
        context.AllowLocking = true;

        return ValueTask.CompletedTask;
    }

    private static readonly string[] _bypassTruthyValues =
    {
        "1", "true", "yes", "on", "no-cache", "no-store", "bypass"
    };

    private static bool ShouldBypassCache(Microsoft.AspNetCore.Http.HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue(BypassHeader, out var bypassValue))
        {
            var v = bypassValue.ToString();

            if (string.IsNullOrWhiteSpace(v)) return false;

            foreach (var truthy in _bypassTruthyValues)
            {
                if (string.Equals(v, truthy, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        var cacheControl = httpContext.Request.Headers.CacheControl.ToString();
        if (!string.IsNullOrEmpty(cacheControl) &&
            (cacheControl.Contains("no-cache", StringComparison.OrdinalIgnoreCase) ||
             cacheControl.Contains("no-store", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
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
