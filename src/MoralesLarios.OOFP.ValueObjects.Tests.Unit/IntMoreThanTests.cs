namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class IntMoreThanTests
{

    [Fact]
    public void IntMoreThan_ValueMoreThanLenght_ReturnsValid()
    {
        var value     = 8;
        var minLenght = 5;

        var result = IntMoreThan.ByIntLength(value, minLenght);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IntMoreThan_ValueMoreThanLenght_ReturnsFail()
    {
        var value     = 2;
        var minLenght = 5;

        var result = IntMoreThan.ByIntLength(value, minLenght);

        result.IsFail.Should().BeTrue();
    }




}
