// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
/// <summary>
/// Representa <c>DecimalNonNegative</c>.
/// </summary>
public partial class DecimalNonNegative
{
    /// <summary>
    /// Ejecuta <c>ByDecimal</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FromDecimal. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<DecimalNonNegative> ByDecimal(decimal value, MlErrorDetails errorsDetails = null !) => FromDecimal(value, errorsDetails);
    /// <summary>
    /// Obtiene el valor de <c>limit</c>.
    /// </summary>
    [global::System.Obsolete("Renamed to MinAllowedValue. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static decimal limit => MinAllowedValue;
}