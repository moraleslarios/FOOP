namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class DecimalBetweenTests
{
    [Fact]
    public void DecimalBetween_ValueBetweenLimits_ReturnsValid()
    {
        decimal value = 5.5m;
        decimal minLength = 4.1m;
        decimal maxLength = 35.7m;

        var result = DecimalBetween.ByDecimalLength(value, minLength, maxLength);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DecimalBetween_ValueLessThanMinLength_ReturnsFail()
    {
        decimal value = 2.0m;
        decimal minLength = 4.1m;
        decimal maxLength = 15.0m;

        var result = DecimalBetween.ByDecimalLength(value, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void DecimalBetween_ValueMoreThanMaxLength_ReturnsFail()
    {
        decimal value = 16.2m;
        decimal minLength = 4.1m;
        decimal maxLength = 15.0m;

        var result = DecimalBetween.ByDecimalLength(value, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }
}
