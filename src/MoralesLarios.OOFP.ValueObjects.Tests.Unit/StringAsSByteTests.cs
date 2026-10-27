// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsSByteTests
{
    [Fact]
    public void FromString_ValidSByteString_ReturnsValid()
    {
        var value = "-123";
        StringAsSByte result = StringAsSByte.CreateFromString(value);
        sbyte expected = -123;
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidSByteString_ThrowsArgumentNullException()
    {
        var value = "ABC";
        Action act = () => StringAsSByte.CreateFromString(value);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ByString_ValidSByteString_ReturnsValidResult()
    {
        var value = "100";
        var result = StringAsSByte.FromString(value);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidSByteString_ReturnsFailResult()
    {
        var value = "XYZ";
        MlResult<StringAsSByte> result = StringAsSByte.FromString(value);
        result.IsFailure.Should().BeTrue();
    }
}

