// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
/// <summary>
/// Representa <c>DoubleLessThan</c>.
/// </summary>
public partial class DoubleLessThan
{
    /// <summary>
    /// Ejecuta <c>ByDoubleLength</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FromDoubleLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<DoubleLessThan> ByDoubleLength(double value, double length, MlErrorDetails errorsDetails = null !) => FromDoubleLength(value, length, errorsDetails);
}
