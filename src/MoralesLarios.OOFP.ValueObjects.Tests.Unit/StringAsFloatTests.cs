// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsFloatTests
{
    [Fact]
    public void FromString_ValidFloatString_ReturnsValid()
    {
        var value = "123,45";
        StringAsFloat result = StringAsFloat.CreateFromString(value);
        float expected = 123.45f;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidFloatString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsFloat.CreateFromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidFloatString_ReturnsValidResult()
    {
        var value = "3.14159";
        var result = StringAsFloat.FromString(value);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidFloatString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsFloat> result = StringAsFloat.FromString(value);
        result.IsFailure.Should().BeTrue();
    }
}

