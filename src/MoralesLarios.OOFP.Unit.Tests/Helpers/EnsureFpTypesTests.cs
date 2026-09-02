namespace MoralesLarios.OOFP.Unit.Tests.Helpers;

public class EnsureFpTypesTests
{

    private enum TestColor
    {
        Red   = 1,
        Green = 2,
        Blue  = 3
    }

    #region Guid

    [Fact]
    public void NotEmptyGuid_withRealGuid_return_Valid()
    {
        var id = Guid.NewGuid();

        var result = EnsureFp.NotEmptyGuid(id, "error");

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(id);
    }

    [Fact]
    public void NotEmptyGuid_withEmptyGuid_return_Fail()
        => EnsureFp.NotEmptyGuid(Guid.Empty, "error").IsFail.Should().BeTrue();

    [Fact]
    public void NotEmptyGuidArg_addParamNameDetail()
    {
        var customerId = Guid.Empty;

        var result = EnsureFp.NotEmptyGuidArg(customerId);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("customerId");
    }

    [Fact]
    public void NotNullNotEmptyGuid_withValue_return_unwrappedValue()
    {
        Guid? id = Guid.NewGuid();

        var result = EnsureFp.NotNullNotEmptyGuid(id, "error");

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(id!.Value);
    }

    [Fact]
    public void NotNullNotEmptyGuid_withNull_return_Fail()
        => EnsureFp.NotNullNotEmptyGuid(null, "error").IsFail.Should().BeTrue();

    [Fact]
    public void NotNullNotEmptyGuidArg_withEmptyGuid_return_Fail()
    {
        Guid? orderId = Guid.Empty;

        var result = EnsureFp.NotNullNotEmptyGuidArg(orderId);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("orderId");
    }

    #endregion

    #region Enumerados

    [Fact]
    public void IsDefined_withDefinedValue_return_Valid()
        => EnsureFp.IsDefined(TestColor.Green, "error").IsValid.Should().BeTrue();

    [Fact]
    public void IsDefined_withUndefinedValue_return_Fail()
        => EnsureFp.IsDefined((TestColor)99, "error").IsFail.Should().BeTrue();

    [Fact]
    public void IsDefinedArg_buildMessageWithEnumType()
    {
        var color = (TestColor)99;

        var result = EnsureFp.IsDefinedArg(color);

        result.IsFail.Should().BeTrue();

        var details = result.SecureFailErrorsDetails();

        details.Details[PARAM_NAME_KEY].Should().Be("color");
        details.Errors.First().Message.Should().Contain(nameof(TestColor));
    }

    #endregion

    #region Fechas

    [Fact]
    public void InFuture_withFutureDate_return_Valid()
        => EnsureFp.InFuture(DateTime.Now.AddDays(1), "error").IsValid.Should().BeTrue();

    [Fact]
    public void InFuture_withPastDate_return_Fail()
        => EnsureFp.InFuture(DateTime.Now.AddDays(-1), "error").IsFail.Should().BeTrue();

    [Fact]
    public void InFuture_withUtcDate_useUtcNow()
        => EnsureFp.InFuture(DateTime.UtcNow.AddMinutes(5), "error").IsValid.Should().BeTrue();

    [Fact]
    public void InPast_withPastDate_return_Valid()
        => EnsureFp.InPast(DateTime.Now.AddDays(-1), "error").IsValid.Should().BeTrue();

    [Fact]
    public void InPast_withFutureDate_return_Fail()
        => EnsureFp.InPast(DateTime.Now.AddDays(1), "error").IsFail.Should().BeTrue();

    [Fact]
    public void InFuture_withDateTimeOffset_return_Valid()
        => EnsureFp.InFuture(DateTimeOffset.UtcNow.AddHours(1), "error").IsValid.Should().BeTrue();

