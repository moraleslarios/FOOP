// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class IntMoreThanTests
{

    [Fact]
    public void IntMoreThan_ValueMoreThanLength_ReturnsValid()
    {
        var value     = 8;
        var minLength = 5;

        var result = IntMoreThan.FromIntLength(value, minLength);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void IntMoreThan_ValueMoreThanLength_ReturnsFail()
    {
        var value     = 2;
        var minLength = 5;

        var result = IntMoreThan.FromIntLength(value, minLength);

        result.IsFailure.Should().BeTrue();
    }




}

