// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class NotEmptyStringTests
{


    [Fact]
    public void ByString_ValidValue_ReturnsValid()
    {
        MlResult<NotEmptyString> result = NotEmptyString.FromString("Valid String");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ByString_FailValue_ReturnsFail()
    {
        MlResult<NotEmptyString> result = NotEmptyString.FromString("");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void FromString_FailValue_ThrowException()
    {
        Action act = () => NotEmptyString.CreateFromString("");

        act.Should().Throw<ArgumentException>();
    }

}

