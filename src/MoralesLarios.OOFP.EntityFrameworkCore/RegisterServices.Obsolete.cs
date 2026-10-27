// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.EntityFrameworkCore;
public static partial class RegisterServices
{
    [global::System.Obsolete("Renamed to GetRepository. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static TRepo ResolveRepoFp<TRepo>() => GetRepository<TRepo>();
    [global::System.Obsolete("Renamed to AddOofpRepositoriesTransient. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddTransientOOFPRepos<T, TContext>(this IServiceCollection services)
        where T : class where TContext : DbContext => AddOofpRepositoriesTransient<T, TContext>(services);
    [global::System.Obsolete("Renamed to AddOofpRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddScopedOOFPRepos<T, TContext>(this IServiceCollection services)
        where T : class where TContext : DbContext => AddOofpRepositories<T, TContext>(services);
    [global::System.Obsolete("Renamed to AddOofpRepositoriesSingleton. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddSingletonOOFPRepos<T, TContext>(this IServiceCollection services)
        where T : class where TContext : DbContext => AddOofpRepositoriesSingleton<T, TContext>(services);
}