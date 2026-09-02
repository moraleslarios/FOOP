// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Unit.Tests.Helpers;

/// <summary>
/// Tests de la agregación de validaciones de EnsureFp: All, AllOrFirst y Any (y sus versiones asíncronas).
/// </summary>
public class EnsureFpAggregationTests
{

    private static Func<string, MlResult<string>> NotEmptyRule
        => x => EnsureFp.That(x, ! string.IsNullOrWhiteSpace(x), "no puede estar vacío");

    private static Func<string, MlResult<string>> MinLengthRule(int min)
        => x => EnsureFp.That(x, x is not null && x.Length >= min, $"longitud mínima {min}");

    private static Func<string, MlResult<string>> StartsWithRule(string prefix)
        => x => EnsureFp.That(x, x is not null && x.StartsWith(prefix), $"debe empezar por {prefix}");


    #region All

    [Fact]
    public void All_allRulesValid_returns_Valid()
    {
        var result = EnsureFp.All("hola mundo", NotEmptyRule, MinLengthRule(3), StartsWithRule("hola"));

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("hola mundo");
    }

    [Fact]
    public void All_severalRulesFail_merges_all_errors()
    {
        var result = EnsureFp.All("ab", MinLengthRule(5), StartsWithRule("hola"));

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.Should().HaveCount(2);
        result.SecureFailErrorsDetails().Errors.Select(x => x.Message)
              .Should().Contain("longitud mínima 5")
              .And.Contain("debe empezar por hola");
    }

    [Fact]
    public void All_oneRuleFails_returns_that_single_error()
    {
        var result = EnsureFp.All("hola", MinLengthRule(3), StartsWithRule("xxx"));

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.Should().HaveCount(1);
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Be("debe empezar por xxx");
    }

    [Fact]
    public void All_evaluates_all_rules_even_after_a_failure()
    {
        var executions = 0;

        Func<int, MlResult<int>> counterRule = x => { executions++; return MlResult<int>.Fail("fallo"); };

        var result = EnsureFp.All(1, counterRule, counterRule, counterRule);

        result.IsFail.Should().BeTrue();
        executions.Should().Be(3);
    }

    [Fact]
    public void All_noValidators_returns_Valid()
    {
        var result = EnsureFp.All("dato");

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("dato");
    }

