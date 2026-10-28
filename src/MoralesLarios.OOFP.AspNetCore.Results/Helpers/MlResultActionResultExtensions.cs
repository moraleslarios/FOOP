// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;


/// <summary>
/// Métodos de extension de <c>MlResultActionResult</c>.
/// </summary>
public static partial class MlResultActionResultExtensions
{
    /// <summary>
    /// Ejecuta <c>ToOkProblemDetailsResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult ToOkProblemDetailsResult<T>(this MlResult<T> source)
    {
        var result = source.Match(
                                    valid: x      => new OkObjectResult(x),
                                    fail : errors => errors.GetProblemDetails()
                                                            .Match(
                                                                        valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                        fail : _              => MlResults.InternalServerError()
                                                                   )
                                );
        return result;
    }


    /// <summary>
    /// Ejecuta <c>ToOkProblemDetailsResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static Task<IActionResult> ToOkProblemDetailsResultAsync<T>(this MlResult<T> source)
        => source.ToOkProblemDetailsResult().ToAsync();

    /// <summary>
    /// Ejecuta <c>ToOkProblemDetailsResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static async Task<IActionResult> ToOkProblemDetailsResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToOkProblemDetailsResult();



    /// <summary>
    /// Ejecuta <c>ToCreatedProblemDetailsResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="uri">Dirección a la que se dirige la llamada.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult ToCreatedProblemDetailsResult<T>(this MlResult<T> source, Uri uri)
    {
        var result = EnsureFp.NotNull(uri, "Uri cannot be null")
                              .Map( _ => source.Match(
                                                        valid: x      => new CreatedResult(uri, x),
                                                        fail : errors => errors.GetProblemDetails()
                                                                                .Match(
                                                                                            valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                                            fail :  _             => MlResults.InternalServerError()
                                                                                       )
                                                    ));
        return result.GetValueOrThrow();
    }

    /// <summary>
    /// Ejecuta <c>ToCreatedProblemDetailsResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult ToCreatedProblemDetailsResult<T>(this MlResult<T> source)
    {
        var result = source.Match(
                                    valid: x      => new CreatedResult(DefaultCreatedLocation, x),
                                    fail : errors => errors.GetProblemDetails()
                                                            .Match(
                                                                        valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                        fail :  _             => MlResults.InternalServerError()
                                                                    )
                                );
        return result;
    }



    /// <summary>
    /// Ejecuta <c>ToCreatedProblemDetailsResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="uri">Dirección a la que se dirige la llamada.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static Task<IActionResult> ToCreatedProblemDetailsResultAsync<T>(this MlResult<T> source, Uri uri)
        => source.ToCreatedProblemDetailsResult(uri).ToAsync();

    /// <summary>
    /// Ejecuta <c>ToPostActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="uri">Dirección a la que se dirige la llamada.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static async Task<IActionResult> ToPostActionResultAsync<T>(this Task<MlResult<T>> sourceAsync, Uri uri)
        => (await sourceAsync).ToCreatedProblemDetailsResult(uri);


    /// <summary>
    /// Ejecuta <c>ToCreatedProblemDetailsResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static Task<IActionResult> ToCreatedProblemDetailsResultAsync<T>(this MlResult<T> source)
        => source.ToCreatedProblemDetailsResult().ToAsync();

    /// <summary>
    /// Ejecuta <c>ToPostActionResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static async Task<IActionResult> ToPostActionResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToCreatedProblemDetailsResult();


    /// <summary>
    /// Ejecuta <c>ToNoContentProblemDetailsResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult ToNoContentProblemDetailsResult<T>(this MlResult<T> source)
    {
        var result = source.Match(
                                valid: _      => new NoContentResult(),
                                fail : errors => errors.GetProblemDetails()
                                                        .Match(
                                                                    valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                    fail : _              => MlResults.InternalServerError()
                                                               )
                            );
    return result;
    }

    /// <summary>
    /// Ejecuta <c>ToNoContentProblemDetailsResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static Task<IActionResult> ToNoContentProblemDetailsResultAsync<T>(this MlResult<T> source)
        => source.ToNoContentProblemDetailsResult().ToAsync();

    /// <summary>
    /// Ejecuta <c>ToNoContentProblemDetailsResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static async Task<IActionResult> ToNoContentProblemDetailsResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToNoContentProblemDetailsResult();


    /// <summary>
    /// Ejecuta <c>ToPatchedProblemDetailsResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult ToPatchedProblemDetailsResult<T>(this MlResult<T> source)
    {
        var result = source.Match(
                                    valid: _      => new NoContentResult(),
                                    fail : errors => errors.GetProblemDetails()
                                                            .Match(
                                                                        valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                        fail : _              => MlResults.InternalServerError()
                                                                   )
                                );
    return result;
    }

    /// <summary>
    /// Ejecuta <c>ToPatchedProblemDetailsResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static Task<IActionResult> ToPatchedProblemDetailsResultAsync<T>(this MlResult<T> source)
        => source.ToPatchedProblemDetailsResult().ToAsync();

    /// <summary>
    /// Ejecuta <c>ToPatchedProblemDetailsResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static async Task<IActionResult> ToPatchedProblemDetailsResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToPatchedProblemDetailsResult();


    /// <summary>
    /// Ejecuta <c>ToDeletedProblemDetailsResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult ToDeletedProblemDetailsResult<T>(this MlResult<T> source)
    {
        var result = source.Match(
                                valid: _      => new NoContentResult(),
                                fail : errors => errors.GetProblemDetails()
                                                        .Match(
                                                                    valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                    fail : _              => MlResults.InternalServerError()
                                                               )
                            );
    return result;
    }

    /// <summary>
    /// Ejecuta <c>ToDeletedProblemDetailsResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static Task<IActionResult> ToDeletedProblemDetailsResultAsync<T>(this MlResult<T> source)
        => source.ToDeletedProblemDetailsResult().ToAsync();

    /// <summary>
    /// Ejecuta <c>ToDeletedProblemDetailsResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static async Task<IActionResult> ToDeletedProblemDetailsResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToDeletedProblemDetailsResult();


    /// <summary>
    /// Ejecuta <c>ToMlActionResult</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    public static IActionResult ToMlActionResult(this ProblemDetailsData source)
    {
        var result = MlResults.CreateProblemsDetails(
            statusCode: source.StatusCode,
            title     : source.Title,
            detail    : source.Detail,
            type      : source.Type,
            errors    : source.Errors
        );

        return result;
    }





}

