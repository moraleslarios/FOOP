// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.FileSystem.Tests.Unit;

public class ExistingDirectoryPathTests
{
    [Fact]
    public void ByString_ExistDirectory_ReturnsValid()
    {
        string dirStr = AppContext.BaseDirectory;
        MlResult<ExistingDirectoryPath> result = ExistingDirectoryPath.FromPath(dirStr);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ByString_NotExistDirectory_ReturnsFail()
    {
        string dirStr = Path.Combine(AppContext.BaseDirectory, "NonExistentDirFolder123");
        MlResult<ExistingDirectoryPath> result = ExistingDirectoryPath.FromPath(dirStr);
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ByString_NotExistDirectory_ReturnsFailWithGoodErrorMessage()
    {
        string dirStr = Path.Combine(AppContext.BaseDirectory, "NonExistentDirFolder123");
        MlResult<ExistingDirectoryPath> result = ExistingDirectoryPath.FromPath(dirStr);
        MlResult<ExistingDirectoryPath> expected = MlResult<ExistingDirectoryPath>.Fail($"{dirStr} not exists");
        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ByString_NullValue_ReturnsFail()
    {
        string? dirStr = null;
        MlResult<ExistingDirectoryPath> result = ExistingDirectoryPath.FromPath(dirStr!);
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ByString_EmptyValue_ReturnsFail()
    {
        MlResult<ExistingDirectoryPath> result = ExistingDirectoryPath.FromPath(string.Empty);
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ByString_NotExistDirectory_WithCustomError_ReturnsCustomError()
    {
        string dirStr = Path.Combine(AppContext.BaseDirectory, "NonExistentDirFolder123");
        MlResult<ExistingDirectoryPath> result = ExistingDirectoryPath.FromPath(dirStr, "Custom dir error");
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ImplicitOperators_ConvertBidirectionally()
    {
        string dirStr = AppContext.BaseDirectory;
        ExistingDirectoryPath dirObject = dirStr; // implicit string -> ExistingDirectoryPath
        string resultStr = dirObject;      // implicit ExistingDirectoryPath -> string
        resultStr.Should().Be(dirStr);
    }

    [Fact]
    public void FromString_ExistDirectory_ReturnsInstance()
    {
        string dirStr = AppContext.BaseDirectory;
        var instance = ExistingDirectoryPath.CreateFromPath(dirStr);
        ((string)instance).Should().Be(dirStr);
    }

    [Fact]
    public void FromString_NotExistDirectory_ThrowsDirectoryNotFoundException()
    {
        string dirStr = Path.Combine(AppContext.BaseDirectory, "NonExistentDirFolder123");
        Action act = () => ExistingDirectoryPath.CreateFromPath(dirStr);
        act.Should().Throw<DirectoryNotFoundException>()
           .WithMessage($"{dirStr} not exists");
    }

    [Fact]
    public void FromString_NullValue_ThrowsArgumentNullException()
    {
        Action act = () => ExistingDirectoryPath.CreateFromPath(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
