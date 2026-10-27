// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringLengthBetweenTests
{
    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    public void StringLengthBetween_should_accept_min_and_max_boundaries(int length)
    {
        var value = new string('A', length);

        var result = StringLengthBetween.FromLength(value, 3, 5);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void StringLengthBetween_error_message_should_print_min_before_max()
    {
        var message = StringLengthBetween.FormatErrorMessage("ABCDE", 3, 5);

        message.Should().Be("ABCDE must be between 3 and 5");
    }

    [Fact]
    public void StringLengthBetween_ValueBetweenLimits_ReturnsValid()
    {
        var validString = "Hello, how are you?";
        var minLength   = 4;
        var maxLength   = 35;

        var result = StringLengthBetween.FromLength(validString, minLength, maxLength);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void StringLengthBetween_EmptyValue_ReturnsFail()
    {
        var validString = "";
        var minLength   = 4;
        var maxLength   = 15;


        var result = StringLengthBetween.FromLength(validString, minLength, maxLength);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void StringLengthBetween_ValueLessThanMinLength_ReturnsFail()
    {
        var validString = "XX";
        var minLength   = 4;
        var maxLength   = 15;

        var result = StringLengthBetween.FromLength(validString, minLength, maxLength);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void StringLengthBetween_ValueMoreThanMaxLength_ReturnsFail()
    {
        var validString = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
        var minLength   = 4;
        var maxLength   = 15;

        var result = StringLengthBetween.FromLength(validString, minLength, maxLength);

        result.IsFailure.Should().BeTrue();
    }

}

