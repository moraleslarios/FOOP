// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Application;
public static partial class RegisterServices
{
    [global::System.Obsolete("Renamed to AddScopedCrudServicesFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddScopedtGenServicesFpWithoutReposGeneral(this IServiceCollection services) => AddScopedCrudServicesFpWithoutRepositories(services);
    [global::System.Obsolete("Renamed to AddScopedCrudServicesDuplexFpWithoutRepositories. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddScopedtGenServicesDuplexFpWithoutReposGeneral(this IServiceCollection services) => AddScopedCrudServicesDuplexFpWithoutRepositories(services);
}