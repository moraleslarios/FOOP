namespace MoralesLarios.OOFP.Application.Helpers;

public static partial class Extensions
{


    public static MlErrorDetails BuildEntityNotFoundError(this string tableName, params object[] pk)
        => MlProblemDetails.NotFoundError(
            detail: $"No data found for the {tableName} table by Id ({pk.FormatCompositeKey()})");

}
