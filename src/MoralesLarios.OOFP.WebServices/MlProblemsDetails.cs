// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace MoralesLarios.OOFP.WebServices;

public static class MlProblemsDetails
{
    private const string ProblemsDetails = nameof(ProblemsDetails);

    public static MlErrorsDetails CreateProblemDetails(int                        statusCode, 
                                                       string                     title      = null!, 
                                                       string                     detail     = null!, 
                                                       string                     type       = null!, 
                                                       Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Error Details",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = statusCode,
                                    Title      = title  ?? "Error Details",
                                    Detail     = detail ?? "Invalid syntax or validation error.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = statusCode
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails BadRequestError(string                     title      = null!, 
                                                  string                     detail     = null!, 
                                                  string                     type       = null!, 
                                                  Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Bad request",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 400,
                                    Title      = title  ?? "Bad request",
                                    Detail     = detail ?? "Invalid syntax or validation error.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 400
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails BadRequestError(string                        title             = null!, 
                                                  string                        detail            = null!, 
                                                  string                        type              = null!, 
                                                  IEnumerable<ValidationResult> validationResults = null!)
    {
        var errors = validationResults?
                        .GroupBy(x => string.Join(", ", x.MemberNames))
                        .ToDictionary(
                            g => string.IsNullOrEmpty(g.Key) ? "validation" : g.Key,
                            g => (object)g.Select(v => v.ErrorMessage).ToList())
                     ?? new Dictionary<string, object>();

        return BadRequestError(title, detail, type, errors);
    }

    public static MlErrorsDetails UnauthorizedError(string                     title      = null!, 
                                                    string                     detail     = null!, 
                                                    string                     type       = null!, 
                                                    Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Unauthorized",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 401,
                                    Title      = title  ?? "Unauthorized",
                                    Detail     = detail ?? "Authentication is missing or invalid.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 401
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails ForbiddenError(string                     title      = null!, 
                                                 string                     detail     = null!, 
                                                 string                     type       = null!, 
                                                 Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Forbidden",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 403,
                                    Title      = title  ?? "Forbidden",
                                    Detail     = detail ?? "Insufficient permissions to access this resource.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 403
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails NotFoundError(string                     title      = null!, 
                                                string                     detail     = null!, 
                                                string                     type       = null!, 
                                                Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Not found",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 404,
                                    Title      = title  ?? "Not found",
                                    Detail     = detail ?? "The requested resource was not found.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 404
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails MethodNotAllowedError(string                     title      = null!, 
                                                        string                     detail     = null!, 
                                                        string                     type       = null!, 
                                                        Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Method not allowed",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 405,
                                    Title      = title  ?? "Method not allowed",
                                    Detail     = detail ?? "The HTTP method is not allowed for this endpoint.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 405
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails ConflictError(string                     title      = null!, 
                                                string                     detail     = null!, 
                                                string                     type       = null!, 
                                                Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Conflict",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 409,
                                    Title      = title  ?? "Conflict",
                                    Detail     = detail ?? "The request conflicts with the current state (duplicate, etc.).",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 409
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails UnprocessableEntityError(string                     title      = null!, 
                                                           string                     detail     = null!, 
                                                           string                     type       = null!, 
                                                           Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Unprocessable entity",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 422,
                                    Title      = title  ?? "Unprocessable entity",
                                    Detail     = detail ?? "The request is well-formed but contains semantic errors.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 422
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails TooManyRequestsError(string                     title      = null!, 
                                                       string                     detail     = null!, 
                                                       string                     type       = null!, 
                                                       Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Too many requests",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 429,
                                    Title      = title  ?? "Too many requests",
                                    Detail     = detail ?? "Rate limit exceeded. Please try again later.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 429
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails InternalServerError(string                     title      = null!, 
                                                      string                     detail     = null!, 
                                                      string                     type       = null!, 
                                                      Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Internal server error",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 500,
                                    Title      = title  ?? "Internal server error",
                                    Detail     = detail ?? "An unexpected error occurred on the server.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 500
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails NotImplementedError(string                     title      = null!, 
                                                      string                     detail     = null!, 
                                                      string                     type       = null!, 
                                                      Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Not implemented",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 501,
                                    Title      = title  ?? "Not implemented",
                                    Detail     = detail ?? "The requested method or endpoint is not implemented.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 501
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails BadGatewayError(string                     title      = null!, 
                                                  string                     detail     = null!, 
                                                  string                     type       = null!, 
                                                  Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Bad gateway",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 502,
                                    Title      = title  ?? "Bad gateway",
                                    Detail     = detail ?? "Invalid response from upstream gateway or server.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 502
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails ServiceUnavailableError(string                     title      = null!, 
                                                          string                     detail     = null!, 
                                                          string                     type       = null!, 
                                                          Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Service unavailable",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 503,
                                    Title      = title  ?? "Service unavailable",
                                    Detail     = detail ?? "The server is temporarily unavailable.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 503
                              }
                          }
                      });
        return result;
    }

    public static MlErrorsDetails GatewayTimeoutError(string                     title      = null!, 
                                                      string                     detail     = null!, 
                                                      string                     type       = null!, 
                                                      Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Gateway timeout",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 504,
                                    Title      = title  ?? "Gateway timeout",
                                    Detail     = detail ?? "The gateway request timed out.",
                                    Type       = type   ?? "https://www.puntonetalpunto.net/",
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 504
                              }
                          }
                      });
        return result;
    }

}

