namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class RegexValueTests
{
    [Fact]
    public void RegexValue_ValueMatchesPattern_ReturnsValid()
    {
        var value   = "Hello";              // Capital first letter then lowercase
        var pattern = "^[A-Z][a-z]+$";      // Pattern requiring first capital then lowercase letters

        var result = RegexValue.ByRegex(value, pattern);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RegexValue_EmptyValue_ReturnsFail()
    {
        var value   = string.Empty;
        var pattern = "^[A-Z][a-z]+$";

        var result = RegexValue.ByRegex(value, pattern);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void RegexValue_ValueDoesNotMatchPattern_ReturnsFail()
    {
        var value   = "XX";         // Not digits
        var pattern = "^[0-9]+$";   // Only digits

        var result = RegexValue.ByRegex(value, pattern);

        result.IsFail.Should().BeTrue();
    }
}
