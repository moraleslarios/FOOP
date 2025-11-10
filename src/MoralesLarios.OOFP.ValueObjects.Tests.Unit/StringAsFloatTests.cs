namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsFloatTests
{
    [Fact]
    public void FromString_ValidFloatString_ReturnsValid()
    {
        var value = "123,45";
        StringAsFloat result = StringAsFloat.FromString(value);
        float expected = 123.45f;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidFloatString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsFloat.FromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidFloatString_ReturnsValidResult()
    {
        var value = "3.14159";
        var result = StringAsFloat.ByString(value);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidFloatString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsFloat> result = StringAsFloat.ByString(value);
        result.IsFail.Should().BeTrue();
    }
}
