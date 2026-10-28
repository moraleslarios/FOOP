// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects.FileSystem;
/// <summary>
/// Representa <c>ExistingFilePath</c>.
/// </summary>
public partial class ExistingFilePath
{
    /// <summary>
    /// Ejecuta <c>ByString</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="pathStr">Valor de <c>pathStr</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FromPath. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public new static MlResult<ExistingFilePath> ByString(string pathStr, MlErrorDetails errorsDetails = null !) => FromPath(pathStr, errorsDetails);
}
