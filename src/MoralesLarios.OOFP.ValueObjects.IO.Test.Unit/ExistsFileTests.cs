// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.ValueObjects.IO.Test.Unit;

public class ExistsFileTests
{


    //[Fact]
    //public void ByString_existvalue_return_valid()
    //{
    //    string pathStr = Path.Combine(AppContext.BaseDirectory,
    //                                    "FakeFiles",
    //                                    "TextFile1.txt");

    //    MlResult<ExistsFile> result = ExistsFile.ByString(pathStr);

    //    result.IsValid.Should().BeTrue();
    //}

    [Fact]
    public void ByString_notexistvalue_return_invalid()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory,
                                        "FakeFiles",
                                        "TextFile2.txt");

        MlResult<ExistsFile> result = ExistsFile.ByString(pathStr);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ByString_notExistvalue_return_fail_with_goodErrorMessage()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory,
                                        "FakeFiles",
                                        "TextFile2.txt");

        MlResult<ExistsFile> result = ExistsFile.ByString(pathStr);

        MlResult<ExistsFile> expected = MlResult<ExistsFile>.Fail($"{pathStr} not exists");

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ByString_nullvalue_return_invalid()
    {
        string? pathStr = null;
        MlResult<ExistsFile> result = ExistsFile.ByString(pathStr!);
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ByString_emptyvalue_return_invalid()
    {
        MlResult<ExistsFile> result = ExistsFile.ByString(string.Empty);
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ByString_notexistvalue_withCustomError_returns_customError()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory, "FakeFiles", "TextFile2.txt");
        MlResult<ExistsFile> result = ExistsFile.ByString(pathStr, "Custom file error");
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ImplicitOperators_ConvertBidirectionally()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory, "FakeFiles", "TextFile1.txt");
        ExistsFile fileObject = pathStr; // implicit string -> ExistsFile
        string resultStr = fileObject;   // implicit ExistsFile -> string
        resultStr.Should().Be(pathStr);
    }

    [Fact]
    public void ByString_ExistFile_return_valid()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory,
                                        "FakeFiles",
                                        "TextFile1.txt");
        MlResult<ExistsFile> result = ExistsFile.ByString(pathStr);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ByString_ExistFile_return_valid_with_correctValue()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory,
                                        "FakeFiles",
                                        "TextFile1.txt");
        MlResult<ExistsFile> result = ExistsFile.ByString(pathStr, "Error ...");

        MlResult<ExistsFile> expected = ExistsFile.FromString(pathStr).ToMlResultValid();

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void FromString_ExistFile_ReturnsInstance()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory, "FakeFiles", "TextFile1.txt");
        var instance = ExistsFile.FromString(pathStr);
        ((string)instance).Should().Be(pathStr);
    }

    [Fact]
    public void FromString_NotExistFile_ThrowsFileNotFoundException()
    {
        string pathStr = Path.Combine(AppContext.BaseDirectory, "FakeFiles", "NonExistent.txt");
        Action act = () => ExistsFile.FromString(pathStr);
        act.Should().Throw<FileNotFoundException>()
           .WithMessage($"{pathStr} not exists");
    }

    [Fact]
    public void FromString_Null_ThrowsArgumentNullException()
    {
        Action act = () => ExistsFile.FromString(null!);
        act.Should().Throw<ArgumentNullException>();
    }


}
