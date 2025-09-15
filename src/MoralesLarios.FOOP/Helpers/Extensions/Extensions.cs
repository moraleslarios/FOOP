using System.ComponentModel.DataAnnotations;

namespace MoralesLarios.OOFP.Helpers.Extensions;
public static class Extensions
{


    //public static string ToDesc<T>(this IEnumerable<T> source) where T : struct => source.Select(x => x.ToString()).ToDesc();


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



    //static void Main()
    //{
    //    Func<int, int> source = x => x + 2;

    //    MethodInfo methodInfo = source.Method;
    //    MethodBody methodBody = methodInfo.GetMethodBody();

    //    byte[] ilBytes = methodBody.GetILAsByteArray();
    //    string body = DisassembleIL(ilBytes);

    //    Console.WriteLine(body);
    //}


    //public static string ToFuncDescBody(this MethodInfo source)
    //{
    //    MethodBody methodBody = source.GetMethodBody();

    //    byte[] ilBytes = methodBody.GetILAsByteArray();
    //    string result = DisassembleIL(ilBytes);

    //    return result;
    //}




    //private static string DisassembleIL(byte[] ilBytes)
    //{
    //    DynamicMethod dynamicMethod = new DynamicMethod("Disassembly", typeof(string), new Type[] { typeof(byte[]) }, typeof(Extensions).Module);
    //    ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
    //    ilGenerator.Emit(OpCodes.Ldarg_0);
    //    ilGenerator.Emit(OpCodes.Ret);

    //    object[] parameters = new object[] { ilBytes };

    //    var data = dynamicMethod.Invoke(null, parameters);

    //    string disassembledIL = (string)dynamicMethod.Invoke(null, parameters);

    //    return disassembledIL;
    //}






}
