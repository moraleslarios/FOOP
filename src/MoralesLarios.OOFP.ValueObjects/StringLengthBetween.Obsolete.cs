// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
/// <summary>
/// Representa <c>StringLengthBetween</c>.
/// </summary>
public partial class StringLengthBetween
{
    /// <summary>
    /// Ejecuta <c>FromStringLenght</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLenght">Valor de <c>minLenght</c>.</param>
    /// <param name="maxLenght">Valor de <c>maxLenght</c>.</param>
    /// <returns>Valor de tipo <c>StringLengthBetween</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to FromStringLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static StringLengthBetween FromStringLenght(string value, int minLenght, int maxLenght) => CreateFromLength(value, minLenght, maxLenght);
    /// <summary>
    /// Ejecuta <c>ByStringLength</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to FromLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<StringLengthBetween> ByStringLength(string value, int minLength, int maxLength, MlErrorDetails errorsDetails = null !) => FromLength(value, minLength, maxLength, errorsDetails);
}