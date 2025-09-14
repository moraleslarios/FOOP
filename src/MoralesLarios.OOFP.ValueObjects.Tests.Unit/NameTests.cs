namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class NameTests
{

    [Fact]
    public void Name_ValueLessThanLenght_ReturnsValid()
    {
        var validString = "Hello";

        var result = Name.ByString(validString);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Name_EmptyValue_ReturnsFail()
    {
        var validString = "";

        var result = Name.ByString(validString);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void Name_ValueLessThanLenght_ReturnsFail()
    {
        var validString = "XX";

        var result = Name.ByString(validString);

        result.IsFail.Should().BeTrue();
    }


}
