

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


}