namespace MoralesLarios.OOFP.Helpers;
public static class EnsureFp
{


    public static MlResult<T> NotNull<T>(T value, string errorMessage)
        => That(value, value is not null, errorMessage);

    public static MlResult<T> NotNull<T>(T value, MlErrorsDetails errorsDetails)
        => That(value, value is not null, errorsDetails);


    public static MlResult<IEnumerable<T>> NotEmpty<T>(IEnumerable<T> value, string message)
        => That(value, value != null && value.Any(), message);

    public static MlResult<IEnumerable<T>> NotEmpty<T>(IEnumerable<T> value, MlErrorsDetails errorsDetails)
        => That(value, value != null && value.Any(), errorsDetails);


    public static MlResult<string> NotNullEmptyOrWhitespace(string value, string errorMessage)
         => That(value, ! string.IsNullOrWhiteSpace(value), errorMessage);

    public static MlResult<string> NotNullEmptyOrWhitespace(string value, MlErrorsDetails errorsDetails)
        => That(value, !string.IsNullOrWhiteSpace(value), errorsDetails);


    public static MlResult<T> That<T>(T value, bool condition, string errorMessage)
        => condition ? MlResult<T>.Valid(value) : MlResult<T>.Fail(errorMessage);


    public static MlResult<T> That<T>(T value, bool condition, MlErrorsDetails errorsDetails)
        => condition ? MlResult<T>.Valid(value) : errorsDetails.ToMlResultFail<T>();


    public static Task<MlResult<T>> ThatAsync<T>(T value, bool condition, string errorMessage)
        => condition ? MlResult<T>.Valid(value).ToAsync() : MlResult<T>.Fail(errorMessage).ToAsync();

    public static Task<MlResult<T>> ThatAsync<T>(T value, bool condition, MlErrorsDetails errorsDetails)
        => condition ? MlResult<T>.Valid(value).ToAsync() : errorsDetails.ToMlResultFail<T>().ToAsync();



    public static Task<MlResult<T>> NotNullAsync<T>(T value, string errorMessage)
        => ThatAsync(value, value is not null, errorMessage);

    public static Task<MlResult<T>> NotNullAsync<T>(T value, MlErrorsDetails errorsDetails)
        => ThatAsync(value, value is not null, errorsDetails);


    public static Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(IEnumerable<T> value, string message)
        => ThatAsync(value, value != null && value.Any(), message);

    public static Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(IEnumerable<T> value, MlErrorsDetails errorsDetails)
        => ThatAsync(value, value != null && value.Any(), errorsDetails);


    public static Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(string value, string errorMessage)
         => ThatAsync(value, ! string.IsNullOrWhiteSpace(value), errorMessage);

    public static Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(string value, MlErrorsDetails errorsDetails)
        => ThatAsync(value, !string.IsNullOrWhiteSpace(value), errorsDetails);






}
