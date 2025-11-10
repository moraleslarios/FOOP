namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsShortTests
{
    [Fact]
    public void FromString_ValidShortString_ReturnsValid()
    {
        var value = "12345";
        StringAsShort result = StringAsShort.FromString(value);
        short expected = 12345;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidShortString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsShort.FromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidShortString_ReturnsValidResult()
    {
        var value = "30000";
        var result = StringAsShort.ByString(value);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidShortString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsShort> result = StringAsShort.ByString(value);
        result.IsFail.Should().BeTrue();
    }
}
