// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.IO.Test.Unit;

public class ExistDirectoryTests
{
    [Fact]
    public void ByString_ExistDirectory_ReturnsValid()
    {
        string dirStr = AppContext.BaseDirectory;
        MlResult<ExistDirectory> result = ExistDirectory.ByString(dirStr);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_NotExistDirectory_ReturnsFail()
    {
        string dirStr = Path.Combine(AppContext.BaseDirectory, "NonExistentDirFolder123");
        MlResult<ExistDirectory> result = ExistDirectory.ByString(dirStr);
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ByString_NotExistDirectory_ReturnsFailWithGoodErrorMessage()
    {
        string dirStr = Path.Combine(AppContext.BaseDirectory, "NonExistentDirFolder123");
        MlResult<ExistDirectory> result = ExistDirectory.ByString(dirStr);
        MlResult<ExistDirectory> expected = MlResult<ExistDirectory>.Fail($"{dirStr} not exists");
        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ByString_NullValue_ReturnsFail()
    {
        string? dirStr = null;
        MlResult<ExistDirectory> result = ExistDirectory.ByString(dirStr!);
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ByString_EmptyValue_ReturnsFail()
    {
        MlResult<ExistDirectory> result = ExistDirectory.ByString(string.Empty);
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ByString_NotExistDirectory_WithCustomError_ReturnsCustomError()
    {
        string dirStr = Path.Combine(AppContext.BaseDirectory, "NonExistentDirFolder123");
        MlResult<ExistDirectory> result = ExistDirectory.ByString(dirStr, "Custom dir error");
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ImplicitOperators_ConvertBidirectionally()
    {
        string dirStr = AppContext.BaseDirectory;
        ExistDirectory dirObject = dirStr; // implicit string -> ExistDirectory
        string resultStr = dirObject;      // implicit ExistDirectory -> string
        resultStr.Should().Be(dirStr);
    }

    [Fact]
    public void FromString_ExistDirectory_ReturnsInstance()
    {
        string dirStr = AppContext.BaseDirectory;
        var instance = ExistDirectory.FromString(dirStr);
        ((string)instance).Should().Be(dirStr);
    }

    [Fact]
    public void FromString_NotExistDirectory_ThrowsDirectoryNotFoundException()
    {
        string dirStr = Path.Combine(AppContext.BaseDirectory, "NonExistentDirFolder123");
        Action act = () => ExistDirectory.FromString(dirStr);
        act.Should().Throw<DirectoryNotFoundException>()
           .WithMessage($"{dirStr} not exists");
    }

    [Fact]
    public void FromString_NullValue_ThrowsArgumentNullException()
    {
        Action act = () => ExistDirectory.FromString(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
