// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.Types;
public static partial class MlResultSeveralExtensions
{


    #region FailIfEmpty



    public static MlResult<IEnumerable<T>>? FailIfEmpty<T>(this IEnumerable<T> items, 
                                                                  MlError        error)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid() 
                : error.ToMlResultFail<IEnumerable<T>>();

    public static MlResult<IEnumerable<T>>? FailIfEmpty<T>(this IEnumerable<T>  items, 
                                                                  MlErrorDetails errorsDetails)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid() 
                : errorsDetails.ToMlResultFail<IEnumerable<T>>();

    public static MlResult<IEnumerable<T>>? FailIfEmpty<T>(this IEnumerable<T> items, 
                                                                  string         messageError)
        => FailIfEmpty(items, MlError.FromErrorMessage(messageError));



    public static Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this IEnumerable<T> items, 
                                                                             MlError        error)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid().ToAsync() 
                : error.ToMlResultFail<IEnumerable<T>>().ToAsync();

    public static Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this IEnumerable<T>  items, 
                                                                             MlErrorDetails errorsDetails)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid().ToAsync() 
                : errorsDetails.ToMlResultFail<IEnumerable<T>>().ToAsync();

    public static Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this IEnumerable<T> items, 
                                                                             string         messageError)
        => FailIfEmpty(items, MlError.FromErrorMessage(messageError)).ToAsync()!;



    public async static Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this Task<IEnumerable<T>> itemsAsync, 
                                                                                   MlError              error)
    {
        var partialMlResult = await itemsAsync;

        var result = partialMlResult.FailIfEmpty(error);

        return result!;
    }

    public static async Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this Task<IEnumerable<T>> itemsAsync, 
                                                                                   MlErrorDetails errorsDetails)
    {
        var partialMlResult = await itemsAsync;

        var result = partialMlResult.FailIfEmpty(errorsDetails);

        return result!;
    }

    public static async Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this Task<IEnumerable<T>> itemsAsync, 
                                                                                   string               messageError)
    {
        var partialMlResult = await itemsAsync;

        var result = partialMlResult.FailIfEmpty(messageError);

        return result!;
    }









    #endregion FailIfEmpty



    #region FailIfNull


    public static MlResult<T> FailIfNull<T>(this T               source,
                                                   MlErrorDetails errorsDetails)
        => source == null ? errorsDetails.ToMlResultFail<T>() : source.ToMlResultValid();

    public static MlResult<T> FailIfNull<T>(this T       source,
                                                   MlError error)
        => source.FailIfNull(MlErrorDetails.FromError(error));


    public static MlResult<T> FailIfNull<T>(this T      source,
                                                   string errorMessage)
        => source.FailIfNull(MlError.FromErrorMessage(errorMessage));



    public static MlResult<T> FailIfNull<T>(this T                   source,
                                                   IEnumerable<string> errorsMessage)
        => source.FailIfNull(MlErrorDetails.FromEnumerableStrings(errorsMessage));


    public static async Task<MlResult<T>> FailIfNullAsync<T>(this T       source,
                                                                    MlError error)
        => await source.FailIfNullAsync(MlErrorDetails.FromError(error));

    public static async Task<MlResult<T>> FailIfNullAsync<T>(this T               source,
                                                                    MlErrorDetails errorsDetails)
        => await source.FailIfNull(errorsDetails).ToAsync();


    public static async Task<MlResult<T>> FailIfNullAsync<T>(this T      source,
                                                                    string errorMessage)
        => await source.FailIfNull(errorMessage).ToAsync();

    public static async Task<MlResult<T>> FailIfNullAsync<T>(this T                   source,
                                                                    IEnumerable<string> errorsMessage)
        => await source.FailIfNullAsync(MlErrorDetails.FromEnumerableStrings(errorsMessage));

    public static async Task<MlResult<T>> FailIfNullAsync<T>(this Task<T> sourceAsync,
                                                                    string  errorMessage)
        => await (await sourceAsync).FailIfNullAsync(errorMessage);

    public static async Task<MlResult<T>> FailIfNullAsync<T>(this Task<T>       sourceAsync,
                                                                    MlError       error)
        => await (await sourceAsync).FailIfNullAsync(error);

    public static async Task<MlResult<T>> FailIfNullAsync<T>(this Task<T>             sourceAsync,
                                                                    MlErrorDetails    errorsDetails)
        => await (await sourceAsync).FailIfNullAsync(errorsDetails);

    public static async Task<MlResult<T>> FailIfNullAsync<T>(this Task<T>             sourceAsync,
                                                                    IEnumerable<string> errorsMessage)
        => await (await sourceAsync).FailIfNullAsync(MlErrorDetails.FromEnumerableStrings(errorsMessage));


    #endregion



    #region FailIfFalse




    public static MlResult<T> FailIfFalse<T>(this T               source,
                                                    bool            condition,
                                                    MlErrorDetails errorsDetails)
        => condition ?  source.ToMlResultValid() : errorsDetails.ToMlResultFail<T>();

    public static MlResult<T> FailIfFalse<T>(this T       source,
                                                   bool    condition,
                                                   MlError error)
        => source.FailIfFalse(condition, MlErrorDetails.FromError(error));


    public static MlResult<T> FailIfFalse<T>(this T      source,
                                                   bool   condition,
                                                   string errorMessage)
        => source.FailIfFalse(condition, MlError.FromErrorMessage(errorMessage));



    public static MlResult<T> FailIfFalse<T>(this T                   source,
                                                   bool                condition,
                                                   IEnumerable<string> errorsMessage)
        => source.FailIfFalse(condition, MlErrorDetails.FromEnumerableStrings(errorsMessage));


    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this T       source,
                                                                    bool    condition,
                                                                    MlError error)
        => await source.FailIfFalseAsync(condition, MlErrorDetails.FromError(error));

    public static Task<MlResult<T>> FailIfFalseAsync<T>(this T               source,
                                                                    bool            condition,
                                                                    MlErrorDetails errorsDetails)
        => source.FailIfFalse(condition, errorsDetails).ToAsync();


    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this T      source,
                                                                    bool   condition,
                                                                    string errorMessage)
        => await source.FailIfFalseAsync(condition, MlError.FromErrorMessage(errorMessage));

    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this T                   source,
                                                                    bool                condition,
                                                                    IEnumerable<string> errorsMessage)
        => await source.FailIfFalseAsync(condition, MlErrorDetails.FromEnumerableStrings(errorsMessage));

    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this Task<T> sourceAsync,
                                                                    bool    condition,
                                                                    string  errorMessage)
        => await (await sourceAsync).FailIfFalseAsync(condition, errorMessage);

    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this Task<T>       sourceAsync,
                                                                    bool          condition,
                                                                    MlError       error)
        => await (await sourceAsync).FailIfFalseAsync(condition, error);

    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this Task<T>             sourceAsync,
                                                                    bool                condition,
                                                                    MlErrorDetails    errorsDetails)
        => await (await sourceAsync).FailIfFalseAsync(condition, errorsDetails);

    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this Task<T>             sourceAsync,
                                                                    bool                condition,
                                                                    IEnumerable<string> errorsMessage)
        => await (await sourceAsync).FailIfFalseAsync(condition, MlErrorDetails.FromEnumerableStrings(errorsMessage));










    public static MlResult<bool> FailIfFalse(this bool            source,
                                                   MlErrorDetails errorsDetails)
        => source ? source.ToMlResultValid() : errorsDetails.ToMlResultFail<bool>();

    public static MlResult<bool> FailIfFalse(this bool    source,
                                                   MlError error)
        => source.FailIfFalse(MlErrorDetails.FromError(error));

    public static MlResult<bool> FailIfFalse(this bool   source,
                                                   string errorMessage)
        => source.FailIfFalse(MlError.FromErrorMessage(errorMessage));

    public static MlResult<bool> FailIfFalse(this bool                source,
                                                   IEnumerable<string> errorsMessage)
        => source.FailIfFalse(MlErrorDetails.FromEnumerableStrings(errorsMessage));


    public static Task<MlResult<bool>> FailIfFalseAsync(this bool            source,
                                                              MlErrorDetails errorsDetails)
        => source.FailIfFalse(errorsDetails).ToAsync();

    public static Task<MlResult<bool>> FailIfFalseAsync(this bool    source,
                                                              MlError error)
        => source.FailIfFalse(MlErrorDetails.FromError(error)).ToAsync();

    public static Task<MlResult<bool>> FailIfFalseAsync(this bool   source,
                                                              string errorMessage)
        => source.FailIfFalse(MlError.FromErrorMessage(errorMessage)).ToAsync();

    public static Task<MlResult<bool>> FailIfFalseAsync(this bool                source,
                                                              IEnumerable<string> errorsMessage)
        => source.FailIfFalse(MlErrorDetails.FromEnumerableStrings(errorsMessage)).ToAsync();


    public static async Task<MlResult<bool>> FailIfFalseAsync(this Task<bool>      sourceAsync,
                                                                    MlErrorDetails errorsDetails)
        => await (await sourceAsync).FailIfFalseAsync(errorsDetails);

    public static async Task<MlResult<bool>> FailIfFalseAsync(this Task<bool> sourceAsync,
                                                                    MlError    error)
        => await (await sourceAsync).FailIfFalseAsync(MlErrorDetails.FromError(error));

    public static async Task<MlResult<bool>> FailIfFalseAsync(this Task<bool> sourceAsync,
                                                                    string     errorMessage)
        => await (await sourceAsync).FailIfFalseAsync(MlError.FromErrorMessage(errorMessage));

    public static async Task<MlResult<bool>> FailIfFalseAsync(this Task<bool>          sourceAsync,
                                                                    IEnumerable<string> errorsMessage)
        => await (await sourceAsync).FailIfFalseAsync(MlErrorDetails.FromEnumerableStrings(errorsMessage));



    #endregion




    /// ********************************************************



    #region Combine


    public static MlResult<(TResult1 value1, TResult2 value2)> CombineAll<TResult1, TResult2>(this MlResult<TResult1> source, 
                                                                                                TResult2           otherValue)
        => source.Match(
                            valid: x            => MlResult<(TResult1 value1, TResult2 value2)>.Valid((x, otherValue)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1 value1, TResult2 value2)>()
                       );

    public static Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAllAsync<TResult1, TResult2>(this MlResult<TResult1> source, 
                                                                                                           TResult2           otherValue)
        => source.CombineAll(otherValue).ToAsync();

    public static async Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAllAsync<TResult1, TResult2>(this Task<MlResult<TResult1>> sourceAsync, 
                                                                                                                 TResult2                 otherValue)
        => await (await sourceAsync).CombineAllAsync(otherValue);





    public static MlResult<(TResult1, TResult2, TResult3)> CombineAll<TResult1, TResult2, TResult3>(this MlResult<TResult1>                 source, 
                                                                                                      (TResult2 value1, TResult3 value2) values)
        => source.Match(
                            valid: x       => MlResult<(TResult1, TResult2, TResult3)>.Valid((x, values.value1, values.value2)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3)>> CombineAllAsync<TResult1, TResult2, TResult3>(this MlResult<TResult1>                 source, 
                                                                                                                 (TResult2 value1, TResult3 value2) values)
        => source.CombineAll(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3)>> CombineAllAsync<TResult1, TResult2, TResult3>(this Task<MlResult<TResult1>>           sourceAsync, 
                                                                                                                       (TResult2 value1, TResult3 value2) values)
        => await (await sourceAsync).CombineAllAsync(values);





    public static MlResult<(TResult1, TResult2, TResult3, TResult4)> CombineAll<TResult1, TResult2, TResult3, TResult4>(this MlResult<TResult1>                                  source, 
                                                                                                                          (TResult2 value1, TResult3 value2, TResult4 value3) values)
        => source.Match(
                            valid: x              => MlResult<(TResult1, TResult2, TResult3, TResult4)>.Valid((x, values.value1, values.value2, values.value3)),
                            fail   : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(this MlResult<TResult1>                                  source,
                                                                                                                                     (TResult2 value1, TResult3 value2, TResult4 value3) values)
        => source.CombineAll(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(this Task<MlResult<TResult1>>                            sourceAsync,
                                                                                                                                           (TResult2 value1, TResult3 value2, TResult4 value3) values)
        => await (await sourceAsync).CombineAllAsync(values);



    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5>(this MlResult<TResult1>                                                   source,
                                                                                                                                              (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>.Valid((x, values.value1, values.value2, values.value3, values.value4)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this MlResult<TResult1>                                                   source,
                                                                                                                                                         (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values)
        => source.CombineAll(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this Task<MlResult<TResult1>>                                             sourceAsync,
                                                                                                                                                               (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values)
        => await (await sourceAsync).CombineAllAsync(values);



    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this MlResult<TResult1> source,
                                                                                                                                                                  (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>.Valid((x, values.value1, values.value2, values.value3, values.value4, values.value5)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this MlResult<TResult1> source,
                                                                                                                                                                             (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values)
        => source.CombineAll(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this Task<MlResult<TResult1>> sourceAsync,
                                                                                                                                                                                   (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values)
        => await (await sourceAsync).CombineAllAsync(values);



    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this MlResult<TResult1> source,
                                                                                                                                                                  (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>.Valid((x, values.value1, values.value2, values.value3, values.value4, values.value5, values.value6)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this MlResult<TResult1> source,
                                                                                                                                                                                                 (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values)
        => source.CombineAll(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this Task<MlResult<TResult1>> sourceAsync,
                                                                                                                                                                                                       (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values)
        => await (await sourceAsync).CombineAllAsync(values);



    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this MlResult<TResult1> source,
                                                                                                                                                                  (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>.Valid((x, values.value1, values.value2, values.value3, values.value4, values.value5, values.value6, values.value7)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>()
                       );

    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this MlResult<TResult1> source,
                                                                                                                                                                                                           (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values)
        => source.CombineAll(values).ToAsync();

    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this Task<MlResult<TResult1>> sourceAsync,
                                                                                                                                                                                                                           (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values)
        => await (await sourceAsync).CombineAllAsync(values);






    public static MlResult<(TResult1 value1, TResult2 value2)> CombineAll<TResult1, TResult2>(this TResult1           source, 
                                                                                                MlResult<TResult2> mlResultValue)
        => mlResultValue.Match(

                                valid: x            => MlResult<(TResult1 value1, TResult2 value2)>.Valid((source, x)),
                                fail : errorDetails => errorDetails.ToMlResultFail<(TResult1 value1, TResult2 value2)>()
                           );

    public static Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAllAsync<TResult1, TResult2>(this TResult1           source,
                                                                                                           MlResult<TResult2> mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    public static async Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAllAsync<TResult1, TResult2>(this TResult1                 source,
                                                                                                                 Task<MlResult<TResult2>> mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);




    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)> CombineAll<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source, 
                                                                                                                           MlResult<TResult3>                 mlResultValue)
        => mlResultValue.Match(

                                valid: x            => MlResult<(TResult1, TResult2, TResult3)>.Valid((source.value1, source.value2, x)),
                                fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3)>()
                           );

    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)>> CombineAllAsync<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source,
                                                                                                                                      MlResult<TResult3>                 mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)>> CombineAllAsync<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source,
                                                                                                                                            Task<MlResult<TResult3>>           mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);




    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)> CombineAll<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source, 
                                                                                                                                                       MlResult<TResult4>                                         mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4)>.Valid((source.value1, source.value2, source.value3, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4)>()
                               );

    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source,
                                                                                                                                                                  MlResult<TResult4>                                  mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source,
                                                                                                                                                                         Task<MlResult<TResult4>>                            mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);



    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source, 
                                                                                                                                                                                 MlResult<TResult5>                                                   mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>.Valid((source.value1, source.value2, source.value3, source.value4, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5)>()
                               );


    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source,
                                                                                                                                                                                             MlResult<TResult5>                                                   mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source,
                                                                                                                                                                                                   Task<MlResult<TResult5>>                                             mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);




    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                                                            MlResult<TResult6> mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>.Valid((source.value1, source.value2, source.value3, source.value4, source.value5, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>()
                               );


    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                                                                        MlResult<TResult6> mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                                                                                Task<MlResult<TResult6>> mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);



    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                                                                        MlResult<TResult7> mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>.Valid((source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>()
                               );


    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                                                                                    MlResult<TResult7> mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                                                                                          Task<MlResult<TResult7>> mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);



    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                                                                                  MlResult<TResult8> mlResultValue)
        => mlResultValue.Match(
                                    valid: x => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>.Valid((source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, source.value7, x)),
                                    fail: errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>()
                               );


    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                                                                                               MlResult<TResult8> mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                                                                                                     Task<MlResult<TResult8>> mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);




    public static MlResult<(TResult1, TResult2)> CombineAll<TResult1, TResult2>(this TResult1 source,
                                                                                  TResult2 value)
        => (source, value).ToMlResultValid();


    public static MlResult<(TResult1, TResult2, TResult3)> CombineAll<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source,
                                                                                                      TResult3 newValue)
        => (source.value1, source.value2, newValue).ToMlResultValid();



    public static MlResult<(TResult1, TResult2, TResult3, TResult4)> CombineAll<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source,
                                                                                                                          TResult4 newValue)
        => (source.value1, source.value2, source.value3, newValue).ToMlResultValid();



    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source,
                                                                                                                                               TResult5 newValue)
        => (source.value1, source.value2, source.value3, source.value4, newValue).ToMlResultValid();

    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                  TResult6 newValue)
        => (source.value1, source.value2, source.value3, source.value4, source.value5, newValue).ToMlResultValid();

    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                      TResult7 newValue)
        => (source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, newValue).ToMlResultValid();

    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                          TResult8 newValue)
        => (source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, source.value7, newValue).ToMlResultValid();



    #endregion




    #region Do


    public static MlResult<TResult> Pipe<T, TResult>(this MlResult<T> source, Func<MlResult<T>, MlResult<TResult>> action)
        => action(source);

    public static async Task<MlResult<TResult>> PipeAsync<T, TResult>(this MlResult<T>                                source, 
                                                                         Func<MlResult<T>, Task<MlResult<TResult>>> actionAsync)
        => await actionAsync(source);

    public static async Task<MlResult<TResult>> PipeAsync<T, TResult>(this Task<MlResult<T>>                                sourceAsync, 
                                                                         Func<Task<MlResult<T>>, Task<MlResult<TResult>>> actionAsync)
        => await actionAsync(sourceAsync);


    #endregion






}

