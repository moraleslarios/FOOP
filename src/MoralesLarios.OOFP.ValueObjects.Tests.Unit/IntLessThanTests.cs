// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class IntLessThanTests
{

    [Fact]
    public void IntLessThan_ValueLessThanLength_ReturnsValid()
    {
        var value     = 5;
        var minLength = 8;

        var result = IntLessThan.FromIntLength(value, minLength);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void IntLessThan_ValueLessThanLength_ReturnsFail()
    {
        var value     = 5;
        var minLength = 2;

        var result = IntLessThan.FromIntLength(value, minLength);

        result.IsFailure.Should().BeTrue();
    }



}

