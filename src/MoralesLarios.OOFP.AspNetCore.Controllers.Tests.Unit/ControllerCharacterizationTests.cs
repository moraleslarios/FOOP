using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using MoralesLarios.OOFP.Application;
using MoralesLarios.OOFP.Application.Services;
using MoralesLarios.OOFP.AspNetCore.Controllers.Controllers;
using MoralesLarios.OOFP.AspNetCore.Controllers.Helpers;
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Types.Errors;
using Xunit;

namespace MoralesLarios.OOFP.AspNetCore.Controllers.Tests.Unit;

public class ControllerCharacterizationTests
{
    [Fact]
    public async Task SimpleController_InvalidIntegerKey_CurrentlyReturns404ForGetAnd500ForPutDelete()
    {
        var service = new Mock<ICrudServiceFp<TestEntity, TestDto>>(MockBehavior.Strict);
        var controller = new SimpleController(service.Object);

        await AssertInvalidIntegerKeyStatusesAsync(
            () => controller.GetByIdAsync("not-an-integer"),
            () => controller.PutAsync("not-an-integer", new TestDto()),
            () => controller.DeleteAsync("not-an-integer"));
    }

    [Fact]
    public async Task DuplexController_InvalidIntegerKey_CurrentlyReturns404ForGetAnd500ForPutDelete()
    {
        var service = new Mock<ICrudServiceFp<TestEntity, TestRequest, TestResponse>>(MockBehavior.Strict);
        var controller = new DuplexController(service.Object);

        await AssertInvalidIntegerKeyStatusesAsync(
            () => controller.GetByIdAsync("not-an-integer"),
            () => controller.PutAsync("not-an-integer", new TestRequest()),
            () => controller.DeleteAsync("not-an-integer"));
    }

    [Fact]
    public async Task CompositeKeyController_InvalidIntegerKey_CurrentlyReturns404ForGetAnd500ForPutDelete()
    {
        var service = new Mock<ICrudServiceFp<TestEntity, TestDto>>(MockBehavior.Strict);
        var controller = new CompositeKeyController(service.Object);

        await AssertInvalidIntegerKeyStatusesAsync(
            () => controller.GetByIdAsync("not-an-integer,2"),
            () => controller.PutAsync("not-an-integer,2", new TestDto()),
            () => controller.DeleteAsync("not-an-integer,2"));
    }

    [Fact]
    public async Task CompositeKeyDuplexController_InvalidIntegerKey_CurrentlyReturns404ForGetAnd500ForPutDelete()
    {
        var service = new Mock<ICrudServiceFp<TestEntity, TestRequest, TestResponse>>(MockBehavior.Strict);
        var controller = new CompositeKeyDuplexController(service.Object);

        await AssertInvalidIntegerKeyStatusesAsync(
            () => controller.GetByIdAsync("not-an-integer,2"),
            () => controller.PutAsync("not-an-integer,2", new TestRequest()),
            () => controller.DeleteAsync("not-an-integer,2"));
    }

    [Fact]
    public async Task GetByIdAsync_InvalidIntegerKey_CurrentProblemDetailIncludesIdentifierAndKeyType()
    {
        const string invalidId = "private-key-value";
        var simpleService = new Mock<ICrudServiceFp<TestEntity, TestDto>>(MockBehavior.Strict);
        var duplexService = new Mock<ICrudServiceFp<TestEntity, TestRequest, TestResponse>>(MockBehavior.Strict);

        var simpleResult = await new SimpleController(simpleService.Object).GetByIdAsync(invalidId);
        var duplexResult = await new DuplexController(duplexService.Object).GetByIdAsync(invalidId);

        AssertCurrentInvalidKeyDetail(simpleResult, invalidId);
        AssertCurrentInvalidKeyDetail(duplexResult, invalidId);
    }

