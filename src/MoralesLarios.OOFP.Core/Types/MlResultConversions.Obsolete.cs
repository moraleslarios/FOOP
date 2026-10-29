// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using System.Reflection;
using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Sobrecargas obsoletas de <see cref="MlResultConversions"/>, conservadas como alias de compatibilidad con la versión 2.0.
/// </summary>
public static partial class MlResultConversions
{
    /// <summary>
    /// Compone el mensaje de error definitivo combinando el mensaje indicado con la información de la excepción capturada.
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
    /// Compone el mensaje de error definitivo aplicando a la excepción capturada la función constructora de mensajes indicada.
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
