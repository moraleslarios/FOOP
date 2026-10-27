// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers;

public class MlResultActionResultExtensionsLegacyTests
{


    [Fact]
    public void ToRepoGetActionResult_withErrorNotFound_returnsNotFound()
    {
        MlResult<DummyEntity> partialResult =
            MlErrorDetails.FromErrorMessageDetails("Error", new Dictionary<string, object> { { "NotFound", 1 } });

        ControllerBase controller = TestControllerFactory.Create(); // If extension method needs controller (adjust as needed)

        IActionResult result = partialResult.ToRepoGetActionResult(controller);

        result.Should().BeOfType<NotFoundObjectResult>();
    }


    [Fact]
    public void ToRepoGetActionResult_withoutErrorNotFound_returnsDistinctNotFound()
    {
        MlResult<DummyEntity> partialResult =
            MlErrorDetails.FromErrorMessageDetails("Error", new Dictionary<string, object> { { "XXX", 1 } });

        ControllerBase controller = TestControllerFactory.Create(); // If extension method needs controller (adjust as needed)

        IActionResult result = partialResult.ToRepoGetActionResult(controller);

        result.Should().NotBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void ToCreatedResult_whenValid_returnsCreated()
    {
        MlResult<DummyEntity> source = MlResult<DummyEntity>.Valid(new DummyEntity(1, "created"));

        IActionResult result = source.ToCreatedResult(TestControllerFactory.Create());

        result.Should().BeOfType<CreatedResult>().Which.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Fact]
    public void ToCreatedResult_whenFail_returnsInternalServerError()
    {
        MlResult<DummyEntity> source = MlResult<DummyEntity>.Fail("repository failed");

        IActionResult result = source.ToCreatedResult(TestControllerFactory.Create());

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public void ToCreatedResult_whenNotFoundFail_returnsNotFound()
    {
        MlResult<DummyEntity> source = MlErrorDetails.FromErrorMessageDetails(
            "not found", new Dictionary<string, object> { { "NotFound", 1 } });

        IActionResult result = source.ToCreatedResult(TestControllerFactory.Create());

        result.Should().BeOfType<NotFoundObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task ToCreatedResultAsync_whenFail_returnsInternalServerError()
    {
        MlResult<DummyEntity> source = MlResult<DummyEntity>.Fail("repository failed");

        IActionResult result = await source.ToCreatedResultAsync(TestControllerFactory.Create());

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task ToCreatedResultAsync_fromTask_whenFail_returnsInternalServerError()
    {
        Task<MlResult<DummyEntity>> sourceAsync = Task.FromResult(MlResult<DummyEntity>.Fail("repository failed"));

        IActionResult result = await sourceAsync.ToCreatedResultAsync(TestControllerFactory.Create());

        result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }




}

public record class DummyEntity(int Id, string Name);

