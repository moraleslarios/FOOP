namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringBetweenLengthTests
{

    [Fact]
    public void StringBetweenLength_ValueBetweenLimits_ReturnsValid()
    {
        var validString = "Hello, how are you?";
        var minLength   = 4;
        var maxLength   = 35;

        var result = StringBetweenLength.ByStringLength(validString, minLength, maxLength);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void StringBetweenLength_EmptyValue_ReturnsFail()
    {
        var validString = "";
        var minLength   = 4;
        var maxLength   = 15;


        var result = StringBetweenLength.ByStringLength(validString, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void StringBetweenLength_ValueLessThanMinLenght_ReturnsFail()
    {
        var validString = "XX";
        var minLength   = 4;
        var maxLength   = 15;

        var result = StringBetweenLength.ByStringLength(validString, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void StringBetweenLength_ValueMoreThanMaxLenght_ReturnsFail()
    {
        var validString = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
        var minLength   = 4;
        var maxLength   = 15;

        var result = StringBetweenLength.ByStringLength(validString, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }

}
