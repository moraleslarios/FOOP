// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers;

public class MlResultActionResultExtensionsTests
{



    [Fact]
    public void ToOkProblemDetailsResult_Should_Return_Ok_When_MlResult_Is_Valid()
    {
        var mlResult = MlResult<string>.Valid("Hello, World!");

        IActionResult result = mlResult.ToOkProblemDetailsResult();

        result.Should().BeEquivalentTo(new OkObjectResult("Hello, World!"));
    }


    [Fact]
    public void ToOkProblemDetailsResult_When_source_is_fail_without_problemDetailsKey_returns_InternalServerError()
    {
        var mlResult = MlResult<string>.Fail(new List<string> { "Error 1", "Error 2" });

        IActionResult result = mlResult.ToOkProblemDetailsResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ToOkProblemDetailsResult_WhenServiceBuildsNotFoundPkError_returnsNotFound()
    {
        MlResult<string> source = "Users".BuildEntityNotFoundError(42);

        IActionResult result = source.ToOkProblemDetailsResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }


    [Theory]
    [InlineData(404)]
    [InlineData(400)]
    public void ToOkProblemDetailsResultToRepoGetPdActionResult_When_source_is_fail_with_problemDetailsKey_returns_ProblemDetails(int expectedStatusCode)
    {
        var mlResult = CreateFailWithProblemDetails<string>(expectedStatusCode);

        IActionResult result = mlResult.ToOkProblemDetailsResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }



    [Fact]
    public async Task ToOkProblemDetailsResultAsync_From_MlResult_Should_Return_Ok_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("Hello, World!");

        IActionResult result = await mlResult.ToOkProblemDetailsResultAsync();

        result.Should().BeEquivalentTo(new OkObjectResult("Hello, World!"));
    }

    [Fact]
    public async Task ToOkProblemDetailsResultAsync_From_Task_Should_Return_Ok_When_Valid()
    {
        var mlResultTask = Task.FromResult(MlResult<string>.Valid("Hello, World!"));

        IActionResult result = await mlResultTask.ToOkProblemDetailsResultAsync();

        result.Should().BeEquivalentTo(new OkObjectResult("Hello, World!"));
    }

    [Fact]
    public void ToPostActionResult_Should_Return_Created_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("created");
        var uri = new Uri("https://localhost/api/items/1");

        IActionResult result = mlResult.ToCreatedProblemDetailsResult(uri);

        result.Should().BeOfType<CreatedResult>().Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public void ToPostActionResult_Should_Return_500_When_Fail_Without_ProblemDetails()
    {
        var mlResult = MlResult<string>.Fail("error");
        var uri = new Uri("https://localhost/api/items/1");

        IActionResult result = mlResult.ToCreatedProblemDetailsResult(uri);

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ToPostActionResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails()
    {
        var mlResult = CreateFailWithProblemDetails<string>(409);
        var uri = new Uri("https://localhost/api/items/1");

        IActionResult result = mlResult.ToCreatedProblemDetailsResult(uri);

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task ToPostActionResultAsync_From_MlResult_Should_Return_Created_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("created");
        var uri = new Uri("https://localhost/api/items/1");

        IActionResult result = await mlResult.ToCreatedProblemDetailsResultAsync(uri);

        result.Should().BeOfType<CreatedResult>().Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task ToPostActionResultAsync_From_Task_Should_Return_Created_When_Valid()
    {
        var mlResultTask = Task.FromResult(MlResult<string>.Valid("created"));
        var uri = new Uri("https://localhost/api/items/1");

        IActionResult result = await mlResultTask.ToPostActionResultAsync(uri);

        result.Should().BeOfType<CreatedResult>().Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public void ToNoContentProblemDetailsResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = mlResult.ToNoContentProblemDetailsResult();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToNoContentProblemDetailsResult_Should_Return_500_When_Fail_Without_ProblemDetails()
    {
        var mlResult = MlResult<string>.Fail("error");

        IActionResult result = mlResult.ToNoContentProblemDetailsResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ToNoContentProblemDetailsResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails()
    {
        var mlResult = CreateFailWithProblemDetails<string>(422);

        IActionResult result = mlResult.ToNoContentProblemDetailsResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(422);
    }

    [Fact]
    public async Task ToNoContentProblemDetailsResultAsync_From_MlResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = await mlResult.ToNoContentProblemDetailsResultAsync();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ToNoContentProblemDetailsResultAsync_From_Task_Should_Return_NoContent_When_Valid()
    {
        var mlResultTask = Task.FromResult(MlResult<string>.Valid("ok"));

        IActionResult result = await mlResultTask.ToNoContentProblemDetailsResultAsync();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToPatchedProblemDetailsResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = mlResult.ToPatchedProblemDetailsResult();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToPatchedProblemDetailsResult_Should_Return_500_When_Fail_Without_ProblemDetails()
    {
        var mlResult = MlResult<string>.Fail("error");

        IActionResult result = mlResult.ToPatchedProblemDetailsResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ToPatchedProblemDetailsResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails()
    {
        var mlResult = CreateFailWithProblemDetails<string>(403);

        IActionResult result = mlResult.ToPatchedProblemDetailsResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task ToPatchedProblemDetailsResultAsync_From_MlResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = await mlResult.ToPatchedProblemDetailsResultAsync();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ToPatchedProblemDetailsResultAsync_From_Task_Should_Return_NoContent_When_Valid()
    {
        var mlResultTask = Task.FromResult(MlResult<string>.Valid("ok"));

        IActionResult result = await mlResultTask.ToPatchedProblemDetailsResultAsync();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToDeletedProblemDetailsResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = mlResult.ToDeletedProblemDetailsResult();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToDeletedProblemDetailsResult_Should_Return_500_When_Fail_Without_ProblemDetails()
    {
        var mlResult = MlResult<string>.Fail("error");

        IActionResult result = mlResult.ToDeletedProblemDetailsResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ToDeletedProblemDetailsResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails()
    {
        var mlResult = CreateFailWithProblemDetails<string>(404);

        IActionResult result = mlResult.ToDeletedProblemDetailsResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ToDeletedProblemDetailsResultAsync_From_MlResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = await mlResult.ToDeletedProblemDetailsResultAsync();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ToDeletedProblemDetailsResultAsync_From_Task_Should_Return_NoContent_When_Valid()
    {
        var mlResultTask = Task.FromResult(MlResult<string>.Valid("ok"));

        IActionResult result = await mlResultTask.ToDeletedProblemDetailsResultAsync();

        result.Should().BeOfType<NoContentResult>();
    }

    private static MlResult<T> CreateFailWithProblemDetails<T>(int statusCode)
        => ("Error Details",
            new Dictionary<string, object>
            {
                { "ProblemsDetails", new
                    {
                        Status     = statusCode,
                        Title      = "Error Details",
                        Detail     = "Invalid syntax or validation error.",
                        Type       = "https://www.puntonetalpunto.net/",
                        Errors     = new Dictionary<string, object>(),
                        StatusCode = statusCode
                    }
                }
            }).ToMlResultFail<T>();
}

