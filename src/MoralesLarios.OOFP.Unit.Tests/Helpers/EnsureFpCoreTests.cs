// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Unit.Tests.Helpers;

/// <summary>
/// Tests del núcleo de EnsureFp: mensajes perezosos, predicados perezosos,
/// guardias con mensaje automático y TryThat.
/// </summary>
public class EnsureFpCoreTests
{

    #region That con mensajes perezosos

    [Fact]
    public void That_lazyMessage_conditionTrue_returns_Valid()
    {
        var result = EnsureFp.That(5, true, () => "no debería construirse");

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(5);
    }

    [Fact]
    public void That_lazyMessage_conditionTrue_does_not_evaluate_builder()
    {
        var evaluated = false;

        var result = EnsureFp.That(5, true, () => { evaluated = true; return "mensaje"; });

        result.IsValid.Should().BeTrue();
        evaluated.Should().BeFalse();
    }

    [Fact]
    public void That_lazyMessage_conditionFalse_returns_Fail_with_message()
    {
        var result = EnsureFp.That(5, false, () => "valor incorrecto");

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Be("valor incorrecto");
    }

    [Fact]
    public void That_lazyErrorsDetails_conditionFalse_returns_Fail_with_details()
    {
        var result = EnsureFp.That(5, false, () => MlErrorsDetails.FromErrorDetails("fallo", "Key1", "Detail1"));

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Be("fallo");
        result.SecureFailErrorsDetails().Details.Should().ContainKey("Key1");
    }

    [Fact]
    public void That_lazyErrorsDetails_nullBuilder_returns_Fail()
    {
        Func<MlErrorsDetails> builder = null!;

        var result = EnsureFp.That(5, false, builder);

        result.IsFail.Should().BeTrue();
    }

    #endregion

    #region That con predicados perezosos

    [Fact]
    public void That_predicate_satisfied_returns_Valid()
    {
        var result = EnsureFp.That(10, x => x > 5, "debe ser mayor que 5");

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(10);
    }

    [Fact]
    public void That_predicate_notSatisfied_returns_Fail()
    {
        var result = EnsureFp.That(3, x => x > 5, "debe ser mayor que 5");

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Be("debe ser mayor que 5");
    }

    [Fact]
    public void That_predicate_null_returns_Fail()
    {
        Func<int, bool> predicate = null!;

        var result = EnsureFp.That(3, predicate, "predicado nulo");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void That_predicate_with_errorsDetails_notSatisfied_returns_Fail()
    {
        var errorsDetails = MlErrorsDetails.FromErrorMessage("fallo detalles");

        var result = EnsureFp.That(3, x => x > 5, errorsDetails);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Be("fallo detalles");
    }

    [Fact]
    public void That_predicate_with_valueBasedMessage_builds_message_from_value()
    {
        var result = EnsureFp.That(3, x => x > 5, x => $"el valor {x} no es mayor que 5");

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Be("el valor 3 no es mayor que 5");
    }

    [Fact]
    public void That_predicate_with_valueBasedDetails_builds_details_from_value()
    {
        var result = EnsureFp.That(3,
                                   x => x > 5,
                                   x => MlErrorsDetails.FromErrorMessageWithValue($"valor {x} inválido", x));

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Be("valor 3 inválido");
    }

    #endregion

    #region TryThat

    [Fact]
    public void TryThat_predicate_satisfied_returns_Valid()
    {
        var result = EnsureFp.TryThat("abc", x => x.Length == 3, "longitud incorrecta");

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("abc");
    }

    [Fact]
    public void TryThat_predicate_notSatisfied_returns_Fail()
    {
        var result = EnsureFp.TryThat("abcd", x => x.Length == 3, "longitud incorrecta");

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Be("longitud incorrecta");
    }

    [Fact]
    public void TryThat_predicate_throws_returns_Fail_with_exception_details()
    {
        string data = null!;

        var result = EnsureFp.TryThat(data, x => x.Length == 3, "longitud incorrecta");

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details.Should().ContainKey(EX_DESC_KEY);
    }

    [Fact]
    public void TryThat_predicate_throws_uses_exception_message_builder()
    {
        string data = null!;

        var result = EnsureFp.TryThat(data, x => x.Length == 3, ex => $"excepción capturada: {ex.GetType().Name}");

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Contain("NullReferenceException");
    }

    [Fact]
    public void TryThat_predicate_throws_with_errorsDetails_returns_Fail()
    {
        string data = null!;

        var result = EnsureFp.TryThat(data, x => x.Length == 3, MlErrorsDetails.FromErrorMessage("no usado"));

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details.Should().ContainKey(EX_DESC_KEY);
    }

    [Fact]
    public void TryThat_nullPredicate_returns_Fail()
    {
        Func<string, bool> predicate = null!;

        var result = EnsureFp.TryThat("abc", predicate, "mensaje");

        result.IsFail.Should().BeTrue();
    }

    #endregion

    #region Guardias con mensaje automático

    [Fact]
    public void NotNullArg_nullValue_returns_Fail_with_paramName_in_message()
    {
        string myArgument = null!;

        var result = EnsureFp.NotNullArg(myArgument);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Contain("myArgument");
    }

    [Fact]
    public void NotNullArg_nullValue_adds_paramName_detail()
    {
        string myArgument = null!;

        var result = EnsureFp.NotNullArg(myArgument);

        result.SecureFailErrorsDetails().Details.Should().ContainKey(PARAM_NAME_KEY);
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("myArgument");
    }

    [Fact]
    public void NotNullArg_notNullValue_returns_Valid()
    {
        var myArgument = "hola";

        var result = EnsureFp.NotNullArg(myArgument);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("hola");
    }

    [Fact]
    public void NotEmptyArg_emptyCollection_returns_Fail_with_paramName()
    {
        var myCollection = Enumerable.Empty<int>();

        var result = EnsureFp.NotEmptyArg(myCollection);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Contain("myCollection");
    }

    [Fact]
    public void NotEmptyArg_withItems_returns_Valid()
    {
        var myCollection = new[] { 1, 2, 3 };

        var result = EnsureFp.NotEmptyArg(myCollection);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NotNullEmptyOrWhitespaceArg_invalidValues_return_Fail(string? value)
    {
        var myText = value!;

        var result = EnsureFp.NotNullEmptyOrWhitespaceArg(myText);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Contain("myText");
    }

    [Fact]
    public void NotNullEmptyOrWhitespaceArg_validValue_returns_Valid()
    {
        var myText = "contenido";

        var result = EnsureFp.NotNullEmptyOrWhitespaceArg(myText);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ThatArg_conditionFalse_returns_Fail_with_paramName()
    {
        var age = 15;

        var result = EnsureFp.ThatArg(age, age >= 18);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Contain("age");
    }

    [Fact]
    public void ThatArg_predicateFalse_returns_Fail_with_value_detail()
    {
        var age = 15;

        var result = EnsureFp.ThatArg(age, x => x >= 18);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details.Should().ContainKey(VALUE_KEY);
        result.SecureFailErrorsDetails().Details[VALUE_KEY].Should().Be(15);
    }

    [Fact]
    public void ThatArg_predicateTrue_returns_Valid()
    {
        var age = 20;

        var result = EnsureFp.ThatArg(age, x => x >= 18);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be(20);
    }

    #endregion

}
