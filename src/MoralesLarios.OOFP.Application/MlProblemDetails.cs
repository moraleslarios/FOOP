// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace MoralesLarios.OOFP.Application;

/// <summary>
/// Representa <c>MlProblemDetails</c>.
/// </summary>
public static class MlProblemDetails
{
    /// <summary>
    /// Crea la instancia solicitada de <c>ProblemDetails</c>.
    /// </summary>
    /// <param name="statusCode">Código de estado HTTP.</param>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails CreateProblemDetails(int                        statusCode, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = statusCode
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>BadRequestError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails BadRequestError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 400
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>BadRequestError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="validationResults">Colección de <c>validationResults</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails BadRequestError(string                        title             = null!, 
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

    /// <summary>
    /// Ejecuta <c>UnauthorizedError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails UnauthorizedError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 401
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>ForbiddenError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails ForbiddenError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 403
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>NotFoundError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails NotFoundError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 404
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>MethodNotAllowedError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails MethodNotAllowedError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 405
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>ConflictError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails ConflictError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 409
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>UnprocessableContentError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails UnprocessableContentError(string                     title      = null!, 
                                                           string                     detail     = null!, 
                                                           string                     type       = null!, 
                                                           Dictionary<string, object> errors     = null!)
    {
        var result = (title ?? "Unprocessable content",
                      new Dictionary<string, object>
                      {
                          { ProblemsDetails, new
                              {
                                    Status     = 422,
                                    Title      = title  ?? "Unprocessable content",
                                    Detail     = detail ?? "The request is well-formed but contains semantic errors.",
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 422
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>TooManyRequestsError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails TooManyRequestsError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 429
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>InternalServerError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails InternalServerError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 500
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>NotImplementedError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails NotImplementedError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 501
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>BadGatewayError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails BadGatewayError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 502
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>ServiceUnavailableError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails ServiceUnavailableError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 503
                              }
                          }
                      });
        return result;
    }

    /// <summary>
    /// Ejecuta <c>GatewayTimeoutError</c>.
    /// </summary>
    /// <param name="title">Valor de <c>title</c>.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <param name="type">Valor de <c>type</c>.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails GatewayTimeoutError(string                     title      = null!, 
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
                                    Type       = type   ?? DefaultProblemType,
                                    Errors     = errors ?? new Dictionary<string, object>(),
                                    StatusCode = 504
                              }
                          }
                      });
        return result;
    }

}

