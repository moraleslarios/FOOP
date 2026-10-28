// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Http.Helpers;

/// <summary>
/// Métodos de extension de <c>MlResponseWeb</c>.
/// </summary>
public static partial class MlResponseWebExtensions
{
    /// <summary>
    /// Ejecuta <c>ToResponseErrorsDescription</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="response">Respuesta recibida.</param>
    /// <returns>Tarea que produce el representación textual obtenida.</returns>
    [global::System.Obsolete("Renamed to ReadErrorDescriptionAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<string> ToResponseErrorsDescriptionAsync(this HttpResponseMessage response) => response.ReadErrorDescriptionAsync();

    /// <summary>
    /// Ejecuta <c>ToResponseErrorsDescription</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="response">Respuesta recibida.</param>
    /// <returns>Representación textual obtenida.</returns>
    [global::System.Obsolete("Replaced by the asynchronous ReadErrorDescriptionAsync. The synchronous version blocks the calling thread. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string ToResponseErrorsDescription(this HttpResponseMessage response) => response.ReadErrorDescriptionAsync().GetAwaiter().GetResult();
}
