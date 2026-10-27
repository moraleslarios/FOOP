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
        var minLenght = 8;

        var result = IntLessThan.ByIntLength(value, minLenght);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void IntLessThan_ValueLessThanLength_ReturnsFail()
    {
        var value     = 5;
        var minLenght = 2;

        var result = IntLessThan.ByIntLength(value, minLenght);

        result.IsFailure.Should().BeTrue();
    }



}

