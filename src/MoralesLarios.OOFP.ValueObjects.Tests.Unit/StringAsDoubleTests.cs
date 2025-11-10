namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsDoubleTests
{
    [Fact]
    public void FromString_ValidDoubleString_ReturnsValid()
    {
        var value = "123,456789";
        StringAsDouble result = StringAsDouble.FromString(value);
        double expected = 123.456789;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidDoubleString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsDouble.FromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidDoubleString_ReturnsValidResult()
    {
        var value = "3.141592653589793";
        var result = StringAsDouble.ByString(value);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidDoubleString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsDouble> result = StringAsDouble.ByString(value);
        result.IsFail.Should().BeTrue();
    }
}
