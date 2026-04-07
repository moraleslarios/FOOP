using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.WebApi.Tests.Unit.Helpers;

public class MlResultWebExtensionsPlusTests
{



    [Fact]
    public void ToGetPdActionResult_Should_Return_Ok_When_MlResult_Is_Valid()
    {
        var mlResult = MlResult<string>.Valid("Hello, World!");

        IActionResult result = mlResult.ToGetPdActionResult();

        result.Should().BeEquivalentTo(new OkObjectResult("Hello, World!"));
    }


    [Fact]
    public void ToGetPdActionResult_When_source_is_fail_without_problemDetailsKey_returns_InternalServerError()
    {
        var mlResult = MlResult<string>.Fail(new List<string> { "Error 1", "Error 2" });

        IActionResult result = mlResult.ToGetPdActionResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }


    [Theory]
    [InlineData(404)]
    [InlineData(400)]
    public void ToGetPdActionResultToRepoGetPdActionResult_When_source_is_fail_with_problemDetailsKey_returns_ProblemDetails(int expectedStatusCode)
    {
        var mlResult = CreateFailWithProblemDetails<string>(expectedStatusCode);

        IActionResult result = mlResult.ToGetPdActionResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(expectedStatusCode);
    }



    [Fact]
    public async Task ToGetPdActionResultAsync_From_MlResult_Should_Return_Ok_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("Hello, World!");

        IActionResult result = await mlResult.ToGetPdActionResultAsync();

        result.Should().BeEquivalentTo(new OkObjectResult("Hello, World!"));
    }

    [Fact]
    public async Task ToGetPdActionResultAsync_From_Task_Should_Return_Ok_When_Valid()
    {
        var mlResultTask = Task.FromResult(MlResult<string>.Valid("Hello, World!"));

        IActionResult result = await mlResultTask.ToGetPdActionResultAsync();

        result.Should().BeEquivalentTo(new OkObjectResult("Hello, World!"));
    }

    [Fact]
    public void ToPostActionResult_Should_Return_Created_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("created");
        var uri = new Uri("https://localhost/api/items/1");

        IActionResult result = mlResult.ToPostPdActionResult(uri);

        result.Should().BeOfType<CreatedResult>().Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public void ToPostActionResult_Should_Return_500_When_Fail_Without_ProblemDetails()
    {
        var mlResult = MlResult<string>.Fail("error");
        var uri = new Uri("https://localhost/api/items/1");

        IActionResult result = mlResult.ToPostPdActionResult(uri);

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ToPostActionResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails()
    {
        var mlResult = CreateFailWithProblemDetails<string>(409);
        var uri = new Uri("https://localhost/api/items/1");

        IActionResult result = mlResult.ToPostPdActionResult(uri);

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task ToPostActionResultAsync_From_MlResult_Should_Return_Created_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("created");
        var uri = new Uri("https://localhost/api/items/1");

        IActionResult result = await mlResult.ToPostPdActionResultAsync(uri);

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
    public void ToPutPdActionResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = mlResult.ToPutPdActionResult();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToPutPdActionResult_Should_Return_500_When_Fail_Without_ProblemDetails()
    {
        var mlResult = MlResult<string>.Fail("error");

        IActionResult result = mlResult.ToPutPdActionResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ToPutPdActionResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails()
    {
        var mlResult = CreateFailWithProblemDetails<string>(422);

        IActionResult result = mlResult.ToPutPdActionResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(422);
    }

    [Fact]
    public async Task ToPutPdActionResultAsync_From_MlResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = await mlResult.ToPutPdActionResultAsync();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ToPutPdActionResultAsync_From_Task_Should_Return_NoContent_When_Valid()
    {
        var mlResultTask = Task.FromResult(MlResult<string>.Valid("ok"));

        IActionResult result = await mlResultTask.ToPutPdActionResultAsync();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToPatchPdActionResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = mlResult.ToPatchPdActionResult();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToPatchPdActionResult_Should_Return_500_When_Fail_Without_ProblemDetails()
    {
        var mlResult = MlResult<string>.Fail("error");

        IActionResult result = mlResult.ToPatchPdActionResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ToPatchPdActionResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails()
    {
        var mlResult = CreateFailWithProblemDetails<string>(403);

        IActionResult result = mlResult.ToPatchPdActionResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task ToPatchPdActionResultAsync_From_MlResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = await mlResult.ToPatchPdActionResultAsync();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ToPatchPdActionResultAsync_From_Task_Should_Return_NoContent_When_Valid()
    {
        var mlResultTask = Task.FromResult(MlResult<string>.Valid("ok"));

        IActionResult result = await mlResultTask.ToPatchPdActionResultAsync();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToDeletePdActionResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = mlResult.ToDeletePdActionResult();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToDeletePdActionResult_Should_Return_500_When_Fail_Without_ProblemDetails()
    {
        var mlResult = MlResult<string>.Fail("error");

        IActionResult result = mlResult.ToDeletePdActionResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ToDeletePdActionResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails()
    {
        var mlResult = CreateFailWithProblemDetails<string>(404);

        IActionResult result = mlResult.ToDeletePdActionResult();

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ToDeletePdActionResultAsync_From_MlResult_Should_Return_NoContent_When_Valid()
    {
        var mlResult = MlResult<string>.Valid("ok");

        IActionResult result = await mlResult.ToDeletePdActionResultAsync();

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ToDeletePdActionResultAsync_From_Task_Should_Return_NoContent_When_Valid()
    {
        var mlResultTask = Task.FromResult(MlResult<string>.Valid("ok"));

        IActionResult result = await mlResultTask.ToDeletePdActionResultAsync();

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
