// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.EntityFrameworkCore;
/// <summary>
/// Representa <c>RegisterServices</c>.
/// </summary>
public static partial class RegisterServices
{
    /// <summary>
    /// Ejecuta <c>ResolveRepoFp</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TRepo">Tipo genérico TRepo.</typeparam>
    /// <returns>Valor de tipo <c>TRepo</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to GetRepository. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static TRepo ResolveRepoFp<TRepo>() => GetRepository<TRepo>();
    /// <summary>
    /// Añade el elemento indicado de <c>TransientOOFPRepos</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddOofpRepositoriesTransient. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddTransientOOFPRepos<T, TContext>(this IServiceCollection services)
        where T : class where TContext : DbContext => AddOofpRepositoriesTransient<T, TContext>(services);
    /// <summary>
    /// Añade el elemento indicado de <c>ScopedOOFPRepos</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddOofpRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddScopedOOFPRepos<T, TContext>(this IServiceCollection services)
        where T : class where TContext : DbContext => AddOofpRepositories<T, TContext>(services);
    /// <summary>
    /// Añade el elemento indicado de <c>SingletonOOFPRepos</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
    /// <param name="services">Colección de servicios donde se registran las dependencias.</param>
    /// <returns>Valor de tipo <c>IServiceCollection</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to AddOofpRepositoriesSingleton. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddSingletonOOFPRepos<T, TContext>(this IServiceCollection services)
        where T : class where TContext : DbContext => AddOofpRepositoriesSingleton<T, TContext>(services);
}