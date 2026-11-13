using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MoralesLarios.OOFP.AspNetCore.Controllers.Tests.Unit;

public class EncodedRouteValueTests
{
    [Fact]
    public async Task Routing_DecodesSingleEncodedCommaBeforeController()
    {
        var routeValue = await GetRouteValueAsync("ACME%2CSL,42");

        Assert.Equal("ACME,SL,42", routeValue);
    }

    [Fact]
    public async Task Routing_PreservesComponentEscapeWhenPathIsDoubleEncoded()
    {
        var routeValue = await GetRouteValueAsync("ACME%252CSL%2C42");

        Assert.Equal("ACME%2CSL,42", routeValue);
    }

    [Fact]
    public async Task Routing_KeepsEncodedSlashInsideOneRouteValue()
    {
        var routeValue = await GetRouteValueAsync("uno%2Fdos");

        Assert.Equal("uno%2Fdos", routeValue);
    }

    private static async Task<string> GetRouteValueAsync(string ids)
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services => services.AddRouting())
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints => endpoints.MapGet(
                    "/id-str/{ids}",
                    context => context.Response.WriteAsync((string)context.Request.RouteValues["ids"]!)));
            });
        using var server = new TestServer(builder);
        using var client = server.CreateClient();
        using var response = await client.GetAsync($"/id-str/{ids}");

        return await response.Content.ReadAsStringAsync();
    }
}