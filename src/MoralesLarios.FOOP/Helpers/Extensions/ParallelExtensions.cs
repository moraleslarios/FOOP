namespace MoralesLarios.OOFP.Helpers.Extensions;
public static class ParallelExtensions
{


    public static Task<T> ToAsync<T>(this T value) => Task.FromResult(value);




}
