// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.WebApi.Helpers;


public static class MlResultWebExtensionsPlus
{
    private const string ProblemsDetails = nameof(ProblemsDetails);



    public static IActionResult ToGetPdActionResult<T>(this MlResult<T> source)
    {
        var result = source.Match(
                                    valid: x      => new OkObjectResult(x),
                                    fail : errors => errors.GetProblemDetails()
                                                            .Match(
                                                                        valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                        fail : _              => MlActionResults.InternalServerError()
                                                                   )
                                );
        return result;
    }


    public static Task<IActionResult> ToGetPdActionResultAsync<T>(this MlResult<T> source)
        => source.ToGetPdActionResult().ToAsync();

    public static async Task<IActionResult> ToGetPdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToGetPdActionResult();



    public static IActionResult ToPostPdActionResult<T>(this MlResult<T> source, Uri uri)
    {
        var result = EnsureFp.NotNull(uri, "Uri cannot be null")
                              .Map( _ => source.Match(
                                                        valid: x      => new CreatedResult(uri, x),
                                                        fail : errors => errors.GetProblemDetails()
                                                                                .Match(
                                                                                            valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                                            fail :  _             => MlActionResults.InternalServerError()
                                                                                       )
                                                    ));
        return result.SecureValidValue();
    }

    public static IActionResult ToPostPdActionResult<T>(this MlResult<T> source)
    {
        var result = source.Match(
                                    valid: x      => new CreatedResult("https://www.netalpunto.net", x),
                                    fail : errors => errors.GetProblemDetails()
                                                            .Match(
                                                                        valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                        fail :  _             => MlActionResults.InternalServerError()
                                                                    )
                                );
        return result;
    }



    public static Task<IActionResult> ToPostPdActionResultAsync<T>(this MlResult<T> source, Uri uri)
        => source.ToPostPdActionResult(uri).ToAsync();

    public static async Task<IActionResult> ToPostActionResultAsync<T>(this Task<MlResult<T>> sourceAsync, Uri uri)
        => (await sourceAsync).ToPostPdActionResult(uri);


    public static Task<IActionResult> ToPostPdActionResultAsync<T>(this MlResult<T> source)
        => source.ToPostPdActionResult().ToAsync();

    public static async Task<IActionResult> ToPostActionResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToPostPdActionResult();


    public static IActionResult ToPutPdActionResult<T>(this MlResult<T> source)
    {
        var result = source.Match(
                                valid: _      => new NoContentResult(),
                                fail : errors => errors.GetProblemDetails()
                                                        .Match(
                                                                    valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                    fail : _              => MlActionResults.InternalServerError()
                                                               )
                            );
    return result;
    }

    public static Task<IActionResult> ToPutPdActionResultAsync<T>(this MlResult<T> source)
        => source.ToPutPdActionResult().ToAsync();

    public static async Task<IActionResult> ToPutPdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToPutPdActionResult();


    public static IActionResult ToPatchPdActionResult<T>(this MlResult<T> source)
    {
        var result = source.Match(
                                    valid: _      => new NoContentResult(),
                                    fail : errors => errors.GetProblemDetails()
                                                            .Match(
                                                                        valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                        fail : _              => MlActionResults.InternalServerError()
                                                                   )
                                );
    return result;
    }

    public static Task<IActionResult> ToPatchPdActionResultAsync<T>(this MlResult<T> source)
        => source.ToPatchPdActionResult().ToAsync();

    public static async Task<IActionResult> ToPatchPdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToPatchPdActionResult();


    public static IActionResult ToDeletePdActionResult<T>(this MlResult<T> source)
    {
        var result = source.Match(
                                valid: _      => new NoContentResult(),
                                fail : errors => errors.GetProblemDetails()
                                                        .Match(
                                                                    valid: problemDetails => problemDetails.ToMlActionResult(),
                                                                    fail : _              => MlActionResults.InternalServerError()
                                                               )
                            );
    return result;
    }

    public static Task<IActionResult> ToDeletePdActionResultAsync<T>(this MlResult<T> source)
        => source.ToDeletePdActionResult().ToAsync();

    public static async Task<IActionResult> ToDeletePdActionResultAsync<T>(this Task<MlResult<T>> sourceAsync)
        => (await sourceAsync).ToDeletePdActionResult();


    public static IActionResult ToMlActionResult(this ProblemDetailsInfo source)
    {
        var result = MlActionResults.CreateProblemsDetails(
            statusCode: source.StatusCode,
            title     : source.Title,
            detail    : source.Detail,
            type      : source.Type,
            errors    : source.Errors
        );

        return result;
    }





}

