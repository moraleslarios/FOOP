namespace MoralesLarios.OOFP.Types;
public static class MlResultActionsExecSelf
{


    #region ExecSelf





    /// <summary>
    /// Execute the actionSuccess if the source is valid, otherwise execute the actionFailure. Return same source every time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="actionSuccess"></param>
    /// <param name="actionFailure"></param>
    /// <returns></returns>
    public static MlResult<T> ExecSelf<T>(this MlResult<T>             source, 
                                               Action<T>               actionValid,
                                               Action<MlErrorsDetails> actionFail)
        => source.Match(
                            valid: x      => { actionValid(x)    ; return source; },
                            fail : errors => { actionFail(errors); return source; }
                        );

    public static async Task<MlResult<T>> ExecSelfAsync<T>(this MlResult<T>                 source,
                                                                Func<T              , Task> actionValidAsync,
                                                                Func<MlErrorsDetails, Task> actionFailAsync)
        => await source.MatchAsync(
                                        validAsync: async x      => { await actionValidAsync(x)    ; return source; },
                                        failAsync : async errors => { await actionFailAsync(errors); return source; }
                                   );

    public static async Task<MlResult<T>> ExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                Func<T              , Task> actionValidAsync,
                                                                Func<MlErrorsDetails, Task> actionFailAsync)
        => await sourceAsync.MatchAsync(
                                            validAsync: async x      => { await actionValidAsync(x    ); return await sourceAsync; },
                                            failAsync : async errors => { await actionFailAsync(errors); return await sourceAsync; }
                                       );

    public static async Task<MlResult<T>> ExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                Action<T>                   actionValid,
                                                                Func<MlErrorsDetails, Task> actionFailAsync)
        => await (await sourceAsync).ExecSelfAsync(actionValid.ToFuncTask(), actionFailAsync);

    public static async Task<MlResult<T>> ExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                Func<T              , Task> actionValidAsync,
                                                                Action<MlErrorsDetails>     actionFail)
        => await (await sourceAsync).ExecSelfAsync(actionValidAsync, actionFail.ToFuncTask());

    public static async Task<MlResult<T>> ExecSelfAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                Action<T>               actionValid,
                                                                Action<MlErrorsDetails> actionFail)
        => await (await sourceAsync).ExecSelfAsync(actionValid.ToFuncTask(), actionFail.ToFuncTask());



    /// <summary>
    /// Execute the actionSuccess if the source is valid, otherwise execute the actionFailure. Return same source every time.
    /// If an exception is thrown, the errorMessageBuilder is used to build the error message.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="actionValid"></param>
    /// <param name="actionFail"></param>
    /// <param name="errorMessageBuilder"></param>
    /// <returns></returns>
    public static MlResult<T> TryExecSelf<T>(this MlResult<T>             source,
                                                  Action<T>               actionValid,
                                                  Action<MlErrorsDetails> actionFail,
                                                  Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x      => actionValid.TryToMlResult(x, errorMessageBuilder),
                            fail : (Func<MlErrorsDetails, MlResult<T>>)(errors => MlResultTransformations.TryToMlResultErrors<T>(actionFail, errors, errorMessageBuilder))
                        );

    public static MlResult<T> TryExecSelf<T>(this MlResult<T>             source,
                                                  Action<T>               actionValid,
                                                  Action<MlErrorsDetails> actionFail,
                                                  string                  errorMessage = null!)
        => source.TryExecSelf(actionValid, actionFail, _ => errorMessage);



    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>                 source,
                                                                   Func<T, Task>               actionValidAsync,
                                                                   Func<MlErrorsDetails, Task> actionFailAsync,
                                                                   Func<Exception, string>     errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: async x      => await actionValidAsync.TryToMlResultAsync         (x     , errorMessageBuilder),
                                        failAsync : async errors => await actionFailAsync .TryToMlResultErrorsAsync<T>(errors, errorMessageBuilder)
                                   );

    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>                 source,
                                                                   Func<T, Task>               actionValidAsync,
                                                                   Func<MlErrorsDetails, Task> actionFailAsync,
                                                                   string                      errorMessage = null!)
        => await source.TryExecSelfAsync(actionValidAsync, actionFailAsync, _ => errorMessage);



    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                   Func<T, Task>               actionValidAsync,
                                                                   Func<MlErrorsDetails, Task> actionFailAsync,
                                                                   Func<Exception, string>     errorMessageBuilder)
        => await sourceAsync.MatchAsync(
                                            validAsync: async x      => await actionValidAsync.TryToMlResultAsync        (x     , errorMessageBuilder),
                                            failAsync : async errors => await actionFailAsync.TryToMlResultErrorsAsync<T>(errors, errorMessageBuilder)
                                        );

    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                   Func<T, Task>               actionValidAsync,
                                                                   Func<MlErrorsDetails, Task> actionFailAsync,
                                                                   string                      errorMessage = null!)
        => await sourceAsync.TryExecSelfAsync(actionValidAsync, actionFailAsync, _ => errorMessage);


    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                   Action<T>                   actionValid,
                                                                   Func<MlErrorsDetails, Task> actionFailAsync,
                                                                   Func<Exception, string>     errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfAsync(actionValid.ToFuncTask(), actionFailAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                   Action<T>                   actionValid,
                                                                   Func<MlErrorsDetails, Task> actionFailAsync,
                                                                   string                      errorMessage = null!)
        => await (await sourceAsync).TryExecSelfAsync(actionValid.ToFuncTask(), actionFailAsync, errorMessage);

    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>             source,
                                                                   Func<T, Task>           actionValidAsync,
                                                                   Action<MlErrorsDetails> actionFail,
                                                                   Func<Exception, string> errorMessageBuilder)
        => await source.TryExecSelfAsync(actionValidAsync, actionFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>             source,
                                                                   Func<T, Task>           actionValidAsync,
                                                                   Action<MlErrorsDetails> actionFail,
                                                                   string                  errorMessage = null!)
        => await source.TryExecSelfAsync(actionValidAsync, actionFail.ToFuncTask(), errorMessage);

    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>             source,
                                                                   Action<T>               actionValid,
                                                                   Action<MlErrorsDetails> actionFail,
                                                                   Func<Exception, string> errorMessageBuilder)
        => await source.TryExecSelfAsync(actionValid.ToFuncTask(), actionFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<T>> TryExecSelfAsync<T>(this MlResult<T>             source,
                                                                   Action<T>               actionValid,
                                                                   Action<MlErrorsDetails> actionFail,
                                                                   string                  errorMessage = null!)
        => await source.TryExecSelfAsync(actionValid.ToFuncTask(), actionFail.ToFuncTask(), errorMessage);






    #endregion


    #region ExecSelfIfValid



    /// <summary>
    /// Execute the actionSuccess if the source is valid, otherwise execute the actionFailure. Return same source every time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="actionValid"></param>
    /// <returns></returns>
    public static MlResult<T> ExecSelfIfValid<T>(this MlResult<T> source,
                                                      Action<T>   actionValid)
        => source.Match(
                            valid: x      => { actionValid(x); return source; },
                            fail : errors => source
                        );
    public static async Task<MlResult<T>> ExecSelfIfValidAsync<T>(this MlResult<T>   source,
                                                                       Func<T, Task> actionValidAsync)
        => await source.MatchAsync(
                                        validAsync: async x      => { await actionValidAsync(x); return source; },
                                        failAsync :       errors => source.ToAsync()
                                   );
    public static async Task<MlResult<T>> ExecSelfIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                       Func<T, Task>     actionValidAsync)
        => await sourceAsync.MatchAsync(
                                            validAsync: async x      => { await actionValidAsync(x); return await sourceAsync; },
                                            failAsync : async errors => await sourceAsync
                                        );

    public static async Task<MlResult<T>> ExecSelfIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                       Action<T>         actionValid)
        => await (await sourceAsync).ExecSelfIfValidAsync(actionValid.ToFuncTask());


    /// <summary>
    /// Execute the actionSuccess if the source is valid, otherwise execute the actionFailure. Return same source every time.
    /// If an exception is thrown, the errorMessageBuilder is used to build the error message.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="actionValid"></param>
    /// <param name="errorMessageBuilder"></param>
    /// <returns></returns>
    public static MlResult<T> TryExecSelfIfValid<T>(this MlResult<T>             source,
                                                         Action<T>               actionValid,
                                                         Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x      => actionValid.TryToMlResult(x, errorMessageBuilder),
                            fail : errors => source
                        );

    public static MlResult<T> TryExecSelfIfValid<T>(this MlResult<T> source,
                                                         Action<T>   actionValid,
                                                         string      errorMessage = null!)
        => source.TryExecSelfIfValid(actionValid, _ => errorMessage);

    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this MlResult<T>             source,
                                                                          Func<T, Task>   actionValidAsync,
                                                                          Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: async x => await actionValidAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                        failAsync : errors => source.ToAsync()
                                   );

    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this MlResult<T>   source,
                                                                          Func<T, Task> actionValidAsync,
                                                                          string        errorMessage = null!)
        => await source.TryExecSelfIfValidAsync(actionValidAsync, _ => errorMessage);

    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                          Func<T, Task>           actionValidAsync,
                                                                          Func<Exception, string> errorMessageBuilder)
        => await sourceAsync.MatchAsync(
                                            validAsync: async x      => await actionValidAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                            failAsync : async errors => await sourceAsync
                                        );

    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                          Func<T, Task>     actionValidAsync,
                                                                          string            errorMessage = null!)
        => await sourceAsync.TryExecSelfIfValidAsync(actionValidAsync, _ => errorMessage);

    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                          Action<T>               actionValid,
                                                                          Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfValidAsync(actionValid.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<T>> TryExecSelfIfValidAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                          Action<T>         actionValid,
                                                                          string            errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfValidAsync(actionValid.ToFuncTask(), errorMessage);


    #endregion


    #region ExecSelfIfFail







    public static MlResult<T> ExecSelfIfFail<T>(this MlResult<T>             source,
                                                     Action<MlErrorsDetails> actionFail)
        => source.Match(
                            valid: x      => x.ToMlResultValid(),
                            fail : errors => { actionFail(errors); return source; }
                        );

    public static async Task<MlResult<T>> ExecSelfIfFailAsync<T>(this MlResult<T>                 source,
                                                                      Func<MlErrorsDetails, Task> actionFailAsync)
        => await source.MatchAsync(
                                        validAsync:      x      => x.ToMlResultValid().ToAsync(),
                                        failAsync: async errors => { await actionFailAsync(errors); return source; }
                                   );

    public static async Task<MlResult<T>> ExecSelfFailAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                    Func<MlErrorsDetails, Task> actionFailAsync)
        => await sourceAsync.MatchAsync(
                                            validAsync: async x      => await sourceAsync,
                                            failAsync : async errors => { await actionFailAsync(errors); return await sourceAsync; }
                                       );

    public static async Task<MlResult<T>> ExecSelfFailAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                    Action<MlErrorsDetails> actionFail)
        => (await sourceAsync).ExecSelfIfFail(actionFail);


    public static MlResult<T> TryExecSelfFail<T>(this MlResult<T>             source,
                                                      Action<MlErrorsDetails> actionFail,
                                                      Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x      => x.ToMlResultValid(),
                            fail : (Func<MlErrorsDetails, MlResult<T>>)(errors => MlResultTransformations.TryToMlResultErrors<T>(actionFail, errors, errorMessageBuilder))
                        );

    public static MlResult<T> TryExecSelfFail<T>(this MlResult<T>             source,
                                                      Action<MlErrorsDetails> actionFail,
                                                      string                  errorMessage = null!)
        => source.TryExecSelfFail(actionFail, _ => errorMessage);

    public static async Task<MlResult<T>> TryExecSelfIfFailAsync<T>(this MlResult<T>                 source,
                                                                         Func<MlErrorsDetails, Task> actionFailAsync,
                                                                         Func<Exception, string>     errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: x => x.ToMlResultValid().ToAsync(),
                                        failAsync : async errors => await actionFailAsync.TryToMlResultErrorsAsync<T>(errors, errorMessageBuilder)
                                   );

    public static async Task<MlResult<T>> TryExecSelfIfFailAsync<T>(this MlResult<T>                 source,
                                                                         Func<MlErrorsDetails, Task> actionFailAsync,
                                                                         string                      errorMessage = null!)
        => await source.TryExecSelfIfFailAsync(actionFailAsync, _ => errorMessage);

    public static async Task<MlResult<T>> TryExecSelfFailAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                       Func<MlErrorsDetails, Task> actionFailAsync,
                                                                       Func<Exception, string>     errorMessageBuilder)
        => await sourceAsync.MatchAsync(
                                            validAsync:       x      => x.ToMlResultValidAsync(),
                                            failAsync : async errors => await actionFailAsync.TryToMlResultErrorsAsync<T>(errors, errorMessageBuilder)
                                        );

    public static async Task<MlResult<T>> TryExecSelfFailAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                       Func<MlErrorsDetails, Task> actionFailAsync,
                                                                       string                      errorMessage = null!)
        => await sourceAsync.TryExecSelfFailAsync(actionFailAsync, _ => errorMessage);

    public static async Task<MlResult<T>> TryExecSelfFailAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                       Action<MlErrorsDetails> actionFail,
                                                                       Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfFailAsync(actionFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<T>> TryExecSelfFailAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                       Action<MlErrorsDetails> actionFail,
                                                                       string                  errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfFailAsync(actionFail.ToFuncTask(), errorMessage);


    #endregion


    #region ExecSelfIfFailWithValue





    public static MlResult<T> ExecSelfIfFailWithValue<T, TValue>(this MlResult<T>    source,
                                                                      Action<TValue> actionFailValue)
        => source.Match(
                            valid: x      => x.ToMlResultValid(),
                            fail : errors => errors.GetDetailValue<TValue>()
                                                        .Bind(value =>
                                                        {
                                                            actionFailValue(value);

                                                            return source;
                                                        })
                        );


    public static async Task<MlResult<T>> ExecSelfIfFailWithValueAsync<T, TValue>(this MlResult<T>        source,
                                                                                       Func<TValue, Task> actionFailValueAsync)
        => await source.MatchAsync(
                                        validAsync: x      => x.ToMlResultValidAsync(),
                                        failAsync : errors => errors.GetDetailValue<TValue>()
                                                                    .BindAsync(async value =>
                                                                    {
                                                                        await actionFailValueAsync(value);

                                                                        return source;
                                                                    })
                                    );


    public static async Task<MlResult<T>> ExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>  sourceAsync,
                                                                                       Func<TValue, Task> actionFailValueAsync)
        => await (await sourceAsync).ExecSelfIfFailWithValueAsync(actionFailValueAsync);

    public static async Task<MlResult<T>> ExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>> sourceAsync,
                                                                                       Action<TValue>    actionFailValue)
        => await (await sourceAsync).ExecSelfIfFailWithValueAsync(actionFailValue.ToFuncTask());



    public static MlResult<T> TryExecSelfIfFailWithValue<T, TValue>(this MlResult<T>             source,
                                                                         Action<TValue>          actionFailValue,
                                                                         Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x      => x.ToMlResultValid(),
                            fail : errors => errors.GetDetailValue<TValue>()
                                                        .Bind(value =>
                                                        {
                                                            var actionResult = actionFailValue.TryToMlResult(value, errorMessageBuilder);

                                                            var result = actionResult.Match(
                                                                                                fail : errorDetails => source.ErrorsDetails.Merge(errorDetails).ToMlResultFail<T>(),
                                                                                                valid: _            => source
                                                                                            );

                                                            return result;
                                                        })
                        );

    public static MlResult<T> TryExecSelfIfFailWithValue<T, TValue>(this MlResult<T>    source,
                                                                         Action<TValue> actionFailValue,
                                                                         string         errorMessage = null!)
        => source.TryExecSelfIfFailWithValue<T, TValue>(actionFailValue, _ => errorMessage);


    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this MlResult<T>             source,
                                                                                          Func<TValue, Task>      actionFailValueAsync,
                                                                                          Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync(
                            validAsync: x      => x.ToMlResultValidAsync(),
                            failAsync : errors => errors.GetDetailValue<TValue>()
                                                        .BindAsync(async value =>
                                                        {
                                                            var actionResult = await actionFailValueAsync.TryToMlResultAsync(value, errorMessageBuilder);

                                                            var result = actionResult.Match(
                                                                                                fail : errorDetails => source.ErrorsDetails.Merge(errorDetails).ToMlResultFail<T>(),
                                                                                                valid: _            => source
                                                                                            );

                                                            return result;
                                                        })
                        );

    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this MlResult<T>        source,
                                                                                          Func<TValue, Task> actionFailValueAsync,
                                                                                          string             errorMessage = null!)
        => await source.TryExecSelfIfFailWithValueAsync(actionFailValueAsync, _ => errorMessage);


    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>       sourceAsync,
                                                                                          Func<TValue, Task>      actionFailValueAsync,
                                                                                          Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfFailWithValueAsync(actionFailValueAsync, errorMessageBuilder);


    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>  sourceAsync,
                                                                                          Func<TValue, Task> actionFailValueAsync,
                                                                                          string             errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfFailWithValueAsync(actionFailValueAsync, errorMessage);

    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>>       sourceAsync,
                                                                                          Action<TValue>          actionFailValue,
                                                                                          Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfFailWithValueAsync(actionFailValue.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<T>> TryExecSelfIfFailWithValueAsync<T, TValue>(this Task<MlResult<T>> sourceAsync,
                                                                                          Action<TValue>    actionFailValue,
                                                                                          string            errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfFailWithValueAsync(actionFailValue.ToFuncTask(), errorMessage);


    #endregion


    #region ExecSelfIfFailWithException





    public static MlResult<T> ExecSelfIfFailWithException<T>(this MlResult<T>       source,
                                                                  Action<Exception> actionFailException)
        => source.Match(
                            valid: x      => x.ToMlResultValid(),
                            fail : errors => errors.GetDetailException()
                                                                .Bind(exception =>
                                                                {
                                                                    actionFailException(exception);

                                                                    return source;
                                                                })
                        );


    public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(this MlResult<T>           source,
                                                                                   Func<Exception, Task> actionFailExceptionAsync)
        => await source.MatchAsync(
                                        validAsync: x      => x.ToMlResultValidAsync(),
                                        failAsync : errors => errors.GetDetailExceptionAsync()
                                                                            .BindAsync(async exception =>
                                                                            {
                                                                                await actionFailExceptionAsync(exception);

                                                                                return source;
                                                                            })
                                    );

    public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>>     sourceAsync,
                                                                                   Func<Exception, Task> actionFailExceptionAsync)
        => await (await sourceAsync).ExecSelfIfFailWithExceptionAsync(actionFailExceptionAsync);

    public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                   Action<Exception> actionFailException)
        => await (await sourceAsync).ExecSelfIfFailWithExceptionAsync(actionFailException.ToFuncTask());



    public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T>             source,
                                                                     Action<Exception>       actionFailException,
                                                                     Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            valid: x => x.ToMlResultValid(),
                            fail: errors => errors.GetDetailException()
                                                        .Bind(exception =>
                                                        {
                                                            var actionResult = actionFailException.TryToMlResult(exception, errorMessageBuilder);

                                                            var result = actionResult.Match(
                                                                                                fail: errorDetails => source.ErrorsDetails.Merge(errorDetails).ToMlResultFail<T>(),
                                                                                                valid: _ => source
                                                                                            );

                                                            return result;
                                                        })
                        );

    public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T>       source,
                                                                     Action<Exception> actionFailException,
                                                                     string            errorMessage = null!)
        => source.TryExecSelfIfFailWithException<T>(actionFailException, _ => errorMessage);


    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this MlResult<T>             source,
                                                                                      Func<Exception, Task>   actionFailExceptionAsync,
                                                                                      Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync(
                                        validAsync: x => x.ToMlResultValidAsync(),
                                        failAsync : errors => errors.GetDetailExceptionAsync()
                                                                    .BindAsync(async exception =>
                                                                    {
                                                                        var actionResult = await actionFailExceptionAsync.TryToMlResultAsync(exception, errorMessageBuilder);

                                                                        var result = await actionResult.MatchAsync(
                                                                                                            failAsync : async errorDetails => await source.ErrorsDetails.Merge(errorDetails).ToMlResultFailAsync<T>(),
                                                                                                            validAsync: async _            => await source.ToAsync()
                                                                                                        );

                                                                        return result;
                                                                    })
                                    );

    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this MlResult<T>            source,
                                                                                      Func<Exception, Task>  actionFailExceptionAsync,
                                                                                      string                 errorMessage = null!)
        => await source.TryExecSelfIfFailWithExceptionAsync(actionFailExceptionAsync, _ => errorMessage);


    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                                      Func<Exception, Task>   actionFailExceptionAsync,
                                                                                      Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfFailWithExceptionAsync(actionFailExceptionAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>>      sourceAsync,
                                                                                      Func<Exception, Task>  actionFailExceptionAsync,
                                                                                      string                 errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfFailWithExceptionAsync(actionFailExceptionAsync, errorMessage);

    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                                      Action<Exception>       actionFailException,
                                                                                      Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryExecSelfIfFailWithExceptionAsync(actionFailException.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                                      Action<Exception> actionFailException,
                                                                                      string            errorMessage = null!)
        => await (await sourceAsync).TryExecSelfIfFailWithExceptionAsync(actionFailException.ToFuncTask(), errorMessage);

    #endregion


    #region ExecSelfIfFailWithoutException


    public static MlResult<T> ExecSelfIfFailWithoutException<T>(this MlResult<T>             source,
                                                                     Action<MlErrorsDetails> actionFail)
        => source.Match(
                            valid: x             => x.ToMlResultValid(),
                            fail : errorsDetails => errorsDetails.GetDetailException()
                                                                .BindIfFail( errorsResult =>
                                                                {
                                                                    actionFail(errorsDetails);

                                                                    return errorsResult;
                                                                })
                                                                .Bind( _ => source)
                        );


    public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(this MlResult<T>                 source,
                                                                                      Func<MlErrorsDetails, Task> actionFailExceptionAsync)
        => await source.MatchAsync(
                                        validAsync: x             => x.ToMlResultValidAsync(),
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync()
                                                                                    .BindIfFailAsync(async errorsResult =>
                                                                                    {
                                                                                        await actionFailExceptionAsync(errorsResult);

                                                                                        return errorsResult;
                                                                                    })
                                                                                    .BindAsync( _ => source.ToAsync())
                                    );

    public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>           sourceAsync,
                                                                                      Func<MlErrorsDetails, Task> actionFailExceptionAsync)
        => await (await sourceAsync).ExecSelfIfFailWithoutExceptionAsync(actionFailExceptionAsync);

    public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                                      Action<MlErrorsDetails> actionFail)
        => await (await sourceAsync).ExecSelfIfFailWithoutExceptionAsync(actionFail.ToFuncTask());


    //public static MlResult<T> TryExecSelfIfFailWithoutException<T>(this MlResult<T>             source,
    //                                                                    Action<MlErrorsDetails> actionFail,
    //                                                                    Func<Exception, string> errorMessageBuilder)
    //    => source.Match(
    //                        valid: x             => x.ToMlResultValid(),
    //                        fail : errorsDetails => errorsDetails.GetDetailException()
    //                                                            .BindIfFail( errorsResult =>
    //                                                            {
    //                                                                var actionResult = actionFail.TryToMlResultErrors<T>(errorsResult, errorMessageBuilder);

    //                                                                var result = actionResult.Match(
    //                                                                                                    fail: errorDetails => source.ErrorsDetails.Merge(errorDetails).ToMlResultFail<T>(),
    //                                                                                                    valid: _ => source
    //                                                                                                );

    //                                                                return result;

    //                                                                //return null!;
    //                                                            })
    //                                                            .Bind( _ => source)
    //                    );








    #endregion




}
