namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringMinLengthTests
{
    [Fact]
    public void StringMinLength_ValueLessThanLenght_ReturnsValid()
    {
        var validString = "Hello";
        var maxLength   = 2;

        var result = StringMinLength.ByStringLength(validString, maxLength);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void StringMinLength_EmptyValue_ReturnsFail()
    {
        var validString = "";
        var maxLength   = 10;

        var result = StringMinLength.ByStringLength(validString, maxLength);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void StringMinLength_ValueLessThanLenght_ReturnsFail()
    {
        var validString = "XX";
        var maxLength   = 5;

        var result = StringMinLength.ByStringLength(validString, maxLength);

        result.IsFail.Should().BeTrue();
    }


}
