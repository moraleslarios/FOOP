namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class IntBetweenTests
{

     [Fact]
    public void IntBetween_ValueBetweenLimits_ReturnsValid()
    {
        var value = 5;
        var minLength   = 4;
        var maxLength   = 35;

        var result = IntBetween.ByIntLength(value, minLength, maxLength);

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public void IntBetween_ValueLessThanMinLenght_ReturnsFail()
    {
        var value = 2;
        var minLength   = 4;
        var maxLength   = 15;

        var result = IntBetween.ByIntLength(value, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void IntBetween_ValueMoreThanMaxLenght_ReturnsFail()
    {
        var value = 16;
        var minLength   = 4;
        var maxLength   = 15;

        var result = IntBetween.ByIntLength(value, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }



}
