// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using System.Text.RegularExpressions;

namespace MoralesLarios.OOFP.ValueObjects;
/// <summary>
/// Representa <c>RegexValue</c>.
/// </summary>
public partial class RegexValue
{
    /// <summary>
    /// Ejecuta <c>ByRegex</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="pattern">Valor de <c>pattern</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FromRegex. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<RegexValue> ByRegex(string value, string pattern, MlErrorDetails errorsDetails = null !) => FromRegex(value, pattern, errorsDetails);
}
