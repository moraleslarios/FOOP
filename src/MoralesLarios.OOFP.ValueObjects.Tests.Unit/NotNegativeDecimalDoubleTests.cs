// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class DecimalNonNegativeTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ByDecimal_should_match_expected_validity(decimal value)
    {
        var result = DecimalNonNegative.FromDecimal(value);

        if (value < 0)
        {
            result.IsFailure.Should().BeTrue();
        }
        else
        {
            result.IsSuccess.Should().BeTrue();
            ((decimal)result.GetValueOrThrow()).Should().Be(value);
        }
    }
}

public class DoubleNonNegativeTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ByDouble_should_match_expected_validity(double value)
    {
        var result = DoubleNonNegative.FromDouble(value);

        if (value < 0)
        {
            result.IsFailure.Should().BeTrue();
        }
        else
        {
            result.IsSuccess.Should().BeTrue();
            ((double)result.GetValueOrThrow()).Should().Be(value);
        }
    }
}
