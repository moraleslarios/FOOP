namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsDecimalTests
{
    [Fact]
    public void FromString_ValidDecimalString_ReturnsValid()
    {
        var value = "123,456789";
        StringAsDecimal result = StringAsDecimal.FromString(value);
        decimal expected = 123.456789m;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidDecimalString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsDecimal.FromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidDecimalString_ReturnsValidResult()
    {
        var value = "99999.99";
        var result = StringAsDecimal.ByString(value);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidDecimalString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsDecimal> result = StringAsDecimal.ByString(value);
        result.IsFail.Should().BeTrue();
    }
}
