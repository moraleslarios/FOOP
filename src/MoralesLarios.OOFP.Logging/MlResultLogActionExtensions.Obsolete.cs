// Puentes [Obsolete] de la v2.2.0: nombres antiguos de la clase Extensions que reenvian al nombre
// nuevo (MlResultLogActionExtensions.LogActionResult). Se eliminan en la v3.0.0.

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Puentes de compatibilidad de la antigua clase <c>Extensions</c>.
/// </summary>
/// <remarks>
/// Clase obsoleta conservada por compatibilidad binaria y de código fuente. Use
/// <see cref="MlResultLogActionExtensions"/>.
/// </remarks>
[global::System.Obsolete("Renamed to MlResultLogActionExtensions. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public static class Extensions
{

    /// <summary>
    /// Registra el desenlace completo de la acción de un método sobre el resultado recibido.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="methodActionDesc">Descripción de la acción del método que se está registrando.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to MlResultLogActionExtensions.LogActionResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> MyMethodFinalLog<T>(this MlResult<T> source, ILogger logger, string methodActionDesc)
        => MlResultLogActionExtensions.LogActionResult<T>(source, logger, methodActionDesc);

    /// <summary>
    /// Registra el desenlace completo de la acción de un método sobre el resultado recibido. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="methodActionDesc">Descripción de la acción del método que se está registrando.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to MlResultLogActionExtensions.LogActionResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> MyMethodFinalLogAsync<T>(this Task<MlResult<T>> sourceAsync, ILogger logger, string methodActionDesc)
        => MlResultLogActionExtensions.LogActionResultAsync<T>(sourceAsync, logger, methodActionDesc);

}
