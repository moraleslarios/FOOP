namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsULongTests
{
    [Fact]
    public void FromString_ValidULongString_ReturnsValid()
    {
        var value = "123456789012345";
        StringAsULong result = StringAsULong.FromString(value);
        ulong expected = 123456789012345;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidULongString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsULong.FromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidULongString_ReturnsValidResult()
    {
        var value = "18000000000000000000";
        var result = StringAsULong.ByString(value);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidULongString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsULong> result = StringAsULong.ByString(value);
        result.IsFail.Should().BeTrue();
    }
}
