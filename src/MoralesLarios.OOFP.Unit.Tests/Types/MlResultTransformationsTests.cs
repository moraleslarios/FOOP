namespace MoralesLarios.OOFP.Unit.Tests.Types;

public class MlResultTransformationsTests
{

    [Fact]
    public void SecureToValueObject_sourceNoMlResult_ThrowArgumentExcepticon()
    {
        object source = new TestType(1, "hi", DateTime.Today);

        Action act = () => source.SecureGetValueFromMlResultBoxed();

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SecureToValueObject_sourceMlResult_ReturnValueObject()
    {
        object source = 1.ToMlResultValid();

        object result = source.SecureGetValueFromMlResultBoxed();

        object expected = 1;

        result.Should().Be(expected);
    }




}
