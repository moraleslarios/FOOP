// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;


public static partial class MlResultActionResultExtensions
{
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


    public static Task<IActionResult> ToOkProblemDetailsResultAsync<T>(this MlResult<T> source)
        => source.ToOkProblemDetailsResult().ToAsync();

    public static async Task<IActionResult> ToOkProblemDetailsResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToOkProblemDetailsResult();



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

    public static IActionResult ToCreatedProblemDetailsResult<T>(this MlResult<T> source)
    {
        var result = source.Match(
                                    valid: x      => new CreatedResult("https://www.netalpunto.net", x),
                                    fail : errors => errors.GetProblemDetails()
                                                            .Match(
                                                                        valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                        fail :  _             => MlResults.InternalServerError()
                                                                    )
                                );
        return result;
    }



    public static Task<IActionResult> ToCreatedProblemDetailsResultAsync<T>(this MlResult<T> source, Uri uri)
        => source.ToCreatedProblemDetailsResult(uri).ToAsync();

    public static async Task<IActionResult> ToPostActionResultAsync<T>(this Task<MlResult<T>> sourceAsync, Uri uri)
        => (await sourceAsync).ToCreatedProblemDetailsResult(uri);


    public static Task<IActionResult> ToCreatedProblemDetailsResultAsync<T>(this MlResult<T> source)
        => source.ToCreatedProblemDetailsResult().ToAsync();

    public static async Task<IActionResult> ToPostActionResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToCreatedProblemDetailsResult();


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

    public static Task<IActionResult> ToNoContentProblemDetailsResultAsync<T>(this MlResult<T> source)
        => source.ToNoContentProblemDetailsResult().ToAsync();

    public static async Task<IActionResult> ToNoContentProblemDetailsResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToNoContentProblemDetailsResult();


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

    public static Task<IActionResult> ToPatchedProblemDetailsResultAsync<T>(this MlResult<T> source)
        => source.ToPatchedProblemDetailsResult().ToAsync();

    public static async Task<IActionResult> ToPatchedProblemDetailsResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToPatchedProblemDetailsResult();


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

    public static Task<IActionResult> ToDeletedProblemDetailsResultAsync<T>(this MlResult<T> source)
        => source.ToDeletedProblemDetailsResult().ToAsync();

    public static async Task<IActionResult> ToDeletedProblemDetailsResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToDeletedProblemDetailsResult();


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

