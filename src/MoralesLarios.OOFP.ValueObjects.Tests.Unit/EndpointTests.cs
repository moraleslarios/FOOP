namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class EndpointTests
{
    [Fact]
    public void Endpoint_ValueMatchesPattern_ReturnsValid()
    {
        var value = "https://server:5590";

        var result = Endpoint.ByString(value);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Endpoint_EmptyValue_ReturnsFail()
    {
        var value = string.Empty;

        var result = Endpoint.ByString(value);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void Endpoint_ValueDoesNotMatchPattern_ReturnsFail()
    {
        var value = "http://server:5590"; // Wrong scheme http

        var result = Endpoint.ByString(value);

        result.IsFail.Should().BeTrue();
    }
}
