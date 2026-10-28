namespace MoralesLarios.OOFP.Application.Helpers;

/// <summary>
/// Métodos de extension de <c></c>.
/// </summary>
public static partial class Extensions
{


    /// <summary>
    /// Construye la instancia solicitada de <c>EntityNotFoundError</c>.
    /// </summary>
    /// <param name="tableName">Nombre del elemento.</param>
    /// <param name="pk">Colección de <c>pk</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails BuildEntityNotFoundError(this string tableName, params object[] pk)
        => MlProblemDetails.NotFoundError(
            detail: $"No data found for the {tableName} table by Id ({pk.FormatCompositeKey()})");

}
