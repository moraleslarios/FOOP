// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit;

public class DataAnnotationsValidatorTests
{
    [Fact]
    public async Task ValidateAsync_object_null_returns_fail()
    {
        User? source = null;

        MlResult<User> result = await DataAnnotationsValidator.ValidateAsync(source!);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_objectTask_null_returns_fail()
    {
        Task<User>? sourceAsync = Task.FromResult<User?>(null);

        MlResult<User> result = await DataAnnotationsValidator.ValidateAsync(sourceAsync!);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_collection_null_returns_fail()
    {
        IEnumerable<User>? source = null;

        MlResult<IEnumerable<User>> result = await DataAnnotationsValidator.ValidateAsync(source!);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_collectionTask_null_returns_fail()
    {
        Task<IEnumerable<User>>? sourceAsync = Task.FromResult<IEnumerable<User>?>(null);

        MlResult<IEnumerable<User>> result = await DataAnnotationsValidator.ValidateAsync(sourceAsync!);

        result.IsFailure.Should().BeTrue();
    }
}
