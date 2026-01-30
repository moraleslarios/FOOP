using System.ComponentModel.DataAnnotations;

namespace MoralesLarios.OOFP.Helpers.Extensions;
public static class Extensions
{

    public static IEnumerable<ValidationResult> ValidateObject(this object source)
    {
        var valContext = new ValidationContext(source, null, null);

        var resultado = new List<ValidationResult>();

        Validator.TryValidateObject(source, valContext, resultado, true);

        return resultado;
    }

    public static T? ToNullable<T>(this T source)
        where T : struct
            => source;


    public static Dictionary<string, object> AppendExDetails(this Dictionary<string, object> source, Exception ex)
    {
        var exKeys = source.Keys.Where(x => x.StartsWith(EX_DESC_KEY)).ToList();

        var exKey = exKeys.Any() ? $"{EX_DESC_KEY}{exKeys.Count + 1}" : EX_DESC_KEY;

        var result = source.ToDictionary(x => x.Key, x => x.Value);

        result.Add(exKey, ex);

        return result;
    }





    public static T With<T>(this T source, params Action<T>[] changes) 
        where T : class
    {
        foreach (var change in changes)
        {
            change(source);
        }

        return source;
    }


    public static Task<T> WithAsync<T>(this T source, params Action<T>[] changes) 
        where T : class
        => source.With(changes).ToAsync();

    public static async Task<T> WithAsync<T>(this Task<T> sourceAsync, params Action<T>[] changes) 
        where T : class
        => await (await sourceAsync).WithAsync(changes);



    public static Task VoidToAsync<T>(this T source, Action<T> voidAction)
    {
        voidAction(source);

        return Task.CompletedTask;
    }


    public static Func<T, Task<TResult>> ToFuncTask<T, TResult>(this Func<T, TResult> func) => x => func(x).ToAsync();

    public static Func<MlErrorsDetails, Task<TResult>> ToFuncTask<TResult>(this Func<MlErrorsDetails, TResult> func) => errorsDetails => func(errorsDetails).ToAsync();

    public static Func<T, Task> ToFuncTask<T>(this Action<T> action)
    {
        return x =>
        {
            action(x);
            return Task.CompletedTask;
        };
    }

    public static Func<MlErrorsDetails, Task> ToFuncTask(this Action<MlErrorsDetails> action)
    {
        return errorsDetails =>
        {
            action(errorsDetails);
            return Task.CompletedTask;
        };
    }






}
