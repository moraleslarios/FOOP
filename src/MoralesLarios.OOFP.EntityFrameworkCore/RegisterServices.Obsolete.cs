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
}
