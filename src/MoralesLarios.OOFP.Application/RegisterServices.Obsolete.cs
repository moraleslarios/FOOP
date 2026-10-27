// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Application;
public static partial class RegisterServices
{
    [global::System.Obsolete("Renamed to AddScopedCrudServicesFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddScopedtGenServicesFpWithoutReposGeneral(this IServiceCollection services) => AddScopedCrudServicesFpWithoutRepositories(services);
    [global::System.Obsolete("Renamed to AddScopedCrudServicesDuplexFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddScopedtGenServicesDuplexFpWithoutReposGeneral(this IServiceCollection services) => AddScopedCrudServicesDuplexFpWithoutRepositories(services);
    [global::System.Obsolete("Renamed to AddTransientCrudServicesFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddTransientGenServicesFpWithoutReposGeneral(this IServiceCollection services) => AddTransientCrudServicesFpWithoutRepositories(services);
    [global::System.Obsolete("Renamed to AddSingletonCrudServicesFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddSingletonGenServicesFpWithoutReposGeneral(this IServiceCollection services) => AddSingletonCrudServicesFpWithoutRepositories(services);
    [global::System.Obsolete("Renamed to AddTransientCrudServicesDuplexFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddTransientGenServicesDuplexFpWithoutReposGeneral(this IServiceCollection services) => AddTransientCrudServicesDuplexFpWithoutRepositories(services);
    [global::System.Obsolete("Renamed to AddSingletonCrudServicesDuplexFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddSingletonGenServicesDuplexFpWithoutReposGeneral(this IServiceCollection services) => AddSingletonCrudServicesDuplexFpWithoutRepositories(services);
}