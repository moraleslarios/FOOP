namespace MoralesLarios.OOFP.Types;
public static class MlResultChangeReturnResult
{


    #region ChangeReturnResult


    public static MlResult<TReturn> ChangeReturnResult<T, TReturn>(this MlResult<T>     source,
                                                                        TReturn         validValue,
                                                                        MlErrorsDetails failValue)
        => source.Match(
                            valid: x      => validValue.ToMlResultValid(),
                            fail : errors => failValue.ToMlResultFail<TReturn>()
                        );

    public static MlResult<TReturn> ChangeReturnResult<T, TReturn>(this MlResult<T> source,
                                                                        TReturn     validValue,
                                                                        MlError     failValue)
        => ChangeReturnResult(source, validValue, failValue.ToMlErrorsDetails());

    public static MlResult<TReturn> ChangeReturnResult<T, TReturn>(this MlResult<T>         source,
                                                                        TReturn             validValue,
                                                                        IEnumerable<string> failValue)
        => ChangeReturnResult(source, validValue, failValue.ToMlErrorsDetails());

    public static MlResult<TReturn> ChangeReturnResult<T, TReturn>(this MlResult<T> source,
                                                                        TReturn     validValue,
                                                                        string      failValue)
        => ChangeReturnResult(source, validValue, failValue.ToMlErrorsDetails());


    public static async Task<MlResult<TReturn>> ChangeReturnResultAsync<T, TReturn>(this MlResult<T>     source,
                                                                                         TReturn         validValue,
                                                                                         MlErrorsDetails failValue)
        => await source.MatchAsync(
                                        validAsync: x      => validValue.ToMlResultValidAsync(),
                                        failAsync : errors => failValue.ToMlResultFailAsync<TReturn>()
                                    );

