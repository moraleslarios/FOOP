// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using System.Reflection;
using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Representa <c>MlResultConversions</c>.
/// </summary>
public static partial class MlResultConversions
{
    /// <summary>
    /// Construye la instancia solicitada de <c>ErrorMessage</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <param name="ex">Excepción capturada.</param>
    /// <returns>Representación textual obtenida.</returns>
    [global::System.Obsolete("Renamed to FormatErrorMessage. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string BuildErrorMessage(string errorMessage, Exception ex) => FormatErrorMessage(errorMessage, ex);
    /// <summary>
    /// Construye la instancia solicitada de <c>ErrorMessage</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="messageBuilder">Función a aplicar.</param>
    /// <param name="ex">Excepción capturada.</param>
    /// <returns>Representación textual obtenida.</returns>
    [global::System.Obsolete("Renamed to FormatErrorMessage. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string BuildErrorMessage(Func<Exception, string> messageBuilder, Exception ex) => FormatErrorMessage(messageBuilder, ex);
}
