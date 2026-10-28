// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoralesLarios.OOFP.Configuration.Config;

namespace MoralesLarios.OOFP.Configuration;
/// <summary>
/// Representa <c>RegisterServices</c>.
/// </summary>
public static partial class RegisterServices
{
    /// <summary>
    /// Añade el elemento indicado de <c>MlUtilitiesConfig</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <param name="configuration">Configuración de la aplicacion.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddMlConfigurationReader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddMlUtilitiesConfig(this IServiceCollection services, IConfiguration configuration) => AddMlConfigurationReader(services, configuration);
}
