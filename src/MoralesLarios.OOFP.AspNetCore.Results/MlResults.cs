// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Results;

/// <summary>
/// Representa <c>ExtendedProblemDetails</c>.
/// </summary>
public class ExtendedProblemDetails : ProblemDetails
{
    /// <summary>
    /// Obtiene o establece el valor de <c>Errors</c>.
    /// </summary>
    public Dictionary<string, object> Errors { get; set; } = new();
}

/// <summary>
/// Representa <c>MlResults</c>.
/// </summary>
public static class MlResults
{

    /// <summary>
    /// Crea la instancia solicitada de <c>ProblemsDetails</c>.
    /// </summary>
    /// <param name="statusCode">Código de estado HTTP.</param>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult CreateProblemsDetails(int                       statusCode, 
                                                     string                     title      = null!, 
                                                     string                     detail     = null!, 
                                                     string                     type       = null!, 
                                                     Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = statusCode,
            Title  = title  ?? "Error Details",
            Detail = detail ?? "An error occurred.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = statusCode
        };

        return result;
    }





    /// <summary>
    /// Ejecuta <c>BadRequest</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult BadRequest(string                     title      = null!, 
                                           string                     detail     = null!, 
                                           string                     type       = null!, 
                                           Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title  = title  ?? "Bad request",
            Detail = detail ?? "Invalid syntax or validation error.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status400BadRequest
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>BadRequest</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="validationResults">Colección de <c>validationResults</c>.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult BadRequest(string                                    title      = null!, 
                                           string                                    detail     = null!, 
                                           string                                    type       = null!, 
                                           IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> validationResults = null!)
    {
        var errors = validationResults?
                        .GroupBy(x => string.Join(", ", x.MemberNames))
                        .ToDictionary(
                            g => string.IsNullOrEmpty(g.Key) ? "validation" : g.Key,
                            g => (object)g.Select(v => v.ErrorMessage).ToList())
                     ?? new Dictionary<string, object>();

        return BadRequest(title, detail, type, errors);
    }

    /// <summary>
    /// Ejecuta <c>Unauthorized</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult Unauthorized(string                     title      = null!, 
                                             string                     detail     = null!, 
                                             string                     type       = null!, 
                                             Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title  = title  ?? "Unauthorized",
            Detail = detail ?? "Authentication is missing or invalid.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>Forbidden</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult Forbidden(string                     title      = null!, 
                                          string                     detail     = null!, 
                                          string                     type       = null!, 
                                          Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title  = title  ?? "Forbidden",
            Detail = detail ?? "Insufficient permissions to access this resource.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>NotFound</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult NotFound(string                     title      = null!, 
                                         string                     detail     = null!, 
                                         string                     type       = null!, 
                                         Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title  = title  ?? "Not found",
            Detail = detail ?? "The requested resource was not found.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status404NotFound
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>MethodNotAllowed</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult MethodNotAllowed(string                     title      = null!, 
                                                 string                     detail     = null!, 
                                                 string                     type       = null!, 
                                                 Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status405MethodNotAllowed,
            Title  = title  ?? "Method not allowed",
            Detail = detail ?? "The HTTP method is not allowed for this endpoint.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status405MethodNotAllowed
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>Conflict</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult Conflict(string                     title      = null!, 
                                         string                     detail     = null!, 
                                         string                     type       = null!, 
                                         Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title  = title  ?? "Conflict",
            Detail = detail ?? "The request conflicts with the current state (duplicate, etc.).",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status409Conflict
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>UnprocessableEntity</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult UnprocessableEntity(string                     title      = null!, 
                                                    string                     detail     = null!, 
                                                    string                     type       = null!, 
                                                    Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title  = title  ?? "Unprocessable entity",
            Detail = detail ?? "The request is well-formed but contains semantic errors.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>TooManyRequests</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult TooManyRequests(string                     title      = null!, 
                                                string                     detail     = null!, 
                                                string                     type       = null!, 
                                                Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status429TooManyRequests,
            Title  = title  ?? "Too many requests",
            Detail = detail ?? "Rate limit exceeded. Please try again later.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status429TooManyRequests
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>InternalServerError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult InternalServerError(string                     title      = null!, 
                                                    string                     detail     = null!, 
                                                    string                     type       = null!, 
                                                    Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title  = title  ?? "Internal server error",
            Detail = detail ?? "An unexpected error occurred on the server.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>NotImplemented</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult NotImplemented(string                     title      = null!, 
                                               string                     detail     = null!, 
                                               string                     type       = null!, 
                                               Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status501NotImplemented,
            Title  = title  ?? "Not implemented",
            Detail = detail ?? "The requested method or endpoint is not implemented.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status501NotImplemented
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>BadGateway</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult BadGateway(string                     title      = null!, 
                                           string                     detail     = null!, 
                                           string                     type       = null!, 
                                           Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status502BadGateway,
            Title  = title  ?? "Bad gateway",
            Detail = detail ?? "Invalid response from upstream gateway or server.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status502BadGateway
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>ServiceUnavailable</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult ServiceUnavailable(string                     title      = null!, 
                                                   string                     detail     = null!, 
                                                   string                     type       = null!, 
                                                   Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status503ServiceUnavailable,
            Title  = title  ?? "Service unavailable",
            Detail = detail ?? "The server is temporarily unavailable.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status503ServiceUnavailable
        };

        return result;
    }

    /// <summary>
    /// Ejecuta <c>GatewayTimeout</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult GatewayTimeout(string                     title      = null!, 
                                               string                     detail     = null!, 
                                               string                     type       = null!, 
                                               Dictionary<string, object> errors     = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status = StatusCodes.Status504GatewayTimeout,
            Title  = title  ?? "Gateway timeout",
            Detail = detail ?? "The gateway request timed out.",
            Type   = type   ?? DefaultProblemType,
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status504GatewayTimeout
        };

        return result;
    }

}

