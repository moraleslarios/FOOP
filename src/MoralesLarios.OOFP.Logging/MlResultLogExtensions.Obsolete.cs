// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.Logging;
public static partial class MlResultLogExtensions
{
    [global::System.Obsolete("Renamed to LogCompletedResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> LogMlResultFinal<T>(this MlResult<T> source, ILogger logger, Func<T, string> validBuildMessage, Func<MlErrorDetails, string> failBuildMessage) => LogCompletedResult<T>(source, logger, validBuildMessage, failBuildMessage);
    [global::System.Obsolete("Renamed to LogCompletedResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> LogMlResultFinal<T>(this MlResult<T> source, ILogger logger, string validMessage, string failMessage) => LogCompletedResult<T>(source, logger, validMessage, failMessage);
    [global::System.Obsolete("Renamed to LogCompletedResult. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> LogMlResultFinal<T>(this Task<MlResult<T>> sourceAsync, ILogger logger, string validMessage, string failMessage) => LogCompletedResult<T>(sourceAsync, logger, validMessage, failMessage);
    [global::System.Obsolete("Renamed to LogCompletedResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> LogMlResultFinalAsync<T>(this MlResult<T> source, ILogger logger, Func<T, string> validBuildMessage, Func<MlErrorDetails, string> failBuildMessage) => LogCompletedResultAsync<T>(source, logger, validBuildMessage, failBuildMessage);
    [global::System.Obsolete("Renamed to LogCompletedResultAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> LogMlResultFinalAsync<T>(this Task<MlResult<T>> sourceAsync, ILogger logger, Func<T, string> validBuildMessage, Func<MlErrorDetails, string> failBuildMessage) => LogCompletedResultAsync<T>(sourceAsync, logger, validBuildMessage, failBuildMessage);
}