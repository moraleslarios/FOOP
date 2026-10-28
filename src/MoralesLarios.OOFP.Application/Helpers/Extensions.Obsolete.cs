// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Application.Helpers;
/// <summary>
/// Métodos de extension de <c></c>.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Construye la instancia solicitada de <c>NotFoundPkError</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="tableName">Nombre del elemento.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to BuildEntityNotFoundError. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlErrorDetails BuildNotFoundPkError(this string tableName, params object[] pk) => BuildEntityNotFoundError(tableName, pk);
}