    [Fact]
    public async Task GetByIdAsync_ValidIntegerKeyForMissingResource_Remains404()
    {
        var service = new Mock<ICrudServiceFp<TestEntity, TestDto>>();
        service.Setup(candidate => candidate.FindByIdProblemsDetailsAsync(
                It.IsAny<MlErrorDetails>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>(),
                It.IsAny<Func<TestDto, string>>(),
                It.IsAny<Func<MlErrorDetails, string>>(),
                It.IsAny<object[]>()))
            .ReturnsAsync(MlResult<TestDto?>.Fail(MlProblemDetails.NotFoundError()));

        var result = await new SimpleController(service.Object).GetByIdAsync("42");

        Assert.Equal(StatusCodes.Status404NotFound, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task GetByIdAsync_Int32RouteKey_CurrentlyPassesParsedValueToService()
    {
        var expectedDto = new TestDto();
        var service = new Mock<ICrudServiceFp<TestEntity, TestDto>>();
        service.Setup(candidate => candidate.FindByIdProblemsDetailsAsync(
                It.IsAny<MlErrorDetails>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>(),
                It.IsAny<Func<TestDto, string>>(),
                It.IsAny<Func<MlErrorDetails, string>>(),
                It.IsAny<object[]>()))
            .ReturnsAsync(MlResult<TestDto?>.Valid(expectedDto));

        var result = await new SimpleController(service.Object).GetByIdAsync("42");

        Assert.Same(expectedDto, Assert.IsType<OkObjectResult>(result).Value);
        service.Verify(candidate => candidate.FindByIdProblemsDetailsAsync(
            It.IsAny<MlErrorDetails>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>(),
            It.IsAny<Func<TestDto, string>>(),
            It.IsAny<Func<MlErrorDetails, string>>(),
            It.Is<object[]>(keys => keys.Length == 1 && keys[0] != null && keys[0].GetType() == typeof(int) && (int)keys[0] == 42)), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_EncodedStringRouteValue_IsDecodedBeforeCallingService()
    {
        var expectedDto = new TestDto();
        var service = new Mock<ICrudServiceFp<TestEntity, TestDto>>();
        service.Setup(candidate => candidate.FindByIdProblemsDetailsAsync(
                It.IsAny<MlErrorDetails>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>(),
                It.IsAny<Func<TestDto, string>>(),
                It.IsAny<Func<MlErrorDetails, string>>(),
                It.IsAny<object[]>()))
            .ReturnsAsync(MlResult<TestDto?>.Valid(expectedDto));

        var result = await new StringKeyController(service.Object).GetByIdAsync("ACME%2CSL%2Fwest");

        Assert.Same(expectedDto, Assert.IsType<OkObjectResult>(result).Value);
        service.Verify(candidate => candidate.FindByIdProblemsDetailsAsync(
            It.IsAny<MlErrorDetails>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>(),
            It.IsAny<Func<TestDto, string>>(),
            It.IsAny<Func<MlErrorDetails, string>>(),
            It.Is<object[]>(keys => keys.Length == 1 && (string)keys[0] == "ACME,SL/west")), Times.Once);
    }

    [Fact]
    public void ParseKeyValues_DecodesComponentsAfterSplittingTheSeparator()
    {
        var values = "caf%C3%A9%2CSL%2Fwest,42".ParseKeyValues<TestEntity>(entity => [entity.Name, entity.Id]);

        Assert.Equal("café,SL/west", values[0]);
        Assert.Equal(42, values[1]);
    }

    [Fact]
    public void ParseKeyValues_PreservesAnEmptyStringComponent()
    {
        var values = ",42".ParseKeyValues<TestEntity>(entity => [entity.Name, entity.Id]);

        Assert.Equal(string.Empty, values[0]);
        Assert.Equal(42, values[1]);
    }

    [Fact]
    public async Task GetByIdAsync_GuidRouteKey_CurrentlyReturnsNotFoundWithoutCallingService()
    {
        const string routeId = "54d28c1d-cba7-4bdf-b4b1-29c6de3d5a77";
        var service = new Mock<ICrudServiceFp<TestEntity, TestDto>>(MockBehavior.Strict);

        var result = await new GuidController(service.Object).GetByIdAsync(routeId);

        AssertCurrentInvalidKeyDetail(result, routeId, "Guid");
    }

    [Fact]
    public async Task PostAsync_SimpleCrudBase_CurrentlyReturnsCreatedAtRootLocation()
    {
        var dto = new TestDto();
        var service = new Mock<ICrudServiceFp<TestEntity, TestDto>>();
        service.Setup(candidate => candidate.CreateAsync(
                dto,
                It.IsAny<CancellationToken>(),
                It.IsAny<string>(),
                It.IsAny<Func<TestDto, string>>(),
                It.IsAny<Func<MlErrorDetails, string>>()))
            .ReturnsAsync(MlResult<TestDto>.Valid(dto));

        var result = await new SimpleController(service.Object).PostAsync(dto);
        var created = Assert.IsType<CreatedResult>(result);

        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        Assert.Equal("/", created.Location);
        Assert.Same(dto, created.Value);
    }

    [Fact]
    public async Task PostAsync_DuplexCrudBase_CurrentlyReturnsCreatedAtRootLocation()
    {
        var request = new TestRequest();
        var response = new TestResponse();
        var service = new Mock<ICrudServiceFp<TestEntity, TestRequest, TestResponse>>();
        service.Setup(candidate => candidate.CreateAsync(
                request,
                It.IsAny<CancellationToken>(),
                It.IsAny<string>(),
                It.IsAny<Func<TestResponse, string>>(),
                It.IsAny<Func<MlErrorDetails, string>>()))
            .ReturnsAsync(MlResult<TestResponse>.Valid(response));

        var result = await new DuplexController(service.Object).PostAsync(request);
        var created = Assert.IsType<CreatedResult>(result);

        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        Assert.Equal("/", created.Location);
        Assert.Same(response, created.Value);
    }

    private static async Task AssertInvalidIntegerKeyStatusesAsync(
        Func<Task<IActionResult>> get,
        Func<Task<IActionResult>> put,
        Func<Task<IActionResult>> delete)
    {
        Assert.Equal(StatusCodes.Status404NotFound, Assert.IsType<ObjectResult>(await get()).StatusCode);
        Assert.Equal(StatusCodes.Status500InternalServerError, Assert.IsType<ObjectResult>(await put()).StatusCode);
        Assert.Equal(StatusCodes.Status500InternalServerError, Assert.IsType<ObjectResult>(await delete()).StatusCode);
    }

    private static void AssertCurrentInvalidKeyDetail(IActionResult result, string id, string keyType = "Int32")
    {
        var problemDetails = Assert.IsAssignableFrom<ProblemDetails>(Assert.IsType<ObjectResult>(result).Value);

        Assert.Equal($"Path {id} not exists or is of a different type than PK of '{keyType}' was not found.", problemDetails.Detail);
    }

    private sealed class SimpleController(ICrudServiceFp<TestEntity, TestDto> service)
        : MlCrudControllerBase<TestEntity, TestDto, int>(service)
    {
    }

    private sealed class DuplexController(ICrudServiceFp<TestEntity, TestRequest, TestResponse> service)
        : MlCrudControllerBase<TestEntity, TestRequest, TestResponse, int>(service)
    {
    }

    private sealed class GuidController(ICrudServiceFp<TestEntity, TestDto> service)
        : MlCrudControllerBase<TestEntity, TestDto, Guid>(service)
    {
    }

    private sealed class StringKeyController(ICrudServiceFp<TestEntity, TestDto> service)
        : MlCrudControllerBase<TestEntity, TestDto, string>(service)
    {
    }

    private sealed class CompositeKeyController(ICrudServiceFp<TestEntity, TestDto> service)
        : MlCompositeKeyCrudControllerBase<TestEntity, TestDto>(service, entity => [entity.Id, entity.TenantId])
    {
    }

    private sealed class CompositeKeyDuplexController(ICrudServiceFp<TestEntity, TestRequest, TestResponse> service)
        : MlCompositeKeyCrudControllerBase<TestEntity, TestRequest, TestResponse>(service, entity => [entity.Id, entity.TenantId])
    {
    }

    public sealed class TestEntity
    {
        public TestEntity()
        {
        }

        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class TestDto
    {
    }

    public sealed class TestRequest
    {
    }

    public sealed class TestResponse
    {
    }
}