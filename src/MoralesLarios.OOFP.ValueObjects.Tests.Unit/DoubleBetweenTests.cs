namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class DoubleBetweenTests
{
    [Fact]
    public void DoubleBetween_ValueBetweenLimits_ReturnsValid()
    {
        var value = 5.5;
        var minLength = 4.1;
        var maxLength = 35.7;

        var result = DoubleBetween.ByDoubleLength(value, minLength, maxLength);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DoubleBetween_ValueLessThanMinLength_ReturnsFail()
    {
        var value = 2.0;
        var minLength = 4.1;
        var maxLength = 15.0;

        var result = DoubleBetween.ByDoubleLength(value, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void DoubleBetween_ValueMoreThanMaxLength_ReturnsFail()
    {
        var value = 16.2;
        var minLength = 4.1;
        var maxLength = 15.0;

        var result = DoubleBetween.ByDoubleLength(value, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }
}
