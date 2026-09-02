// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit;

public class DataannotationsValidatorTests
{
    [Fact]
    public async Task ValidateAsync_object_null_returns_fail()
    {
        User? source = null;

        MlResult<User> result = await DataannotationsValidator.ValidateAsync(source!);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_objectTask_null_returns_fail()
    {
        Task<User>? sourceAsync = Task.FromResult<User?>(null);

        MlResult<User> result = await DataannotationsValidator.ValidateAsync(sourceAsync!);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_collection_null_returns_fail()
    {
        IEnumerable<User>? source = null;

        MlResult<IEnumerable<User>> result = await DataannotationsValidator.ValidateAsync(source!);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_collectionTask_null_returns_fail()
    {
        Task<IEnumerable<User>>? sourceAsync = Task.FromResult<IEnumerable<User>?>(null);

        MlResult<IEnumerable<User>> result = await DataannotationsValidator.ValidateAsync(sourceAsync!);

        result.IsFail.Should().BeTrue();
    }
}
