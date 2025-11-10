namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsSByteTests
{
    [Fact]
    public void FromString_ValidSByteString_ReturnsValid()
    {
        var value = "-123";
        StringAsSByte result = StringAsSByte.FromString(value);
        sbyte expected = -123;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidSByteString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsSByte.FromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidSByteString_ReturnsValidResult()
    {
        var value = "100";
        var result = StringAsSByte.ByString(value);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidSByteString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsSByte> result = StringAsSByte.ByString(value);
        result.IsFail.Should().BeTrue();
    }
}
