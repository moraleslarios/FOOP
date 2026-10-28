
namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative;
/// <summary>
/// Contrato de <c>IGetContextable</c>.
/// </summary>
public interface IGetContextable
{
    /// <summary>
    /// Obtiene el elemento solicitado de <c>Context</c>.
    /// </summary>
    /// <returns>Valor de tipo <c>DbContext</c> obtenido por la operación.</returns>
    DbContext GetContext();
}
