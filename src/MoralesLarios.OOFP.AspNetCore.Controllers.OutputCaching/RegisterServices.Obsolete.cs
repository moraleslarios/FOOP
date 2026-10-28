// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching;
/// <summary>
/// Representa <c>RegisterServices</c>.
/// </summary>
public static partial class RegisterServices
{
    /// <summary>
    /// Añade el elemento indicado de <c>WebControllersCache</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <param name="options">Opciones que modifican el comportamiento de la operación.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddMlControllersOutputCaching. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddWebControllersCache(this IServiceCollection services, Action<OutputCacheOptions> options = null !) => AddMlControllersOutputCaching(services, options);
}
