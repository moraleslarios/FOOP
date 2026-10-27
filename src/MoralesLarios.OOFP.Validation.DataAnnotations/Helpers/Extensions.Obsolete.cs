// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using System.ComponentModel.DataAnnotations;

namespace MoralesLarios.OOFP.Validation.DataAnnotations.Helpers;
public static partial class Extensions
{
    [global::System.Obsolete("Renamed to ValidateDataAnnotations. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static IEnumerable<ValidationResult> ValidateObject(this object source) => ValidateDataAnnotations(source);
    [global::System.Obsolete("Renamed to ValidateWithDataAnnotations. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> ValidateWithDataannotations<T>(this T source) => ValidateWithDataAnnotations<T>(source);
    [global::System.Obsolete("Renamed to ValidateWithDataAnnotations. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<T>> ValidateWithDataannotations<T>(this IEnumerable<T> source) => ValidateWithDataAnnotations<T>(source);
    [global::System.Obsolete("Renamed to ValidateWithDataAnnotationsAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> ValidateWithDataannotationsAsync<T>(this T source) => ValidateWithDataAnnotationsAsync<T>(source);
    [global::System.Obsolete("Renamed to ValidateWithDataAnnotationsAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> ValidateWithDataannotationsAsync<T>(this Task<T> sourceAsync) => ValidateWithDataAnnotationsAsync<T>(sourceAsync);
    [global::System.Obsolete("Renamed to ValidateWithDataAnnotationsAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>> ValidateWithDataannotationsAsync<T>(this IEnumerable<T> source) => ValidateWithDataAnnotationsAsync<T>(source);
    [global::System.Obsolete("Renamed to ValidateWithDataAnnotationsAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>> ValidateWithDataannotationsAsync<T>(this Task<IEnumerable<T>> sourceAsync) => ValidateWithDataAnnotationsAsync<T>(sourceAsync);
}