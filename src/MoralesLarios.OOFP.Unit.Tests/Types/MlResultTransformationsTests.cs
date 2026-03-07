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

    [Fact]
    public void BuildErrorMessage_withCustomMessage_returnCustomMessage()
    {
        var ex = new InvalidOperationException("boom");

        string result = MlResultTransformations.BuildErrorMessage("custom-message", ex);

        result.Should().Be("custom-message");
    }

    [Fact]
    public void BuildErrorMessage_withoutCustomMessage_returnDefaultMessage()
    {
        var ex = new InvalidOperationException("boom");

        string result = MlResultTransformations.BuildErrorMessage((string)null!, ex);
        string expected = DEFAULT_EX_ERROR_MESSAGE(ex);

        result.Should().Be(expected);
    }




}
