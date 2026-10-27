// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsShortTests
{
    [Fact]
    public void FromString_ValidShortString_ReturnsValid()
    {
        var value = "12345";
        StringAsShort result = StringAsShort.CreateFromString(value);
        short expected = 12345;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidShortString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsShort.CreateFromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidShortString_ReturnsValidResult()
    {
        var value = "30000";
        var result = StringAsShort.FromString(value);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidShortString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsShort> result = StringAsShort.FromString(value);
        result.IsFailure.Should().BeTrue();
    }
}