    [Fact]
    public void InPast_withDateOnly_return_Valid()
        => EnsureFp.InPast(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), "error").IsValid.Should().BeTrue();

    [Fact]
    public void InFutureArg_addParamNameDetail()
    {
        var expirationDate = DateTime.Now.AddYears(-1);

        var result = EnsureFp.InFutureArg(expirationDate);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("expirationDate");
    }

    #endregion

    #region NotDefault

    [Fact]
    public void NotDefault_withNonDefaultValue_return_Valid()
        => EnsureFp.NotDefault(7, "error").IsValid.Should().BeTrue();

    [Fact]
    public void NotDefault_withDefaultValue_return_Fail()
        => EnsureFp.NotDefault(0, "error").IsFail.Should().BeTrue();

    [Fact]
    public void NotDefault_withNullReference_return_Fail()
        => EnsureFp.NotDefault<string>(null!, "error").IsFail.Should().BeTrue();

    [Fact]
    public void NotDefaultArg_addParamNameDetail()
    {
        var startDate = default(DateTime);

        var result = EnsureFp.NotDefaultArg(startDate);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("startDate");
    }

    #endregion

    #region Uri

    [Fact]
    public void IsAbsoluteUri_withAbsoluteUri_return_Valid()
        => EnsureFp.IsAbsoluteUri(new Uri("https://moraleslarios.com"), "error").IsValid.Should().BeTrue();

    [Fact]
    public void IsAbsoluteUri_withRelativeUri_return_Fail()
        => EnsureFp.IsAbsoluteUri(new Uri("/api/values", UriKind.Relative), "error").IsFail.Should().BeTrue();

    [Fact]
    public void IsAbsoluteUri_withNull_return_Fail()
        => EnsureFp.IsAbsoluteUri(null!, "error").IsFail.Should().BeTrue();

    [Fact]
    public void IsValidUri_withValidUrl_return_builtUri()
    {
        var result = EnsureFp.IsValidUri("https://moraleslarios.com/api", "error");

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Host.Should().Be("moraleslarios.com");
    }

    [Fact]
    public void IsValidUri_withInvalidUrl_return_Fail()
        => EnsureFp.IsValidUri("no es una uri", "error").IsFail.Should().BeTrue();

    [Fact]
    public void IsValidUriArg_addParamNameDetail()
    {
        var endpoint = "%%%";

        var result = EnsureFp.IsValidUriArg(endpoint);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("endpoint");
    }

    #endregion

    #region Correo electrónico

    [Theory]
    [InlineData("moraleslarios@gmail.com")]
    [InlineData("juan.francisco@sub.dominio.es")]
    public void IsValidEmail_withValidEmails_return_Valid(string email)
        => EnsureFp.IsValidEmail(email, "error").IsValid.Should().BeTrue();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("sin-arroba")]
    [InlineData("sin@dominio")]
    [InlineData("dos@@arrobas.com")]
    public void IsValidEmail_withInvalidEmails_return_Fail(string? email)
        => EnsureFp.IsValidEmail(email!, "error").IsFail.Should().BeTrue();

    [Fact]
    public void IsValidEmailArg_addParamNameDetail()
    {
        var userEmail = "malo";

        var result = EnsureFp.IsValidEmailArg(userEmail);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("userEmail");
    }

    #endregion

    #region Sistema de ficheros

    [Fact]
    public void FileExists_withExistingFile_return_Valid()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");

        File.WriteAllText(path, "contenido");

        try
        {
            EnsureFp.FileExists(path, "error").IsValid.Should().BeTrue();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void FileExists_withMissingFile_return_Fail()
        => EnsureFp.FileExists(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt"), "error")
                   .IsFail.Should().BeTrue();

    [Fact]
    public void DirectoryExists_withExistingDirectory_return_Valid()
        => EnsureFp.DirectoryExists(Path.GetTempPath(), "error").IsValid.Should().BeTrue();

    [Fact]
    public void DirectoryExistsArg_withMissingDirectory_return_FailWithPathInMessage()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var result = EnsureFp.DirectoryExistsArg(directory);

        result.IsFail.Should().BeTrue();

        var details = result.SecureFailErrorsDetails();

        details.Details[PARAM_NAME_KEY].Should().Be("directory");
        details.Errors.First().Message.Should().Contain(directory);
    }

    #endregion

    #region Nullables de tipo valor

    [Fact]
    public void NotNullValue_withValue_return_unwrappedValue()
    {
        int? age = 42;

        var result = EnsureFp.NotNullValue(age, "error");

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(42);
    }

    [Fact]
    public void NotNullValue_withNull_return_Fail()
        => EnsureFp.NotNullValue<int>(null, "error").IsFail.Should().BeTrue();

    [Fact]
    public void NotNullValueArg_addParamNameDetail()
    {
        DateTime? birthDate = null;

        var result = EnsureFp.NotNullValueArg(birthDate);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("birthDate");
    }

    [Fact]
    public void NotNullValueThat_withValueAndPredicateOk_return_Valid()
        => EnsureFp.NotNullValueThat<int>(10, x => x > 5, "error").IsValid.Should().BeTrue();

    [Fact]
    public void NotNullValueThat_withValueAndPredicateKo_return_Fail()
        => EnsureFp.NotNullValueThat<int>(1, x => x > 5, "error").IsFail.Should().BeTrue();

    #endregion

}
