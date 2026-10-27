namespace MoralesLarios.OOFP.FileSystem;

public interface IMlFileSystem
{
    MlResult<IEnumerable<ExistingFilePath>> EnumerateFiles(string directoryStr);

    MlResult<string> ReadAllText(string filePathStr);

    MlResult<IEnumerable<string>> ReadAllLines(string filePathStr);
}