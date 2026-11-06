
namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
/// <summary>
/// Provides access to the EF Core context used by an implementation.
/// </summary>
public interface IGetContextable
{
    /// <summary>
    /// Gets the EF Core context used by the implementation.
    /// </summary>
    /// <returns>The EF Core context.</returns>
    DbContext GetContext();
}
