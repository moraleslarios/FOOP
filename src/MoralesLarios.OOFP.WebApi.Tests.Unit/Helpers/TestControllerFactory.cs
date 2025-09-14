using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MoralesLarios.OOFP.WebApi.Tests.Unit.Helpers;

internal static class TestControllerFactory
{
    // Define a minimal concrete controller for testing purposes
    private class TestController : ControllerBase { }

    public static ControllerBase Create(Action<DefaultHttpContext>? configureHttpContext = null)
    {
        var httpContext = new DefaultHttpContext();

        // Provide a minimal IServiceProvider if something resolves services later
        httpContext.RequestServices = new ServiceCollection().BuildServiceProvider();

        configureHttpContext?.Invoke(httpContext);

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ControllerActionDescriptor()
            {
                ControllerName = "Test",
                ActionName = "TestAction",
            });

        return new TestController
        {
            ControllerContext = new ControllerContext(actionContext)
        };
    }
}
