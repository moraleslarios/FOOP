namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsByteTests
{
    [Fact]
    public void FromString_ValidByteString_ReturnsValid()
    {
        var value = "123";
        StringAsByte result = StringAsByte.FromString(value);
        byte expected = 123;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidByteString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsByte.FromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidByteString_ReturnsValidResult()
    {
        var value = "250";
        var result = StringAsByte.ByString(value);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidByteString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsByte> result = StringAsByte.ByString(value);
        result.IsFail.Should().BeTrue();
    }
}
