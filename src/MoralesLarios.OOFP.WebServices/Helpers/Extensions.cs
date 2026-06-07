namespace MoralesLarios.OOFP.WebServices.Helpers;

public static class Extensions
{


    public static MlErrorsDetails BuildNotFoundPkError(this string tableName, params object[] pk)
        => MlErrorsDetails.FromErrorMessageDetails($"No data found for the {tableName} table by Id ({pk.GetPkValues()})",
                                                   new Dictionary<string, object>() { ["NotFound"] = $"No data found for the {tableName} table by Id ({pk.GetPkValues()})" });

}
