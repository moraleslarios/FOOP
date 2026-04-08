// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class IntNotNegativeTests
{

    [Fact]
    public void IntNotNegative_ValueMoreThanLenght_ReturnsValid()
    {
        var value     = 8;

        var result = IntNotNegative.ByInt(value);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IntNotNegative_ValueMoreThanLenght_ReturnsFail()
    {
        var value     = -2;

        var result = IntNotNegative.ByInt(value);

        result.IsFail.Should().BeTrue();
    }




}

