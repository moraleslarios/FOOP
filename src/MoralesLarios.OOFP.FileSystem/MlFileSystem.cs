// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.FileSystem;

/// <summary>
/// Representa <c>MlFileSystem</c>.
/// </summary>
public class MlFileSystem : IMlFileSystem
{

    /// <summary>
    /// Ejecuta <c>EnumerateFiles</c>.
    /// </summary>
    /// <param name="directoryStr">Valor de <c>directoryStr</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<IEnumerable<ExistingFilePath>> EnumerateFiles(string directoryStr)
    {
        var result = ExistingDirectoryPath.FromPath(directoryStr)
                                    .Map( _     => Directory.EnumerateFiles(directoryStr))
                                    .Map( files => files.Select(file => ExistingFilePath.CreateFromPath(file)));
        return result;
    }

    /// <summary>
    /// Lee el contenido de <c>AllText</c>.
    /// </summary>
    /// <param name="filePathStr">Valor de <c>filePathStr</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<string> ReadAllText(string filePathStr)
    {
        var result = ExistingFilePath.FromPath(filePathStr)
                                .Map( _ => File.ReadAllText(filePathStr));
        return result;
    }

    /// <summary>
    /// Lee el contenido de <c>AllLines</c>.
    /// </summary>
    /// <param name="filePathStr">Valor de <c>filePathStr</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<IEnumerable<string>> ReadAllLines(string filePathStr)
    {
        var result = ExistingFilePath.FromPath(filePathStr)
                                .Map(_ => File.ReadLines(filePathStr));
        return result;
    }


}

