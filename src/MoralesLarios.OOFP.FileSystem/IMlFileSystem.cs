namespace MoralesLarios.OOFP.FileSystem;

/// <summary>
/// Contrato de <c>IMlFileSystem</c>.
/// </summary>
public interface IMlFileSystem
{
    /// <summary>
    /// Ejecuta <c>EnumerateFiles</c>.
    /// </summary>
    /// <param name="directoryStr">Valor de <c>directoryStr</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<IEnumerable<ExistingFilePath>> EnumerateFiles(string directoryStr);

    /// <summary>
    /// Lee el contenido de <c>AllText</c>.
    /// </summary>
    /// <param name="filePathStr">Valor de <c>filePathStr</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<string> ReadAllText(string filePathStr);

    /// <summary>
    /// Lee el contenido de <c>AllLines</c>.
    /// </summary>
    /// <param name="filePathStr">Valor de <c>filePathStr</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<IEnumerable<string>> ReadAllLines(string filePathStr);
}