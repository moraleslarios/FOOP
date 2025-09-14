namespace MoralesLarios.OOFP.Types;

public static class MlResultActions
{

    public static MlResult<T> AddMlErrorDetailIfFail<T>(this MlResult<T> source, 
                                                             string      errorKey, 
                                                             object      errorValue)
        => source.Match
                    (
                        fail : errorsDetails => errorsDetails.AddDetail(errorKey, errorValue),
                        valid: _ => source
                    );


    public static MlResult<T> AddValueDetailIfFail<T>(this MlResult<T> source, 
                                                           object      errorValue)
        => source.AddMlErrorDetailIfFail(VALUE_KEY, errorValue);

    public static Task<MlResult<T>> AddMlErrorDetailIfFailAsync<T>(this MlResult<T> source, 
                                                                              string      errorKey, 
                                                                              object      errorValue)
        => source.AddMlErrorDetailIfFail(errorKey, errorValue).ToAsync();


    public static async Task<MlResult<T>> AddMlErrorDetailIfFailAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                              string            errorKey, 
                                                                              object            errorValue)
        => await (await sourceAsync).AddMlErrorDetailIfFailAsync(errorKey, errorValue);

    public static Task<MlResult<T>> AddValueDetailIfFailAsync<T>(this MlResult<T> source,
                                                                            object      errorValue)
        => source.AddValueDetailIfFail(errorValue).ToAsync();


    public static async Task<MlResult<T>> AddValueDetailIfFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                            object            errorValue)
        => await (await sourceAsync).AddValueDetailIfFailAsync(errorValue);










    #region CompleteWithValue

    /// <summary>
    /// Returns a new MlResult with the value of the source and the value passed as a parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="completeFunc"></param>
    /// <returns></returns>
    public static MlResult<TReturn> CompleteWithDataValueIfValid<T, TReturn>(this MlResult<T>      source,
                                                                                  Func<T, TReturn> completeFunc)
        => source.Match(
                            fail : errorDetails => errorDetails.ToMlResultFail<TReturn>(),
                            valid: x            => MlResult<TReturn>.Valid(completeFunc(x))
                        );



    public static async Task<MlResult<TReturn>> CompleteWithDataValueIfValidAsync<T, TReturn>(this MlResult<T>            source,
                                                                                                   Func<T, Task<TReturn>> completeFuncAsync)
        => await source.MatchAsync(
                                        failAsync :       errorDetails => errorDetails.ToMlResultFailAsync<TReturn>(),
                                        validAsync: async x            => MlResult<TReturn>.Valid(await completeFuncAsync(x))
                                    );


    public static async Task<MlResult<TReturn>> CompleteWithDataValueIfValidAsync<T, TReturn>(this Task<MlResult<T>>      sourceAsync,
                                                                                                   Func<T, Task<TReturn>> completeFuncAsync)
        => await (await sourceAsync).CompleteWithDataValueIfValidAsync(completeFuncAsync);





    /// <summary>
    /// Populate the ErrorDetails information with a Key value entry, if the MlResult result is fail
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns>Fail MlResult with value ErrorDetailData</returns>
    public static MlResult<T> CompleteWithDetailsValueIfFail<T, TValue>(this MlResult<T> source, 
                                                                             TValue      value)
        => source.Match (
                            fail : errorDetails => errorDetails.AddDetailValue(value),
                            valid: x            => source
                        );

    public static Task<MlResult<T>> CompleteWithDetailsValueIfFailAsync<T, TValue>(this MlResult<T> source, 
                                                                                        TValue      value)
        => source.CompleteWithDetailsValueIfFail(value).ToAsync();

    public async static Task<MlResult<T>> CompleteWithDetailsValueIfFailAsync<T, TValue>(this Task<MlResult<T>> sourceAsync, 
                                                                                              TValue            value)
        => (await sourceAsync).CompleteWithDetailsValueIfFail(value);




    /// <summary>
    /// If the source is valid, it returns a new MlResult with the value of the source and the value passed as a parameter.
    /// If the source is fail, populate the ErrorDetails information with a Key value entry, if the MlResult result is fail
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="completeFunc"></param>
    /// <returns></returns>
    public static MlResult<TReturn> CompleteWithDataValue<T, TValue, TReturn>(this MlResult<T>      source, 
                                                                                   TValue           value,
                                                                                   Func<T, TReturn> completeFunc)
        => source.Match(
                            fail : errorDetails => errorDetails.AddDetailValue(value).ToMlResultFail<TReturn>(),
                            valid: x            => MlResult<TReturn>.Valid(completeFunc(x))
                        );


    public static async Task<MlResult<TReturn>> CompleteWithDataValueAsync<T, TValue, TReturn>(this MlResult<T>            source, 
                                                                                                    TValue                 value,
                                                                                                    Func<T, Task<TReturn>> completeFuncAsync)
        => await source.MatchAsync(
                                        failAsync :       errorDetails => errorDetails.AddDetailValue(value).ToMlResultFailAsync<TReturn>(),
                                        validAsync: async x            => MlResult<TReturn>.Valid(await completeFuncAsync(x))
                                    );


    public static async Task<MlResult<TReturn>> CompleteWithDataValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>      sourceAsync, 
                                                                                                    TValue                 value,
                                                                                                    Func<T, Task<TReturn>> completeFuncAsync)
        => await (await sourceAsync).CompleteWithDataValueAsync(value, completeFuncAsync);


    #endregion CompleteWithValue




    #region SecureValidValue



    public static T SecureValidValue<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the secure value from MlResult in Fail state")
        => source.Match(valid: x => x,
                        fail : _ => throw new InvalidProgramException(exceptionMessage));

    public static async Task<T> SecureValidValueAsync<T>(this Task<MlResult<T>> sourceAsync, string exceptionMessage = "Cannot obtain the secure value from MlResult in Fail state")
        => await sourceAsync.MatchAsync(valid     : x => x,
                                        failAsync : _ => throw new InvalidProgramException(exceptionMessage));

    public static MlErrorsDetails SecureFailErrorsDetails<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the MlErrorsDetails from MlResult in Valid state")
        => source.Match(valid: _ => throw new InvalidProgramException(exceptionMessage),
                        fail : errors => errors);

    public static Task<MlErrorsDetails> SecureFailErrorsDetailsAsync<T>(this MlResult<T> source, string exceptionMessage = "Cannot obtain the MlErrorsDetails from MlResult in Valid state")
        => source.SecureFailErrorsDetails(exceptionMessage).ToAsync();

    public static async Task<MlErrorsDetails> SecureFailErrorsDetailsAsync<T>(this Task<MlResult<T>> sourceAsync, string exceptionMessage = "Cannot obtain the MlErrorsDetails from MlResult in Valid state")
        => await sourceAsync.SecureFailErrorsDetailsAsync(exceptionMessage);


    #endregion





    #region CreateCompleteMlResult



    /// **************************** MLResult 2 Generics

    public static MlResult<(T1, T2)> CreateCompleteMlResult<T1, T2>(this MlResult<T1> source1,
                                                                         MlResult<T2> source2)
        => source1.Match(
                            fail : errorDetails1 => errorDetails1.MergeErrorsDetails<T2, (T1, T2)>(source2),
                            valid: x1 => source2.Match(
                                                            fail : errorDetails2 => errorDetails2.MergeErrorsDetails<T1, (T1, T2)>(source1),
                                                            valid: x2            => MlResult<(T1, T2)>.Valid((x1, x2))
                                                      )
                        );


    public static Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this MlResult<T1> source1,
                                                                                    MlResult<T2> source2)
        => source1.CreateCompleteMlResult(source2).ToAsync();

    public static async Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this Task<MlResult<T1>> source1Async,
                                                                                          MlResult<T2>       source2)
    {
        var source1 = await source1Async;

        var result = await source1Async.MatchAsync(
                                                        failAsync : errorDetails1 => errorDetails1.MergeErrorsDetailsAsync<T2, (T1, T2)>(source2),
                                                        validAsync: x1            => source2.Match(
                                                                                                       fail : errorDetails2 => errorDetails2.MergeErrorsDetails<T1, (T1, T2)>(source1),
                                                                                                       valid: x2            => MlResult<(T1, T2)>.Valid((x1, x2))
                                                                                                   ).ToAsync()
                                                    );

        return result;
    }


    public static async Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this MlResult<T1>       source1,
                                                                                          Task<MlResult<T2>> source2Async)
    {
        var source2 = await source2Async;

        var result = source1.Match(
                                    fail : errorDetails1 => errorDetails1.MergeErrorsDetailsAsync<T2, (T1, T2)>(source2),
                                    valid: x1            => source2Async.MatchAsync(
                                                                                       failAsync : errorDetails2 => errorDetails2.MergeErrorsDetailsAsync<T1, (T1, T2)>(source1),
                                                                                       validAsync: x2 => MlResult<(T1, T2)>.ValidAsync((x1, x2))
                                                                                   )
                                );

        return await result;
    }                   

    public static Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this Task<MlResult<T1>> source1Async,
                                                                                    Task<MlResult<T2>> source2Async)
        => source1Async.MatchAsync(
                                    failAsync : errorDetails1 => errorDetails1.MergeErrorsDetailsAsync<T2, (T1, T2)>(source2Async),
                                    validAsync: x1            => source2Async.MatchAsync(
                                                                                            failAsync : errorDetails2 => errorDetails2.MergeErrorsDetailsAsync<T1, (T1, T2)>(source1Async),
                                                                                            validAsync: x2            => MlResult<(T1, T2)>.ValidAsync((x1, x2))
                                                                                        )
                                );





    /// **************************** MLResult 3 Generics



    public static MlResult<(T1, T2, T3)> CreateCompleteMlResult<T1, T2, T3>(this MlResult<T1> source1,
                                                                                 MlResult<T2> source2,
                                                                                 MlResult<T3> source3)
        => source1.Match(
                            fail : errorDetails1 => errorDetails1.ToMlResultFail<(T1, T2, T3)>(),
                            valid: x1            => source2.Match(
                                                                        fail : errorDetails2 => errorDetails2.ToMlResultFail<(T1, T2, T3)>(),
                                                                        valid: x2            => source3.Match(
                                                                                                                   fail : errorDetails3 => errorDetails3.ToMlResultFail<(T1, T2, T3)>(),
                                                                                                                   valid: x3            => MlResult<(T1, T2, T3)>.Valid((x1, x2, x3))
                                                                                                               )
                                                                  )
                        );













    /// ******************************* Object 2 Generics


    public static MlResult<(T1, T2)> CreateCompleteMlResult<T1, T2>(this T1           source1,
                                                                         MlResult<T2> source2)
        => source2.Match(
                            fail : errorDetails2 => errorDetails2.ToMlResultFail<(T1, T2)>(),
                            valid: x2            => MlResult<(T1, T2)>.Valid((source1, x2))
                        );

    public static Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this T1           source1,
                                                                                    MlResult<T2> source2)
        => source2.Match(
                            fail : errorDetails2 => errorDetails2.ToMlResultFail<(T1, T2)>(),
                            valid: x2            => MlResult<(T1, T2)>.Valid((source1, x2))
                        ).ToAsync();

    public static async Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this Task<T1>     source1Async,
                                                                                          MlResult<T2> source2)
    {
        var source1 = await source1Async;

        var result = source2.Match(
                                        fail : errorDetails2 => errorDetails2.ToMlResultFail<(T1, T2)>(),
                                        valid: x2            => MlResult<(T1, T2)>.Valid((source1, x2))
                                  );

        return result;
    }
                                 

    public static Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this T1                 source1,
                                                                                    Task<MlResult<T2>> source2Async)
        => source2Async.MatchAsync(
                                      failAsync : errorDetails2 => errorDetails2.ToMlResultFailAsync<(T1, T2)>(),
                                      validAsync: x2            => MlResult<(T1, T2)>.ValidAsync((source1, x2))
                                  );

    public static async Task<MlResult<(T1, T2)>> CreateCompleteMlResultAsync<T1, T2>(this Task<T1>           source1Async,
                                                                                          Task<MlResult<T2>> source2Async)
    {
        var source1 = await source1Async;

        var result = await source1.CreateCompleteMlResultAsync(source2Async);

        return result;
    }







    #endregion




}
