namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsIntTests
{

    [Fact]
    public void FromString_ValidIntString_ReturnsValid()
    {
        var value = "12345";
        StringAsInt result = StringAsInt.FromString(value);
        int expected = 12345;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidIntString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsInt.FromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidIntString_ReturnsValidResult()
    {
        var value = "67890";
        var result = StringAsInt.ByString(value);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidIntString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsInt> result = StringAsInt.ByString(value);
        result.IsFail.Should().BeTrue();
    }



}
