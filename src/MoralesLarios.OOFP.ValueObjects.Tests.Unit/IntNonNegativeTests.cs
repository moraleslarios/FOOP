// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class IntNonNegativeTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ByInt_should_match_expected_validity(int value)
    {
        var result = IntNonNegative.FromInt(value);

        if (value < 0)
        {
            result.IsFailure.Should().BeTrue();
        }
        else
        {
            result.IsSuccess.Should().BeTrue();
            ((int)result.GetValueOrThrow()).Should().Be(value);
        }
    }

    [Fact]
    public void IntNonNegative_should_not_expose_a_mutable_static_limit_field()
    {
        typeof(IntNonNegative)
            .GetField("limit", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
            .Should().BeNull();
    }
}

