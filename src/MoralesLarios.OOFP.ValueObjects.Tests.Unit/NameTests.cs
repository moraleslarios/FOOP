// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class NameTests
{
    [Fact]
    public void Name_ValueLessThanLenght_ReturnsValid()
    {
        var validString = "Hello";

        var result = Name.ByString(validString);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Name_EmptyValue_ReturnsFail()
    {
        var validString = "";

        var result = Name.ByString(validString);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void Name_ValueLessThanLenght_ReturnsFail()
    {
        var validString = "XX";

        var result = Name.ByString(validString);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void Name_IsValid_should_use_the_received_length()
    {
        Name.IsValid("ABCD", 5).Should().BeFalse();
        Name.IsValid("ABCDE", 5).Should().BeTrue();
    }

    [Fact]
    public void Name_should_not_expose_public_constructors()
    {
        typeof(Name)
            .GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            .Should().BeEmpty();
    }
}