    [Fact]
    public void All_nullValidatorsArray_returns_Valid()
    {
        Func<string, MlResult<string>>[] validators = null!;

        var result = EnsureFp.All("dato", validators);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void All_withEnumerableOfValidators_works()
    {
        var validators = new List<Func<string, MlResult<string>>> { MinLengthRule(5), StartsWithRule("hola") };

        var result = EnsureFp.All("ab", validators);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.Should().HaveCount(2);
    }

    [Fact]
    public void AllResults_allValid_returns_Valid()
    {
        var result = EnsureFp.AllResults("dato", MlResult<string>.Valid("dato"), MlResult<string>.Valid("dato"));

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("dato");
    }

    [Fact]
    public void AllResults_someFail_merges_errors()
    {
        var result = EnsureFp.AllResults("dato",
                                         MlResult<string>.Valid("dato"),
                                         MlResult<string>.Fail("error 1"),
                                         MlResult<string>.Fail("error 2"));

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.Should().HaveCount(2);
    }

    [Fact]
    public void All_mergesDetails_from_failed_rules()
    {
        Func<int, MlResult<int>> rule1 = _ => MlErrorsDetails.FromErrorDetails("e1", "Key1", "V1").ToMlResultFail<int>();
        Func<int, MlResult<int>> rule2 = _ => MlErrorsDetails.FromErrorDetails("e2", "Key2", "V2").ToMlResultFail<int>();

        var result = EnsureFp.All(1, rule1, rule2);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details.Should().ContainKey("Key1");
        result.SecureFailErrorsDetails().Details.Should().ContainKey("Key2");
    }

    #endregion

    #region AllOrFirst

    [Fact]
    public void AllOrFirst_allRulesValid_returns_Valid()
    {
        var result = EnsureFp.AllOrFirst("hola mundo", NotEmptyRule, MinLengthRule(3), StartsWithRule("hola"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void AllOrFirst_returns_only_the_first_error()
    {
        var result = EnsureFp.AllOrFirst("ab", MinLengthRule(5), StartsWithRule("hola"));

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.Should().HaveCount(1);
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Be("longitud mínima 5");
    }

    [Fact]
    public void AllOrFirst_shortcircuits_remaining_rules()
    {
        var executions = 0;

        Func<int, MlResult<int>> counterRule = x => { executions++; return MlResult<int>.Fail("fallo"); };

        var result = EnsureFp.AllOrFirst(1, counterRule, counterRule, counterRule);

        result.IsFail.Should().BeTrue();
        executions.Should().Be(1);
    }

    [Fact]
    public void AllOrFirst_noValidators_returns_Valid()
    {
        var result = EnsureFp.AllOrFirst("dato");

        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Any

    [Fact]
    public void Any_oneRuleValid_returns_Valid()
    {
        var result = EnsureFp.Any("hola", MinLengthRule(50), StartsWithRule("hola"));

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("hola");
    }

    [Fact]
    public void Any_shortcircuits_when_a_rule_succeeds()
    {
        var executions = 0;

        Func<int, MlResult<int>> okRule   = x => { executions++; return MlResult<int>.Valid(x); };
        Func<int, MlResult<int>> failRule = x => { executions++; return MlResult<int>.Fail("fallo"); };

        var result = EnsureFp.Any(1, okRule, failRule, failRule);

        result.IsValid.Should().BeTrue();
        executions.Should().Be(1);
    }

    [Fact]
    public void Any_allRulesFail_merges_all_errors()
    {
        var result = EnsureFp.Any("ab", MinLengthRule(5), StartsWithRule("hola"));

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.Should().HaveCount(2);
    }

    [Fact]
    public void Any_noValidators_returns_Valid()
    {
        var result = EnsureFp.Any("dato");

        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Versiones asíncronas

    [Fact]
    public async Task AllAsync_allRulesValid_returns_Valid()
    {
        var result = await EnsureFp.AllAsync("hola",
                                             x => MlResult<string>.Valid(x).ToAsync(),
                                             x => MlResult<string>.Valid(x).ToAsync());

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().Be("hola");
    }

    [Fact]
    public async Task AllAsync_severalRulesFail_merges_all_errors()
    {
        var result = await EnsureFp.AllAsync("hola",
                                             _ => MlResult<string>.Fail("error 1").ToAsync(),
                                             _ => MlResult<string>.Fail("error 2").ToAsync());

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task AllAsync_noValidators_returns_Valid()
    {
        var result = await EnsureFp.AllAsync("dato");

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task AllOrFirstAsync_returns_only_the_first_error()
    {
        var executions = 0;

        Func<string, Task<MlResult<string>>> failRule = _ => { executions++; return MlResult<string>.Fail("fallo").ToAsync(); };

        var result = await EnsureFp.AllOrFirstAsync("dato", failRule, failRule);

        result.IsFail.Should().BeTrue();
        executions.Should().Be(1);
    }

    [Fact]
    public async Task AllOrFirstAsync_allValid_returns_Valid()
    {
        var result = await EnsureFp.AllOrFirstAsync("dato", x => MlResult<string>.Valid(x).ToAsync());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_oneRuleValid_returns_Valid()
    {
        var result = await EnsureFp.AnyAsync("dato",
                                             _ => MlResult<string>.Fail("error 1").ToAsync(),
                                             x => MlResult<string>.Valid(x).ToAsync());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_allRulesFail_merges_all_errors()
    {
        var result = await EnsureFp.AnyAsync("dato",
                                             _ => MlResult<string>.Fail("error 1").ToAsync(),
                                             _ => MlResult<string>.Fail("error 2").ToAsync());

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.Should().HaveCount(2);
    }

    #endregion

}
