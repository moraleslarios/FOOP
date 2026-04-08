// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class IntLessThanTests
{

    [Fact]
    public void IntLessThan_ValueLessThanLenght_ReturnsValid()
    {
        var value     = 5;
        var minLenght = 8;

        var result = IntLessThan.ByIntLength(value, minLenght);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IntLessThan_ValueLessThanLenght_ReturnsFail()
    {
        var value     = 5;
        var minLenght = 2;

        var result = IntLessThan.ByIntLength(value, minLenght);

        result.IsFail.Should().BeTrue();
    }



}

