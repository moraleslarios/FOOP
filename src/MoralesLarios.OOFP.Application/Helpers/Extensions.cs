namespace MoralesLarios.OOFP.Application.Helpers;

public static class Extensions
{


    public static MlErrorDetails BuildNotFoundPkError(this string tableName, params object[] pk)
        => MlProblemsDetails.NotFoundError(
            detail: $"No data found for the {tableName} table by Id ({pk.GetPkValues()})");

}
