namespace MoralesLarios.OOFP.WebControllers.Helpers;

public class ExtendedProblemDetails : ProblemDetails
{
    public Dictionary<string, object> Extensions { get; set; } = new();
}

public static class MlActionResults
{

    public static IActionResult NotFound(string                     title      = null!, 
                                         string                     detail     = null!, 
                                         string                     type       = null!, 
                                         string                     traceId    = null!, 
                                         Dictionary<string, object> extensions = null!)
    {
        var result = new ObjectResult(new ExtendedProblemDetails
        {
            Status     = StatusCodes.Status404NotFound,
            Title      = title      ?? "Not found",
            Detail     = detail     ?? string.Empty,
            Type       = type       ?? "https://www.puntonetalpunto.net/",
            Extensions = extensions ?? new Dictionary<string, object>()
        })
        {
            StatusCode = StatusCodes.Status404NotFound
        };

        return result;
    }


}
