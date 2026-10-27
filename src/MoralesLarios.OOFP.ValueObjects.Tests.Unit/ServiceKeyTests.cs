// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class ServiceKeyTests
{
    [Fact]
    public void ServiceKey_ValueWithOneChar_ReturnsValid()
    {
        var value = "A";

        var result = ServiceKey.FromString(value);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ServiceKey_EmptyValue_ReturnsFail()
    {
        var value = "";

        var result = ServiceKey.FromString(value);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ServiceKey_ValueLongerStillValid()
    {
        var value = "ABC";

        var result = ServiceKey.FromString(value);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ServiceKey_IsValid_should_use_the_received_length()
    {
        ServiceKey.IsValid("AB", 3).Should().BeFalse();
        ServiceKey.IsValid("ABC", 3).Should().BeTrue();
    }

    [Fact]
    public void ServiceKey_should_not_expose_public_constructors()
    {
        typeof(ServiceKey)
            .GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            .Should().BeEmpty();
    }
}

