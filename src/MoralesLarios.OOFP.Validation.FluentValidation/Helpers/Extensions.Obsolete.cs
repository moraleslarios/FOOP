// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Validation.FluentValidation.Helpers;
public static partial class Extensions
{
    [global::System.Obsolete("Renamed to ValidateWithFluentValidation. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<T> ValidateWithFluentValidations<T, TValidator>(this T source)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidation<T, TValidator>(source);
    [global::System.Obsolete("Renamed to ValidateWithFluentValidation. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IEnumerable<T>> ValidateWithFluentValidations<T, TValidator>(this IEnumerable<T> source)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidation<T, TValidator>(source);
    [global::System.Obsolete("Renamed to ValidateWithFluentValidationAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> ValidateWithFluentValidationsAsync<T, TValidator>(this T source)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidationAsync<T, TValidator>(source);
    [global::System.Obsolete("Renamed to ValidateWithFluentValidationAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<T>> ValidateWithFluentValidationsAsync<T, TValidator>(this Task<T> sourceAsync)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidationAsync<T, TValidator>(sourceAsync);
    [global::System.Obsolete("Renamed to ValidateWithFluentValidationAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>> ValidateWithFluentValidationsAsync<T, TValidator>(this IEnumerable<T> source)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidationAsync<T, TValidator>(source);
    [global::System.Obsolete("Renamed to ValidateWithFluentValidationAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IEnumerable<T>>> ValidateWithFluentValidationsAsync<T, TValidator>(this Task<IEnumerable<T>> sourceAsync)
        where T : class where TValidator : AbstractValidator<T>, new() => ValidateWithFluentValidationAsync<T, TValidator>(sourceAsync);
}