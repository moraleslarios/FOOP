// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types;

namespace MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit.Helpers;
public class ExtensionsTests
{


    [Fact]
    public void ValidateWithDataAnnotations_objectValid_return_valid()
    {
        User source = new("user", DateTime.Now, "password", "password");

        MlResult<User> result = source.ValidateWithDataAnnotations();

        result.IsSuccess.Should().BeTrue();
    }



    [Fact]
    public void ValidateWithDataAnnotations_objectFail_return_fail()
    {
        User source = new("", DateTime.Now, "", "password");

        MlResult<User> result = source.ValidateWithDataAnnotations();

        result.IsFailure.Should().BeTrue();
    }


    [Fact]
    public void ValidateWithDataAnnotationsAsync_colec_AllValid_return_valid()
    {
        IEnumerable<User> source = new List<User>
        {
            new("user", DateTime.Now, "password", "password"),
            new("user2", DateTime.Now, "password", "password")
        };

        MlResult<IEnumerable<User>> result = source.ValidateWithDataAnnotations();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ValidateWithDataAnnotationsAsync_colec_AllFail_return_Fail()
    {
        IEnumerable<User> source = new List<User>
        {
            new("user", DateTime.Now, "password", "password_diferewt1"),
            new("user2", DateTime.Now, "password", "password_diferewt2")
        };

        MlResult<IEnumerable<User>> result = source.ValidateWithDataAnnotations();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ValidateWithDataAnnotationsAsync_colec_AllFail_return_2_errors()
    {
        IEnumerable<User> source = new List<User>
        {
            new("user", DateTime.Now, "password", "password_diferewt1"),
            new("user2", DateTime.Now, "password", "password_diferewt2")
        };

        MlResult<IEnumerable<User>> result = source.ValidateWithDataAnnotations();

        var resultErrors = result.GetFailureDetailsOrThrow().Errors.Count();

        resultErrors.Should().Be(2);
    }

    [Fact]
    public void ValidateWithDataAnnotationsAsync_colec_fail_1_element_return_Fail()
    {
        IEnumerable<User> source = new List<User>
        {
            new("user", DateTime.Now, "password", "password_diferewt1"),
            new("user2", DateTime.Now, "password", "password")
        };

        MlResult<IEnumerable<User>> result = source.ValidateWithDataAnnotations();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ValidateWithDataAnnotationsAsync_colec_fail_1_element_return_1_errors()
    {
        IEnumerable<User> source = new List<User>
        {
            new("user", DateTime.Now, "password", "password_diferewt1"),
            new("user2", DateTime.Now, "password", "password")
        };

        MlResult<IEnumerable<User>> result = source.ValidateWithDataAnnotations();

        var resultErrors = result.GetFailureDetailsOrThrow().Errors.Count();

        resultErrors.Should().Be(1);
    }
}

