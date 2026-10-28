// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types.Errors;
/// <summary>
/// Métodos de extension de <c>MlErrorDetails</c>.
/// </summary>
public static partial class MlErrorDetailsExtensions
{
    /// <summary>
    /// Ejecuta <c>ToErrorsMessages</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    [global::System.Obsolete("Renamed to GetErrorMessages. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IEnumerable<string> ToErrorsMessages(this MlErrorDetails source) => GetErrorMessages(source);
    /// <summary>
    /// Ejecuta <c>ToErrorsDescription</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Representación textual obtenida.</returns>
    [global::System.Obsolete("Renamed to GetErrorsSummary. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string ToErrorsDescription(this IEnumerable<MlError> source) => GetErrorsSummary(source);
    /// <summary>
    /// Ejecuta <c>ToErrorsDescription</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Representación textual obtenida.</returns>
    [global::System.Obsolete("Renamed to GetErrorsSummary. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string ToErrorsDescription(this MlErrorDetails source) => GetErrorsSummary(source);
    /// <summary>
    /// Ejecuta <c>ToDetailsDescription</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Representación textual obtenida.</returns>
    [global::System.Obsolete("Renamed to GetMetadataSummary. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string ToDetailsDescription(this MlErrorDetails source) => GetMetadataSummary(source);
}