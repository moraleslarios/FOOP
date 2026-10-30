// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.Core.Tests.Unit.Types;

public class MlResultTryGetTests
{



    #region TryGetValue



    [Fact]
    public void TryGetValue_IsValid_ReturnsTrue_AndValue_OK()
    {
        MlResult<int> source = 42;

        var result = source.TryGetValue(out var value);

        result.Should().BeTrue();
        value .Should().Be(42);
    }



    [Fact]
    public void TryGetValue_IsFail_ReturnsFalse_AndDefault_OK()
    {
        MlResult<int> source = "miError".ToMlResultFail<int>();

        var result = source.TryGetValue(out var value);

        result.Should().BeFalse();
        value .Should().Be(default(int));
    }



    [Fact]
    public void TryGetValue_IsFail_ReferenceType_ReturnsNull_OK()
    {
        MlResult<string> source = "miError".ToMlResultFail<string>();

        var result = source.TryGetValue(out var value);

        result.Should().BeFalse();
        value .Should().BeNull();
    }



    [Fact]
    public void TryGetValue_IsValid_ReferenceType_ReturnsSameInstance_OK()
    {
        var                 expected = new List<int> { 1, 2, 3 };
        MlResult<List<int>> source   = expected;

        var result = source.TryGetValue(out var value);

        result.Should().BeTrue();
        value .Should().BeSameAs(expected);
    }



    #endregion



    #region TryGetErrors



    [Fact]
    public void TryGetErrors_IsFail_ReturnsTrue_AndDetails_OK()
    {
        MlResult<int> source = "miError".ToMlResultFail<int>();

        var result = source.TryGetErrors(out var errorsDetails);

        result                          .Should().BeTrue();
        errorsDetails                   .Should().NotBeNull();
        errorsDetails.GetErrorMessages().Should().Contain("miError");
    }



    [Fact]
    public void TryGetErrors_IsValid_ReturnsFalse_AndNull_OK()
    {
        MlResult<int> source = 42;

        var result = source.TryGetErrors(out var errorsDetails);

        result       .Should().BeFalse();
        errorsDetails.Should().BeNull();
    }



    [Fact]
    public void TryGetErrors_IsFail_SeveralErrors_ReturnsAllOfThem_OK()
    {
        MlResult<int> source = new List<MlError> { "miError1", "miError2", "miError3" }.ToMlResultFail<int>();

        var result = source.TryGetErrors(out var errorsDetails);

        result              .Should().BeTrue();
        errorsDetails       .Should().NotBeNull();
        errorsDetails.Errors.Should().HaveCount(3);
    }



    #endregion



    #region Complementariedad



    [Theory]
    [InlineData(true )]
    [InlineData(false)]
    public void TryGetValue_And_TryGetErrors_AreComplementary_OK(bool isValid)
    {
        MlResult<int> source = isValid ? 42 : "miError".ToMlResultFail<int>();

        var hasValue  = source.TryGetValue (out _);
        var hasErrors = source.TryGetErrors(out _);

        hasValue .Should().Be( isValid);
        hasErrors.Should().Be(!isValid);
        (hasValue ^ hasErrors).Should().BeTrue();
    }



    #endregion



}
