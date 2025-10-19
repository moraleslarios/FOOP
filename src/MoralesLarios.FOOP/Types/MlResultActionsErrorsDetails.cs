using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Types;

public static class MlResultActionsErrorsDetails
{

    public static MlResult<T> GetDetail<T>(this MlErrorsDetails source, string key)
    {
        if ( ! source.Details.ContainsKey(key)) return source.AddError($"The key {key} does not exist in the details");
        
        var result = source.Details[key] is T value 
                            ? MlResult<T>.Valid(value) 
                            : source.AddError($"The key {key} does not contain a value of type {typeof(T).Name}");

        return result;
    }



    public static MlResult<T> MergeErrorsDetailsIfFail<T>(this MlResult<T> source,
                                                               MlResult<T> secondary)
        => source.Match(
                            fail : errorDetails => secondary.Match(
                                                                        fail : errorDetails2 => errorDetails.Merge(errorDetails2),
                                                                        valid: _             => errorDetails
                                                                    ),
                            valid: _ => source
                        );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this MlResult<T> source,
                                                                                MlResult<T> secondary)
        => await source.MatchAsync(
                                    failAsync : errorDetails => secondary.MatchAsync(
                                                                                failAsync : errorDetails2 => errorDetails.Merge(errorDetails2).ToMlResultFailAsync<T>(),
                                                                                validAsync: _             => errorDetails.ToMlResultFailAsync<T>()
                                                                            ),
                                    validAsync: _ => source.ToAsync()
                                );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                MlResult<T>       secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFail(secondary);

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                Task<MlResult<T>> secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFail(await secondary);

    public static MlResult<T> MergeErrorsDetailsIfFailDiferentTypes<T,T2>(this MlResult<T > source,
                                                                               MlResult<T2> secondary)
        => source.Match(
                                    fail : errorDetails => secondary.Match(
                                                                                fail : errorDetails2 => errorDetails.Merge(errorDetails2),
                                                                                valid: _             => errorDetails
                                                                            ),
                                    valid: _ => source
                                );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T,T2>(this MlResult<T > source,
                                                                                                MlResult<T2> secondary)
        => await source.MatchAsync(
                                    failAsync : errorDetails => secondary.MatchAsync(
                                                                                        failAsync : errorDetails2 => errorDetails.Merge(errorDetails2).ToMlResultFailAsync<T>(),
                                                                                        validAsync: _             => errorDetails.ToMlResultFailAsync<T>()
                                                                            ),
                                    validAsync: _ => source.ToAsync()
                                );

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T,T2>(this Task<MlResult<T>> sourceAsync,
                                                                                                MlResult<T2>      secondary)
        => (await sourceAsync).MergeErrorsDetailsIfFailDiferentTypes(secondary);

    public static async Task<MlResult<T>> MergeErrorsDetailsIfFailDiferentTypesAsync<T,T2>(this Task<MlResult<T>>  sourceAsync,
                                                                                                Task<MlResult<T2>> secondaryAsync)
        => (await sourceAsync).MergeErrorsDetailsIfFailDiferentTypes(await secondaryAsync);


    public static MlResult<T> GetDetailValue<T>(this MlErrorsDetails source)
    {
        if ( ! source.Details.ContainsKey(VALUE_KEY)) return source.AddError($"The key {VALUE_KEY} does not exist in the details");  //MlResult<T>.Fail($"The key {VALUE_KEY} does not exist in the details");
        
        var result = source.Details[VALUE_KEY] is T value 
                            ? MlResult<T>.Valid(value) 
                            : source.AddError($"The key {VALUE_KEY} does not contain a value of type {typeof(T).Name}");

        return result;
    }


    public static MlResult<T> AddIfFailValue<T, TValue>(this MlResult<T> source,
                                                             TValue      value)
        => source.Match(
                            fail : errorsDetails => errorsDetails.AddDetailValue(value),
                            valid: _             => source
                        );

    public static async Task<MlResult<T>> AddIfFailValueAsync<T, TValue>(this MlResult<T> source,
                                                                              TValue      value)
        => await source.AddIfFailValue(value).ToAsync();

    public static async Task<MlResult<T>> AddIfFailValue<T, TValue>(this Task<MlResult<T>> sourceAsync,
                                                             TValue            value)
    {
        var result = await sourceAsync.MatchAsync(
                                                    failAsync : errorsDetails => errorsDetails.AddDetailValue(value).ToMlResultFailAsync<T>(),
                                                    validAsync: _             => sourceAsync
                                                );
        return result;
    }



    public static Task<MlResult<T>> GetDetailValueAsync<T>(this MlErrorsDetails source) => source.GetDetailValue<T>().ToAsync();


    public static MlResult<T> GetDetailException<T>(this MlErrorsDetails source) where T : Exception => source.GetDetail<T>(EX_DESC_KEY);
    public static Task<MlResult<T>> GetDetailExceptionAsync<T>(this MlErrorsDetails source) where T : Exception => source.GetDetail<T>(EX_DESC_KEY).ToAsync();
    public static MlResult<Exception> GetDetailException(this MlErrorsDetails source) => source.GetDetail<Exception>(EX_DESC_KEY);
    public static Task<MlResult<Exception>> GetDetailExceptionAsync(this MlErrorsDetails source) => source.GetDetail<Exception>(EX_DESC_KEY).ToAsync();
}
