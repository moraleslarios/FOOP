namespace MoralesLarios.OOFP.Unit.Tests.Helpers;

public class EnsureFpNumbersTests
{

    #region Comparaciones

    [Theory]
    [InlineData(10, 5, true)]
    [InlineData(5, 5, false)]
    [InlineData(1, 5, false)]
    public void GreaterThan_evaluateValue(int value, int limit, bool expectedValid)
        => EnsureFp.GreaterThan(value, limit, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData(10, 5, true)]
    [InlineData(5, 5, true)]
    [InlineData(1, 5, false)]
    public void GreaterOrEqual_evaluateValue(int value, int limit, bool expectedValid)
        => EnsureFp.GreaterOrEqual(value, limit, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData(1, 5, true)]
    [InlineData(5, 5, false)]
    [InlineData(10, 5, false)]
    public void LessThan_evaluateValue(int value, int limit, bool expectedValid)
        => EnsureFp.LessThan(value, limit, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData(1, 5, true)]
    [InlineData(5, 5, true)]
    [InlineData(10, 5, false)]
    public void LessOrEqual_evaluateValue(int value, int limit, bool expectedValid)
        => EnsureFp.LessOrEqual(value, limit, "error").IsValid.Should().Be(expectedValid);

    [Fact]
    public void GreaterThan_withDecimals_return_Valid()
        => EnsureFp.GreaterThan(10.5m, 10.4m, "error").IsValid.Should().BeTrue();

    [Fact]
    public void GreaterThan_withDates_return_Valid()
        => EnsureFp.GreaterThan(new DateTime(2024, 1, 2), new DateTime(2024, 1, 1), "error").IsValid.Should().BeTrue();

    [Fact]
    public void GreaterThanArg_addExpectedDetail()
    {
        var age = 3;

        var result = EnsureFp.GreaterThanArg(age, 18);

        result.IsFail.Should().BeTrue();

        var details = result.SecureFailErrorsDetails();

        details.Details[PARAM_NAME_KEY].Should().Be("age");
        details.Details[EXPECTED_KEY].Should().Be(18);
        details.Details[VALUE_KEY].Should().Be(3);
    }

    #endregion

    #region Rangos

    [Theory]
    [InlineData(5, 1, 10, true)]
    [InlineData(1, 1, 10, true)]
    [InlineData(10, 1, 10, true)]
    [InlineData(0, 1, 10, false)]
    [InlineData(11, 1, 10, false)]
    public void InRange_evaluateRange(int value, int min, int max, bool expectedValid)
        => EnsureFp.InRange(value, min, max, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData(5, 1, 10, false)]
    [InlineData(0, 1, 10, true)]
    [InlineData(11, 1, 10, true)]
    public void OutOfRange_evaluateRange(int value, int min, int max, bool expectedValid)
        => EnsureFp.OutOfRange(value, min, max, "error").IsValid.Should().Be(expectedValid);

    [Fact]
    public void InRangeArg_buildAutomaticMessage()
    {
        var percentage = 150;

        var result = EnsureFp.InRangeArg(percentage, 0, 100);

        result.IsFail.Should().BeTrue();

        var message = result.SecureFailErrorsDetails().Errors.First().Message;

        message.Should().Contain("percentage");
        message.Should().Contain("100");
    }

    #endregion

    #region Signos

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void Positive_evaluateSign(int value, bool expectedValid)
        => EnsureFp.Positive(value, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, true)]
    [InlineData(-1, false)]
    public void NotNegative_evaluateSign(int value, bool expectedValid)
        => EnsureFp.NotNegative(value, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, false)]
    [InlineData(1, false)]
    public void Negative_evaluateSign(int value, bool expectedValid)
        => EnsureFp.Negative(value, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData(-1, true)]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void NotZero_evaluateValue(int value, bool expectedValid)
        => EnsureFp.NotZero(value, "error").IsValid.Should().Be(expectedValid);

    [Fact]
    public void Positive_withDecimal_return_Fail()
        => EnsureFp.Positive(-0.01m, "error").IsFail.Should().BeTrue();

    [Fact]
    public void NotZeroArg_addParamNameDetail()
    {
        var divisor = 0;

        var result = EnsureFp.NotZeroArg(divisor);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("divisor");
    }

    [Fact]
    public void PositiveArg_withValidValue_return_sameValue()
    {
        var amount = 250.75m;

        var result = EnsureFp.PositiveArg(amount);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(250.75m);
    }

    #endregion

}
