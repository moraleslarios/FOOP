namespace MoralesLarios.OOFP.Validation.Dataannotations;

public static class DataannotationsValidator
{

    public static MlResult<T> Validate<T>(T source)
    {
        var result = EnsureFp.NotNull(source, $"{nameof(source)} no be null")
                                .Bind( _ => source.ValidateWithDataannotations());

        return result;
    }


    public static Task<MlResult<T>> ValidateAsync<T>(T source) => source.ValidateWithDataannotations().ToAsync();

    public static async Task<MlResult<T>> ValidateAsync<T>(Task<T> sourceAsync) 
        => await (await sourceAsync).ValidateWithDataannotationsAsync();


    public static MlResult<IEnumerable<T>> Validate<T>(IEnumerable<T> source)
    {
        var result = EnsureFp.NotNull(source, $"{nameof(source)} no be null")
                                .Bind( _ => EnsureFp.NotEmpty(source, $"{nameof(source)} no be empty"))
                                .Bind( _ => source.ValidateWithDataannotations());
        return result;
    }

    public static Task<MlResult<IEnumerable<T>>> ValidateAsync<T>(IEnumerable<T> source) => source.ValidateWithDataannotations().ToAsync();

    public static async Task<MlResult<IEnumerable<T>>> ValidateAsync<T>(Task<IEnumerable<T>> sourceAsync)
        => await (await sourceAsync).ValidateWithDataannotationsAsync();
}
