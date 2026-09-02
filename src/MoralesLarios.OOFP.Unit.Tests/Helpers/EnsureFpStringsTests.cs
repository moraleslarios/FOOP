using System.Text.RegularExpressions;

namespace MoralesLarios.OOFP.Unit.Tests.Helpers;

public class EnsureFpStringsTests
{

    #region NotNullOrEmpty

    [Fact]
    public void NotNullOrEmpty_withWhitespace_return_Valid()
    {
        var result = EnsureFp.NotNullOrEmpty("   ", "error");

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("   ");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NotNullOrEmpty_withNullOrEmpty_return_Fail(string? value)
    {
        var result = EnsureFp.NotNullOrEmpty(value!, "error");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void NotNullOrEmptyArg_addParamNameDetail()
    {
        var myText = string.Empty;

        var result = EnsureFp.NotNullOrEmptyArg(myText);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("myText");
    }

    #endregion

    #region MaxLength / MinLength / LengthBetween / LengthExactly

    [Theory]
    [InlineData("abc", 3, true)]
    [InlineData("abcd", 3, false)]
    [InlineData("", 3, true)]
    public void MaxLength_evaluateLength(string value, int maxLength, bool expectedValid)
    {
        var result = EnsureFp.MaxLength(value, maxLength, "error");

        result.IsValid.Should().Be(expectedValid);
    }

    [Fact]
    public void MaxLength_withNullValue_return_Fail()
    {
        var result = EnsureFp.MaxLength(null!, 10, "error");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void MaxLengthArg_addExpectedDetail()
    {
        var code = "123456";

        var result = EnsureFp.MaxLengthArg(code, 3);

        result.IsFail.Should().BeTrue();

        var details = result.SecureFailErrorsDetails();

        details.Details[PARAM_NAME_KEY].Should().Be("code");
        details.Details[EXPECTED_KEY].Should().Be(3);
        details.Errors.First().Message.Should().Contain("6");
    }

    [Theory]
    [InlineData("abc", 3, true)]
    [InlineData("ab", 3, false)]
    public void MinLength_evaluateLength(string value, int minLength, bool expectedValid)
        => EnsureFp.MinLength(value, minLength, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData("abc", 2, 4, true)]
    [InlineData("a", 2, 4, false)]
    [InlineData("abcde", 2, 4, false)]
    public void LengthBetween_evaluateRange(string value, int min, int max, bool expectedValid)
        => EnsureFp.LengthBetween(value, min, max, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData("abcd", 4, true)]
    [InlineData("abc", 4, false)]
    public void LengthExactly_evaluateLength(string value, int length, bool expectedValid)
        => EnsureFp.LengthExactly(value, length, "error").IsValid.Should().Be(expectedValid);

    #endregion

    #region Matches / NotMatches

    [Fact]
    public void Matches_withPatternOk_return_Valid()
        => EnsureFp.Matches("A123", @"^[A-Z]\d{3}$", "error").IsValid.Should().BeTrue();

    [Fact]
    public void Matches_withPatternKo_return_Fail()
        => EnsureFp.Matches("123A", @"^[A-Z]\d{3}$", "error").IsFail.Should().BeTrue();

    [Fact]
    public void Matches_withCompiledRegex_return_Valid()
    {
        var regex = new Regex(@"^\d+$", RegexOptions.Compiled);

        EnsureFp.Matches("998877", regex, "error").IsValid.Should().BeTrue();
    }

    [Fact]
    public void Matches_withNullValue_return_Fail()
        => EnsureFp.Matches(null!, @"^\d+$", "error").IsFail.Should().BeTrue();

    [Fact]
    public void NotMatches_withPatternKo_return_Valid()
        => EnsureFp.NotMatches("hello", @"^\d+$", "error").IsValid.Should().BeTrue();

    [Fact]
    public void NotMatches_withPatternOk_return_Fail()
        => EnsureFp.NotMatches("12345", @"^\d+$", "error").IsFail.Should().BeTrue();

    [Fact]
    public void MatchesArg_buildAutomaticMessage()
    {
        var nif = "XX";

        var result = EnsureFp.MatchesArg(nif, @"^\d{8}[A-Z]$");

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("nif");
    }

    #endregion

    #region StartsWith / EndsWith / Contains

    [Fact]
    public void StartsWith_withOrdinalIgnoreCase_return_Valid()
        => EnsureFp.StartsWith("HolaMundo", "hola", "error", StringComparison.OrdinalIgnoreCase).IsValid.Should().BeTrue();

    [Fact]
    public void StartsWith_withOrdinal_return_Fail()
        => EnsureFp.StartsWith("HolaMundo", "hola", "error").IsFail.Should().BeTrue();

    [Fact]
    public void EndsWith_withSuffixOk_return_Valid()
        => EnsureFp.EndsWith("fichero.txt", ".txt", "error").IsValid.Should().BeTrue();

    [Fact]
    public void EndsWith_withSuffixKo_return_Fail()
        => EnsureFp.EndsWith("fichero.txt", ".csv", "error").IsFail.Should().BeTrue();

    [Fact]
    public void ContainsText_withSubstringOk_return_Valid()
        => EnsureFp.ContainsText("abcdef", "cde", "error").IsValid.Should().BeTrue();

    [Fact]
    public void ContainsText_withSubstringKo_return_Fail()
        => EnsureFp.ContainsText("abcdef", "xyz", "error").IsFail.Should().BeTrue();

    [Fact]
    public void NotContainsText_withSubstringKo_return_Valid()
        => EnsureFp.NotContainsText("abcdef", "xyz", "error").IsValid.Should().BeTrue();

    [Fact]
    public void NotContainsTextArg_addParamNameDetail()
    {
        var password = "admin1234";

        var result = EnsureFp.NotContainsTextArg(password, "admin");

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("password");
    }

    #endregion

    #region IsOneOf

    [Fact]
    public void IsOneOf_withAllowedValue_return_Valid()
        => EnsureFp.IsOneOf("B", new[] { "A", "B", "C" }, "error").IsValid.Should().BeTrue();

    [Fact]
    public void IsOneOf_withNotAllowedValue_return_Fail()
        => EnsureFp.IsOneOf("Z", new[] { "A", "B", "C" }, "error").IsFail.Should().BeTrue();

    [Fact]
    public void IsOneOf_withIgnoreCaseComparer_return_Valid()
        => EnsureFp.IsOneOf("b", new[] { "A", "B", "C" }, "error", StringComparer.OrdinalIgnoreCase)
                   .IsValid.Should().BeTrue();

    [Fact]
    public void IsOneOfArg_generic_buildMessageWithAllowedValues()
    {
        var level = 7;

        var result = EnsureFp.IsOneOfArg(level, new[] { 1, 2, 3 });

        result.IsFail.Should().BeTrue();

        var details = result.SecureFailErrorsDetails();

        details.Details[PARAM_NAME_KEY].Should().Be("level");
        details.Errors.First().Message.Should().Contain("1");
    }

    #endregion

}
