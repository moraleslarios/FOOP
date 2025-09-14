using System.ComponentModel.DataAnnotations;


namespace MoralesLarios.OOFP.Validation.Dataannotations.Helpers;
public static class Extensions
{

    public static IEnumerable<ValidationResult> ValidateObject(this object source)
    {
        ValidationContext valContext = new ValidationContext(source, null, null);
        var result     = new List<ValidationResult>();
        Validator.TryValidateObject(source, valContext, result, true);
        
        return result;
    } 


    public static MlResult<T> ValidateWithDataannotations<T>(this T source)
    {
        var result = source!.ValidateObject().ToMlResultValid()
                                .Map ( valResults => valResults.Select(x => x.ErrorMessage))
                                .Bind( errors     => errors.Any() ? errors!.ToMlResultFail<T>() : source.ToMlResultValid<T>());
        return result;

    }

    public static Task<MlResult<T>> ValidateWithDataannotationsAsync<T>(this T source)
        => source!.ValidateWithDataannotations().ToAsync();

    public static async Task<MlResult<T>> ValidateWithDataannotationsAsync<T>(this Task<T> sourceAsync)
        => await (await sourceAsync).ValidateWithDataannotationsAsync();


    public static MlResult<IEnumerable<T>> ValidateWithDataannotations<T>(this IEnumerable<T> source)
    {
        var result = source.Select(x => x.ValidateWithDataannotations())
                            .FusionErrosIfExists();
        return result;
    }

    public static Task<MlResult<IEnumerable<T>>> ValidateWithDataannotationsAsync<T>(this IEnumerable<T> source)
        => source.ValidateWithDataannotations().ToAsync();

    public static async Task<MlResult<IEnumerable<T>>> ValidateWithDataannotationsAsync<T>(this Task<IEnumerable<T>> sourceAsync)
        => await (await sourceAsync).ValidateWithDataannotationsAsync();




}