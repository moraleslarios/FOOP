namespace MoralesLarios.OOFP.Helpers;
public static class Constants
{
    public static string DEFAULT_ERROR_MESSAGE { get; } = "Without custom error eessage. For more info, view 'Ex(s) details exceptions.";
    public static string EX_DESC_KEY           { get; } = "Ex";
    public static string VALUE_KEY             { get; } = "Value";



    public static string DEFAULT_EX_ERROR_MESSAGE(Exception ex) => $"An error occurred while executing the function. Error: {ex.Message}.More info in Ex Details.";

}
