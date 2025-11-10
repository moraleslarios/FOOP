namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsLongTests
{
    [Fact]
    public void FromString_ValidLongString_ReturnsValid()
    {
        var value = "123456789012345";
        StringAsLong result = StringAsLong.FromString(value);
        long expected = 123456789012345;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidLongString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsLong.FromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidLongString_ReturnsValidResult()
    {
        var value = "9000000000000000000";
        var result = StringAsLong.ByString(value);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidLongString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsLong> result = StringAsLong.ByString(value);
        result.IsFail.Should().BeTrue();
    }
}
