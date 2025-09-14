using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.WebApi.Tests.Unit.Helpers;

public class MlResultWebExtensionsTests
{


    [Fact]
    public void ToRepoGetActionResult_withErrorNotFound_returnsNotFound()
    {
        MlResult<DummyEntity> partialResult =
            MlErrorsDetails.FromErrorMessageDetails("Error", new Dictionary<string, object> { { "NotFound", 1 } });

        ControllerBase controller = TestControllerFactory.Create(); // If extension method needs controller (adjust as needed)

        IActionResult result = partialResult.ToRepoGetActionResult(controller);

        result.Should().BeOfType<NotFoundObjectResult>();
    }


    [Fact]
    public void ToRepoGetActionResult_withoutErrorNotFound_returnsDistinctNotFound()
    {
        MlResult<DummyEntity> partialResult =
            MlErrorsDetails.FromErrorMessageDetails("Error", new Dictionary<string, object> { { "XXX", 1 } });

        ControllerBase controller = TestControllerFactory.Create(); // If extension method needs controller (adjust as needed)

        IActionResult result = partialResult.ToRepoGetActionResult(controller);

        result.Should().NotBeOfType<NotFoundObjectResult>();
    }





}

public record class DummyEntity(int Id, string Name);
