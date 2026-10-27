// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringMaxLengthTests
{

    [Fact]
    public void StringMaxLength_ValueLessThanLength_ReturnsValid()
    {
        var validString = "Hello";
        var maxLength   = 10;

        var result = StringMaxLength.FromLength(validString, maxLength);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void StringMaxLength_EmptyValue_ReturnsFail()
    {
        var validString = "";
        var maxLength   = 10;

        var result = StringMaxLength.FromLength(validString, maxLength);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void StringMaxLength_ValueLessThanLength_ReturnsFail()
    {
        var validString = "XX";
        var maxLength   = 1;

        var result = StringMaxLength.FromLength(validString, maxLength);

        result.IsFailure.Should().BeTrue();
    }

}

