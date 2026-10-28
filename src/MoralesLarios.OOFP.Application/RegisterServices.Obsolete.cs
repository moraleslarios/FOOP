// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Application;
/// <summary>
/// Representa <c>RegisterServices</c>.
/// </summary>
public static partial class RegisterServices
{
    /// <summary>
    /// Añade el elemento indicado de <c>ScopedtGenServicesFpWithoutReposGeneral</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddScopedCrudServicesFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddScopedtGenServicesFpWithoutReposGeneral(this IServiceCollection services) => AddScopedCrudServicesFpWithoutRepositories(services);
    /// <summary>
    /// Añade el elemento indicado de <c>ScopedtGenServicesDuplexFpWithoutReposGeneral</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddScopedCrudServicesDuplexFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddScopedtGenServicesDuplexFpWithoutReposGeneral(this IServiceCollection services) => AddScopedCrudServicesDuplexFpWithoutRepositories(services);
    /// <summary>
    /// Añade el elemento indicado de <c>TransientGenServicesFpWithoutReposGeneral</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddTransientCrudServicesFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddTransientGenServicesFpWithoutReposGeneral(this IServiceCollection services) => AddTransientCrudServicesFpWithoutRepositories(services);
    /// <summary>
    /// Añade el elemento indicado de <c>SingletonGenServicesFpWithoutReposGeneral</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddSingletonCrudServicesFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddSingletonGenServicesFpWithoutReposGeneral(this IServiceCollection services) => AddSingletonCrudServicesFpWithoutRepositories(services);
    /// <summary>
    /// Añade el elemento indicado de <c>TransientGenServicesDuplexFpWithoutReposGeneral</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddTransientCrudServicesDuplexFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddTransientGenServicesDuplexFpWithoutReposGeneral(this IServiceCollection services) => AddTransientCrudServicesDuplexFpWithoutRepositories(services);
    /// <summary>
    /// Añade el elemento indicado de <c>SingletonGenServicesDuplexFpWithoutReposGeneral</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddSingletonCrudServicesDuplexFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddSingletonGenServicesDuplexFpWithoutReposGeneral(this IServiceCollection services) => AddSingletonCrudServicesDuplexFpWithoutRepositories(services);
}