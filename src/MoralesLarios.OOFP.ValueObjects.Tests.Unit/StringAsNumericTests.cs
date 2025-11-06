namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsNumericTests
{

    [Fact]
    public void FromString_ValidNumericString_ReturnsValid()
    {
        var value = "123,45";

        StringAsNumeric<decimal> result = StringAsNumeric<decimal>.FromString(value);

        decimal expected = 123.45m;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidNumericString_ThrowsArgumentNullException()
    {
        var value = "ABC";

        Action act = () => StringAsNumeric<decimal>.FromString(value);

        act.Should().Throw<FormatException>();
    }


    [Fact]
    public void ByString_ValidNumericString_ReturnsValidResult()
    {
        var value = "678.90";

        var result = StringAsNumeric<decimal>.ByString(value);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidNumericString_ReturnsFailResult()
    {
        var value = "XYZ";

        MlResult<StringAsNumeric<decimal>> result = StringAsNumeric<decimal>.ByString(value);

        result.IsFail.Should().BeTrue();
    }







}