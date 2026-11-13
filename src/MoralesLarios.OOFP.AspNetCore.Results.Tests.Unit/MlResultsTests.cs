using Microsoft.AspNetCore.Mvc;

namespace MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit;

public class MlResultsTests
{
    [Fact]
    public void BadRequest_WithoutType_UsesAboutBlankTypeByDefault()
    {
        var result = MlResults.BadRequest(errors: (Dictionary<string, object>)null!);

        var details = result.Should().BeOfType<ObjectResult>()
                            .Which.Value.Should().BeOfType<ExtendedProblemDetails>()
                            .Subject;

        details.Type.Should().Be("about:blank");
    }
}