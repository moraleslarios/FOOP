// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class DoubleMoreThanTests
{
    [Fact]
    public void DoubleMoreThan_ValueMoreThanLength_ReturnsValid()
    {
        var value = 8.5;
        var length = 5.2;

        var result = DoubleMoreThan.ByDoubleLength(value, length);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DoubleMoreThan_ValueNotMoreThanLength_ReturnsFail()
    {
        var value = 2.1;
        var length = 5.0;

        var result = DoubleMoreThan.ByDoubleLength(value, length);

        result.IsFail.Should().BeTrue();
    }
}

