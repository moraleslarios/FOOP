
namespace MoralesLarios.OOFP.IO;

public interface IWrapperIO
{
    MlResult<IEnumerable<ExistsFile>> EnumerateFiles(string directoryStr);

    MlResult<string> ReadAllText(string filePathStr);

    MlResult<IEnumerable<string>> ReadAllLines(string filePathStr);
}