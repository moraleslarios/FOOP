namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsUShortTests
{
    [Fact]
    public void FromString_ValidUShortString_ReturnsValid()
    {
        var value = "45678";
        StringAsUShort result = StringAsUShort.FromString(value);
        ushort expected = 45678;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidUShortString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsUShort.FromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidUShortString_ReturnsValidResult()
    {
        var value = "60000";
        var result = StringAsUShort.ByString(value);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidUShortString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsUShort> result = StringAsUShort.ByString(value);
        result.IsFail.Should().BeTrue();
    }
}
