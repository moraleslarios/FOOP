namespace MoralesLarios.OOFP.WebServices.Helpers;

public static class Extensions
{


    public static MlErrorsDetails BuildNotFoundPkError(this string tableName, params object[] pk)
        => MlProblemsDetails.NotFoundError(
            detail: $"No data found for the {tableName} table by Id ({pk.GetPkValues()})");

}
