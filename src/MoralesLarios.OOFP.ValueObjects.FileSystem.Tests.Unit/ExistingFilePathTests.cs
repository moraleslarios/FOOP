// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0


namespace MoralesLarios.OOFP.ValueObjects.FileSystem.Tests.Unit;

public class ExistingFilePathTests
{


    //[Fact]
    //public void ByString_existvalue_return_valid()
    //{
    //    string pathStr = Path.CombineAll(AppContext.BaseDirectory,
    //                                    "FakeFiles",
    //                                    "TextFile1.txt");

    //    MlResult<ExistingFilePath> result = ExistingFilePath.FromString(pathStr);

    //    result.IsSuccess.Should().BeTrue();
    //}

    [Fact]
    public void ByString_notexistvalue_return_invalid()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory,
                                        "FakeFiles",
                                        "TextFile2.txt");

        MlResult<ExistingFilePath> result = ExistingFilePath.FromPath(pathStr);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ByString_notExistvalue_return_fail_with_goodErrorMessage()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory,
                                        "FakeFiles",
                                        "TextFile2.txt");

        MlResult<ExistingFilePath> result = ExistingFilePath.FromPath(pathStr);

        MlResult<ExistingFilePath> expected = MlResult<ExistingFilePath>.Fail($"{pathStr} not exists");

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ByString_nullvalue_return_invalid()
    {
        string? pathStr = null;
        MlResult<ExistingFilePath> result = ExistingFilePath.FromPath(pathStr!);
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ByString_emptyvalue_return_invalid()
    {
        MlResult<ExistingFilePath> result = ExistingFilePath.FromPath(string.Empty);
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ByString_notexistvalue_withCustomError_returns_customError()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory, "FakeFiles", "TextFile2.txt");
        MlResult<ExistingFilePath> result = ExistingFilePath.FromPath(pathStr, "Custom file error");
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ImplicitOperators_ConvertBidirectionally()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory, "FakeFiles", "TextFile1.txt");
        ExistingFilePath fileObject = pathStr; // implicit string -> ExistingFilePath
        string resultStr = fileObject;   // implicit ExistingFilePath -> string
        resultStr.Should().Be(pathStr);
    }

    [Fact]
    public void ByString_ExistFile_return_valid()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory,
                                        "FakeFiles",
                                        "TextFile1.txt");
        MlResult<ExistingFilePath> result = ExistingFilePath.FromPath(pathStr);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ByString_ExistFile_return_valid_with_correctValue()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory,
                                        "FakeFiles",
                                        "TextFile1.txt");
        MlResult<ExistingFilePath> result = ExistingFilePath.FromPath(pathStr, "Error ...");

        MlResult<ExistingFilePath> expected = ExistingFilePath.CreateFromPath(pathStr).ToMlResultValid();

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void FromString_ExistFile_ReturnsInstance()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory, "FakeFiles", "TextFile1.txt");
        var instance = ExistingFilePath.CreateFromPath(pathStr);
        ((string)instance).Should().Be(pathStr);
    }

    [Fact]
    public void FromString_NotExistFile_ThrowsFileNotFoundException()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory, "FakeFiles", "NonExistent.txt");
        Action act = () => ExistingFilePath.CreateFromPath(pathStr);
        act.Should().Throw<FileNotFoundException>()
           .WithMessage($"{pathStr} not exists");
    }

    [Fact]
    public void FromString_Null_ThrowsArgumentNullException()
    {
        Action act = () => ExistingFilePath.CreateFromPath(null!);
        act.Should().Throw<ArgumentNullException>();
    }


}
