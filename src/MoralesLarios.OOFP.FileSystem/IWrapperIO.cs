namespace MoralesLarios.OOFP.FileSystem;

public interface IWrapperIO
{
    MlResult<IEnumerable<ExistingFilePath>> EnumerateFiles(string directoryStr);

    MlResult<string> ReadAllText(string filePathStr);

    MlResult<IEnumerable<string>> ReadAllLines(string filePathStr);
}