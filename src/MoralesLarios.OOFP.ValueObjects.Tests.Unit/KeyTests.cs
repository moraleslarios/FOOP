// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class KeyTests
{
    [Fact]
    public void Key_ValueWithOneChar_ReturnsValid()
    {
        var value = "A";

        var result = Key.ByString(value);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Key_EmptyValue_ReturnsFail()
    {
        var value = "";

        var result = Key.ByString(value);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void Key_ValueLongerStillValid()
    {
        var value = "ABC";

        var result = Key.ByString(value);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Key_IsValid_should_use_the_received_length()
    {
        Key.IsValid("AB", 3).Should().BeFalse();
        Key.IsValid("ABC", 3).Should().BeTrue();
    }

    [Fact]
    public void Key_should_not_expose_public_constructors()
    {
        typeof(Key)
            .GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            .Should().BeEmpty();
    }
}

