namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class DoubleLessThanTests
{
    [Fact]
    public void DoubleLessThan_ValueLessThanLength_ReturnsValid()
    {
        var value = 5.5;
        var length = 8.2;

        var result = DoubleLessThan.ByDoubleLength(value, length);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DoubleLessThan_ValueNotLessThanLength_ReturnsFail()
    {
        var value = 9.1;
        var length = 5.0;

        var result = DoubleLessThan.ByDoubleLength(value, length);

        result.IsFail.Should().BeTrue();
    }
}
