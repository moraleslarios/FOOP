// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.FileSystem;

public class WrapperIO : IWrapperIO
{

    public MlResult<IEnumerable<ExistingFilePath>> EnumerateFiles(string directoryStr)
    {
        var result = ExistingDirectoryPath.FromPath(directoryStr)
                                    .Map( _     => Directory.EnumerateFiles(directoryStr))
                                    .Map( files => files.Select(file => ExistingFilePath.CreateFromPath(file)));
        return result;
    }

    public MlResult<string> ReadAllText(string filePathStr)
    {
        var result = ExistingFilePath.FromPath(filePathStr)
                                .Map( _ => File.ReadAllText(filePathStr));
        return result;
    }

    public MlResult<IEnumerable<string>> ReadAllLines(string filePathStr)
    {
        var result = ExistingFilePath.FromPath(filePathStr)
                                .Map(_ => File.ReadLines(filePathStr));
        return result;
    }


}

