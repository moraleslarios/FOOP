// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
/// <summary>
/// Representa <c>IntLessThan</c>.
/// </summary>
public partial class IntLessThan
{
    /// <summary>
    /// Ejecuta <c>FromIntLenght</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="lenght">Valor de <c>lenght</c>.</param>
    /// <returns>Valor de tipo <c>IntLessThan</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to FromIntLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IntLessThan FromIntLenght(int value, int lenght) => CreateFromIntLength(value, lenght);
    /// <summary>
    /// Ejecuta <c>ByIntLength</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FromIntLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IntLessThan> ByIntLength(int value, int length, MlErrorDetails errorsDetails = null !) => FromIntLength(value, length, errorsDetails);
}