// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;
/// <summary>
/// Métodos de extension de <c>MlErrorDetails</c>.
/// </summary>
public static partial class MlErrorDetailsExtensions
{
    /// <summary>
    /// Ejecuta <c>ToProblemsDetailsInfo</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="obj">Obj.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to ToProblemDetails. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<ProblemDetailsData> ToProblemsDetailsInfo(this object obj) => ToProblemDetails(obj);
}
