// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class DecimalNotNegativeTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ByDecimal_should_match_expected_validity(decimal value)
    {
        var result = DecimalNotNegative.ByDecimal(value);

        if (value < 0)
        {
            result.IsFail.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeTrue();
            ((decimal)result.SecureValidValue()).Should().Be(value);
        }
    }
}

public class DoubleNotNegativeTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ByDouble_should_match_expected_validity(double value)
    {
        var result = DoubleNotNegative.ByDouble(value);

        if (value < 0)
        {
            result.IsFail.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeTrue();
            ((double)result.SecureValidValue()).Should().Be(value);
        }
    }
}
