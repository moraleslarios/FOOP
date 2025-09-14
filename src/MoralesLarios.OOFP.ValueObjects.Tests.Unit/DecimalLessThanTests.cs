namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class DecimalLessThanTests
{
    [Fact]
    public void DecimalLessThan_ValueLessThanLength_ReturnsValid()
    {
        decimal value = 5.5m;
        decimal length = 8.2m;

        var result = DecimalLessThan.ByDecimalLength(value, length);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DecimalLessThan_ValueNotLessThanLength_ReturnsFail()
    {
        decimal value = 9.1m;
        decimal length = 5.0m;

        var result = DecimalLessThan.ByDecimalLength(value, length);

        result.IsFail.Should().BeTrue();
    }
}
