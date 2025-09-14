namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class DecimalMoreThanTests
{
    [Fact]
    public void DecimalMoreThan_ValueMoreThanLength_ReturnsValid()
    {
        decimal value = 8.5m;
        decimal length = 5.2m;

        var result = DecimalMoreThan.ByDecimalLength(value, length);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DecimalMoreThan_ValueNotMoreThanLength_ReturnsFail()
    {
        decimal value = 2.1m;
        decimal length = 5.0m;

        var result = DecimalMoreThan.ByDecimalLength(value, length);

        result.IsFail.Should().BeTrue();
    }
}
