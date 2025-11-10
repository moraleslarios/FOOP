namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsUIntTests
{
    [Fact]
    public void FromString_ValidUIntString_ReturnsValid()
    {
        var value = "123456789";
        StringAsUInt result = StringAsUInt.FromString(value);
        uint expected = 123456789;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidUIntString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsUInt.FromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidUIntString_ReturnsValidResult()
    {
        var value = "4000000000";
        var result = StringAsUInt.ByString(value);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidUIntString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsUInt> result = StringAsUInt.ByString(value);
        result.IsFail.Should().BeTrue();
    }
}