    public static async Task<MlResult<TReturn>> ChangeReturnResultAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                                         TReturn           validValue,
                                                                                         MlErrorsDetails   failValue)
        => (await sourceAsync).Match(
                                        valid: x      => validValue.ToMlResultValid(),
                                        fail : errors => failValue.ToMlResultFail<TReturn>()
                                    );




    #endregion ChangeReturnResult


    #region ChangeReturnResultAlwaisValid



    public static MlResult<TReturn> ChangeReturnResultAlwaisValid<T, TReturn>(this MlResult<T> source,
                                                                                   TReturn     validValue,
                                                                                   TReturn     failValidValue)
        => source.Match(
                            valid: x      => validValue    .ToMlResultValid(),
                            fail : errors => failValidValue.ToMlResultValid()
                        );

    public static async Task<MlResult<TReturn>> ChangeReturnResultAlwaisValidAsync<T, TReturn>(this MlResult<T> source,
                                                                                                    TReturn     validValue,
                                                                                                    TReturn     failValidValue)
        => await source.MatchAsync(
                                    validAsync: x      => validValue    .ToMlResultValidAsync(),
                                    failAsync : errors => failValidValue.ToMlResultValidAsync()
                                );

    public static async Task<MlResult<TReturn>> ChangeReturnResultAlwaisValidAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                                                    TReturn     validValue,
                                                                                                    TReturn     failValidValue)
        => (await sourceAsync).Match(
                                        valid: x      => validValue    .ToMlResultValid(),
                                        fail : errors => failValidValue.ToMlResultValid()
                                    );




    #endregion ChangeReturnResultAlwaisValid


    #region ChangeReturnResultAlwaisFail



    public static MlResult<TReturn> ChangeReturnResultAlwaisFail<T, TReturn>(this MlResult<T>     source,
                                                                                  MlErrorsDetails validFailValue,
                                                                                  MlErrorsDetails failValue)
        => source.Match(
                            valid: x      => validFailValue.ToMlResultFail<TReturn>(),
                            fail : errors => failValue     .ToMlResultFail<TReturn>()
                        );

    public static MlResult<TReturn> ChangeReturnResultAlwaisFail<T, TReturn>(this MlResult<T> source,
                                                                                  MlError     validFailValue,
                                                                                  MlError     failValue)
        => ChangeReturnResultAlwaisFail<T, TReturn>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    public static MlResult<TReturn> ChangeReturnResultAlwaisFail<T, TReturn>(this MlResult<T>         source,
                                                                                  IEnumerable<string> validFailValue,
                                                                                  IEnumerable<string> failValue)
        => ChangeReturnResultAlwaisFail<T, TReturn>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    public static MlResult<TReturn> ChangeReturnResultAlwaisFail<T, TReturn>(this MlResult<T> source,
                                                                                  string      validFailValue,
                                                                                  string      failValue)
        => ChangeReturnResultAlwaisFail<T, TReturn>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());




    public static async Task<MlResult<TReturn>> ChangeReturnResultAlwaisFailAsync<T, TReturn>(this MlResult<T>     source,
                                                                                                   MlErrorsDetails validFailValue,
                                                                                                   MlErrorsDetails failValue)
        => await source.MatchAsync(
                                        validAsync: x      => validFailValue.ToMlResultFailAsync<TReturn>(),
                                        failAsync : errors => failValue     .ToMlResultFailAsync<TReturn>()
                                  );

    public static async Task<MlResult<TReturn>> ChangeReturnResultAlwaisFailAsync<T, TReturn>(this MlResult<T>     source,
                                                                                                   MlError     validFailValue,
                                                                                                   MlError     failValue)
        => await ChangeReturnResultAlwaisFailAsync<T, TReturn>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    public static async Task<MlResult<TReturn>> ChangeReturnResultAlwaisFailAsync<T, TReturn>(this MlResult<T>        source,
                                                                                                  IEnumerable<string> validFailValue,
                                                                                                  IEnumerable<string> failValue)
        => await ChangeReturnResultAlwaisFailAsync<T, TReturn>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    public static async Task<MlResult<TReturn>> ChangeReturnResultAlwaisFailAsync<T, TReturn>(this MlResult<T> source,
                                                                                                   string      validFailValue,
                                                                                                   string      failValue)
        => await ChangeReturnResultAlwaisFailAsync<T, TReturn>(source, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());



    public static async Task<MlResult<TReturn>> ChangeReturnResultAlwaisFailAsync<T, TReturn>(this Task<MlResult<T>>     sourceAsync,
                                                                                  MlErrorsDetails validFailValue,
                                                                                  MlErrorsDetails failValue)
        => (await sourceAsync).Match(
                                        valid: x      => validFailValue.ToMlResultFail<TReturn>(),
                                        fail : errors => failValue     .ToMlResultFail<TReturn>()
                                    );

    public static async Task<MlResult<TReturn>> ChangeReturnResultAlwaisFailAsync<T, TReturn>(this Task<MlResult<T>>     sourceAsync,
                                                                                  MlError     validFailValue,
                                                                                  MlError     failValue)
        => await ChangeReturnResultAlwaisFailAsync<T, TReturn>(sourceAsync, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    public static async Task<MlResult<TReturn>> ChangeReturnResultAlwaisFailAsync<T, TReturn>(this Task<MlResult<T>>     sourceAsync,
                                                                                  IEnumerable<string> validFailValue,
                                                                                  IEnumerable<string> failValue)
        => await ChangeReturnResultAlwaisFailAsync<T, TReturn>(sourceAsync, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());

    public static async Task<MlResult<TReturn>> ChangeReturnResultAlwaisFailAsync<T, TReturn>(this Task<MlResult<T>>     sourceAsync,
                                                                                  string      validFailValue,
                                                                                  string      failValue)
        => await ChangeReturnResultAlwaisFailAsync<T, TReturn>(sourceAsync, validFailValue.ToMlErrorsDetails(), failValue.ToMlErrorsDetails());




    #endregion ChangeReturnResultAlwaisFail


    #region ChangeReturnResultIfValid



    public static MlResult<T> ChangeReturnResultIfValid<T>(this MlResult<T> source,
                                                                T           returnValidValue)
        => source.Match(
                            valid: x      => returnValidValue.ToMlResultValid(),
                            fail : errors => errors.ToMlResultFail<T>()
                        );
    

    public static async Task<MlResult<T>> ChangeReturnResultIfValidAsync<T>(this MlResult<T> source,
                                                                                 T           returnValidValue)
        => await source.MatchAsync(
                                        validAsync: x      => returnValidValue.ToMlResultValidAsync(),
                                        failAsync : errors => errors          .ToMlResultFailAsync<T>()
                                  );


    public static async Task<MlResult<T>> ChangeReturnResultIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                 T                 returnValidValue)
        => (await sourceAsync).Match(
                                        valid: x      => returnValidValue.ToMlResultValid(),
                                        fail : errors => errors          .ToMlResultFail<T>()
                                    );


    #endregion ChangeReturnResultIfValid


    #region ChangeReturnResultIfValidToFail



    public static MlResult<T> ChangeReturnResultIfValidToFail<T>(this MlResult<T>     source,
                                                                      MlErrorsDetails returnFailValue)
        => source.Match(
                            valid: x      => returnFailValue.ToMlResultFail<T>(),
                            fail : errors => errors.ToMlResultFail<T>()
                        );

    public static MlResult<T> ChangeReturnResultIfValidToFail<T>(this MlResult<T> source,
                                                                      MlError     returnFailValue)
        => ChangeReturnResultIfValidToFail(source, returnFailValue.ToMlErrorsDetails());

    public static MlResult<T> ChangeReturnResultIfValidToFail<T>(this MlResult<T>         source,
                                                                      IEnumerable<string> returnFailValues)
        => ChangeReturnResultIfValidToFail(source, returnFailValues.ToMlErrorsDetails());

    public static MlResult<T> ChangeReturnResultIfValidToFail<T>(this MlResult<T> source,
                                                                      string      returnFailValue)
        => ChangeReturnResultIfValidToFail(source, returnFailValue.ToMlErrorsDetails());



    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this MlResult<T>     source,
                                                                                       MlErrorsDetails returnFailValue)
        => await source.MatchAsync(
                            validAsync: x      => returnFailValue.ToMlResultFailAsync<T>(),
                            failAsync : errors => errors         .ToMlResultFailAsync<T>()
                        );

    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this MlResult<T>     source,
                                                                      MlError     returnFailValue)
        => await ChangeReturnResultIfValidToFailAsync(source, returnFailValue.ToMlErrorsDetails());

    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this MlResult<T>     source,
                                                                      IEnumerable<string> returnFailValues)
        => await ChangeReturnResultIfValidToFailAsync(source, returnFailValues.ToMlErrorsDetails());

    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this MlResult<T>     source,
                                                                      string      returnFailValue)
        => await ChangeReturnResultIfValidToFailAsync(source, returnFailValue.ToMlErrorsDetails());



    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                       MlErrorsDetails   returnFailValue)
        => (await sourceAsync).Match(
                                        valid: x      => returnFailValue.ToMlResultFail<T>(),
                                        fail : errors => errors.ToMlResultFail<T>()
                                    );

    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                       MlError           returnFailValue)
        => await ChangeReturnResultIfValidToFailAsync(sourceAsync, returnFailValue.ToMlErrorsDetails());

    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this Task<MlResult<T>>   sourceAsync,
                                                                                       IEnumerable<string> returnFailValues)
        => await ChangeReturnResultIfValidToFailAsync(sourceAsync, returnFailValues.ToMlErrorsDetails());

    public static async Task<MlResult<T>> ChangeReturnResultIfValidToFailAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                       string            returnFailValue)
        => await ChangeReturnResultIfValidToFailAsync(sourceAsync, returnFailValue.ToMlErrorsDetails());





    #endregion ChangeReturnResultIfValidToFail



    #region ChangeReturnResultIfFailToValid



    public static MlResult<T> ChangeReturnResultIfFailToValid<T>(this MlResult<T> source,
                                                                      T           returnValidValue)
        => source.Match(
                            valid: x      => source,
                            fail : errors => returnValidValue.ToMlResultValid()
                        );


    public static async Task<MlResult<T>> ChangeReturnResultIfFailToValidAsync<T>(this MlResult<T> source,
                                                                                       T           returnValidValue)
        => await source.MatchAsync(
                            valid     : x      => source,
                            failAsync : errors => returnValidValue.ToMlResultValidAsync()
                        );

    public static async Task<MlResult<T>> ChangeReturnResultIfFailToValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                       T                 returnValidValue)
        => (await sourceAsync).ChangeReturnResultIfFailToValid(returnValidValue);




    #endregion ChangeReturnResultIfFailToValid



    #region Sin_Usar



    //public static MlResult<TReturn> ResultSimpleReturn<T>(this MlResult<TReturn> source,
    //                                             T validValue,
    //                                             IEnumerable<string> failValue)
    //=> source.Match<T, T>(
    //                        valid: x => validValue,
    //                        fail: errors => failValue.ToMlResultFail<T>()
    //                     );


    //public static MlResult<TReturn> ResultSimpleReturn<T>(this MlResult<TReturn> source,
    //                                             T         validValue,
    //                                             T         failValidValue)
    //    => source.Match<T, T>(
    //                            valid: x      => validValue      .ToMlResultValid(),
    //                            fail: errors => failValidValue.ToMlResultValid()
    //                         );

    //public static MlResult<TReturn> ResultSimpleReturn<T>(this MlResult<TReturn>           source,
    //                                             IEnumerable<string> validFailValue,
    //                                             IEnumerable<string> failValue)
    //    => source.Match<T, T>(
    //                                    valid: x      => validFailValue.ToMlResultFail<T>(),
    //                                    fail: errors => failValue      .ToMlResultFail<T>()
    //                               );


    //public static MlResult<TReturn> ResultSimpleReturnSuccess<T>(this MlResult<TReturn> source,
    //                                                    T         returnValidValue)
    //    => source.Match<T, T>(
    //                                    valid: x      => returnValidValue.ToMlResultValid(),
    //                                    fail: errors => errors.ToMlResultFail<T>()
    //                               );

    //public static MlResult<TReturn> ResultSimpleReturnSuccess<T>(this MlResult<TReturn>           source,
    //                                                    IEnumerable<string> returnFailValue)
    //    => source.Match<T, T>(
    //                                    valid: x      => returnFailValue.ToMlResultFail<T>(),
    //                                    fail: errors => errors.ToMlResultFail<T>()
    //                               );


    //public static MlResult<TReturn> ResultSimpleReturnFaillure<T>(this MlResult<TReturn> source,
    //                                                     T         returnValidValue)
    //    => source.Match<T, T>(
    //                                    valid: x      => returnValidValue.ToMlResultValid(),
    //                                    fail: errors => errors.ToMlResultFail<T>()
    //                               );

    //public static MlResult<TReturn> ResultSimpleReturnFaillure<T>(this MlResult<TReturn>           source,
    //                                                     IEnumerable<string> returnFailValue)
    //    => source.Match<T, T>(
    //                                    valid: x      => source,
    //                                    fail: errors => returnFailValue.ToMlResultFail<T>()
    //                               );



    #endregion Sin_Usar




    //public static Task<MlResult<TReturn>> ChangeReturnResultAsync<T, TReturn>(this MlResult<TReturn>           source,
    //                                                                       TReturn             validValue,
    //                                                                       IEnumerable<string> failValue)
    //    => (source.Match(
    //                        valid: x      => validValue.ToMlResultValid(),
    //                        fail: errors => failValue.ToMlResultFail<TReturn>()
    //                    )).ToAsync();


    //public static Task<MlResult<TReturn>> ChangeReturnResultAsync<T, TReturn>(this MlResult<TReturn> source,
    //                                                                       TReturn   validValue,
    //                                                                       TReturn   failValidValue)
    //    => (source.Match(
    //                        valid: x      => validValue      .ToMlResultValid(),
    //                        fail: errors => failValidValue.ToMlResultValid()
    //                    )).ToAsync();

    //public static Task<MlResult<TReturn>> ChangeReturnResultAsync<T, TReturn>(this MlResult<TReturn>           source,
    //                                                                       IEnumerable<string> validFailValue,
    //                                                                       IEnumerable<string> failValue)
    //    => (source.Match(
    //                        valid: x      => validFailValue.ToMlResultFail<TReturn>(),
    //                        fail: errors => failValue       .ToMlResultFail<TReturn>()
    //                     )).ToAsync();


    //public static Task<MlResult<TReturn>> ChangeReturnResultSuccessAsync<T, TReturn>(this MlResult<TReturn> source,
    //                                                                              TReturn   returnValidValue)
    //    => (source.Match(
    //                        valid: x      => returnValidValue.ToMlResultValid(),
    //                        fail: errors => errors.ToMlResultFail<TReturn>()
    //                    )).ToAsync();

    //public static Task<MlResult<TReturn>> ChangeReturnResultSuccessAsync<T, TReturn>(this MlResult<TReturn>           source,
    //                                                                              IEnumerable<string> returnFailValue)
    //    => (source.Match(
    //                        valid: x      => returnFailValue.ToMlResultFail<TReturn>(),
    //                        fail: errors => errors.ToMlResultFail<TReturn>()
    //                    )).ToAsync();


    //public static Task<MlResult<TReturn>> ChangeReturnResultFaillureAsync<T, TReturn>(this MlResult<TReturn> source,
    //                                                                               TReturn   returnValidValue)
    //    => (source.Match(
    //                        valid: x      => returnValidValue.ToMlResultValid(),
    //                        fail: errors => errors.ToMlResultFail<TReturn>()
    //                    )).ToAsync();



    //    public static Task<MlResult<TReturn>> ResultSimpleReturnAsync<T>(this MlResult<TReturn>           source,
    //                                                            T                   validValue,
    //                                                            IEnumerable<string> failValue)
    //    => (source.Match(
    //                        valid: x      => validValue.ToMlResultValid(),
    //                        fail: errors => failValue.ToMlResultFail<T>()
    //                    )).ToAsync();


    //public static Task<MlResult<TReturn>> ResultSimpleReturnAsync<T>(this MlResult<TReturn> source,
    //                                                        T         validValue,
    //                                                        T         failValidValue)
    //    => (source.Match(
    //                        valid: x      => validValue      .ToMlResultValid(),
    //                        fail: errors => failValidValue.ToMlResultValid()
    //                    )).ToAsync();

    //public static Task<MlResult<TReturn>> ResultSimpleReturnAsync<T>(this MlResult<TReturn>           source,
    //                                                        IEnumerable<string> validFailValue,
    //                                                        IEnumerable<string> failValue)
    //    => (source.Match(
    //                        valid: x      => validFailValue.ToMlResultFail<T>(),
    //                        fail: errors => failValue      .ToMlResultFail<T>()
    //                    )).ToAsync();


    //public static Task<MlResult<TReturn>> ResultSimpleReturnSuccessAsync<T>(this MlResult<TReturn> source,
    //                                                               T         returnValidValue)
    //    => (source.Match(
    //                        valid: x      => returnValidValue.ToMlResultValid(),
    //                        fail: errors => errors.ToMlResultFail<T>()
    //                    )).ToAsync();

    //public static Task<MlResult<TReturn>> ResultSimpleReturnSuccessAsync<T>(this MlResult<TReturn>           source,
    //                                                               IEnumerable<string> returnFailValue)
    //    => (source.Match(
    //                        valid: x      => returnFailValue.ToMlResultFail<T>(),
    //                        fail: errors => errors.ToMlResultFail<T>()
    //                    )).ToAsync();


    //public static Task<MlResult<TReturn>> ResultSimpleReturnFaillureAsync<T>(this MlResult<TReturn> source,
    //                                                                T         returnValidValue)
    //    => (source.Match(
    //                        valid: x      => returnValidValue.ToMlResultValid(),
    //                        fail: errors => errors.ToMlResultFail<T>()
    //                    )).ToAsync();

    //public static Task<MlResult<TReturn>> ResultSimpleReturnFaillureAsync<T>(this MlResult<TReturn>           source,
    //                                                                IEnumerable<string> returnFailValue)
    //    => (source.Match(
    //                        valid: x      => source,
    //                        fail: errors => returnFailValue.ToMlResultFail<T>()
    //                    )).ToAsync();
















    //public static async Task<MlResult<TReturn>> ChangeReturnResultAsync<T, TReturn>(this Task<MlResult<TReturn>>     sourceAsync,
    //                                                                             TReturn             validValue,
    //                                                                             IEnumerable<string> failValue)
    //    => (await sourceAsync).Match(
    //                                    valid: x      => validValue.ToMlResultValid(),
    //                                    fail: errors => failValue.ToMlResultFail<TReturn>()
    //                                );


    //public static async Task<MlResult<TReturn>> ChangeReturnResultAsync<T, TReturn>(this Task<MlResult<TReturn>> sourceAsync,
    //                                                                             TReturn         validValue,
    //                                                                             TReturn         failValidValue)
    //    => (await sourceAsync).Match(
    //                                    valid: x      => validValue      .ToMlResultValid(),
    //                                    fail: errors => failValidValue.ToMlResultValid()
    //                                );

    //public async static Task<MlResult<TReturn>> ChangeReturnResultAsync<T, TReturn>(this Task<MlResult<TReturn>>     sourceAsync,
    //                                                                             IEnumerable<string> validFailValue,
    //                                                                             IEnumerable<string> failValue)
    //    => (await sourceAsync).Match(
    //                                    valid: x      => validFailValue.ToMlResultFail<TReturn>(),
    //                                    fail: errors => failValue      .ToMlResultFail<TReturn>()
    //                                );


    //public async static Task<MlResult<TReturn>> ChangeReturnResultSuccessAsync<T, TReturn>(this Task<MlResult<TReturn>> sourceAsync,
    //                                                                                    TReturn         returnValidValue)
    //    => (await sourceAsync).Match(
    //                                    valid: x      => returnValidValue.ToMlResultValid(),
    //                                    fail: errors => errors.ToMlResultFail<TReturn>()
    //                                );

    //public async static Task<MlResult<TReturn>> ChangeReturnResultSuccessAsync<T, TReturn>(this Task<MlResult<TReturn>>     sourceAsync,
    //                                                                                    IEnumerable<string> returnFailValue)
    //    => (await sourceAsync).Match(
    //                                    valid: x      => returnFailValue.ToMlResultFail<TReturn>(),
    //                                    fail: errors => errors.ToMlResultFail<TReturn>()
    //                                );


    ////public async static Task<MlResult<TReturn>> ChangeReturnResultFaillureAsync<T, TReturn>(this Task<MlResult<TReturn>> sourceAsync,
    ////                                                                                     TReturn         returnValidValue)
    ////{
    ////    var result = (await sourceAsync).Match(
    ////                                    valid: x      => sourceAsync,
    ////                                    fail: errors => returnValidValue.ToMlResultValid()
    ////                                );
    ////    return result;
    ////}


    //public static async Task<MlResult<TReturn>> ResultSimpleReturnAsync<T>(this Task<MlResult<TReturn>>     sourceAsync,
    //                                                              T                   validValue,
    //                                                              IEnumerable<string> failValue)
    //    => (await sourceAsync).Match(
    //                                    valid: x      => validValue.ToMlResultValid(),
    //                                    fail: errors => failValue.ToMlResultFail<T>()
    //                                );


    //public static async Task<MlResult<TReturn>> ResultSimpleReturnAsync<T>(this Task<MlResult<TReturn>> sourceAsync,
    //                                                              T               validValue,
    //                                                              T               failValidValue)
    //    => (await sourceAsync).Match(
    //                                    valid: x      => validValue      .ToMlResultValid(),
    //                                    fail: errors => failValidValue.ToMlResultValid()
    //                                );

    //public static async Task<MlResult<TReturn>> ResultSimpleReturnAsync<T>(this Task<MlResult<TReturn>>     sourceAsync,
    //                                                              IEnumerable<string> validFailValue,
    //                                                              IEnumerable<string> failValue)
    //    => (await sourceAsync).Match(
    //                                    valid: x      => validFailValue.ToMlResultFail<T>(),
    //                                    fail: errors => failValue      .ToMlResultFail<T>()
    //                                );


    //public static async Task<MlResult<TReturn>> ResultSimpleReturnSuccessAsync<T>(this Task<MlResult<TReturn>> sourceAsync,
    //                                                                     T               returnValidValue)
    //    => (await sourceAsync).Match(
    //                                    valid: x      => returnValidValue.ToMlResultValid(),
    //                                    fail: errors => errors.ToMlResultFail<T>()
    //                                );

    //public static async Task<MlResult<TReturn>> ResultSimpleReturnSuccessAsync<T>(this Task<MlResult<TReturn>>     sourceAsync,
    //                                                                     IEnumerable<string> returnFailValue)
    //    => (await sourceAsync).Match(
    //                                    valid: x      => returnFailValue.ToMlResultFail<T>(),
    //                                    fail: errors => errors.ToMlResultFail<T>()
    //                                );


    //public static async Task<MlResult<TReturn>> ResultSimpleReturnFaillureAsync<T>(this Task<MlResult<TReturn>> sourceAsync,
    //                                                                      T               returnValidValue)
    //{
    //    var result =  await sourceAsync.MatchAsync(
    //                                    successAsync: x      => sourceAsync,
    //                                    fail     : errors => returnValidValue.ToMlResultValid()
    //                                );

    //    return result;
    //}

    //public static async Task<MlResult<TReturn>> ResultSimpleReturnFaillureAsync<T>(this Task<MlResult<TReturn>>     sourceAsync,
    //                                                                      IEnumerable<string> returnFailValue)
    //    => await sourceAsync.MatchAsync(
    //                                        successAsync: x      => sourceAsync,
    //                                        failureAsync: errors => returnFailValue.ToMlResultFail<T>().ToAsync()
    //                                    );




















    //public static MlResult<TReturn> ChangeReturn<T, TReturn>(this MlResult<TReturn>           source,
    //                                                            Func<T, TReturn>    successFunc,
    //                                                            IEnumerable<string> failValue)
    //    => source.Match<T, TReturn>(
    //                                    valid: x      => successFunc(x).ToMlResultValid(),
    //                                    fail: errors => failValue.ToMlResultFail<TReturn>());


    ////public static MlResult<TReturn> ChangeReturnFailure<T, TReturn>(this MlResult<TReturn> source,
    ////                                                                   IEnumerable<string> failValue)
    ////    => source.Match<T, TReturn>(
    ////                                    valid: x => successFunc(x).ToMlResultValid(),
    ////                                    fail: errors => failValue.ToMlResultFail<TReturn>()
    ////                                );

    //public static MlResult<TReturn> ChangeReturnSuccess<T>(this MlResult<TReturn> source,
    //                                                    T         returnValidValue)
    //    => source.Mapp(_ => returnValidValue);

    //public static MlResult<TReturn> ChangeReturnFailure<T>(this MlResult<TReturn> source,
    //                                                    T         returnValidValue)
    //    => source.Match<T, T>(
    //                                valid: x      => source,
    //                                fail: errors => returnValidValue
    //                          );



}
