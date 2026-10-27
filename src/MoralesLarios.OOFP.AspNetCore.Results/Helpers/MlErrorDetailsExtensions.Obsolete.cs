// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;
public static partial class MlErrorDetailsExtensions
{
    [global::System.Obsolete("Renamed to ToProblemDetails. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<ProblemDetailsData> ToProblemsDetailsInfo(this object obj) => ToProblemDetails(obj);
}
