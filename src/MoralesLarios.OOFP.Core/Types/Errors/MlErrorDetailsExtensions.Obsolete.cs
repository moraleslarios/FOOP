// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Types.Errors;
public static partial class MlErrorDetailsExtensions
{
    [global::System.Obsolete("Renamed to GetErrorMessages. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IEnumerable<string> ToErrorsMessages(this MlErrorDetails source) => GetErrorMessages(source);
    [global::System.Obsolete("Renamed to GetErrorsSummary. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string ToErrorsDescription(this IEnumerable<MlError> source) => GetErrorsSummary(source);
    [global::System.Obsolete("Renamed to GetErrorsSummary. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string ToErrorsDescription(this MlErrorDetails source) => GetErrorsSummary(source);
    [global::System.Obsolete("Renamed to GetMetadataSummary. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string ToDetailsDescription(this MlErrorDetails source) => GetMetadataSummary(source);
}