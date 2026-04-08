// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class StringMaxLengthTests
{

    [Fact]
    public void StringMaxLength_ValueLessThanLenght_ReturnsValid()
    {
        var validString = "Hello";
        var maxLength   = 10;

        var result = StringMaxLength.ByStringLength(validString, maxLength);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void StringMaxLength_EmptyValue_ReturnsFail()
    {
        var validString = "";
        var maxLength   = 10;

        var result = StringMaxLength.ByStringLength(validString, maxLength);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void StringMaxLength_ValueLessThanLenght_ReturnsFail()
    {
        var validString = "XX";
        var maxLength   = 1;

        var result = StringMaxLength.ByStringLength(validString, maxLength);

        result.IsFail.Should().BeTrue();
    }

}

