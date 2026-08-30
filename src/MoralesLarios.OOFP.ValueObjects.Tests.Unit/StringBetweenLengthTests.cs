// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringBetweenLengthTests
{
    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    public void StringBetweenLength_should_accept_min_and_max_boundaries(int length)
    {
        var value = new string('A', length);

        var result = StringBetweenLength.ByStringLength(value, 3, 5);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void StringBetweenLength_error_message_should_print_min_before_max()
    {
        var message = StringBetweenLength.BuildErrorMessage("ABCDE", 3, 5);

        message.Should().Be("ABCDE must be between 3 and 5");
    }

    [Fact]
    public void StringBetweenLength_ValueBetweenLimits_ReturnsValid()
    {
        var validString = "Hello, how are you?";
        var minLength   = 4;
        var maxLength   = 35;

        var result = StringBetweenLength.ByStringLength(validString, minLength, maxLength);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void StringBetweenLength_EmptyValue_ReturnsFail()
    {
        var validString = "";
        var minLength   = 4;
        var maxLength   = 15;


        var result = StringBetweenLength.ByStringLength(validString, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void StringBetweenLength_ValueLessThanMinLenght_ReturnsFail()
    {
        var validString = "XX";
        var minLength   = 4;
        var maxLength   = 15;

        var result = StringBetweenLength.ByStringLength(validString, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void StringBetweenLength_ValueMoreThanMaxLenght_ReturnsFail()
    {
        var validString = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
        var minLength   = 4;
        var maxLength   = 15;

        var result = StringBetweenLength.ByStringLength(validString, minLength, maxLength);

        result.IsFail.Should().BeTrue();
    }

}

