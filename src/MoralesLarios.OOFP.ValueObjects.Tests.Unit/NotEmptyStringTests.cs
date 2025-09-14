namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class NotEmptyStringTests
{


    [Fact]
    public void ByString_ValidValue_ReturnsValid()
    {
        MlResult<NotEmptyString> result = NotEmptyString.ByString("Valid String");

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_FailValue_ReturnsFail()
    {
        MlResult<NotEmptyString> result = NotEmptyString.ByString("");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void FromString_FailValue_ThrowException()
    {
        Action act = () => NotEmptyString.FromString("");

        act.Should().Throw<ArgumentException>();
    }

}
