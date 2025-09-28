using MoralesLarios.OOFP.Types.Errors;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.Types;
public static class MlResultActionsSeveral
{


    #region EmptyToFailed



    public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T> items, 
                                                                  MlError        error)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid() 
                : error.ToMlResultFail<IEnumerable<T>>();

    public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T>  items, 
                                                                  MlErrorsDetails errorsDetails)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid() 
                : errorsDetails.ToMlResultFail<IEnumerable<T>>();

    public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T> items, 
                                                                  string         messageError)
        => EmptyToFailed(items, MlError.FromErrorMessage(messageError));



    public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this IEnumerable<T> items, 
                                                                             MlError        error)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid().ToAsync() 
                : error.ToMlResultFail<IEnumerable<T>>().ToAsync();

    public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this IEnumerable<T>  items, 
                                                                             MlErrorsDetails errorsDetails)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid().ToAsync() 
                : errorsDetails.ToMlResultFail<IEnumerable<T>>().ToAsync();

    public static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this IEnumerable<T> items, 
                                                                             string         messageError)
        => EmptyToFailed(items, MlError.FromErrorMessage(messageError)).ToAsync()!;



    public async static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this Task<IEnumerable<T>> itemsAsync, 
                                                                                   MlError              error)
    {
        var partialMlResult = await itemsAsync;

        var result = partialMlResult.EmptyToFailed(error);

        return result!;
    }

    public static async Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this Task<IEnumerable<T>> itemsAsync, 
                                                                                   MlErrorsDetails errorsDetails)
    {
        var partialMlResult = await itemsAsync;

        var result = partialMlResult.EmptyToFailed(errorsDetails);

        return result!;
    }

    public async static Task<MlResult<IEnumerable<T>>>? EmptyToFailedAsync<T>(this Task<IEnumerable<T>> itemsAsync, 
                                                                                   string               messageError)
    {
        var partialMlResult = await itemsAsync;

        var result = partialMlResult.EmptyToFailed(messageError);

        return result!;
    }


    #endregion EmptyToFailed



    #region NullToFailed


    public static MlResult<T> NullToFailed<T>(this T               source,
                                                   MlErrorsDetails errorsDetails)
        => source == null ? errorsDetails.ToMlResultFail<T>() : source.ToMlResultValid();

    public static MlResult<T> NullToFailed<T>(this T       source,
                                                   MlError error)
        => source.NullToFailed(MlErrorsDetails.FromError(error));


    public static MlResult<T> NullToFailed<T>(this T      source,
                                                   string errorMessage)
        => source.NullToFailed(MlError.FromErrorMessage(errorMessage));



    public static MlResult<T> NullToFailed<T>(this T                   source,
                                                   IEnumerable<string> errorsMessage)
        => source.NullToFailed(MlErrorsDetails.FromEnumerableStrings(errorsMessage));


    public static async Task<MlResult<T>> NullToFailedAsync<T>(this T       source,
                                                                    MlError error)
        => await source.NullToFailedAsync(MlErrorsDetails.FromError(error));

    public static async Task<MlResult<T>> NullToFailedAsync<T>(this T               source,
                                                                    MlErrorsDetails errorsDetails)
        => await source.NullToFailed(errorsDetails).ToAsync();


    public static async Task<MlResult<T>> NullToFailedAsync<T>(this T      source,
                                                                    string errorMessage)
        => await source.NullToFailed(errorMessage).ToAsync();

    public static async Task<MlResult<T>> NullToFailedAsync<T>(this T                   source,
                                                                    IEnumerable<string> errorsMessage)
        => await source.NullToFailedAsync(MlErrorsDetails.FromEnumerableStrings(errorsMessage));

    public static async Task<MlResult<T>> NullToFailedAsync<T>(this Task<T> sourceAsync,
                                                                    string  errorMessage)
        => await (await sourceAsync).NullToFailedAsync(errorMessage);

    public static async Task<MlResult<T>> NullToFailedAsync<T>(this Task<T>             sourceAsync,
                                                                    IEnumerable<string> errorsMessage)
        => await (await sourceAsync).NullToFailedAsync(MlErrorsDetails.FromEnumerableStrings(errorsMessage));


    #endregion



    #region BoolToResult




    public static MlResult<T> BoolToResult<T>(this T               source,
                                                    bool            condition,
                                                    MlErrorsDetails errorsDetails)
        => condition ?  source.ToMlResultValid() : errorsDetails.ToMlResultFail<T>();

    public static MlResult<T> BoolToResult<T>(this T       source,
                                                   bool    condition,
                                                   MlError error)
        => source.BoolToResult(condition, MlErrorsDetails.FromError(error));


    public static MlResult<T> BoolToResult<T>(this T      source,
                                                   bool   condition,
                                                   string errorMessage)
        => source.BoolToResult(condition, MlError.FromErrorMessage(errorMessage));



    public static MlResult<T> BoolToResult<T>(this T                   source,
                                                   bool                condition,
                                                   IEnumerable<string> errorsMessage)
        => source.BoolToResult(condition, MlErrorsDetails.FromEnumerableStrings(errorsMessage));


    public static async Task<MlResult<T>> BoolToResultAsync<T>(this T       source,
                                                                    bool    condition,
                                                                    MlError error)
        => await source.BoolToResultAsync(condition, MlErrorsDetails.FromError(error));

    public static Task<MlResult<T>> BoolToResultAsync<T>(this T               source,
                                                                    bool            condition,
                                                                    MlErrorsDetails errorsDetails)
        => source.BoolToResult(condition, errorsDetails).ToAsync();


    public static async Task<MlResult<T>> BoolToResultAsync<T>(this T      source,
                                                                    bool   condition,
                                                                    string errorMessage)
        => await source.BoolToResultAsync(condition, MlError.FromErrorMessage(errorMessage));

    public static async Task<MlResult<T>> BoolToResultAsync<T>(this T                   source,
                                                                    bool                condition,
                                                                    IEnumerable<string> errorsMessage)
        => await source.BoolToResultAsync(condition, MlErrorsDetails.FromEnumerableStrings(errorsMessage));

    public static async Task<MlResult<T>> BoolToResultAsync<T>(this Task<T> sourceAsync,
                                                                    bool    condition,
                                                                    string  errorMessage)
        => await (await sourceAsync).BoolToResultAsync(condition, errorMessage);

    public static async Task<MlResult<T>> BoolToResultAsync<T>(this Task<T>             sourceAsync,
                                                                    bool                condition,
                                                                    IEnumerable<string> errorsMessage)
        => await (await sourceAsync).BoolToResultAsync(condition, MlErrorsDetails.FromEnumerableStrings(errorsMessage));










    public static MlResult<bool> BoolToResult(this bool            source,
                                                   MlErrorsDetails errorsDetails)
        => source ? source.ToMlResultValid() : errorsDetails.ToMlResultFail<bool>();

    public static MlResult<bool> BoolToResult(this bool    source,
                                                   MlError error)
        => source.BoolToResult(MlErrorsDetails.FromError(error));

    public static MlResult<bool> BoolToResult(this bool   source,
                                                   string errorMessage)
        => source.BoolToResult(MlError.FromErrorMessage(errorMessage));

    public static MlResult<bool> BoolToResult(this bool                source,
                                                   IEnumerable<string> errorsMessage)
        => source.BoolToResult(MlErrorsDetails.FromEnumerableStrings(errorsMessage));


    public static Task<MlResult<bool>> BoolToResultAsync(this bool            source,
                                                              MlErrorsDetails errorsDetails)
        => source.BoolToResult(errorsDetails).ToAsync();

    public static Task<MlResult<bool>> BoolToResultAsync(this bool    source,
                                                              MlError error)
        => source.BoolToResult(MlErrorsDetails.FromError(error)).ToAsync();

    public static Task<MlResult<bool>> BoolToResultAsync(this bool   source,
                                                              string errorMessage)
        => source.BoolToResult(MlError.FromErrorMessage(errorMessage)).ToAsync();

    public static Task<MlResult<bool>> BoolToResultAsync(this bool                source,
                                                              IEnumerable<string> errorsMessage)
        => source.BoolToResult(MlErrorsDetails.FromEnumerableStrings(errorsMessage)).ToAsync();


    public static async Task<MlResult<bool>> BoolToResultAsync(this Task<bool>      sourceAsync,
                                                                    MlErrorsDetails errorsDetails)
        => await (await sourceAsync).BoolToResultAsync(errorsDetails);

    public static async Task<MlResult<bool>> BoolToResultAsync(this Task<bool> sourceAsync,
                                                                    MlError    error)
        => await (await sourceAsync).BoolToResultAsync(MlErrorsDetails.FromError(error));

    public static async Task<MlResult<bool>> BoolToResultAsync(this Task<bool> sourceAsync,
                                                                    string     errorMessage)
        => await (await sourceAsync).BoolToResultAsync(MlError.FromErrorMessage(errorMessage));

    public static async Task<MlResult<bool>> BoolToResultAsync(this Task<bool>          sourceAsync,
                                                                    IEnumerable<string> errorsMessage)
        => await (await sourceAsync).BoolToResultAsync(MlErrorsDetails.FromEnumerableStrings(errorsMessage));



    #endregion




    /// ********************************************************



    #region Combine


    public static MlResult<(TResult1 value1, TResult2 value2)> Combine<TResult1, TResult2>(this MlResult<TResult1> source, 
                                                                                                TResult2           otherValue)
        => source.Match(
                            valid: x            => MlResult<(TResult1 value1, TResult2 value2)>.Valid((x, otherValue)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1 value1, TResult2 value2)>()
                       );

    public static Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAsync<TResult1, TResult2>(this MlResult<TResult1> source, 
                                                                                                           TResult2           otherValue)
        => source.Combine(otherValue).ToAsync();

    public static async Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAsync<TResult1, TResult2>(this Task<MlResult<TResult1>> sourceAsync, 
                                                                                                                 TResult2                 otherValue)
        => await (await sourceAsync).CombineAsync(otherValue);





    public static MlResult<(TResult1, TResult2, TResult3)> Combine<TResult1, TResult2, TResult3>(this MlResult<TResult1>                 source, 
                                                                                                      (TResult2 value1, TResult3 value2) values)
        => source.Match(
                            valid: x       => MlResult<(TResult1, TResult2, TResult3)>.Valid((x, values.value1, values.value2)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3)>> CombineAsync<TResult1, TResult2, TResult3>(this MlResult<TResult1>                 source, 
                                                                                                                 (TResult2 value1, TResult3 value2) values)
        => source.Combine(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3)>> CombineAsync<TResult1, TResult2, TResult3>(this Task<MlResult<TResult1>>           sourceAsync, 
                                                                                                                       (TResult2 value1, TResult3 value2) values)
        => await (await sourceAsync).CombineAsync(values);





    public static MlResult<(TResult1, TResult2, TResult3, TResult4)> Combine<TResult1, TResult2, TResult3, TResult4>(this MlResult<TResult1>                                  source, 
                                                                                                                          (TResult2 value1, TResult3 value2, TResult4 value3) values)
        => source.Match(
                            valid: x              => MlResult<(TResult1, TResult2, TResult3, TResult4)>.Valid((x, values.value1, values.value2, values.value3)),
                            fail   : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4)>> CombineAsync<TResult1, TResult2, TResult3, TResult4>(this MlResult<TResult1>                                  source,
                                                                                                                                     (TResult2 value1, TResult3 value2, TResult4 value3) values)
        => source.Combine(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4)>> CombineAsync<TResult1, TResult2, TResult3, TResult4>(this Task<MlResult<TResult1>>                            sourceAsync,
                                                                                                                                           (TResult2 value1, TResult3 value2, TResult4 value3) values)
        => await (await sourceAsync).CombineAsync(values);





    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5>(this MlResult<TResult1>                                                   source,
                                                                                                                                              (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>.Valid((x, values.value1, values.value2, values.value3, values.value4)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this MlResult<TResult1>                                                   source,
                                                                                                                                                         (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values)
        => source.Combine(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this Task<MlResult<TResult1>>                                             sourceAsync,
                                                                                                                                                               (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values)
        => await (await sourceAsync).CombineAsync(values);



    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this MlResult<TResult1> source,
                                                                                                                                                                  (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>.Valid((x, values.value1, values.value2, values.value3, values.value4, values.value5)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this MlResult<TResult1> source,
                                                                                                                                                                             (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values)
        => source.Combine(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this Task<MlResult<TResult1>> sourceAsync,
                                                                                                                                                                                   (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values)
        => await (await sourceAsync).CombineAsync(values);



    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this MlResult<TResult1> source,
                                                                                                                                                                  (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>.Valid((x, values.value1, values.value2, values.value3, values.value4, values.value5, values.value6)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this MlResult<TResult1> source,
                                                                                                                                                                                                 (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values)
        => source.Combine(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this Task<MlResult<TResult1>> sourceAsync,
                                                                                                                                                                                                       (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values)
        => await (await sourceAsync).CombineAsync(values);



    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this MlResult<TResult1> source,
                                                                                                                                                                  (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>.Valid((x, values.value1, values.value2, values.value3, values.value4, values.value5, values.value6, values.value7)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this MlResult<TResult1> source,
                                                                                                                                                                                                           (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values)
        => source.Combine(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this Task<MlResult<TResult1>> sourceAsync,
                                                                                                                                                                                                                           (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values)
        => await (await sourceAsync).CombineAsync(values);






    public static MlResult<(TResult1 value1, TResult2 value2)> Combine<TResult1, TResult2>(this TResult1           source, 
                                                                                                MlResult<TResult2> mlResultValue)
        => mlResultValue.Match(

                                valid: x            => MlResult<(TResult1 value1, TResult2 value2)>.Valid((source, x)),
                                fail : errorDetails => errorDetails.ToMlResultFail<(TResult1 value1, TResult2 value2)>()
                           );

    public static Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAsync<TResult1, TResult2>(this TResult1           source,
                                                                                                           MlResult<TResult2> mlResultValue)
        => source.Combine(mlResultValue).ToAsync();

    public static async Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAsync<TResult1, TResult2>(this TResult1                 source,
                                                                                                                 Task<MlResult<TResult2>> mlResultValueAsync)
        => await source.CombineAsync(await mlResultValueAsync);




    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)> Combine<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source, 
                                                                                                                           MlResult<TResult3>                 mlResultValue)
        => mlResultValue.Match(

                                valid: x            => MlResult<(TResult1, TResult2, TResult3)>.Valid((source.value1, source.value2, x)),
                                fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3)>()
                           );

    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)>?> CombineAsync<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source,
                                                                                                                                       MlResult<TResult3>                 mlResultValue)
        => source.Combine(mlResultValue).ToAsync()!;

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)>?> CombineAsync<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source,
                                                                                                                                             Task<MlResult<TResult3>>           mlResultValueAsync)
        => await source.CombineAsync(await mlResultValueAsync);


    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)> Combine<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source, 
                                                                                                                                                       MlResult<TResult4>                                         mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4)>.Valid((source.value1, source.value2, source.value3, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4)>()
                               );

    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)>?> CombineAsync<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source,
                                                                                                                                                                  MlResult<TResult4>                                  mlResultValue)
        => source.Combine(mlResultValue).ToAsync()!;

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)>?> CombineAsync<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source,
                                                                                                                                                                         Task<MlResult<TResult4>>                            mlResultValueAsync)
        => await source.CombineAsync(await mlResultValueAsync);



    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source, 
                                                                                                                                                                                 MlResult<TResult5>                                                   mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>.Valid((source.value1, source.value2, source.value3, source.value4, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5)>()
                               );


    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)>?> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source,
                                                                                                                                                                                             MlResult<TResult5>                                                   mlResultValue)
        => source.Combine(mlResultValue).ToAsync()!;

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)>?> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source,
                                                                                                                                                                                                   Task<MlResult<TResult5>>                                             mlResultValueAsync)
        => await source.CombineAsync(await mlResultValueAsync);




    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                                                            MlResult<TResult6> mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>.Valid((source.value1, source.value2, source.value3, source.value4, source.value5, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>()
                               );


    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)>?> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                                                                        MlResult<TResult6> mlResultValue)
        => source.Combine(mlResultValue).ToAsync()!;

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)>?> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                                                                                Task<MlResult<TResult6>> mlResultValueAsync)
        => await source.CombineAsync(await mlResultValueAsync);



    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                                                                        MlResult<TResult7> mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>.Valid((source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>()
                               );


    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)>?> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                                                                                    MlResult<TResult7> mlResultValue)
        => source.Combine(mlResultValue).ToAsync()!;

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)>?> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                                                                                          Task<MlResult<TResult7>> mlResultValueAsync)
        => await source.CombineAsync(await mlResultValueAsync);



    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                                                                                  MlResult<TResult8> mlResultValue)
        => mlResultValue.Match(
                                    valid: x => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>.Valid((source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, source.value7, x)),
                                    fail: errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>()
                               );


    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)>?> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                                                                                               MlResult<TResult8> mlResultValue)
        => source.Combine(mlResultValue).ToAsync()!;

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)>?> CombineAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                                                                                                     Task<MlResult<TResult8>> mlResultValueAsync)
        => await source.CombineAsync(await mlResultValueAsync);




    public static MlResult<(TResult1, TResult2)> Combine<TResult1, TResult2>(this TResult1 source,
                                                                                  TResult2 value)
        => (source, value).ToMlResultValid();


    public static MlResult<(TResult1, TResult2, TResult3)> Combine<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source,
                                                                                                      TResult3 newValue)
        => (source.value1, source.value2, newValue).ToMlResultValid();



    public static MlResult<(TResult1, TResult2, TResult3, TResult4)> Combine<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source,
                                                                                                                          TResult4 newValue)
        => (source.value1, source.value2, source.value3, newValue).ToMlResultValid();



    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source,
                                                                                                                                               TResult5 newValue)
        => (source.value1, source.value2, source.value3, source.value4, newValue).ToMlResultValid();

    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                  TResult6 newValue)
        => (source.value1, source.value2, source.value3, source.value4, source.value5, newValue).ToMlResultValid();

    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                      TResult7 newValue)
        => (source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, newValue).ToMlResultValid();

    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)> Combine<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                          TResult8 newValue)
        => (source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, source.value7, newValue).ToMlResultValid();



    #endregion











}
