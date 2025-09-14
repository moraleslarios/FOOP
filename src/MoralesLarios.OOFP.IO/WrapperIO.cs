namespace MoralesLarios.OOFP.IO;

public class WrapperIO : IWrapperIO
{

    public MlResult<IEnumerable<ExistsFile>> EnumerateFiles(string directoryStr)
    {
        var result = ExistDirectory.ByString(directoryStr)
                                    .Map( _     => Directory.EnumerateFiles(directoryStr))
                                    .Map( files => files.Select(file => ExistsFile.FromString(file)));
        return result;
    }

    public MlResult<string> ReadAllText(string filePathStr)
    {
        var result = ExistsFile.ByString(filePathStr)
                                .Map( _ => File.ReadAllText(filePathStr));
        return result;
    }

    public MlResult<IEnumerable<string>> ReadAllLines(string filePathStr)
    {
        var result = ExistsFile.ByString(filePathStr)
                                .Map(_ => File.ReadLines(filePathStr));
        return result;
    }


}
