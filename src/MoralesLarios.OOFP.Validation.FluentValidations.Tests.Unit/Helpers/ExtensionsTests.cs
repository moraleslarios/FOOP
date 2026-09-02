// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Validation.FluentValidations.Helpers;

namespace MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit;
public class ExtensionsTests
{


    [Fact]
    public void ValidateWithFluentValidations_objectValid_return_valid()
    {
        User source = new("user", DateTime.Now, "password", "password");

        MlResult<User> result = source.ValidateWithFluentValidations<User, UserValidator>();

        result.IsValid.Should().BeTrue();
    }



    [Fact]
    public void ValidateWithFluentValidations_objectFail_return_fail()
    {
        User source = new("", DateTime.Now, "", "password");

        MlResult<User> result = source.ValidateWithFluentValidations<User, UserValidator>();

        result.IsFail.Should().BeTrue();
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
    public void ValidateWithFluentValidations_collectionValid_return_valid()
    {
        IEnumerable<User> source = [
            new("user", DateTime.Now, "password", "password"),
            new("other", DateTime.Now, "password", "password")
        ];

        MlResult<IEnumerable<User>> result = source.ValidateWithFluentValidations<User, UserValidator>();

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateWithFluentValidations_collectionWithInvalidItem_return_fail()
    {
        IEnumerable<User> source = [
            new("user", DateTime.Now, "password", "password"),
            new("", DateTime.Now, "password", "password")
        ];

        MlResult<IEnumerable<User>> result = source.ValidateWithFluentValidations<User, UserValidator>();

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ValidateWithFluentValidations_collectionWithTwoInvalidItems_returnsTwoErrors()
    {
        IEnumerable<User> source = [
            new("a", DateTime.Now, "password", "password"),
            new("b", DateTime.Now, "password", "password")
        ];

        MlResult<IEnumerable<User>> result = source.ValidateWithFluentValidations<User, UserValidator>();

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.Count().Should().Be(2);
    }

    [Fact]
    public void ValidateWithFluentValidations_collectionWithOneInvalidItem_returnsOneError()
    {
        IEnumerable<User> source = [
            new("a", DateTime.Now, "password", "password"),
            new("user", DateTime.Now, "password", "password")
        ];

        MlResult<IEnumerable<User>> result = source.ValidateWithFluentValidations<User, UserValidator>();

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.Count().Should().Be(1);
    }

    [Fact]
    public async Task ValidateWithFluentValidationsAsync_taskObject_return_valid()
    {
        Task<User> sourceAsync = Task.FromResult(new User("user", DateTime.Now, "password", "password"));

        MlResult<User> result = await sourceAsync.ValidateWithFluentValidationsAsync<User, UserValidator>();

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateWithFluentValidationsAsync_taskCollection_return_fail()
    {
        Task<IEnumerable<User>> sourceAsync = Task.FromResult<IEnumerable<User>>([
            new("", DateTime.Now, "password", "password")
        ]);

        MlResult<IEnumerable<User>> result = await sourceAsync.ValidateWithFluentValidationsAsync<User, UserValidator>();

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void FluentValidationsValidator_nullObject_return_fail()
    {
        User? source = null;

        MlResult<User> result = FluentValidationsValidator.Validate<User, UserValidator>(source!);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void FluentValidationsValidator_emptyCollection_return_fail()
    {
        IEnumerable<User> source = [];

        MlResult<IEnumerable<User>> result = FluentValidationsValidator.Validate<User, UserValidator>(source);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task FluentValidationsValidator_asyncDirectObject_return_fail()
    {
        User source = new("", DateTime.Now, "password", "password");

        MlResult<User> result = await FluentValidationsValidator.ValidateAsync<User, UserValidator>(source);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task FluentValidationsValidator_asyncDirectCollection_return_valid()
    {
        IEnumerable<User> source = [new("user", DateTime.Now, "password", "password")];

        MlResult<IEnumerable<User>> result = await FluentValidationsValidator.ValidateAsync<User, UserValidator>(source);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task FluentValidationsValidator_asyncObject_return_valid()
    {
        MlResult<User> result = await FluentValidationsValidator.ValidateAsync<User, UserValidator>(
            Task.FromResult(new User("user", DateTime.Now, "password", "password")));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task FluentValidationsValidator_asyncCollectionWithInvalidItem_return_fail()
    {
        Task<IEnumerable<User>> sourceAsync = Task.FromResult<IEnumerable<User>>([
            new("", DateTime.Now, "password", "password")
        ]);

        MlResult<IEnumerable<User>> result = await FluentValidationsValidator.ValidateAsync<User, UserValidator>(sourceAsync);

        result.IsFail.Should().BeTrue();
    }
}

