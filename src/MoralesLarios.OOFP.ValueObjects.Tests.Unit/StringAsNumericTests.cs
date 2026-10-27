// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringAsNumericTests
{

    [Fact]
    public void FromString_ValidNumericString_ReturnsValid()
    {
        var value = "123,45";

        StringAsNumeric<decimal> result = StringAsNumeric<decimal>.CreateFromString(value);

        decimal expected = 123.45m;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void FromString_InvalidNumericString_ThrowsArgumentNullException()
    {
        var value = "ABC";

        Action act = () => StringAsNumeric<decimal>.CreateFromString(value);

        act.Should().Throw<FormatException>();
    }


    [Fact]
    public void ByString_ValidNumericString_ReturnsValidResult()
    {
        var value = "678.90";

        var result = StringAsNumeric<decimal>.FromString(value);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ByString_InvalidNumericString_ReturnsFailResult()
    {
        var value = "XYZ";

        MlResult<StringAsNumeric<decimal>> result = StringAsNumeric<decimal>.FromString(value);

        result.IsFailure.Should().BeTrue();
    }







}
