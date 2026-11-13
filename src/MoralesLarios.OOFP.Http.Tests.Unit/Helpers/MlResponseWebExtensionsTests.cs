using System.Net;
using MoralesLarios.OOFP.Http.Helpers;

namespace MoralesLarios.OOFP.Http.Tests.Unit.Helpers;

public class MlResponseWebExtensionsTests
{
    [Fact]
    public async Task ReadErrorDescriptionAsync_OmitsResponseBodyByDefault()
    {
        MlResponseWebExtensions.IncludeResponseBodyInErrors.Should().BeFalse();
        using var response = new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
            Content = new StringContent("respuesta remota")
        };

        var description = await response.ReadErrorDescriptionAsync();

        description.Should().Contain("Detalle error: [Contenido del error omitido por configuración]");
        description.Should().NotContain("respuesta remota");
    }
}