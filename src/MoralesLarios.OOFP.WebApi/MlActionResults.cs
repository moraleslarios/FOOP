namespace MoralesLarios.OOFP.WebApi;

public class ExtendedProblemDetails : ProblemDetails
{
    public Dictionary<string, object> Errors { get; set; } = new();
}

public static class MlActionResults
{

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = statusCode
        };

        return result;
    }





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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status400BadRequest
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status404NotFound
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status405MethodNotAllowed
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status409Conflict
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status429TooManyRequests
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status501NotImplemented
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status502BadGateway
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status503ServiceUnavailable
        };

        return result;
    }

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
            Type   = type   ?? "https://www.puntonetalpunto.net/",
            Errors = errors ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status504GatewayTimeout
        };

        return result;
    }

}
