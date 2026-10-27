// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Validation.FluentValidation.Helpers;

namespace MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit;
public class ExtensionsTests
{


    [Fact]
    public void ValidateWithFluentValidation_objectValid_return_valid()
    {
        User source = new("user", DateTime.Now, "password", "password");

        MlResult<User> result = source.ValidateWithFluentValidation<User, UserValidator>();

        result.IsSuccess.Should().BeTrue();
    }



    [Fact]
    public void ValidateWithFluentValidation_objectFail_return_fail()
    {
        User source = new("", DateTime.Now, "", "password");

        MlResult<User> result = source.ValidateWithFluentValidation<User, UserValidator>();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ValidateWithFluentValidationResult_objectFail_preservesRawDiagnostics()
    {
        User source = new("", DateTime.Now, "password", "password");

        var result = source.ValidateWithFluentValidationResult<User, UserValidator>();

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(User.UserName));
    }

    [Fact]
    public void ValidateWithFluentValidation_collectionValid_return_valid()
    {
        IEnumerable<User> source = [
            new("user", DateTime.Now, "password", "password"),
            new("other", DateTime.Now, "password", "password")
        ];

        MlResult<IEnumerable<User>> result = source.ValidateWithFluentValidation<User, UserValidator>();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ValidateWithFluentValidation_collectionWithInvalidItem_return_fail()
    {
        IEnumerable<User> source = [
            new("user", DateTime.Now, "password", "password"),
            new("", DateTime.Now, "password", "password")
        ];

        MlResult<IEnumerable<User>> result = source.ValidateWithFluentValidation<User, UserValidator>();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ValidateWithFluentValidation_collectionWithTwoInvalidItems_returnsTwoErrors()
    {
        IEnumerable<User> source = [
            new("a", DateTime.Now, "password", "password"),
            new("b", DateTime.Now, "password", "password")
        ];

        MlResult<IEnumerable<User>> result = source.ValidateWithFluentValidation<User, UserValidator>();

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Errors.Count().Should().Be(2);
    }

    [Fact]
    public void ValidateWithFluentValidation_collectionWithOneInvalidItem_returnsOneError()
    {
        IEnumerable<User> source = [
            new("a", DateTime.Now, "password", "password"),
            new("user", DateTime.Now, "password", "password")
        ];

        MlResult<IEnumerable<User>> result = source.ValidateWithFluentValidation<User, UserValidator>();

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Errors.Count().Should().Be(1);
    }

    [Fact]
    public async Task ValidateWithFluentValidationAsync_taskObject_return_valid()
    {
        Task<User> sourceAsync = Task.FromResult(new User("user", DateTime.Now, "password", "password"));

        MlResult<User> result = await sourceAsync.ValidateWithFluentValidationAsync<User, UserValidator>();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateWithFluentValidationAsync_taskCollection_return_fail()
    {
        Task<IEnumerable<User>> sourceAsync = Task.FromResult<IEnumerable<User>>([
            new("", DateTime.Now, "password", "password")
        ]);

        MlResult<IEnumerable<User>> result = await sourceAsync.ValidateWithFluentValidationAsync<User, UserValidator>();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void FluentValidationValidator_nullObject_return_fail()
    {
        User? source = null;

        MlResult<User> result = FluentValidationValidator.Validate<User, UserValidator>(source!);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void FluentValidationValidator_emptyCollection_return_fail()
    {
        IEnumerable<User> source = [];

        MlResult<IEnumerable<User>> result = FluentValidationValidator.Validate<User, UserValidator>(source);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task FluentValidationValidator_asyncDirectObject_return_fail()
    {
        User source = new("", DateTime.Now, "password", "password");

        MlResult<User> result = await FluentValidationValidator.ValidateAsync<User, UserValidator>(source);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task FluentValidationValidator_asyncDirectCollection_return_valid()
    {
        IEnumerable<User> source = [new("user", DateTime.Now, "password", "password")];

        MlResult<IEnumerable<User>> result = await FluentValidationValidator.ValidateAsync<User, UserValidator>(source);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task FluentValidationValidator_asyncObject_return_valid()
    {
        MlResult<User> result = await FluentValidationValidator.ValidateAsync<User, UserValidator>(
            Task.FromResult(new User("user", DateTime.Now, "password", "password")));

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task FluentValidationValidator_asyncCollectionWithInvalidItem_return_fail()
    {
        Task<IEnumerable<User>> sourceAsync = Task.FromResult<IEnumerable<User>>([
            new("", DateTime.Now, "password", "password")
        ]);

        MlResult<IEnumerable<User>> result = await FluentValidationValidator.ValidateAsync<User, UserValidator>(sourceAsync);

        result.IsFailure.Should().BeTrue();
    }
}

