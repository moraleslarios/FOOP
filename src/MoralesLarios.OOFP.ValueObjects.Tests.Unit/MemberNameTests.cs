// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class MemberNameTests
{
    [Fact]
    public void MemberName_ValueLessThanLength_ReturnsValid()
    {
        var validString = "Hello";

        var result = MemberName.FromString(validString);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void MemberName_EmptyValue_ReturnsFail()
    {
        var validString = "";

        var result = MemberName.FromString(validString);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void MemberName_ValueLessThanLength_ReturnsFail()
    {
        var validString = "XX";

        var result = MemberName.FromString(validString);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void MemberName_IsValid_should_use_the_received_length()
    {
        MemberName.IsValid("ABCD", 5).Should().BeFalse();
        MemberName.IsValid("ABCDE", 5).Should().BeTrue();
    }

    [Fact]
    public void MemberName_should_not_expose_public_constructors()
    {
        typeof(MemberName)
            .GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            .Should().BeEmpty();
    }
}

