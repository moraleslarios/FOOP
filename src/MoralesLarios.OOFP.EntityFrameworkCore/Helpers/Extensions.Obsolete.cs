// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.EntityFrameworkCore.Helpers;
/// <summary>
/// Métodos de extension de <c></c>.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Obtiene el elemento solicitado de <c>PkValues</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Representación textual obtenida.</returns>
    [global::System.Obsolete("Renamed to FormatCompositeKey. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string GetPkValues(this object[] source) => FormatCompositeKey(source);
}
