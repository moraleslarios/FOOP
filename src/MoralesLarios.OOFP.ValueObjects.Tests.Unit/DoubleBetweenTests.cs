// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class DoubleBetweenTests
{
    [Fact]
    public void DoubleBetween_ValueBetweenLimits_ReturnsValid()
    {
        var value = 5.5;
        var minLength = 4.1;
        var maxLength = 35.7;

        var result = DoubleBetween.FromDoubleLength(value, minLength, maxLength);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void DoubleBetween_ValueLessThanMinLength_ReturnsFail()
    {
        var value = 2.0;
        var minLength = 4.1;
        var maxLength = 15.0;

        var result = DoubleBetween.FromDoubleLength(value, minLength, maxLength);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void DoubleBetween_ValueMoreThanMaxLength_ReturnsFail()
    {
        var value = 16.2;
        var minLength = 4.1;
        var maxLength = 15.0;

        var result = DoubleBetween.FromDoubleLength(value, minLength, maxLength);

        result.IsFailure.Should().BeTrue();
    }
}

