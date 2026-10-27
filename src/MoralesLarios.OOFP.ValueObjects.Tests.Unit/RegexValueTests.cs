// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class RegexValueTests
{
    [Fact]
    public void RegexValue_ValueMatchesPattern_ReturnsValid()
    {
        var value   = "Hello";              // Capital first letter then lowercase
        var pattern = "^[A-Z][a-z]+$";      // Pattern requiring first capital then lowercase letters

        var result = RegexValue.FromRegex(value, pattern);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void RegexValue_EmptyValue_ReturnsFail()
    {
        var value   = string.Empty;
        var pattern = "^[A-Z][a-z]+$";

        var result = RegexValue.FromRegex(value, pattern);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void RegexValue_ValueDoesNotMatchPattern_ReturnsFail()
    {
        var value   = "XX";         // Not digits
        var pattern = "^[0-9]+$";   // Only digits

        var result = RegexValue.FromRegex(value, pattern);

        result.IsFailure.Should().BeTrue();
    }
}

