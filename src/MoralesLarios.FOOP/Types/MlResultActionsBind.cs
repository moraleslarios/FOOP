namespace MoralesLarios.OOFP.Types;
public static class MlResultActionsBind
{



    #region Bind


    /// <summary>
    /// Execute the function if the source is valid, otherwise return the source
    /// </summary>
    /// <typeparam name="T">Input Type</typeparam>
    /// <typeparam name="TReturn">Return Type</typeparam>
    /// <param name="source">MlResult source</param>
    /// <param name="func">Method to execute (return MlResult<TReturn>)</param>
    /// <returns></returns>
    public static MlResult<TReturn> Bind<T, TReturn>(this MlResult<T>                source, 
                                                          Func<T, MlResult<TReturn>> func)
        => source.Match
        (
            fail : MlResult<TReturn>.Fail,
            valid: value => func(value)
        );

    public static Task<MlResult<TReturn>> BindAsync<T, TReturn>(this MlResult<T>                source, 
                                                                     Func<T, MlResult<TReturn>> func)
        => source.Bind(func).ToAsync();



    /// <summary>
    /// Execute the function if the source is valid, otherwise return the source
    /// </summary>
    /// <typeparam name="T">Input Type</typeparam>
    /// <typeparam name="TReturn">Return Type</typeparam>
    /// <param name="source"></param>
    /// <param name="funcAsync"></param>
    /// <returns></returns>
    public static async Task<MlResult<TReturn>> BindAsync<T, TReturn>(this MlResult<T>                      source,
                                                                           Func<T, Task<MlResult<TReturn>>> funcAsync)
        => await source.MatchAsync
        (
            failAsync :       errorsDetails =>        Task.FromResult(MlResult<TReturn>.Fail(errorsDetails)),
            validAsync: async value         => await funcAsync(value)
        );


    public static async Task<MlResult<TReturn>> BindAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                           Func<T, Task<MlResult<TReturn>>> funcAsync)
    {
        var partialMlResult = await sourceAsync;

        var result = await partialMlResult.MatchAsync
        (
            failAsync :       errorsDetails =>       Task.FromResult(MlResult<TReturn>.Fail(errorsDetails)),
            validAsync: async value         => await funcAsync(value)
        );

        return result;
    }

    public static async Task<MlResult<TReturn>> BindAsync<T, TReturn>(this Task<MlResult<T>>          sourceAsync,
                                                                           Func<T, MlResult<TReturn>> func)
        => await (await sourceAsync).BindAsync(func.ToFuncTask());



    /// <summary>
    /// Execute the function if the source is valid, otherwise return the source.
    /// If method func throw Exception, this saves in ErrorDetails.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    /// <param name="source"></param>
    /// <param name="func"></param>
    /// <param name="exceptionAditionalMessage"></param>
    /// <returns></returns>
    public static MlResult<TReturn> TryBind<T, TReturn>(this MlResult<T>                source,
                                                             Func<T, MlResult<TReturn>> func,
                                                             string                     exceptionAditionalMessage = null!)
        => TryBind(source, func, _ => exceptionAditionalMessage!);

    public static Task<MlResult<TReturn>> TryBindAsync<T, TReturn>(this MlResult<T>                source,
                                                                        Func<T, MlResult<TReturn>> func,
                                                                        string                     exceptionAditionalMessage = null!)
        => source.TryBind(func, exceptionAditionalMessage).ToAsync();


    public static MlResult<TReturn> TryBind<T, TReturn>(this MlResult<T>                source,
                                                             Func<T, MlResult<TReturn>> func,
                                                             Func<Exception, string>    errorMessageBuilder)
        => source.Match
        (
            fail : MlResult<TReturn>.Fail,
            valid: value => func.TryToMlResult(source.Value, errorMessageBuilder)
        );

    public static Task<MlResult<TReturn>> TryBindAsync<T, TReturn>(this MlResult<T>                source,
                                                                        Func<T, MlResult<TReturn>> func,
                                                                        Func<Exception, string>    errorMessageBuilder)
        => source.TryBind(func, errorMessageBuilder).ToAsync();


    public static async Task<MlResult<TReturn>> TryBindAsync<T, TReturn>(this MlResult<T>                      source,
                                                                              Func<T, Task<MlResult<TReturn>>> funcAsync,
                                                                              string                           exceptionAditionalMessage = null!)
        => await TryBindAsync(source, funcAsync, _ => exceptionAditionalMessage!);


    public static async Task<MlResult<TReturn>> TryBindAsync<T, TReturn>(this MlResult<T>                      source,
                                                                              Func<T, Task<MlResult<TReturn>>> funcAsync,
                                                                              Func<Exception, string>          errorMessageBuilder)
        => await source.MatchAsync
        (
            failAsync :       errorsDetails =>       MlResult<TReturn>.Fail(errorsDetails).ToAsync(),
            validAsync: async value         => await funcAsync.TryToMlResultAsync(source.Value, errorMessageBuilder)
        );


    public static async Task<MlResult<TReturn>> TryBindAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                              Func<T, Task<MlResult<TReturn>>> funcAsync,
                                                                              string                           exceptionAditionalMessage = null!)
        => await TryBindAsync(sourceAsync, funcAsync, _ => exceptionAditionalMessage!);

    public static async Task<MlResult<TReturn>> TryBindAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                              Func<T, Task<MlResult<TReturn>>> funcAsync,
                                                                              Func<Exception, string>          errorMessageBuilder)
    {
        var partialMlResult = await sourceAsync;

        var result = await partialMlResult.MatchAsync
        (
            failAsync :       errorsDetails =>       MlResult<TReturn>.Fail(errorsDetails).ToAsync(),
            validAsync: async value         => await funcAsync.TryToMlResultAsync((await sourceAsync).Value, errorMessageBuilder)
        );

        return result;
    }

    public static async Task<MlResult<TReturn>> TryBindAsync<T, TReturn>(this Task<MlResult<T>>          sourceAsync,
                                                                              Func<T, MlResult<TReturn>> func,
                                                                              Func<Exception, string>    errorMessageBuilder)
        => await (await sourceAsync).TryBindAsync(func.ToFuncTask(), errorMessageBuilder);

        public static async Task<MlResult<TReturn>> TryBindAsync<T, TReturn>(this Task<MlResult<T>>          sourceAsync,
                                                                                  Func<T, MlResult<TReturn>> func,
                                                                                  string                     exceptionAditionalMessage = null!)
            => await (await sourceAsync).TryBindAsync(func.ToFuncTask(), exceptionAditionalMessage);



    #endregion


    #region BindMulti






    public static MlResult<TReturn> BindMulti<T, TReturn>(this   MlResult<T>                  source,
                                                                 Func<T, MlResult<TReturn>>   returnFunc,
                                                          params Func<T, MlResult<TReturn>>[] funcs)
        => source.Match(
                            fail: errors => errors,
                            valid: value => value.ToMlResultValid()
                                                    .Map(x => funcs.Select(func => func(value)).ToList())
                                                    .Bind(resultData => resultData.Any(x => x.IsFail)
                                                                            ? resultData.FusionFailErros().SecureFailErrorsDetails()
                                                                            : returnFunc(value)));


    public static Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this   MlResult<T>                  source,
                                                                            Func<T, MlResult<TReturn>>   returnFunc,
                                                                     params Func<T, MlResult<TReturn>>[] funcs)
        => source.BindMulti(returnFunc, funcs).ToAsync();


    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this   MlResult<T>                      source,
                                                                                  Func<T, Task<MlResult<TReturn>>> returnFuncAsync,
                                                                           params Func<T, MlResult<TReturn>>[]     funcs)
        => await source.MatchAsync
        (
            failAsync :       errorsDetails => MlResult<TReturn>.FailAsync(errorsDetails),
            validAsync: async value         =>
            {
                var resultsData = funcs.Select(func => func(value)).ToList();
                var partialResult = resultsData.Any(x => x.IsFail)
                                        ? await resultsData.FusionFailErrosAsync().SecureFailErrorsDetailsAsync()
                                        : await returnFuncAsync(value);
                return partialResult;
            }
        );


    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this   MlResult<T>                        source,
                                                                                  Func<T, Task<MlResult<TReturn>>>   returnFuncAsync,
                                                                           params Func<T, Task<MlResult<TReturn>>>[] funcsAsync)
        => await source.MatchAsync
        (
            failAsync: errorsDetails => MlResult<TReturn>.FailAsync(errorsDetails),
            validAsync: async value =>
            {
                List<MlResult<TReturn>> resultsData = [];

                foreach (var funcAsync in funcsAsync)
                {
                    var funcResult = await funcAsync(value);

                    resultsData.Add(funcResult);
                }

                var partialResult = resultsData.Any(x => x.IsFail)
                                        ? await resultsData.FusionFailErrosAsync().SecureFailErrorsDetailsAsync()
                                        : await returnFuncAsync(value);
                return partialResult;
            }
        );

    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this   Task<MlResult<T>>            sourceAsync,
                                                                                  Func<T, MlResult<TReturn>>   returnFunc,
                                                                           params Func<T, MlResult<TReturn>>[] funcs)
        => await (await sourceAsync).BindMultiAsync(returnFunc, funcs);

    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this   Task<MlResult<T>>                sourceAsync,
                                                                                  Func<T, Task<MlResult<TReturn>>> returnFuncAsync,
                                                                           params Func<T, MlResult<TReturn>>[]     funcs)
        => await (await sourceAsync).BindMultiAsync(returnFuncAsync, funcs);

    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this   Task<MlResult<T>>                  sourceAsync,
                                                                                  Func<T, Task<MlResult<TReturn>>>   returnFuncAsync,
                                                                           params Func<T, Task<MlResult<TReturn>>>[] funcsAsync)
        => await (await sourceAsync).BindMultiAsync(returnFuncAsync, funcsAsync);







    public static MlResult<TReturn> BindMulti<T, TReturn>(this   MlResult<T>                  source,
                                                                 Func<T,
                                                                      IEnumerable<TReturn>,
                                                                      MlResult   <TReturn>>   returnFunc,
                                                          params Func<T, MlResult<TReturn>>[] funcs)
        => source.BindMulti<T, TReturn, TReturn>(returnFunc, funcs);


    public static Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this MlResult<T>                    source,
                                                                            Func<T,
                                                                            IEnumerable<TReturn>,
                                                                            MlResult<TReturn>>           returnFunc,
                                                                     params Func<T, MlResult<TReturn>>[] funcs)
        => source.BindMulti<T, TReturn, TReturn>(returnFunc, funcs).ToAsync();

    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this MlResult<T> source,
                                                                                  Func<T,
                                                                                       IEnumerable<TReturn>,
                                                                                       Task<MlResult<TReturn>>> returnFuncAsync,
                                                                           params Func<T, MlResult<TReturn>>[] funcs)
        => await source.BindMultiAsync<T, TReturn, TReturn>(returnFuncAsync, funcs);

    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this   MlResult<T>                        source,
                                                                                  Func<T,
                                                                                       IEnumerable<TReturn>,
                                                                                       Task<MlResult<TReturn>>>      returnFuncAsync,
                                                                           params Func<T, Task<MlResult<TReturn>>>[] funcsAsync)
        => await source.BindMultiAsync<T, TReturn, TReturn>(returnFuncAsync, funcsAsync);   

    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this   Task<MlResult<T>>            sourceAsync,
                                                                                  Func<T,
                                                                                       IEnumerable<TReturn>,
                                                                                       MlResult<TReturn>>      returnFunc,
                                                                           params Func<T, MlResult<TReturn>>[] funcs)
        => await (await sourceAsync).BindMultiAsync<T, TReturn, TReturn>(returnFunc, funcs);

    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this   Task<MlResult<T>>             sourceAsync,
                                                                                  Func<T,
                                                                                       IEnumerable<TReturn>,
                                                                                       Task<MlResult<TReturn>>> returnFuncAsync,
                                                                           params Func<T, MlResult<TReturn>>[]  funcs)
        => await (await sourceAsync).BindMultiAsync<T, TReturn, TReturn>(returnFuncAsync, funcs);


    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                  Func<T,
                                                                                       IEnumerable<TReturn>,
                                                                                       Task<MlResult<TReturn>>>      returnFuncAsync,
                                                                           params Func<T, Task<MlResult<TReturn>>>[] funcsAsync)
        => await (await sourceAsync).BindMultiAsync<T, TReturn, TReturn>(returnFuncAsync, funcsAsync);









    public static MlResult<TReturn> BindMulti<T, TReturn, TFuncColec>(this   MlResult<T>                     source,
                                                                             Func<T,
                                                                                  IEnumerable<TFuncColec>,
                                                                                  MlResult<TReturn>>         returnFunc,
                                                                      params Func<T, MlResult<TFuncColec>>[] funcs)
    {
        var result = source.Match
                (
                    fail: errors => errors,
                    valid: value =>
                    {
                        var resultsData = funcs.Select(func => func(value)).ToList();

                        var partialResult = resultsData.Any(x => x.IsFail)
                                                ? resultsData.FusionFailErros().SecureFailErrorsDetails()
                                                : returnFunc(value,
                                                             resultsData.Where(x => x.IsValid).Select(y => y.SecureValidValue()));

                        return partialResult;

                    }
                );

        return result;
    }

    public static Task<MlResult<TReturn>> BindMultiAsync<T, TReturn, TFuncColec>(this   MlResult<T>                     source,
                                                                                        Func<T,
                                                                                             IEnumerable<TFuncColec>,
                                                                                             MlResult<TReturn>>         returnFunc,
                                                                                 params Func<T, MlResult<TFuncColec>>[] funcs)
        => source.BindMulti(returnFunc, funcs).ToAsync();


    public static Task<MlResult<TReturn>> BindMultiAsync<T, TReturn, TFuncColec>(this   MlResult<T>                     source,
                                                                                        Func<T,
                                                                                             IEnumerable<TFuncColec>,
                                                                                             Task<MlResult<TReturn>>>   returnFuncAsync,
                                                                                 params Func<T, MlResult<TFuncColec>>[] funcs)
        => source.MatchAsync
        (
            failAsync: errorsDetails => MlResult<TReturn>.FailAsync(errorsDetails),
            validAsync: async value =>
            {
                var resultsData = funcs.Select(func => func(value)).ToList();
                var partialResult = resultsData.Any(x => x.IsFail)
                                        ? await resultsData.FusionFailErrosAsync().SecureFailErrorsDetailsAsync()
                                        : await returnFuncAsync(value,
                                                                resultsData.Where(x => x.IsValid).Select(y => y.SecureValidValue()));
                return partialResult;
            }
        );

    public static Task<MlResult<TReturn>> BindMultiAsync<T, TReturn, TFuncColec>(this   MlResult<T>                           source,
                                                                                        Func<T,
                                                                                             IEnumerable<TFuncColec>,
                                                                                             Task<MlResult<TReturn>>>         returnFuncAsync,
                                                                                 params Func<T, Task<MlResult<TFuncColec>>>[] funcsAsync)
        => source.MatchAsync
        (
            failAsync: errorsDetails => MlResult<TReturn>.FailAsync(errorsDetails),
            validAsync: async value =>
            {
                List<MlResult<TFuncColec>> resultsData = [];
                foreach (var funcAsync in funcsAsync)
                {
                    var funcResult = await funcAsync(value);
                    resultsData.Add(funcResult);
                }
                var partialResult = resultsData.Any(x => x.IsFail)
                                        ? await resultsData.FusionFailErrosAsync().SecureFailErrorsDetailsAsync()
                                        : await returnFuncAsync(value,
                                                                resultsData.Where(x => x.IsValid).Select(y => y.SecureValidValue()));
                return partialResult;
            }
        );

    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn, TFuncColec>(this   Task<MlResult<T>>               sourceAsync,
                                                                                              Func<T,
                                                                                                   IEnumerable<TFuncColec>,
                                                                                                   MlResult<TReturn>>         returnFunc,
                                                                                       params Func<T, MlResult<TFuncColec>>[] funcs)
        => await (await sourceAsync).BindMultiAsync(returnFunc, funcs);


    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn, TFuncColec>(this Task<MlResult<T>>                 sourceAsync,
                                                                                              Func<T,
                                                                                                   IEnumerable<TFuncColec>,
                                                                                                   Task<MlResult<TReturn>>>   returnFuncAsync,
                                                                                       params Func<T, MlResult<TFuncColec>>[] funcs)
        => await (await sourceAsync).BindMultiAsync(returnFuncAsync, funcs);

    public static async Task<MlResult<TReturn>> BindMultiAsync<T, TReturn, TFuncColec>(this   Task<MlResult<T>> sourceAsync,
                                                                                              Func<T,
                                                                                                   IEnumerable<TFuncColec>,
                                                                                                   Task<MlResult<TReturn>>> returnFuncAsync,
                                                                                       params Func<T, Task<MlResult<TFuncColec>>>[] funcs)
        => await (await sourceAsync).BindMultiAsync(returnFuncAsync, funcs);









    #endregion


    #region BindSaveValueInDetailsIfFaildFuncResultAsync

    /// <summary>
    /// Execute the function if the source is valid, otherwise return the source.
    /// If the func method fails, the 'Value' of the source is added as ErrorDetails.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    /// <param name="source"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static MlResult<TReturn> BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(this MlResult<T>                source, 
                                                                                             Func<T, MlResult<TReturn>> func)
        => source.Match
        (
            fail : errorDetails => errorDetails,
            valid: value =>
            {
                var result = func(value);

                if (result.IsFail) result.AddValueDetailIfFail(value!);

                return result;
            }
        );


    public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(this MlResult<T>                      source, 
                                                                                                              Func<T, Task<MlResult<TReturn>>> funcAsync)
        => await source.MatchAsync
                                (
                                    failAsync : errorsDetails => MlResult<TReturn>.FailAsync(errorsDetails),
                                    validAsync: async value =>
                                    {
                                        var result = await funcAsync(value);

                                        if (result.IsFail) result.AddValueDetailIfFail(value!);

                                        return result;
                                    }
                                );
                

    public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync, 
                                                                                                              Func<T, Task<MlResult<TReturn>>> funcAsync)
        => await (await sourceAsync).BindSaveValueInDetailsIfFaildFuncResultAsync(funcAsync);

    public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(this Task<MlResult<T>>          sourceAsync, 
                                                                                                              Func<T, MlResult<TReturn>> func)
        => await (await sourceAsync).BindSaveValueInDetailsIfFaildFuncResultAsync(func.ToFuncTask());


    public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(this MlResult<T>                source, 
                                                                                                Func<T, MlResult<TReturn>> func,
                                                                                                Func<Exception, string>    errorMessageBuilder)
        => source.Match
        (
            fail : errorDetails => errorDetails,
            valid: value =>
            {
                var result = func.TryToMlResult(value, errorMessageBuilder);

                if (result.IsFail) result.AddValueDetailIfFail(value!);

                return result;
            }
        );

    public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(this MlResult<T>                source, 
                                                                                                Func<T, MlResult<TReturn>> func,
                                                                                                string                     errorMessage = null!)
        => source.TryBindSaveValueInDetailsIfFaildFuncResult(func, _ => errorMessage!);

    public static Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(this MlResult<T>                source, 
                                                                                                           Func<T, MlResult<TReturn>> func,
                                                                                                           Func<Exception, string>    errorMessageBuilder)
        => TryBindSaveValueInDetailsIfFaildFuncResult(source, func, errorMessageBuilder).ToAsync();

    public static Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(this MlResult<T>                source, 
                                                                                                           Func<T, MlResult<TReturn>> func,
                                                                                                           string                     errorMessage = null!)
        => TryBindSaveValueInDetailsIfFaildFuncResult(source, func, errorMessage).ToAsync();

    public static async Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(this Task<MlResult<T>>          sourceAsync, 
                                                                                                                 Func<T, MlResult<TReturn>> func,
                                                                                                                 Func<Exception, string>    errorMessageBuilder)
        => (await sourceAsync).TryBindSaveValueInDetailsIfFaildFuncResult(func, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(this Task<MlResult<T>>          sourceAsync, 
                                                                                                                 Func<T, MlResult<TReturn>> func,
                                                                                                                 string                     errorMessage = null!)
        => (await sourceAsync).TryBindSaveValueInDetailsIfFaildFuncResult(func, errorMessage);

    public static async Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync, 
                                                                                                                 Func<T, Task<MlResult<TReturn>>> funcAsync,
                                                                                                                 Func<Exception, string>          errorMessageBuilder)
        => await sourceAsync.MatchAsync
        (
            failAsync : errorDetails => errorDetails.ToMlResultFailAsync<TReturn>(),
            validAsync: async value =>
            {
                var result = await funcAsync.TryToMlResultAsync(value, errorMessageBuilder);

                if (result.IsFail) result.AddValueDetailIfFail(value!);

                return result;
            }
        );

    public static async Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync, 
                                                                                                                 Func<T, Task<MlResult<TReturn>>> funcAsync,
                                                                                                                 string                           errorMessage = null!)
        => await sourceAsync.TryBindSaveValueInDetailsIfFaildFuncResultAsync(funcAsync, _ => errorMessage!);




    #endregion


    #region BindIf


    public static MlResult<TReturn> BindIf<T, TReturn>(this MlResult<T>                source,
                                                            Func<T, bool>              condition,
                                                            Func<T, MlResult<TReturn>> funcTrue,
                                                            Func<T, MlResult<TReturn>> funcFalse)
    {
        var result = source.Match(
                                       valid: x => condition(x) ? funcTrue(x) : funcFalse(x),
                                       fail :      MlResult<TReturn>.Fail
                                 );
        return result;
    }

    public static Task<MlResult<TReturn>> BindIfAsync<T, TReturn>(this MlResult<T>                source,
                                                                       Func<T, bool>              condition,
                                                                       Func<T, MlResult<TReturn>> funcTrue,
                                                                       Func<T, MlResult<TReturn>> funcFalse)
        => source.BindIf(condition, funcTrue, funcFalse).ToAsync();

    public static async Task<MlResult<TReturn>> BindIfAsync<T, TReturn>(this MlResult<T>                      source,
                                                                             Func<T, bool>                    condition,
                                                                             Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                             Func<T,      MlResult<TReturn>>  funcFalse)
    {
        var result = await source.MatchAsync( 
                                               validAsync: async x => condition(x) 
                                                                    ? await funcTrueAsync(x) 
                                                                    : await funcFalse(x).ToAsync(),
                                               fail      : MlResult<TReturn>.Fail
                                            );
        return result;
    }

    public static async Task<MlResult<TReturn>> BindIfAsync<T, TReturn>(this MlResult<T>                      source,
                                                                             Func<T, bool>                    condition,
                                                                             Func<T,      MlResult<TReturn>>  funcTrue,
                                                                             Func<T, Task<MlResult<TReturn>>> funcFalseAsync)
    {
        var result = await source.MatchAsync( 
                                               validAsync: async x => condition(x) 
                                                                    ? await funcTrue(x).ToAsync()
                                                                    : await funcFalseAsync(x),
                                               fail      : MlResult<TReturn>.Fail
                                            );
        return result;
    }

    public static async Task<MlResult<TReturn>> BindIfAsync<T, TReturn>(this MlResult<T>                      source,
                                                                             Func<T, bool>                    condition,
                                                                             Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                             Func<T, Task<MlResult<TReturn>>> funcFalseAsync)
    {
        var result = await source.MatchAsync( 
                                               validAsync: async x => condition(x) 
                                                                    ? await funcTrueAsync(x)
                                                                    : await funcFalseAsync(x),
                                               fail      : MlResult<TReturn>.Fail
                                            );
        return result;
    }

    public static async Task<MlResult<TReturn>> BindIfAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                             Func<T, bool>                    condition,
                                                                             Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                             Func<T,      MlResult<TReturn>>  funcFalse)
        => await (await sourceAsync).BindIfAsync(condition, funcTrueAsync, funcFalse);

    public static async Task<MlResult<TReturn>> BindIfAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                             Func<T, bool>                    condition,
                                                                             Func<T,      MlResult<TReturn>>  funcTrue,
                                                                             Func<T, Task<MlResult<TReturn>>> funcFalseAsync)
        => await (await sourceAsync).BindIfAsync(condition, funcTrue, funcFalseAsync);

    public static async Task<MlResult<TReturn>> BindIfAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                             Func<T, bool>                    condition,
                                                                             Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                             Func<T, Task<MlResult<TReturn>>> funcFalseAsync)
        => await (await sourceAsync).BindIfAsync(condition, funcTrueAsync, funcFalseAsync);



    public static MlResult<T> BindIf<T>(this MlResult<T>          source,
                                             Func<T, bool>        condition,
                                             Func<T, MlResult<T>> func)
    {
        var result = source.Match(
                                       valid: x => condition(x) 
                                                        ? func(x) 
                                                        : x,
                                       fail :      MlResult<T>.Fail
                                 );
        return result;
    }

    public static Task<MlResult<T>> BindIfAsync<T>(this MlResult<T>          source,
                                                        Func<T, bool>        condition,
                                                        Func<T, MlResult<T>> func)
        => source.BindIf(condition, func).ToAsync();

    public static async Task<MlResult<T>> BindIfAsync<T>(this MlResult<T>                source,
                                                              Func<T, bool>              condition,
                                                              Func<T, Task<MlResult<T>>> funcAsync)
    {
        var result = await source.MatchAsync(
                                                   validAsync: async x => condition(x) 
                                                                            ? await funcAsync(x) 
                                                                            : x,
                                                   fail       : MlResult<T>.Fail
                                             );
        return result;
    }

    public static async Task<MlResult<T>> BindIfAsync<T>(this Task<MlResult<T>>          sourceAsync,
                                                              Func<T, bool>              condition,
                                                              Func<T, Task<MlResult<T>>> funcAsync)
        => await (await sourceAsync).BindIfAsync(condition, funcAsync);

    public static async Task<MlResult<T>> BindIfAsync<T>(this Task<MlResult<T>>    sourceAsync,
                                                              Func<T, bool>        condition,
                                                              Func<T, MlResult<T>> func)
        => (await sourceAsync).BindIf(condition, func);



    public static MlResult<TReturn> TryBindIf<T, TReturn>(this MlResult<T>                source,
                                                               Func<T, bool>              condition,
                                                               Func<T, MlResult<TReturn>> funcTrue,
                                                               Func<T, MlResult<TReturn>> funcFalse,
                                                               Func<Exception, string>    errorMessageBuilder)
    {
        var result = source.Match(
                                       valid: x => condition(x) 
                                                        ? funcTrue .TryToMlResult(x, errorMessageBuilder)
                                                        : funcFalse.TryToMlResult(x, errorMessageBuilder),
                                       fail :      MlResult<TReturn>.Fail
                                 );
        return result;
    }

    public static Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this MlResult<T>                source,
                                                                          Func<T, bool>              condition,
                                                                          Func<T, MlResult<TReturn>> funcTrue,
                                                                          Func<T, MlResult<TReturn>> funcFalse,
                                                                          Func<Exception, string>    errorMessageBuilder)
        => source.TryBindIf(condition, funcTrue, funcFalse, errorMessageBuilder).ToAsync();

    public static Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this MlResult<T>                source,
                                                                          Func<T, bool>              condition,
                                                                          Func<T, MlResult<TReturn>> funcTrue,
                                                                          Func<T, MlResult<TReturn>> funcFalse,
                                                                          string                     exceptionAditionalMessage = null!)
        => source.TryBindIf(condition, funcTrue, funcFalse, _ => exceptionAditionalMessage!).ToAsync();

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this MlResult<T>                      source,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                                Func<T,      MlResult<TReturn>>  funcFalse,
                                                                                Func<Exception, string>          errorMessageBuilder)
    {
        var result = await source.MatchAsync(
                                                  validAsync: async x => condition(x) 
                                                                   ? await funcTrueAsync .TryToMlResultAsync(x, errorMessageBuilder)
                                                                   : await funcFalse     .TryToMlResult     (x, errorMessageBuilder).ToAsync(),
                                                  fail : MlResult<TReturn>.Fail
                                            );
        return result;
    }

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this MlResult<T>                      source,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                                Func<T,      MlResult<TReturn>>  funcFalse,
                                                                                string                           exceptionAditionalMessage = null!)
        => await source.TryBindIfAsync(condition, funcTrueAsync, funcFalse, _ => exceptionAditionalMessage!);

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this MlResult<T>                      source,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T,      MlResult<TReturn>>  funcTrue,
                                                                                Func<T, Task<MlResult<TReturn>>> funcFalseAsync,
                                                                                Func<Exception, string>          errorMessageBuilder)
    {
        var result = await source.MatchAsync(
                                                  validAsync: async x => condition(x) 
                                                                   ? await funcTrue      .TryToMlResult     (x, errorMessageBuilder).ToAsync()
                                                                   : await funcFalseAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                                  fail : MlResult<TReturn>.Fail
                                            );
        return result;
    }

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this MlResult<T>                      source,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T,      MlResult<TReturn>>  funcTrue,
                                                                                Func<T, Task<MlResult<TReturn>>> funcFalseAsync,
                                                                                string                           exceptionAditionalMessage = null!)
        => await source.TryBindIfAsync(condition, funcTrue, funcFalseAsync, _ => exceptionAditionalMessage!);

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this MlResult<T>                      source,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                                Func<T, Task<MlResult<TReturn>>> funcFalseAsync,
                                                                                Func<Exception, string>          errorMessageBuilder)
    {
        var result = await source.MatchAsync(
                                                  validAsync: async x => condition(x) 
                                                                   ? await funcTrueAsync .TryToMlResultAsync(x, errorMessageBuilder)
                                                                   : await funcFalseAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                                  fail : MlResult<TReturn>.Fail
                                            );
        return result;
    }

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this MlResult<T>                      source,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                                Func<T, Task<MlResult<TReturn>>> funcFalseAsync,
                                                                                string                           exceptionAditionalMessage = null!)
        => await source.TryBindIfAsync(condition, funcTrueAsync, funcFalseAsync, _ => exceptionAditionalMessage!);

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this Task<MlResult<T>>          sourceAsync,
                                                                                Func<T, bool>              condition,
                                                                                Func<T, MlResult<TReturn>> funcTrue,
                                                                                Func<T, MlResult<TReturn>> funcFalse,
                                                                                Func<Exception, string>    errorMessageBuilder)
        => await (await sourceAsync).TryBindIfAsync(condition, funcTrue, funcFalse, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this Task<MlResult<T>>          sourceAsync,
                                                                                Func<T, bool>              condition,
                                                                                Func<T, MlResult<TReturn>> funcTrue,
                                                                                Func<T, MlResult<TReturn>> funcFalse,
                                                                                string                     exceptionAditionalMessage = null!)
        => await (await sourceAsync).TryBindIfAsync(condition, funcTrue, funcFalse, _ => exceptionAditionalMessage!);


    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                                Func<T,      MlResult<TReturn>>  funcFalse,
                                                                                Func<Exception, string>          errorMessageBuilder)
        => await (await sourceAsync).TryBindIfAsync(condition, funcTrueAsync, funcFalse, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                                Func<T,      MlResult<TReturn>>  funcFalse,
                                                                                string                           exceptionAditionalMessage = null!)
        => await (await sourceAsync).TryBindIfAsync(condition, funcTrueAsync, funcFalse, _ => exceptionAditionalMessage!);

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T,      MlResult<TReturn>>  funcTrue,
                                                                                Func<T, Task<MlResult<TReturn>>> funcFalseAsync,
                                                                                Func<Exception, string>          errorMessageBuilder)
        => await (await sourceAsync).TryBindIfAsync(condition, funcTrue, funcFalseAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T,      MlResult<TReturn>>  funcTrue,
                                                                                Func<T, Task<MlResult<TReturn>>> funcFalseAsync,
                                                                                string                           exceptionAditionalMessage = null!)
        => await (await sourceAsync).TryBindIfAsync(condition, funcTrue, funcFalseAsync, _ => exceptionAditionalMessage!);


    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                                Func<T, Task<MlResult<TReturn>>> funcFalseAsync,
                                                                                Func<Exception, string>          errorMessageBuilder)
        => await (await sourceAsync).TryBindIfAsync(condition, funcTrueAsync, funcFalseAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfAsync<T, TReturn>(this Task<MlResult<T>>                sourceAsync,
                                                                                Func<T, bool>                    condition,
                                                                                Func<T, Task<MlResult<TReturn>>> funcTrueAsync,
                                                                                Func<T, Task<MlResult<TReturn>>> funcFalseAsync,
                                                                                string                           exceptionAditionalMessage = null!)
        => await (await sourceAsync).TryBindIfAsync(condition, funcTrueAsync, funcFalseAsync, _ => exceptionAditionalMessage!);



    public static MlResult<T> TryBindIf<T>(this MlResult<T>             source,
                                                Func<T, bool>           condition,
                                                Func<T, MlResult<T>>    func,
                                                Func<Exception, string> errorMessageBuilder)
         => source.Match
                        (
                            valid: x => condition(x)
                                            ? func.TryToMlResult(x, errorMessageBuilder)
                                            : x.ToMlResultValid(),
                            fail: MlResult<T>.Fail
                        );

    public static MlResult<T> TryBindIf<T>(this MlResult<T>             source,
                                                Func<T, bool>           condition,
                                                Func<T, MlResult<T>>    func,
                                                string                  errorMessage = null!)
        => source.TryBindIf(condition, func, _ => errorMessage!);

    public static Task<MlResult<T>> TryBindIfAsync<T>(this MlResult<T>             source,
                                                           Func<T, bool>           condition,
                                                           Func<T, MlResult<T>>    func,
                                                           Func<Exception, string> errorMessageBuilder)
        => source.TryBindIf(condition, func, errorMessageBuilder).ToAsync();

    public static Task<MlResult<T>> TryBindIfAsync<T>(this MlResult<T>             source,
                                                           Func<T, bool>           condition,
                                                           Func<T, MlResult<T>>    func,
                                                           string                  errorMessage = null!)
        => source.TryBindIf(condition, func, _ => errorMessage!).ToAsync();

    public static async Task<MlResult<T>> TryBindIfAsync<T>(this MlResult<T>                source,
                                                                 Func<T, bool>              condition,
                                                                 Func<T, Task<MlResult<T>>> funcAsync,
                                                                 Func<Exception, string>    errorMessageBuilder)
         => await source.MatchAsync(
                                        validAsync: async x => condition(x)
                                                                    ? await funcAsync.TryToMlResultAsync(x, errorMessageBuilder)
                                                                    : await x.ToMlResultValidAsync(),
                                        fail       : MlResult<T>.Fail
                                    );

    public static async Task<MlResult<T>> TryBindIfAsync<T>(this MlResult<T>                source,
                                                                 Func<T, bool>              condition,
                                                                 Func<T, Task<MlResult<T>>> funcAsync,
                                                                 string                     errorMessage = null!)
        => await source.TryBindIfAsync(condition, funcAsync, _ => errorMessage!);

    public static async Task<MlResult<T>> TryBindIfAsync<T>(this Task<MlResult<T>>          sourceAsync,
                                                                 Func<T, bool>              condition,
                                                                 Func<T, Task<MlResult<T>>> funcAsync,
                                                                 Func<Exception, string>    errorMessageBuilder)
        => await (await sourceAsync).TryBindIfAsync(condition, funcAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> TryBindIfAsync<T>(this Task<MlResult<T>>          sourceAsync,
                                                                 Func<T, bool>              condition,
                                                                 Func<T, Task<MlResult<T>>> funcAsync,
                                                                 string                     errorMessage = null!)
        => await (await sourceAsync).TryBindIfAsync(condition, funcAsync, _ => errorMessage!);


    public static async Task<MlResult<T>> TryBindIfAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                 Func<T, bool>           condition,
                                                                 Func<T, MlResult<T>>    func,
                                                                 Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryBindIf(condition, func, errorMessageBuilder);

    public static async Task<MlResult<T>> TryBindIfAsync<T>(this Task<MlResult<T>>    sourceAsync,
                                                                 Func<T, bool>        condition,
                                                                 Func<T, MlResult<T>> func,
                                                                 string               errorMessage = null!)
        => (await sourceAsync).TryBindIf(condition, func, _ => errorMessage!);


    #endregion


    #region BindIfFail





    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static MlResult<T> BindIfFail<T>(this MlResult<T>                        source, 
                                                 Func<MlErrorsDetails, MlResult<T>> func)
        => source.Match
                        (
                            fail : func,
                            valid: value => value
                        );


    public static async Task<MlResult<T>> BindIfFailAsync<T>(this MlResult<T>                              source, 
                                                                  Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync)
        => await source.MatchAsync
                        (
                            failAsync : funcAsync,
                            validAsync: value => value.ToMlResultValidAsync()
                        );

    public static async Task<MlResult<T>> BindIfFailAsync<T>(this Task<MlResult<T>>                        sourceAsync, 
                                                                  Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync)
        => await (await sourceAsync).BindIfFailAsync(funcAsync);

    public static async Task<MlResult<T>> BindIfFailAsync<T>(this Task<MlResult<T>>                  sourceAsync, 
                                                                  Func<MlErrorsDetails, MlResult<T>> func)
        => await (await sourceAsync).BindIfFailAsync(func.ToFuncTask());


    public static MlResult<T> TryBindIfFail<T>(this MlResult<T>                        source, 
                                                    Func<MlErrorsDetails, MlResult<T>> func,
                                                    Func<Exception, string>            errorMessageBuilder)
        => source.Match
                        (
                            fail : errorDetails => func.TryToMlResult(errorDetails, errorMessageBuilder),
                            valid: value => value
                        );


    public static MlResult<T> TryBindIfFail<T>(this MlResult<T>                        source, 
                                                    Func<MlErrorsDetails, MlResult<T>> func,
                                                    string                             errorMessage = null!)
        => source.TryBindIfFail(func, _ => errorMessage!);


    public static async Task<MlResult<T>> TryBindIfFailAsync<T>(this MlResult<T>                              source, 
                                                                     Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync,
                                                                     Func<Exception, string>                  errorMessageBuilder)
        => await source.MatchAsync
                        (
                            failAsync : errorDetails => funcAsync.TryToMlResultAsync(errorDetails, errorMessageBuilder),
                            validAsync: value        => value.ToMlResultValidAsync()
                        );

    public static async Task<MlResult<T>> TryBindIfFailAsync<T>(this MlResult<T>                              source, 
                                                                     Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync,
                                                                     string                                   errorMessage = null!)
        => await source.TryBindIfFailAsync(funcAsync, _ => errorMessage!);



    public static async Task<MlResult<T>> TryBindIfFailAsync<T>(this Task<MlResult<T>>                        sourceAsync, 
                                                                     Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync,
                                                                     Func<Exception, string>                  errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailAsync(funcAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> TryBindIfFailAsync<T>(this Task<MlResult<T>>                        sourceAsync, 
                                                                     Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync,
                                                                     string                                   errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailAsync(funcAsync, errorMessage);

    public static async Task<MlResult<T>> TryBindIfFailAsync<T>(this Task<MlResult<T>>                  sourceAsync, 
                                                                     Func<MlErrorsDetails, MlResult<T>> func,
                                                                     Func<Exception, string>            errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailAsync(func.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<T>> TryBindIfFailAsync<T>(this Task<MlResult<T>>                  sourceAsync, 
                                                                     Func<MlErrorsDetails, MlResult<T>> func,
                                                                     string                             errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailAsync(func.ToFuncTask(), errorMessage);




    public static MlResult<TReturn> BindIfFail<T, TReturn>(this MlResult<T>                              source,
                                                                Func<T              , MlResult<TReturn>> funcValid,
                                                                Func<MlErrorsDetails, MlResult<TReturn>> funcFail)
        => source.Match
                        (
                            fail: funcFail,
                            valid: funcValid
                        );

    public static async Task<MlResult<TReturn>> BindIfFailAsync<T, TReturn>(this MlResult<T>                                    source,
                                                                                 Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                 Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync)
        => await source.MatchAsync
                        (
                            failAsync: funcFailAsync,
                            validAsync: funcValidAsync
                        );

    public static async Task<MlResult<TReturn>> BindIfFailAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                 Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                 Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync)
        => await (await sourceAsync).BindIfFailAsync(funcValidAsync, funcFailAsync);



    public static MlResult<TReturn> TryBindIfFail<T, TReturn>(this MlResult<T>                              source,
                                                                   Func<T              , MlResult<TReturn>> funcValid,
                                                                   Func<MlErrorsDetails, MlResult<TReturn>> funcFail,
                                                                   Func<Exception, string>                  errorMessageBuilder)
        => source.Match(
                            fail: errorsDetails => funcFail.TryToMlResult(errorsDetails, errorMessageBuilder),
                            valid: x => funcValid.TryToMlResult(x, errorMessageBuilder)
                        );

    public static MlResult<TReturn> TryBindIfFail<T, TReturn>(this MlResult<T>                              source,
                                                                   Func<T              , MlResult<TReturn>> funcValid,
                                                                   Func<MlErrorsDetails, MlResult<TReturn>> funcFail,
                                                                   string                                   errorMessage = null!)
        => source.TryBindIfFail(funcValid, funcFail, _ => errorMessage!);

    public static async Task<MlResult<TReturn>> TryBindIfFailAsync<T, TReturn>(this MlResult<T>                                    source,
                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                    Func<Exception, string>                        errorMessageBuilder)
        => await source.MatchAsync(
                            failAsync: errorsDetails => funcFailAsync.TryToMlResultAsync(errorsDetails, errorMessageBuilder),
                            validAsync: x => funcValidAsync.TryToMlResultAsync(x, errorMessageBuilder)
                        );

    public static async Task<MlResult<TReturn>> TryBindIfFailAsync<T, TReturn>(this MlResult<T>                                    source,
                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsyc,
                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                    string                                         errorMessage = null!)
        => await source.TryBindIfFailAsync(funcValidAsyc, funcFailAsync, _ => errorMessage!);

    public static async Task<MlResult<TReturn>> TryBindIfFailAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                    Func<Exception, string>                        errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailAsync(funcValidAsync, funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                    string                                         errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailAsync(funcValidAsync, funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                    Func<T              , MlResult<TReturn>>       funcValid,
                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                    Func<Exception, string>                        errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                    Func<T              , MlResult<TReturn>>       funcValid,
                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                    string                                         errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                    Func<MlErrorsDetails, MlResult<TReturn>>       funcFail,
                                                                                    Func<Exception, string>                        errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                    Func<MlErrorsDetails, MlResult<TReturn>>       funcFail,
                                                                                    string                                         errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                    Func<T              , MlResult<TReturn>> funcValid,
                                                                                    Func<MlErrorsDetails, MlResult<TReturn>> funcFail,
                                                                                    Func<Exception, string>                  errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailAsync(funcValid.ToFuncTask(), funcFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                    Func<T              , MlResult<TReturn>> funcValid,
                                                                                    Func<MlErrorsDetails, MlResult<TReturn>> funcFail,
                                                                                    string                                   errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailAsync(funcValid.ToFuncTask(), funcFail.ToFuncTask(), errorMessage);

    #endregion


    #region BindIfFailWithValue




    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// source parameter has a prevous 'Value' execution
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="funcValue"></param>
    /// <returns></returns>
    public static MlResult<T> BindIfFailWithValue<T>(this MlResult<T> source,
                                                          Func<T, MlResult<T>> funcValue)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetDetailValue<T>().Bind(funcValue),
                            valid: value         => value
                        );



    public static async Task<MlResult<T>> BindIfFailWithValueAsync<T>(this MlResult<T>                source,
                                                                           Func<T, Task<MlResult<T>>> funcValueAsync)
        => await source.MatchAsync
                        (
                            failAsync : errorsDetails => errorsDetails.GetDetailValue<T>().BindAsync(funcValueAsync),
                            validAsync: value         => value.ToMlResultValidAsync()
                        );


    public static async Task<MlResult<T>> BindIfFailWithValueAsync<T>(this Task<MlResult<T>>          sourceAsync,
                                                                           Func<T, Task<MlResult<T>>> funcValueAsync)
        => await (await sourceAsync).BindIfFailWithValueAsync(funcValueAsync);

    public static async Task<MlResult<T>> BindIfFailWithValueAsync<T>(this Task<MlResult<T>>    sourceAsync,
                                                                           Func<T, MlResult<T>> funcValue)
        => await (await sourceAsync).BindIfFailWithValueAsync(funcValue.ToFuncTask());



    public static MlResult<T> TryBindIfFailWithValue<T>(this MlResult<T>             source,
                                                             Func<T, MlResult<T>>    funcValue,
                                                             Func<Exception, string> errorMessageBuilder)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetDetailValue<T>().Bind(x => funcValue.TryToMlResult(x, errorMessageBuilder)),
                            valid: value         => value
                        );

    public static MlResult<T> TryBindIfFailWithValue<T>(this MlResult<T>          source,
                                                             Func<T, MlResult<T>> funcValue,
                                                             string               errorMessage = null!)
        => source.TryBindIfFailWithValue(funcValue, _ => errorMessage!);

    public static async Task<MlResult<T>> TryBindIfFailWithValueAsync<T>(this MlResult<T>                source,
                                                                              Func<T, Task<MlResult<T>>> funcValueAsync,
                                                                              Func<Exception, string>    errorMessageBuilder)
        => await source.MatchAsync
                        (
                            failAsync : errorsDetails => errorsDetails.GetDetailValue<T>().BindAsync(x => funcValueAsync.TryToMlResultAsync(x, errorMessageBuilder)),
                            validAsync: value         => value.ToMlResultValidAsync()
                        );

    public static async Task<MlResult<T>> TryBindIfFailWithValueAsync<T>(this MlResult<T>                source,
                                                                              Func<T, Task<MlResult<T>>> funcValueAsync,
                                                                              string                     errorMessage = null!)
        => await source.TryBindIfFailWithValueAsync(funcValueAsync, _ => errorMessage!);


    public static async Task<MlResult<T>> TryBindIfFailWithValueAsync<T>(this Task<MlResult<T>>          sourceAsync,
                                                                              Func<T, Task<MlResult<T>>> funcValueAsync,
                                                                              Func<Exception, string>    errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithValueAsync(funcValueAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> TryBindIfFailWithValueAsync<T>(this Task<MlResult<T>>          sourceAsync,
                                                                              Func<T, Task<MlResult<T>>> funcValueAsync,
                                                                              string                     errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithValueAsync(funcValueAsync, errorMessage);

    public static async Task<MlResult<T>> TryBindIfFailWithValueAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                              Func<T, MlResult<T>>    funcValue,
                                                                              Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithValueAsync(funcValue.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<T>> TryBindIfFailWithValueAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                              Func<T, MlResult<T>>    funcValue,
                                                                              string                  errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithValueAsync(funcValue.ToFuncTask(), errorMessage);


    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// source parameter has a prevous 'Value' execution
    /// -- 2 funcs (valid and fail) are necessary, since they have to preserve output compatibility --
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    /// <param name="source"></param>
    /// <param name="funcValid"></param>
    /// <param name="funcFail"></param>
    /// <returns></returns>
    public static MlResult<TReturn> BindIfFailWithValue<T, TValue, TReturn>(this MlResult<T>                     source,
                                                                                 Func<T     , MlResult<TReturn>> funcValid,
                                                                                 Func<TValue, MlResult<TReturn>> funcFail)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetDetailValue<TValue>().Bind(value => funcFail(value)),
                            valid: value         => funcValid(value)
                        );


    public static async Task<MlResult<TReturn>> BindIfFailWithValueAsync<T, TValue, TReturn>(this MlResult<T>                           source,
                                                                                                  Func<T     , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                  Func<TValue, Task<MlResult<TReturn>>> funcFailAsync)
        => await source.MatchAsync
                        (
                            failAsync : errorsDetails => errorsDetails.GetDetailValueAsync<TValue>().BindAsync(value => funcFailAsync(value)),
                            validAsync: value         => funcValidAsync(value)
                        );


    public static async Task<MlResult<TReturn>> BindIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                  Func<T     , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                  Func<TValue, Task<MlResult<TReturn>>> funcFailAsync)
        => await (await sourceAsync).BindIfFailWithValueAsync(funcValidAsync, funcFailAsync);



    public static MlResult<TReturn> TryBindIfFailWithValue<T, TValue, TReturn>(this MlResult<T>                     source,
                                                                                    Func<T     , MlResult<TReturn>> funcValid,
                                                                                    Func<TValue, MlResult<TReturn>> funcFail,
                                                                                    Func<Exception, string>         errorMessageBuilder)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetDetailValue<TValue>().Bind(value => funcFail.TryToMlResult(value, errorMessageBuilder)),
                            valid: value         => funcValid.TryToMlResult(source.Value, errorMessageBuilder)
                        );

    public static MlResult<TReturn> TryBindIfFailWithValue<T, TValue, TReturn>(this MlResult<T>                     source,
                                                                                    Func<T     , MlResult<TReturn>> funcValid,
                                                                                    Func<TValue, MlResult<TReturn>> funcFail,
                                                                                    string                          errorMessage = null!)
        => source.TryBindIfFailWithValue(funcValid, funcFail, _ => errorMessage!);


    public static async Task<MlResult<TReturn>> TryBindIfFailWithValueAsync<T, TValue, TReturn>(this MlResult<T>                           source,
                                                                                                     Func<T     , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                     Func<TValue, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                     Func<Exception, string>               errorMessageBuilder)
        => await source.MatchAsync
                                 (
                                     failAsync : errorsDetails => errorsDetails.GetDetailValueAsync<TValue>().BindAsync(value => funcFailAsync.TryToMlResultAsync(value, errorMessageBuilder)),
                                     validAsync: value         => funcValidAsync.TryToMlResultAsync(source.Value, errorMessageBuilder)
                                 );


    public static async Task<MlResult<TReturn>> TryBindIfFailWithValueAsync<T, TValue, TReturn>(this MlResult<T>                           source,
                                                                                                     Func<T     , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                     Func<TValue, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                     string                                errorMessage = null!)
        => await source.TryBindIfFailWithValueAsync(funcValidAsync, funcFailAsync, _ => errorMessage!);


    public static async Task<MlResult<TReturn>> TryBindIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     Func<T     , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                     Func<TValue, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                     Func<Exception, string>               errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithValueAsync(funcValidAsync, funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     Func<T     , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                     Func<TValue, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                     string                                errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithValueAsync(funcValidAsync, funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     Func<T     , MlResult<TReturn>>       funcValid,
                                                                                                     Func<TValue, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                     Func<Exception, string>               errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithValueAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     Func<T     , MlResult<TReturn>>       funcValid,
                                                                                                     Func<TValue, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                     string                                errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithValueAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     Func<T     , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                     Func<TValue, MlResult<TReturn>>       funcFail,
                                                                                                     Func<Exception, string>               errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithValueAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>                     sourceAsync,
                                                                                                     Func<T     , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                     Func<TValue, MlResult<TReturn>>       funcFail,
                                                                                                     string                                errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithValueAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>               sourceAsync,
                                                                                                     Func<T     , MlResult<TReturn>> funcValid,
                                                                                                     Func<TValue, MlResult<TReturn>> funcFail,
                                                                                                     Func<Exception, string>         errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithValueAsync(funcValid.ToFuncTask(), funcFail.ToFuncTask(), errorMessageBuilder);






    #endregion


    #region BindIfFailWithException


    /// En el caso de BindIfFailWithException es diferente al BindIfFailWithValue. 
    /// 
    ///     1.- BindIfFailWithValue: Si recibe un MlResult Fail sin ValueDetail, añadira´un nuevo Error al que le viene de la ejecución anterior
    ///     
    ///     2.- BindIfFailWithException: Si recibe un MlResult Fail sin ExceptionDetail, Devolvera el MlResult Fail, igual que le vino


    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// source parameter has a prevous Exception execution or 'ex' ErrorDetail
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static MlResult<T> BindIfFailWithException<T>(this MlResult<T>                  source,
                                                              Func<Exception, MlResult<T>> funcException)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(),
                                                                                                valid: funcException
                                                                                            ),
                            valid: value         => value
                        );

    public static async Task<MlResult<T>> BindIfFailWithExceptionAsync<T>(this MlResult<T>                        source,
                                                                               Func<Exception, Task<MlResult<T>>> funcExceptionAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailException().MatchAsync(
                                                                                                                        failAsync : exErrorsDetails => exErrorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                        validAsync: funcExceptionAsync
                                                                                                                    ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );


    public static async Task<MlResult<T>> BindIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                  sourceAsync,
                                                                               Func<Exception, Task<MlResult<T>>> funcExceptionAsync)
        => await (await sourceAsync).BindIfFailWithExceptionAsync(funcExceptionAsync);

    public static async Task<MlResult<T>> BindIfFailWithExceptionAsync<T>(this Task<MlResult<T>>            sourceAsync,
                                                                               Func<Exception, MlResult<T>> funcException)
        => await (await sourceAsync).BindIfFailWithExceptionAsync(funcException.ToFuncTask());

    public static MlResult<T> TryBindIfFailWithException<T>(this MlResult<T>                  source,
                                                                 Func<Exception, MlResult<T>> funcException,
                                                                 Func<Exception, string>      errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : _  => errorsDetails.ToMlResultFail<T>(),
                                                                                                valid: ex => funcException.TryToMlResult(ex, errorMessageBuilder)
                                                                                                                            .MergeErrorsDetailsIfFail(source)
                                                                                            ),
                            valid: value         => value
                        );

    public static MlResult<T> TryBindIfFailWithException<T>(this MlResult<T>                  source,
                                                                 Func<Exception, MlResult<T>> funcException,
                                                                 string                       errorMessage = null!)
        => source.TryBindIfFailWithException(funcException, _ => errorMessage!);


    public static async Task<MlResult<T>> TryBindIfFailWithExceptionAsync<T>(this MlResult<T>                        source,
                                                                                  Func<Exception, Task<MlResult<T>>> funcExceptionAsync,
                                                                                  Func<Exception, string>            errorMessageBuilder)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                                            failAsync : _  => errorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                            validAsync: ex => funcExceptionAsync.TryToMlResultAsync(ex, errorMessageBuilder)
                                                                                                                                                                .MergeErrorsDetailsIfFailAsync(source)
                                                                                                                        ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );

    public static async Task<MlResult<T>> TryBindIfFailWithExceptionAsync<T>(this MlResult<T>                        source,
                                                                                  Func<Exception, Task<MlResult<T>>> funcExceptionAsync,
                                                                                  string                             errorMessage = null!)
        => await source.TryBindIfFailWithExceptionAsync(funcExceptionAsync, _ => errorMessage!);


    public static async Task<MlResult<T>> TryBindIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                  sourceAsync,
                                                                                  Func<Exception, Task<MlResult<T>>> funcExceptionAsync,
                                                                                  Func<Exception, string>            errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithExceptionAsync(funcExceptionAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> TryBindIfFailWithExceptionAsync<T>(this Task<MlResult<T>>                  sourceAsync,
                                                                                  Func<Exception, Task<MlResult<T>>> funcExceptionAsync,
                                                                                  string                             errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithExceptionAsync(funcExceptionAsync, errorMessage);

    public static async Task<MlResult<T>> TryBindIfFailWithExceptionAsync<T>(this Task<MlResult<T>>            sourceAsync,
                                                                                  Func<Exception, MlResult<T>> funcException,
                                                                                  Func<Exception, string>      errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithExceptionAsync(funcException.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<T>> TryBindIfFailWithExceptionAsync<T>(this Task<MlResult<T>>            sourceAsync,
                                                                                  Func<Exception, MlResult<T>> funcException,
                                                                                  string                       errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithExceptionAsync(funcException.ToFuncTask(), errorMessage);






    public static MlResult<TReturn> BindIfFailWithException<T, TReturn>(this MlResult<T>                        source,
                                                                             Func<T        , MlResult<TReturn>> funcValid,
                                                                             Func<Exception, MlResult<TReturn>> funcFail)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<TReturn>(),
                                                                                                valid: funcFail
                                                                                            ),
                            valid: funcValid
                        );

    public static async Task<MlResult<TReturn>> BindIfFailWithExceptionAsync<T, TReturn>(this MlResult<T>                              source,
                                                                                              Func<T        , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                              Func<Exception, Task<MlResult<TReturn>>> funcFailAsync)
        => await source.MatchAsync(
                            failAsync : errorsDetails => errorsDetails.GetDetailException().MatchAsync(
                                                                                                failAsync : exErrorsDetails => exErrorsDetails.ToMlResultFailAsync<TReturn>(),
                                                                                                validAsync: funcFailAsync
                                                                                            ),
                            validAsync: funcValidAsync
                        );

    public static async Task<MlResult<TReturn>> BindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                              Func<T        , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                              Func<Exception, Task<MlResult<TReturn>>> funcFailAsync)
        => await (await sourceAsync).BindIfFailWithExceptionAsync(funcValidAsync, funcFailAsync);

    public static async Task<MlResult<TReturn>> BindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                              Func<T        , MlResult<TReturn>>       funcValid,
                                                                                              Func<Exception, Task<MlResult<TReturn>>> funcFailAsync)
        => await (await sourceAsync).BindIfFailWithExceptionAsync(funcValid.ToFuncTask(), funcFailAsync);

    public static async Task<MlResult<TReturn>> BindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                              Func<T        , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                              Func<Exception, MlResult<TReturn>>       funcFail)
        => await (await sourceAsync).BindIfFailWithExceptionAsync(funcValidAsync, funcFail.ToFuncTask());

    public static async Task<MlResult<TReturn>> BindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                  sourceAsync,
                                                                                              Func<T        , MlResult<TReturn>> funcValid,
                                                                                              Func<Exception, MlResult<TReturn>> funcFail)
        => await (await sourceAsync).BindIfFailWithExceptionAsync(funcValid.ToFuncTask(), funcFail.ToFuncTask());

    public static MlResult<TReturn> TryBindIfFailWithException<T, TReturn>(this MlResult<T>                        source,
                                                                                Func<T        , MlResult<TReturn>> funcValid,
                                                                                Func<Exception, MlResult<TReturn>> funcFail,
                                                                                Func<Exception, string>            errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<TReturn>(),
                                                                                                valid: ex              => funcFail.TryToMlResult(ex, errorMessageBuilder)
                                                                                                                                  .MergeErrorsDetailsIfFailDiferentTypes(source)
                                                                                            ),
                            valid: x => funcValid.TryToMlResult(x, errorMessageBuilder)
                        );

    public static MlResult<TReturn> TryBindIfFailWithException<T, TReturn>(this MlResult<T>                        source,
                                                                                Func<T        , MlResult<TReturn>> funcValid,
                                                                                Func<Exception, MlResult<TReturn>> funcFail,
                                                                                string                             errorMessage = null!)
        => source.TryBindIfFailWithException(funcValid, funcFail, _ => errorMessage!);


    public static async Task<MlResult<TReturn>> TryBindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                  sourceAsync,
                                                                                                 Func<T        , MlResult<TReturn>> funcValid,
                                                                                                 Func<Exception, MlResult<TReturn>> funcFail,
                                                                                                 Func<Exception, string>            errorMessageBuilder)
        => (await sourceAsync).TryBindIfFailWithException(funcValid, funcFail, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                  sourceAsync,
                                                                                                 Func<T        , MlResult<TReturn>> funcValid,
                                                                                                 Func<Exception, MlResult<TReturn>> funcFail,
                                                                                                 string                             errorMessage = null!)
        => await sourceAsync.TryBindIfFailWithExceptionAsync(funcValid, funcFail, _ => errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 Func<T        , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                 Func<Exception, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                 Func<Exception, string>                  errorMessageBuilder)
        => await sourceAsync.MatchAsync(
                            failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                failAsync : exErrorsDetails => exErrorsDetails.ToMlResultFailAsync<TReturn>(),
                                                                                                validAsync: ex              => funcFailAsync.TryToMlResultAsync(ex, errorMessageBuilder)
                                                                                                                                             .MergeErrorsDetailsIfFailDiferentTypesAsync(sourceAsync)
                                                                                            ),
                            validAsync: x => funcValidAsync.TryToMlResultAsync(x, errorMessageBuilder)
                        );

    public static async Task<MlResult<TReturn>> TryBindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 Func<T        , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                 Func<Exception, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                 string                                   errorMessage = null!)
        => await sourceAsync.TryBindIfFailWithExceptionAsync(funcValidAsync, funcFailAsync, _ => errorMessage!);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 Func<T        , MlResult<TReturn>>       funcValid,
                                                                                                 Func<Exception, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                 Func<Exception, string>                  errorMessageBuilder)
        => await sourceAsync.TryBindIfFailWithExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 Func<T        , MlResult<TReturn>>       funcValid,
                                                                                                 Func<Exception, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                 string                                   errorMessage = null!)
        => await sourceAsync.TryBindIfFailWithExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 Func<T        , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                 Func<Exception, MlResult<TReturn>>       funcFail,
                                                                                                 Func<Exception, string>                  errorMessageBuilder)
        => await sourceAsync.TryBindIfFailWithExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 Func<T        , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                 Func<Exception, MlResult<TReturn>>       funcFail,
                                                                                                 string                                   errorMessage = null!)
        => await sourceAsync.TryBindIfFailWithExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    //public static async Task<MlResult<TReturn>> TryBindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                  sourceAsync,
    //                                                                                             Func<T        , MlResult<TReturn>> funcValidAsync,
    //                                                                                             Func<Exception, MlResult<TReturn>> funcFail,
    //                                                                                             Func<Exception, string>            errorMessageBuilder)
    //    => await sourceAsync.TryBindIfFailWithExceptionAsync(funcValidAsync.ToFuncTask(), funcFail.ToFuncTask(), errorMessageBuilder);

    //public static async Task<MlResult<TReturn>> TryBindIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>                  sourceAsync,
    //                                                                                             Func<T        , MlResult<TReturn>> funcValidAsync,
    //                                                                                             Func<Exception, MlResult<TReturn>> funcFail,
    //                                                                                             string                             errorMessage = null!)
    //    => await sourceAsync.TryBindIfFailWithExceptionAsync(funcValidAsync.ToFuncTask(), funcFail.ToFuncTask(), errorMessage);






    #endregion


    #region BindIfFailWithoutException


    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// The previous run cannot have raised an Exception. If this case occurs, the 'func' will not be executed
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static MlResult<T> BindIfFailWithoutException<T>(this MlResult<T>                        source,
                                                                 Func<MlErrorsDetails, MlResult<T>> func)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : func,
                                                                                                valid: _ => errorsDetails
                                                                                            ),
                            valid: x             => x
                        );


    public static async Task<MlResult<T>> BindIfFailWithoutExceptionAsync<T>(this MlResult<T>                              source,
                                                                                  Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                                            failAsync : async errorDetails2 => await funcAsync(errorDetails2),
                                                                                                                            validAsync: async _             => await errorsDetails.ToMlResultFailAsync<T>()
                                                                                                                        ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );

    public static async Task<MlResult<T>> BindIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                  Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync)
        => await (await sourceAsync).BindIfFailWithoutExceptionAsync(funcAsync);

    public static async Task<MlResult<T>> BindIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>                  sourceAsync,
                                                                                  Func<MlErrorsDetails, MlResult<T>> funcAsync)
        => await (await sourceAsync).BindIfFailWithoutExceptionAsync(funcAsync.ToFuncTask());

    public static MlResult<T> TryBindIfFailWithoutException<T>(this MlResult<T>                        source,
                                                                    Func<MlErrorsDetails, MlResult<T>> func,
                                                                    Func<Exception, string>            errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : _ => func.TryToMlResult(errorsDetails, errorMessageBuilder),
                                                                                                valid: _ => errorsDetails
                                                                                            ),
                            valid: x             => x
                        );

    public static MlResult<T> TryBindIfFailWithoutException<T>(this MlResult<T>                        source,
                                                                    Func<MlErrorsDetails, MlResult<T>> func,
                                                                    string                             errorMessage = null!)
        => source.TryBindIfFailWithoutException(func, _ => errorMessage!);

    public static async Task<MlResult<T>> TryBindIfFailWithoutExceptionAsync<T>(this MlResult<T>                              source,
                                                                                     Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync,
                                                                                     Func<Exception, string>                  errorMessageBuilder)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                                            failAsync : _ => funcAsync.TryToMlResultAsync(errorsDetails, errorMessageBuilder),
                                                                                                                            validAsync: _ => errorsDetails.ToMlResultFailAsync<T>()
                                                                                                                        ),
                                        validAsync: x             => x.ToMlResultValidAsync()
                                    );

    public static async Task<MlResult<T>> TryBindIfFailWithoutExceptionAsync<T>(this MlResult<T>                              source,
                                                                                     Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync,
                                                                                     string                                   errorMessage = null!)
        => await source.TryBindIfFailWithoutExceptionAsync(funcAsync, _ => errorMessage!);


    public static async Task<MlResult<T>> TryBindIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                     Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync,
                                                                                     Func<Exception, string>                  errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> BindExIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>                        sourceAsync,
                                                                                    Func<MlErrorsDetails, Task<MlResult<T>>> funcAsync,
                                                                                    string                                   errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcAsync, errorMessage);

    public static async Task<MlResult<T>> TryBindIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>                  sourceAsync,
                                                                                     Func<MlErrorsDetails, MlResult<T>> funcAsync,
                                                                                     Func<Exception, string>            errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcAsync.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<T>> TryBindIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>                  sourceAsync,
                                                                                     Func<MlErrorsDetails, MlResult<T>> funcAsync,
                                                                                     string                             errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcAsync.ToFuncTask(), errorMessage);




    public static MlResult<TReturn> BindIfFailWithoutException<T, TReturn>(this MlResult<T>                              source,
                                                                                Func<T              , MlResult<TReturn>> funcValid,
                                                                                Func<MlErrorsDetails, MlResult<TReturn>> funcFail)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : funcFail,
                                                                                                valid: _ => errorsDetails
                                                                                            ),
                            valid: funcValid
                        );

    public static async Task<MlResult<TReturn>> BindIfFailWithoutExceptionAsync<T, TReturn>(this MlResult<T>                                    source,
                                                                                                 Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                 Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                                            failAsync : funcFailAsync,
                                                                                                                            validAsync: _ => errorsDetails.ToMlResultFailAsync<TReturn>()
                                                                                                                        ),
                                        validAsync: funcValidAsync
                                    );

    public static async Task<MlResult<TReturn>> BindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                                 Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                 Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync)
        => await (await sourceAsync).BindIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync);

    public static async Task<MlResult<TReturn>> BindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                                 Func<T              , MlResult<TReturn>>       funcValid,
                                                                                                 Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync)
        => await (await sourceAsync).BindIfFailWithoutExceptionAsync(funcValid.ToFuncTask(), funcFailAsync);

    public static async Task<MlResult<TReturn>> BindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                                 Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                 Func<MlErrorsDetails, MlResult<TReturn>>       funcFail)
        => await (await sourceAsync).BindIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask());

    public static async Task<MlResult<TReturn>> BindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                 Func<T              , MlResult<TReturn>> funcValidAsync,
                                                                                                 Func<MlErrorsDetails, MlResult<TReturn>> funcFail)
        => await (await sourceAsync).BindIfFailWithoutExceptionAsync(funcValidAsync.ToFuncTask(), funcFail.ToFuncTask());

    public static MlResult<TReturn> TryBindIfFailWithoutException<T, TReturn>(this MlResult<T>                              source,
                                                                                   Func<T              , MlResult<TReturn>> funcValid,
                                                                                   Func<MlErrorsDetails, MlResult<TReturn>> funcFail,
                                                                                   Func<Exception, string>                  errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : _ => funcFail.TryToMlResult(errorsDetails, errorMessageBuilder),
                                                                                                valid: _ => errorsDetails
                                                                                            ),
                            valid: x             => funcValid.TryToMlResult(x, errorMessageBuilder)
                        );

    public static MlResult<TReturn> TryBindIfFailWithoutException<T, TReturn>(this MlResult<T>                              source,
                                                                                   Func<T              , MlResult<TReturn>> funcValid,
                                                                                   Func<MlErrorsDetails, MlResult<TReturn>> funcFail,
                                                                                   string                                   errorMessage = null!)
        => source.TryBindIfFailWithoutException(funcValid, funcFail, _ => errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithoutExceptionAsync<T, TReturn>(this MlResult<T>                                    source,
                                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                    Func<Exception, string>                        errorMessageBuilder)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                                            failAsync : _ => funcFailAsync.TryToMlResultAsync(errorsDetails, errorMessageBuilder),
                                                                                                                            validAsync: _ => errorsDetails.ToMlResultFailAsync<TReturn>()
                                                                                                                        ),
                                        validAsync: x             => funcValidAsync.TryToMlResultAsync(x, errorMessageBuilder)
                                    );

    public static async Task<MlResult<TReturn>> TryBindIfFailWithoutExceptionAsync<T, TReturn>(this MlResult<T>                                    source,
                                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                    string                                         errorMessage = null!)
        => await source.TryBindIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync, _ => errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                    Func<Exception, string>                        errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                    string                                         errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                                    Func<T              , MlResult<TReturn>>       funcValid,
                                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                    Func<Exception, string>                        errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                                    Func<T              , MlResult<TReturn>>       funcValid,
                                                                                                    Func<MlErrorsDetails, Task<MlResult<TReturn>>> funcFailAsync,
                                                                                                    string                                         errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                    Func<MlErrorsDetails, MlResult<TReturn>>       funcFail,
                                                                                                    Func<Exception, string>                        errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                              sourceAsync,
                                                                                                    Func<T              , Task<MlResult<TReturn>>> funcValidAsync,
                                                                                                    Func<MlErrorsDetails, MlResult<TReturn>>       funcFail,
                                                                                                    string                                         errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                    Func<T              , MlResult<TReturn>> funcValidAsync,
                                                                                                    Func<MlErrorsDetails, MlResult<TReturn>> funcFail,
                                                                                                    Func<Exception, string>                  errorMessageBuilder)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcValidAsync.ToFuncTask(), funcFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryBindIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                        sourceAsync,
                                                                                                    Func<T              , MlResult<TReturn>> funcValidAsync,
                                                                                                    Func<MlErrorsDetails, MlResult<TReturn>> funcFail,
                                                                                                    string                                   errorMessage = null!)
        => await (await sourceAsync).TryBindIfFailWithoutExceptionAsync(funcValidAsync.ToFuncTask(), funcFail.ToFuncTask(), errorMessage);


    #endregion


    #region BindAlways



    public static MlResult<TReturn> BindAlways<T, TReturn>(this MlResult<T>             source, 
                                                                Func<MlResult<TReturn>> funcAlways)
        => funcAlways();

    public static Task<MlResult<TReturn>> BindAlwaysAsync<T, TReturn>(this MlResult<T>             source, 
                                                                           Func<MlResult<TReturn>> funcAlways)
        => source.BindAlways(funcAlways).ToAsync();

    public static async Task<MlResult<TReturn>> BindAlwaysAsync<T, TReturn>(this MlResult<T>                   source, 
                                                                                 Func<Task<MlResult<TReturn>>> funcAlwaysAsync)
        => await funcAlwaysAsync();


    public static async Task<MlResult<TReturn>> BindAlwaysAsync<T, TReturn>(this Task<MlResult<T>>             sourceAsync, 
                                                                                 Func<Task<MlResult<TReturn>>> funcAlwaysAsync)
        => await funcAlwaysAsync();

    public static async Task<MlResult<TReturn>> BindAlwaysAsync<T, TReturn>(this Task<MlResult<T>>       sourceAsync, 
                                                                                 Func<MlResult<TReturn>> funcAlways)
         => await (await sourceAsync).BindAlwaysAsync<T, TReturn>(funcAlways);



    //public static MlResult<TReturn> TryBindAlways<T, TReturn>(this MlResult<T> source,
    //                                                               Func<MlResult<TReturn>> funcAlways,
    //                                                               Func<Exception, string> errorMessageBuilder)
    //    => source.Match<T, MlResult<TReturn>>(valid: _ => funcAlways.TryToMlResult(errorMessageBuilder),
    //                                          fail : _ => funcAlways.TryToMlResult(errorMessageBuilder));

    public static MlResult<TReturn> TryBindAlways<T, TReturn>(this MlResult<T>             source,
                                                                   Func<MlResult<TReturn>> funcAlways,
                                                                   Func<Exception, string> errorMessageBuilder)
        => funcAlways.TryToMlResult(errorMessageBuilder);






    public static MlResult<TReturn> TryBindAlways<T, TReturn>(this MlResult<T>             source,
                                                                   Func<MlResult<TReturn>> funcAlways,
                                                                   string                  errorMessage = null!)
        => source.TryBindAlways(funcAlways, _ => errorMessage!);

    public static Task<MlResult<TReturn>> TryBindAlwaysAsync<T, TReturn>(this MlResult<T>             source,
                                                                              Func<MlResult<TReturn>> funcAlways,
                                                                              Func<Exception, string> errorMessageBuilder)
        => funcAlways.TryToMlResult(errorMessageBuilder).ToAsync();

    public static Task<MlResult<TReturn>> TryBindAlwaysAsync<T, TReturn>(this MlResult<T>             source,
                                                                              Func<MlResult<TReturn>> funcAlways,
                                                                              string                  errorMessage = null!)
        => source.TryBindAlwaysAsync(funcAlways, _ => errorMessage!);


    public static Task<MlResult<TReturn>> TryBindAlwaysAsync<T, TReturn>(this MlResult<T>                   source,
                                                                              Func<Task<MlResult<TReturn>>> funcAlwaysAsync,
                                                                              Func<Exception, string>       errorMessageBuilder)
        => source.TryBindAlwaysAsync(funcAlwaysAsync, errorMessageBuilder);

    public static Task<MlResult<TReturn>> TryBindAlwaysAsync<T, TReturn>(this MlResult<T>                   source,
                                                                              Func<Task<MlResult<TReturn>>> funcAlwaysAsync,
                                                                              string                        errorMessage = null!)
        => source.TryBindAlwaysAsync(funcAlwaysAsync, _ => errorMessage!);











    public async static Task<MlResult<TReturn>> TryBindAlwaysAsync<T, TReturn>(this Task<MlResult<T>>             sourceAsync,
                                                                                    Func<Task<MlResult<TReturn>>> funcAlwaysAsync,
                                                                                    Func<Exception, string>       errorMessageBuilder)
        => await (await sourceAsync).TryBindAlwaysAsync(funcAlwaysAsync, errorMessageBuilder);

    public async static Task<MlResult<TReturn>> TryBindAlwaysAsync<T, TReturn>(this Task<MlResult<T>>             sourceAsync,
                                                                                    Func<Task<MlResult<TReturn>>> funcAlwaysAsync,
                                                                                    string                        errorMessage = null!)
        => await (await sourceAsync).TryBindAlwaysAsync(funcAlwaysAsync, errorMessage);

    public async static Task<MlResult<TReturn>> TryBindAlwaysAsync<T, TReturn>(this Task<MlResult<T>>       sourceAsync,
                                                                                    Func<MlResult<TReturn>> funcAlways,
                                                                                    Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryBindAlways<T, TReturn>(funcAlways, errorMessageBuilder);

    public async static Task<MlResult<TReturn>> TryBindAlwaysAsync<T, TReturn>(this Task<MlResult<T>>       sourceAsync,
                                                                                    Func<MlResult<TReturn>> funcAlways,
                                                                                    string                  errorMessage = null!)
        => (await sourceAsync).TryBindAlways<T, TReturn>(funcAlways, _ => errorMessage);





    /// <summary>
    /// Ejecutará la acción, utilizando cada una de las funciones, según el estado del MlResult.
    /// Necesita 2 funciones:
    ///     1.- Función que se ejecutará si el MlResult es válido (facilita el value valido por parámetros)
    ///     2.- Función que se ejecutará si el MlResult es fallido (facilita el errorDetails por parámetros)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="funcValidAlways"></param>
    /// <param name="funcFailAlways"></param>
    /// <returns></returns>
    public static MlResult<TResult> BindAlways<T, TResult>(this MlResult<T>                              source,
                                                                Func<T              , MlResult<TResult>> funcValidAlways,
                                                                Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways)
        => source.Match(
                            valid: funcValidAlways,
                            fail : funcFailAlways
                        );

    public static Task<MlResult<TResult>> BindAlwaysAsync<T, TResult>(this MlResult<T>                              source,
                                                                           Func<T              , MlResult<TResult>> funcValidAlways,
                                                                           Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways)
        => source.BindAlways(funcValidAlways, funcFailAlways).ToAsync();

    public static async Task<MlResult<TResult>> BindAlwaysAsync<T, TResult>(this MlResult<T>                                    source,
                                                                                 Func<T              , Task<MlResult<TResult>>> funcValidAlwaysAsync,
                                                                                 Func<MlErrorsDetails, Task<MlResult<TResult>>> funcFailAlwaysAsync)
        => await source.MatchAsync(validAsync: funcValidAlwaysAsync,
                                   failAsync : funcFailAlwaysAsync);

    public static async Task<MlResult<TResult>> BindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                              sourceAsync, 
                                                                                 Func<T              , Task<MlResult<TResult>>> funcValidAlwaysAsync,
                                                                                 Func<MlErrorsDetails, Task<MlResult<TResult>>> funcFailAlwaysAsync)
        => await (await sourceAsync).BindAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync);

    public static async Task<MlResult<TResult>> BindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                              sourceAsync, 
                                                                                 Func<T              ,      MlResult<TResult>>  funcValidAlways,
                                                                                 Func<MlErrorsDetails, Task<MlResult<TResult>>> funcFailAlwaysAsync)
        => await (await sourceAsync).BindAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync);

    public static async Task<MlResult<TResult>> BindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                 Func<T              , Task<MlResult<TResult>>> funcValidAlwaysAsync,
                                                                                 Func<MlErrorsDetails,      MlResult<TResult>>  funcFailAlways)
        => await (await sourceAsync).BindAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask());

    public static async Task<MlResult<TResult>> BindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                        sourceAsync, 
                                                                                 Func<T              , MlResult<TResult>> funcValidAlways,
                                                                                 Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways)
        => (await sourceAsync).BindAlways(funcValidAlways, funcFailAlways);

    public static MlResult<TResult> TryBindAlways<T, TResult>(this MlResult<T>                              source, 
                                                                   Func<T              , MlResult<TResult>> funcValidAlways,
                                                                   Func<MlErrorsDetails, MlResult<TResult>> funcFailAlwaiys,
                                                                   Func<Exception, string>                  errorMessageBuilder)
        => source.Match(valid: value        => funcValidAlways.TryToMlResult(value       , errorMessageBuilder),
                        fail : errorDetails => funcFailAlwaiys.TryToMlResult(errorDetails, errorMessageBuilder));

    public static MlResult<TResult> TryBindAlways<T, TResult>(this MlResult<T>                              source, 
                                                                   Func<T              , MlResult<TResult>> funcValidAlways,
                                                                   Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways,
                                                                   string                                   errorMessage = null!)
        => source.TryBindAlways(funcValidAlways, funcFailAlways, _ => errorMessage);

    public static Task<MlResult<TResult>> TryBindAlwaysAsync<T, TResult>(this MlResult<T>                                    source, 
                                                                              Func<T              , Task<MlResult<TResult>>> funcValidAlwaysAsync,
                                                                              Func<MlErrorsDetails, Task<MlResult<TResult>>> funcFailAlwaysAsync,
                                                                              Func<Exception, string>                        errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => funcValidAlwaysAsync.TryToMlResultAsync(value       , errorMessageBuilder),
                             failAsync : errorDetails => funcFailAlwaysAsync .TryToMlResultAsync(errorDetails, errorMessageBuilder));

    public static Task<MlResult<TResult>> TryBindAlwaysAsync<T, TResult>(this MlResult<T>                                    source, 
                                                                              Func<T              , Task<MlResult<TResult>>> funcValidAlwaysAsync,
                                                                              Func<MlErrorsDetails, Task<MlResult<TResult>>> funcFailAlwaysAsync,
                                                                              string                                         errorMessage = null!)
        => source.TryBindAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync, _ => errorMessage);


    public async static Task<MlResult<TResult>> TryBindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                              sourceAsync, 
                                                                                    Func<T              , Task<MlResult<TResult>>> funcValidAlwaysAsync,
                                                                                    Func<MlErrorsDetails, Task<MlResult<TResult>>> funcFailAlwaysAsync,
                                                                                    Func<Exception, string>                        errorMessageBuilder)
        => await (await sourceAsync).TryBindAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryBindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                              sourceAsync, 
                                                                                    Func<T              , Task<MlResult<TResult>>> funcValidAlwaysAsync,
                                                                                    Func<MlErrorsDetails, Task<MlResult<TResult>>> funcFailAlwaysAsync,
                                                                                    string                                         errorMessage = null!)
        => await (await sourceAsync).TryBindAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync, errorMessage);

    public async static Task<MlResult<TResult>> TryBindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                              sourceAsync, 
                                                                                    Func<T              ,      MlResult<TResult>>  funcValidAlways,
                                                                                    Func<MlErrorsDetails, Task<MlResult<TResult>>> funcFailAlwaysAsync,
                                                                                    Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryBindAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryBindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                              sourceAsync, 
                                                                                    Func<T              ,      MlResult<TResult>>  funcValidAlways,
                                                                                    Func<MlErrorsDetails, Task<MlResult<TResult>>> funcFailAlwaysAsync,
                                                                                    string                               errorMessage = null!)
        => await (await sourceAsync).TryBindAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync, errorMessage);

    public async static Task<MlResult<TResult>> TryBindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                              sourceAsync, 
                                                                                    Func<T              , Task<MlResult<TResult>>> funcValidAlwaysAsync,
                                                                                    Func<MlErrorsDetails,      MlResult<TResult>>  funcFailAlways,
                                                                                    Func<Exception, string>                        errorMessageBuilder)
        => await (await sourceAsync).TryBindAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask(), errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryBindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                              sourceAsync, 
                                                                                    Func<T              , Task<MlResult<TResult>>> funcValidAlwaysAsync,
                                                                                    Func<MlErrorsDetails,      MlResult<TResult>>  funcFailAlways,
                                                                                    string                                         errorMessage = null!)
        => await (await sourceAsync).TryBindAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask(), errorMessage);

    public async static Task<MlResult<TResult>> TryBindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                        sourceAsync, 
                                                                                    Func<T              , MlResult<TResult>> funcValidAlways,
                                                                                    Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways,
                                                                                    Func<Exception, string>                  errorMessageBuilder)
        => (await sourceAsync).TryBindAlways(funcValidAlways, funcFailAlways, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryBindAlwaysAsync<T, TResult>(this Task<MlResult<T>>                        sourceAsync, 
                                                                                    Func<T              , MlResult<TResult>> funcValidAlways,
                                                                                    Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways,
                                                                                    string                                   errorMessage = null!)
        => (await sourceAsync).TryBindAlways(funcValidAlways, funcFailAlways, errorMessage);

    #endregion


    #region TryBindBuild

    public static MlResult<TResult> TryBindBuild<T, TResult>(this   MlResult<T>                 source,
                                                             params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuild<T, TResult>(false, errorMessageBuilder: null!, funcArgs: funcArgs);

    public static MlResult<TResult> TryBindBuild<T, TResult>(this   MlResult<T>                 source,
                                                                    Func<Exception, string>     errorMessageBuilder,
                                                             params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuild<T, TResult>(false, errorMessageBuilder, funcArgs);

    public static MlResult<TResult> TryBindBuild<T, TResult>(this   MlResult<T>                 source,
                                                                    string                      exceptionAditionalMessage,
                                                             params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuild<T, TResult>(false, exceptionAditionalMessage, funcArgs);


    public static Task<MlResult<TResult>> TryBindBuildSyncAsync<T, TResult>(this   MlResult<T>                 source,
                                                                            params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuildSyncAsync<T, TResult>(false, errorMessageBuilder: null!, funcArgs: funcArgs);

    public static Task<MlResult<TResult>> TryBindBuildSyncAsync<T, TResult>(this   MlResult<T>                 source,
                                                                                   Func<Exception, string>     errorMessageBuilder,
                                                                            params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuildSyncAsync<T, TResult>(false, errorMessageBuilder, funcArgs);

    public static Task<MlResult<TResult>> TryBindBuildSyncAsync<T, TResult>(this   MlResult<T>                 source,
                                                                                   string                      exceptionAditionalMessage,
                                                                            params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuildSyncAsync<T, TResult>(false, exceptionAditionalMessage, funcArgs);

    public static async Task<MlResult<TResult>> TryBindBuildSyncAsync<T, TResult>(this   Task<MlResult<T>>           sourceAsync,
                                                                                  params Func<T, MlResult<object>>[] funcArgs)
        => await sourceAsync.InternalTryBindBuildSyncAsync<T, TResult>(false, errorMessageBuilder: null!, funcArgs: funcArgs);

    public static async Task<MlResult<TResult>> TryBindBuildSyncAsync<T, TResult>(this   Task<MlResult<T>>           sourceAsync,
                                                                                         Func<Exception, string>     errorMessageBuilder,
                                                                                  params Func<T, MlResult<object>>[] funcArgs)
        => await sourceAsync.InternalTryBindBuildSyncAsync<T, TResult>(false, errorMessageBuilder, funcArgs);

        public static async Task<MlResult<TResult>> TryBindBuildSyncAsync<T, TResult>(this   Task<MlResult<T>>           sourceAsync,
                                                                                             string                      exceptionAditionalMessage,
                                                                                      params Func<T, MlResult<object>>[] funcArgs)
        => await sourceAsync.InternalTryBindBuildSyncAsync<T, TResult>(false, exceptionAditionalMessage, funcArgs);

    public static async Task<MlResult<TResult>> TryBindBuildAsync<T, TResult>(this   Task<MlResult<T>>                 sourceAsync,
                                                                              params Func<T, Task<MlResult<object>>>[] funcArgsAsync)
        => await sourceAsync.InternalTryBindBuildAsync<T, TResult>(false, errorMessageBuilder: null!, funcArgsAsync: funcArgsAsync);

    public static async Task<MlResult<TResult>> TryBindBuildAsync<T, TResult>(this   Task<MlResult<T>>                 sourceAsync,
                                                                                     Func<Exception, string>           errorMessageBuilder,
                                                                              params Func<T, Task<MlResult<object>>>[] funcArgsAsync)
        => await sourceAsync.InternalTryBindBuildAsync<T, TResult>(false, errorMessageBuilder, funcArgsAsync);

    public static async Task<MlResult<TResult>> TryBindBuildAsync<T, TResult>(this   Task<MlResult<T>>                 sourceAsync,
                                                                                     string                            exceptionAditionalMessage,
                                                                              params Func<T, Task<MlResult<object>>>[] funcArgsAsync)
        => await sourceAsync.InternalTryBindBuildAsync<T, TResult>(false, ex => exceptionAditionalMessage, funcArgsAsync);


    private static MlResult<TResult> InternalTryBindBuild<T, TResult>(this   MlResult<T>                 source,
                                                                             bool                        breakInError,
                                                                             Func<Exception, string>     errorMessageBuilder = null!,
                                                                      params Func<T, MlResult<object>>[] funcArgs)
    {
        var result = EnsureFp.NotEmpty(funcArgs, $"The parameter {nameof(funcArgs)}, can't be empty.")
                              .Bind     ( _                        => source)
                              .TryBind  (func               : x    => ApplyValues(x, breakInError, funcArgs), 
                                         errorMessageBuilder: ex   => CreatePartialErrorMessage(ex,$"Unexpected error applying the functions in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                              .TryMap   (func:                args => Activator.CreateInstance(typeof(TResult), args.ToArray())! ,
                                         errorMessageBuilder: ex   => $"Unexpected error creating the instance of {typeof(TResult).Name} in {nameof(TryBindBuild)}. The instance must have the same parameters and in the same order as the constructor of the type {typeof(TResult).Name}, and these must be constructed with each of the calls to each element of the parameter {nameof(funcArgs)}: {ex.Message}" )
                              .MapEnsure(partialResult             => partialResult is TResult,
                                         errorDetailsResult:          CreatePartialErrorMessage(new Exception(), $"The created instance is not of type {typeof(TResult).Name} in {nameof(TryBindBuild)}.", errorMessageBuilder))
                              .Map      (partialResult             => (TResult)partialResult);
        return result;
    }

        private static MlResult<TResult> InternalTryBindBuild<T, TResult>(this   MlResult<T>                 source,
                                                                                 bool                        breakInError,
                                                                                 string                      exceptionAditionalMessage,
                                                                          params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuild<T, TResult>(breakInError, ex => exceptionAditionalMessage, funcArgs);


    private static Task<MlResult<TResult>> InternalTryBindBuildSyncAsync<T, TResult>(this   MlResult<T>                 source,
                                                                                            bool                        breakInError,
                                                                                            Func<Exception, string>     errorMessageBuilder = null!,
                                                                                     params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuild<T, TResult>(breakInError, errorMessageBuilder, funcArgs).ToAsync();

    private static Task<MlResult<TResult>> InternalTryBindBuildSyncAsync<T, TResult>(this   MlResult<T>                 source,
                                                                                            bool                        breakInError,
                                                                                            string                      exceptionAditionalMessage,
                                                                                     params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuild<T, TResult>(breakInError, exceptionAditionalMessage, funcArgs).ToAsync();


    private static async Task<MlResult<TResult>> InternalTryBindBuildSyncAsync<T, TResult>(this   Task<MlResult<T>>           sourceAsync,
                                                                                                  bool                        breakInError,
                                                                                                  Func<Exception, string>     errorMessageBuilder = null!,
                                                                                           params Func<T, MlResult<object>>[] funcArgs)
        => await (await sourceAsync).InternalTryBindBuildSyncAsync<T, TResult>(breakInError, errorMessageBuilder, funcArgs);

    private static async Task<MlResult<TResult>> InternalTryBindBuildSyncAsync<T, TResult>(this   Task<MlResult<T>>           sourceAsync,
                                                                                                  bool                        breakInError,
                                                                                                  string                      exceptionAditionalMessage,
                                                                                           params Func<T, MlResult<object>>[] funcArgs)
        => await (await sourceAsync).InternalTryBindBuildSyncAsync<T, TResult>(breakInError, exceptionAditionalMessage, funcArgs);

    private static async Task<MlResult<TResult>> InternalTryBindBuildAsync<T, TResult>(this   Task<MlResult<T>>                 sourceAsync,
                                                                                              bool                              breakInError,
                                                                                              Func<Exception, string>           errorMessageBuilder = null!,
                                                                                       params Func<T, Task<MlResult<object>>>[] funcArgsAsync)
    {
        var result = await EnsureFp.NotEmptyAsync(funcArgsAsync, $"The parameter {nameof(funcArgsAsync)}, can't be empty.")
                      .BindAsync     ( _                        => sourceAsync)
                      .TryBindAsync  (funcAsync          : x    => ApplyValuesAsync(x, breakInError, funcArgsAsync),
                                      errorMessageBuilder: ex   => CreatePartialErrorMessage(ex, $"Unexpected error applying the functions in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                      .TryMapAsync   (func               : args => Activator.CreateInstance(typeof(TResult), args.ToArray())!,
                                      errorMessageBuilder: ex   => $"Unexpected error creating the instance of {typeof(TResult).Name} in {nameof(TryBindBuild)}. The instance must have the same parameters and in the same order as the constructor of the type {typeof(TResult).Name}, and these must be constructed with each of the calls to each element of the parameter {nameof(funcArgsAsync)}: {ex.Message}")
                      .MapEnsureAsync(partialResult             => partialResult is TResult,
                                      errorDetailsResult:          CreatePartialErrorMessage(new Exception(), $"The created instance is not of type {typeof(TResult).Name} in {nameof(TryBindBuild)}.", errorMessageBuilder))
                      .MapAsync      (partialResult             => (TResult)partialResult);
        return result;
    }




    #endregion


    #region TryBindBuild Tuple




    public static MlResult<(TR1, TR2)> TryBindBuild<T, TR1, TR2>(this MlResult<T>             source,
                                                                      Func<T, MlResult<TR1>>  funcBuild1,
                                                                      Func<T, MlResult<TR2>>  funcBuild2)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, errorMessageBuilder: null!);

    public static MlResult<(TR1, TR2)> TryBindBuild<T, TR1, TR2>(this MlResult<T>             source,
                                                                      Func<T, MlResult<TR1>>  funcBuild1,
                                                                      Func<T, MlResult<TR2>>  funcBuild2,
                                                                      Func<Exception, string> errorMessageBuilder)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, errorMessageBuilder: errorMessageBuilder);

    public static MlResult<(TR1, TR2)> TryBindBuild<T, TR1, TR2>(this MlResult<T>            source,
                                                                      Func<T, MlResult<TR1>> funcBuild1,
                                                                      Func<T, MlResult<TR2>> funcBuild2,
                                                                      string                 exceptionAditionalMessage)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, errorMessageBuilder: ex => exceptionAditionalMessage);


    public static Task<MlResult<(TR1, TR2)>> TryBindBuildAsync<T, TR1, TR2>(this MlResult<T>             source,
                                                                                 Func<T, MlResult<TR1>>  funcBuild1,
                                                                                 Func<T, MlResult<TR2>>  funcBuild2)
        => source.TryBindBuild(funcBuild1, funcBuild2).ToAsync();

    public static Task<MlResult<(TR1, TR2)>> TryBindBuildAsync<T, TR1, TR2>(this MlResult<T>             source,
                                                                                 Func<T, MlResult<TR1>>  funcBuild1,
                                                                                 Func<T, MlResult<TR2>>  funcBuild2,
                                                                                 Func<Exception, string> errorMessageBuilder)
        => source.TryBindBuild(funcBuild1, funcBuild2, errorMessageBuilder: errorMessageBuilder).ToAsync();

    public static Task<MlResult<(TR1, TR2)>> TryBindBuildAsync<T, TR1, TR2>(this MlResult<T>            source,
                                                                                 Func<T, MlResult<TR1>> funcBuild1,
                                                                                 Func<T, MlResult<TR2>> funcBuild2,
                                                                                 string                 exceptionAditionalMessage)
        => source.TryBindBuild(funcBuild1, funcBuild2, errorMessageBuilder: ex => exceptionAditionalMessage).ToAsync();

    public static async Task<MlResult<(TR1, TR2)>> TryBindBuildAsync<T, TR1, TR2>(this Task<MlResult<T>>      sourceAsync,
                                                                                       Func<T, MlResult<TR1>> funcBuild1,
                                                                                       Func<T, MlResult<TR2>> funcBuild2)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2);

    public static async Task<MlResult<(TR1, TR2)>> TryBindBuildAsync<T, TR1, TR2>(this Task<MlResult<T>>       sourceAsync,
                                                                                       Func<T, MlResult<TR1>>  funcBuild1,
                                                                                       Func<T, MlResult<TR2>>  funcBuild2,
                                                                                       Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, errorMessageBuilder: errorMessageBuilder);

    public static async Task<MlResult<(TR1, TR2)>> TryBindBuildAsync<T, TR1, TR2>(this Task<MlResult<T>>      sourceAsync,
                                                                                       Func<T, MlResult<TR1>> funcBuild1,
                                                                                       Func<T, MlResult<TR2>> funcBuild2,
                                                                                       string                 exceptionAditionalMessage)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2)>> TryBindBuildAsync<T, TR1, TR2>(this Task<MlResult<T>>            sourceAsync,
                                                                                 Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                 Func<T, Task<MlResult<TR2>>> funcBuild2Async)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, errorMessageBuilder: null!);

    public static Task<MlResult<(TR1, TR2)>> TryBindBuildAsync<T, TR1, TR2>(this Task<MlResult<T>>            sourceAsync,
                                                                                 Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                 Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                 Func<Exception, string>      errorMessageBuilder)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, errorMessageBuilder: errorMessageBuilder);

    public static Task<MlResult<(TR1, TR2)>> TryBindBuildAsync<T, TR1, TR2>(this Task<MlResult<T>>            sourceAsync,
                                                                                 Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                 Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                 string                        exceptionAditionalMessage)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, errorMessageBuilder: ex => exceptionAditionalMessage);


    // TR3
    public static MlResult<(TR1, TR2, TR3)> TryBindBuild<T, TR1, TR2, TR3>(this MlResult<T>             source,
                                                                                Func<T, MlResult<TR1>>  funcBuild1,
                                                                                Func<T, MlResult<TR2>>  funcBuild2,
                                                                                Func<T, MlResult<TR3>>  funcBuild3)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, errorMessageBuilder: null!);

    public static MlResult<(TR1, TR2, TR3)> TryBindBuild<T, TR1, TR2, TR3>(this MlResult<T>             source,
                                                                                Func<T, MlResult<TR1>>  funcBuild1,
                                                                                Func<T, MlResult<TR2>>  funcBuild2,
                                                                                Func<T, MlResult<TR3>>  funcBuild3,
                                                                                Func<Exception, string> errorMessageBuilder)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, errorMessageBuilder: errorMessageBuilder);

    public static MlResult<(TR1, TR2, TR3)> TryBindBuild<T, TR1, TR2, TR3>(this MlResult<T>            source,
                                                                                Func<T, MlResult<TR1>> funcBuild1,
                                                                                Func<T, MlResult<TR2>> funcBuild2,
                                                                                Func<T, MlResult<TR3>> funcBuild3,
                                                                                string                 exceptionAditionalMessage)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3)>> TryBindBuildAsync<T, TR1, TR2, TR3>(this MlResult<T>             source,
                                                                                           Func<T, MlResult<TR1>>  funcBuild1,
                                                                                           Func<T, MlResult<TR2>>  funcBuild2,
                                                                                           Func<T, MlResult<TR3>>  funcBuild3)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3)>> TryBindBuildAsync<T, TR1, TR2, TR3>(this MlResult<T>             source,
                                                                                           Func<T, MlResult<TR1>>  funcBuild1,
                                                                                           Func<T, MlResult<TR2>>  funcBuild2,
                                                                                           Func<T, MlResult<TR3>>  funcBuild3,
                                                                                           Func<Exception, string> errorMessageBuilder)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, errorMessageBuilder: errorMessageBuilder).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3)>> TryBindBuildAsync<T, TR1, TR2, TR3>(this MlResult<T>            source,
                                                                                           Func<T, MlResult<TR1>> funcBuild1,
                                                                                           Func<T, MlResult<TR2>> funcBuild2,
                                                                                           Func<T, MlResult<TR3>> funcBuild3,
                                                                                           string                 exceptionAditionalMessage)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, errorMessageBuilder: ex => exceptionAditionalMessage).ToAsync();

    public static async Task<MlResult<(TR1, TR2, TR3)>> TryBindBuildAsync<T, TR1, TR2, TR3>(this Task<MlResult<T>>      sourceAsync,
                                                                                                  Func<T, MlResult<TR1>> funcBuild1,
                                                                                                  Func<T, MlResult<TR2>> funcBuild2,
                                                                                                  Func<T, MlResult<TR3>> funcBuild3)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3);

    public static async Task<MlResult<(TR1, TR2, TR3)>> TryBindBuildAsync<T, TR1, TR2, TR3>(this Task<MlResult<T>>       sourceAsync,
                                                                                                  Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                  Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                  Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                  Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, errorMessageBuilder: errorMessageBuilder);

    public static async Task<MlResult<(TR1, TR2, TR3)>> TryBindBuildAsync<T, TR1, TR2, TR3>(this Task<MlResult<T>>      sourceAsync,
                                                                                                  Func<T, MlResult<TR1>> funcBuild1,
                                                                                                  Func<T, MlResult<TR2>> funcBuild2,
                                                                                                  Func<T, MlResult<TR3>> funcBuild3,
                                                                                                  string                 exceptionAditionalMessage)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3)>> TryBindBuildAsync<T, TR1, TR2, TR3>(this Task<MlResult<T>>            sourceAsync,
                                                                                           Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                           Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                           Func<T, Task<MlResult<TR3>>> funcBuild3Async)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, errorMessageBuilder: null!);

    public static Task<MlResult<(TR1, TR2, TR3)>> TryBindBuildAsync<T, TR1, TR2, TR3>(this Task<MlResult<T>>            sourceAsync,
                                                                                           Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                           Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                           Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                           Func<Exception, string>      errorMessageBuilder)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, errorMessageBuilder: errorMessageBuilder);

    public static Task<MlResult<(TR1, TR2, TR3)>> TryBindBuildAsync<T, TR1, TR2, TR3>(this Task<MlResult<T>>            sourceAsync,
                                                                                           Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                           Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                           Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                           string                        exceptionAditionalMessage)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, errorMessageBuilder: ex => exceptionAditionalMessage);

    // TR4
    public static MlResult<(TR1, TR2, TR3, TR4)> TryBindBuild<T, TR1, TR2, TR3, TR4>(this MlResult<T>             source,
                                                                                          Func<T, MlResult<TR1>>  funcBuild1,
                                                                                          Func<T, MlResult<TR2>>  funcBuild2,
                                                                                          Func<T, MlResult<TR3>>  funcBuild3,
                                                                                          Func<T, MlResult<TR4>>  funcBuild4)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, errorMessageBuilder: null!);

    public static MlResult<(TR1, TR2, TR3, TR4)> TryBindBuild<T, TR1, TR2, TR3, TR4>(this MlResult<T>             source,
                                                                                          Func<T, MlResult<TR1>>  funcBuild1,
                                                                                          Func<T, MlResult<TR2>>  funcBuild2,
                                                                                          Func<T, MlResult<TR3>>  funcBuild3,
                                                                                          Func<T, MlResult<TR4>>  funcBuild4,
                                                                                          Func<Exception, string> errorMessageBuilder)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, errorMessageBuilder: errorMessageBuilder);

    public static MlResult<(TR1, TR2, TR3, TR4)> TryBindBuild<T, TR1, TR2, TR3, TR4>(this MlResult<T>            source,
                                                                                          Func<T, MlResult<TR1>> funcBuild1,
                                                                                          Func<T, MlResult<TR2>> funcBuild2,
                                                                                          Func<T, MlResult<TR3>> funcBuild3,
                                                                                          Func<T, MlResult<TR4>> funcBuild4,
                                                                                          string                 exceptionAditionalMessage)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3, TR4)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4>(this MlResult<T>             source,
                                                                                                     Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                     Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                     Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                     Func<T, MlResult<TR4>>  funcBuild4)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3, TR4)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4>(this MlResult<T>             source,
                                                                                                     Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                     Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                     Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                     Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                     Func<Exception, string> errorMessageBuilder)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, errorMessageBuilder: errorMessageBuilder).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3, TR4)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4>(this MlResult<T>            source,
                                                                                                     Func<T, MlResult<TR1>> funcBuild1,
                                                                                                     Func<T, MlResult<TR2>> funcBuild2,
                                                                                                     Func<T, MlResult<TR3>> funcBuild3,
                                                                                                     Func<T, MlResult<TR4>> funcBuild4,
                                                                                                     string                 exceptionAditionalMessage)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, errorMessageBuilder: ex => exceptionAditionalMessage).ToAsync();

    public static async Task<MlResult<(TR1, TR2, TR3, TR4)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4>(this Task<MlResult<T>>      sourceAsync,
                                                                                                            Func<T, MlResult<TR1>> funcBuild1,
                                                                                                            Func<T, MlResult<TR2>> funcBuild2,
                                                                                                            Func<T, MlResult<TR3>> funcBuild3,
                                                                                                            Func<T, MlResult<TR4>> funcBuild4)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4);

    public static async Task<MlResult<(TR1, TR2, TR3, TR4)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4>(this Task<MlResult<T>>       sourceAsync,
                                                                                                            Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                            Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                            Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                            Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                            Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, errorMessageBuilder: errorMessageBuilder);

    public static async Task<MlResult<(TR1, TR2, TR3, TR4)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4>(this Task<MlResult<T>>      sourceAsync,
                                                                                                            Func<T, MlResult<TR1>> funcBuild1,
                                                                                                            Func<T, MlResult<TR2>> funcBuild2,
                                                                                                            Func<T, MlResult<TR3>> funcBuild3,
                                                                                                            Func<T, MlResult<TR4>> funcBuild4,
                                                                                                            string                 exceptionAditionalMessage)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3, TR4)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4>(this Task<MlResult<T>>            sourceAsync,
                                                                                                     Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                     Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                     Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                     Func<T, Task<MlResult<TR4>>> funcBuild4Async)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, errorMessageBuilder: null!);

    public static Task<MlResult<(TR1, TR2, TR3, TR4)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4>(this Task<MlResult<T>>            sourceAsync,
                                                                                                     Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                     Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                     Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                     Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                     Func<Exception, string>      errorMessageBuilder)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, errorMessageBuilder: errorMessageBuilder);

    public static Task<MlResult<(TR1, TR2, TR3, TR4)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4>(this Task<MlResult<T>>            sourceAsync,
                                                                                                     Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                     Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                     Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                     Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                     string                        exceptionAditionalMessage)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, errorMessageBuilder: ex => exceptionAditionalMessage);

    // TR5
    public static MlResult<(TR1, TR2, TR3, TR4, TR5)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5>(this MlResult<T>             source,
                                                                                                    Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                    Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                    Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                    Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                    Func<T, MlResult<TR5>>  funcBuild5)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, errorMessageBuilder: null!);

    public static MlResult<(TR1, TR2, TR3, TR4, TR5)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5>(this MlResult<T>             source,
                                                                                                    Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                    Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                    Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                    Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                    Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                    Func<Exception, string> errorMessageBuilder)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, errorMessageBuilder: errorMessageBuilder);

    public static MlResult<(TR1, TR2, TR3, TR4, TR5)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5>(this MlResult<T>            source,
                                                                                                    Func<T, MlResult<TR1>> funcBuild1,
                                                                                                    Func<T, MlResult<TR2>> funcBuild2,
                                                                                                    Func<T, MlResult<TR3>> funcBuild3,
                                                                                                    Func<T, MlResult<TR4>> funcBuild4,
                                                                                                    Func<T, MlResult<TR5>> funcBuild5,
                                                                                                    string                 exceptionAditionalMessage)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5>(this MlResult<T>             source,
                                                                                                               Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                               Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                               Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                               Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                               Func<T, MlResult<TR5>>  funcBuild5)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5>(this MlResult<T>             source,
                                                                                                               Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                               Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                               Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                               Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                               Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                               Func<Exception, string> errorMessageBuilder)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, errorMessageBuilder: errorMessageBuilder).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5>(this MlResult<T>            source,
                                                                                                               Func<T, MlResult<TR1>> funcBuild1,
                                                                                                               Func<T, MlResult<TR2>> funcBuild2,
                                                                                                               Func<T, MlResult<TR3>> funcBuild3,
                                                                                                               Func<T, MlResult<TR4>> funcBuild4,
                                                                                                               Func<T, MlResult<TR5>> funcBuild5,
                                                                                                               string                 exceptionAditionalMessage)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, errorMessageBuilder: ex => exceptionAditionalMessage).ToAsync();

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5>(this Task<MlResult<T>>      sourceAsync,
                                                                                                                      Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                      Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                      Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                      Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                      Func<T, MlResult<TR5>> funcBuild5)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5);

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5>(this Task<MlResult<T>>       sourceAsync,
                                                                                                                      Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                      Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                      Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                      Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                      Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                      Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, errorMessageBuilder: errorMessageBuilder);

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5>(this Task<MlResult<T>>      sourceAsync,
                                                                                                                      Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                      Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                      Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                      Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                      Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                      string                 exceptionAditionalMessage)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5>(this Task<MlResult<T>>            sourceAsync,
                                                                                                               Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                               Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                               Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                               Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                               Func<T, Task<MlResult<TR5>>> funcBuild5Async)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, errorMessageBuilder: null!);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5>(this Task<MlResult<T>>            sourceAsync,
                                                                                                               Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                               Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                               Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                               Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                               Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                               Func<Exception, string>      errorMessageBuilder)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, errorMessageBuilder: errorMessageBuilder);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5>(this Task<MlResult<T>>            sourceAsync,
                                                                                                               Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                               Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                               Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                               Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                               Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                               string                        exceptionAditionalMessage)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, errorMessageBuilder: ex => exceptionAditionalMessage);

    // TR6
    public static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6>(this MlResult<T>             source,
                                                                                                              Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                              Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                              Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                              Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                              Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                              Func<T, MlResult<TR6>>  funcBuild6)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, errorMessageBuilder: null!);

    public static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6>(this MlResult<T>             source,
                                                                                                              Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                              Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                              Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                              Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                              Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                              Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                              Func<Exception, string> errorMessageBuilder)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, errorMessageBuilder: errorMessageBuilder);

    public static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6>(this MlResult<T>            source,
                                                                                                              Func<T, MlResult<TR1>> funcBuild1,
                                                                                                              Func<T, MlResult<TR2>> funcBuild2,
                                                                                                              Func<T, MlResult<TR3>> funcBuild3,
                                                                                                              Func<T, MlResult<TR4>> funcBuild4,
                                                                                                              Func<T, MlResult<TR5>> funcBuild5,
                                                                                                              Func<T, MlResult<TR6>> funcBuild6,
                                                                                                              string                 exceptionAditionalMessage)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6>(this MlResult<T>             source,
                                                                                                                         Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                         Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                         Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                         Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                         Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                         Func<T, MlResult<TR6>>  funcBuild6)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6>(this MlResult<T>             source,
                                                                                                                         Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                         Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                         Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                         Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                         Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                         Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                         Func<Exception, string> errorMessageBuilder)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, errorMessageBuilder: errorMessageBuilder).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6>(this MlResult<T>            source,
                                                                                                                         Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                         Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                         Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                         Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                         Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                         Func<T, MlResult<TR6>> funcBuild6,
                                                                                                                         string                 exceptionAditionalMessage)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, errorMessageBuilder: ex => exceptionAditionalMessage).ToAsync();

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6>(this Task<MlResult<T>>      sourceAsync,
                                                                                                                                Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                                Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                                Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                                Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                                Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                                Func<T, MlResult<TR6>> funcBuild6)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6);

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6>(this Task<MlResult<T>>       sourceAsync,
                                                                                                                                Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                                Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                                Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                                Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                                Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                                Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                                Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, errorMessageBuilder: errorMessageBuilder);

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6>(this Task<MlResult<T>>      sourceAsync,
                                                                                                                                Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                                Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                                Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                                Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                                Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                                Func<T, MlResult<TR6>> funcBuild6,
                                                                                                                                string                 exceptionAditionalMessage)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6>(this Task<MlResult<T>>            sourceAsync,
                                                                                                                         Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                         Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                         Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                         Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                         Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                         Func<T, Task<MlResult<TR6>>> funcBuild6Async)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, errorMessageBuilder: null!);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6>(this Task<MlResult<T>>            sourceAsync,
                                                                                                                         Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                         Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                         Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                         Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                         Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                         Func<T, Task<MlResult<TR6>>> funcBuild6Async,
                                                                                                                         Func<Exception, string>      errorMessageBuilder)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, errorMessageBuilder: errorMessageBuilder);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6>(this Task<MlResult<T>>            sourceAsync,
                                                                                                                         Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                         Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                         Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                         Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                         Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                         Func<T, Task<MlResult<TR6>>> funcBuild6Async,
                                                                                                                         string                        exceptionAditionalMessage)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, errorMessageBuilder: ex => exceptionAditionalMessage);

    // TR7
    public static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this MlResult<T>             source,
                                                                                                                        Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                        Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                        Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                        Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                        Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                        Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                        Func<T, MlResult<TR7>>  funcBuild7)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, errorMessageBuilder: null!);

    public static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this MlResult<T>             source,
                                                                                                                        Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                        Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                        Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                        Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                        Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                        Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                        Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                        Func<Exception, string> errorMessageBuilder)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, errorMessageBuilder: errorMessageBuilder);

    public static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this MlResult<T>            source,
                                                                                                                        Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                        Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                        Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                        Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                        Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                        Func<T, MlResult<TR6>> funcBuild6,
                                                                                                                        Func<T, MlResult<TR7>> funcBuild7,
                                                                                                                        string                 exceptionAditionalMessage)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this MlResult<T>             source,
                                                                                                                                   Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                                   Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                                   Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                                   Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                                   Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                                   Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                                   Func<T, MlResult<TR7>>  funcBuild7)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this MlResult<T>             source,
                                                                                                                                   Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                                   Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                                   Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                                   Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                                   Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                                   Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                                   Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                                   Func<Exception, string> errorMessageBuilder)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, errorMessageBuilder: errorMessageBuilder).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this MlResult<T>            source,
                                                                                                                                   Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                                   Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                                   Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                                   Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                                   Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                                   Func<T, MlResult<TR6>> funcBuild6,
                                                                                                                                   Func<T, MlResult<TR7>> funcBuild7,
                                                                                                                                   string                 exceptionAditionalMessage)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, errorMessageBuilder: ex => exceptionAditionalMessage).ToAsync();

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this Task<MlResult<T>>      sourceAsync,
                                                                                                                                          Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                                          Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                                          Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                                          Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                                          Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                                          Func<T, MlResult<TR6>> funcBuild6,
                                                                                                                                          Func<T, MlResult<TR7>> funcBuild7)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7);

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this Task<MlResult<T>>       sourceAsync,
                                                                                                                                          Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                                          Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                                          Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                                          Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                                          Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                                          Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                                          Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                                          Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, errorMessageBuilder: errorMessageBuilder);

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this Task<MlResult<T>>      sourceAsync,
                                                                                                                                          Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                                          Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                                          Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                                          Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                                          Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                                          Func<T, MlResult<TR6>> funcBuild6,
                                                                                                                                          Func<T, MlResult<TR7>> funcBuild7,
                                                                                                                                          string                 exceptionAditionalMessage)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this Task<MlResult<T>>            sourceAsync,
                                                                                                                                   Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                                   Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                                   Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                                   Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                                   Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                                   Func<T, Task<MlResult<TR6>>> funcBuild6Async,
                                                                                                                                   Func<T, Task<MlResult<TR7>>> funcBuild7Async)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, funcBuild7Async, errorMessageBuilder: null!);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this Task<MlResult<T>>            sourceAsync,
                                                                                                                                   Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                                   Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                                   Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                                   Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                                   Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                                   Func<T, Task<MlResult<TR6>>> funcBuild6Async,
                                                                                                                                   Func<T, Task<MlResult<TR7>>> funcBuild7Async,
                                                                                                                                   Func<Exception, string>      errorMessageBuilder)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, funcBuild7Async, errorMessageBuilder: errorMessageBuilder);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this Task<MlResult<T>>            sourceAsync,
                                                                                                                                   Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                                   Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                                   Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                                   Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                                   Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                                   Func<T, Task<MlResult<TR6>>> funcBuild6Async,
                                                                                                                                   Func<T, Task<MlResult<TR7>>> funcBuild7Async,
                                                                                                                                   string                        exceptionAditionalMessage)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, funcBuild7Async, errorMessageBuilder: ex => exceptionAditionalMessage);





    public static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this MlResult<T>             source,
                                                                                                                        Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                        Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                        Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                        Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                        Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                        Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                        Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                        Func<T, MlResult<TR8>>  funcBuild8)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, funcBuild8, errorMessageBuilder: null!);

    public static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this MlResult<T>             source,
                                                                                                                        Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                        Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                        Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                        Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                        Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                        Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                        Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                        Func<T, MlResult<TR8>> funcBuild8,
                                                                                                                        Func<Exception, string> errorMessageBuilder)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, funcBuild8, errorMessageBuilder: errorMessageBuilder);

    public static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)> TryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this MlResult<T>            source,
                                                                                                                        Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                        Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                        Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                        Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                        Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                        Func<T, MlResult<TR6>> funcBuild6,
                                                                                                                        Func<T, MlResult<TR7>> funcBuild7,
                                                                                                                        Func<T, MlResult<TR8>> funcBuild8,
                                                                                                                        string                 exceptionAditionalMessage)
        => source.InternalTryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, funcBuild8, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this MlResult<T>             source,
                                                                                                                                   Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                                   Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                                   Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                                   Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                                   Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                                   Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                                   Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                                   Func<T, MlResult<TR8>> funcBuild8)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, funcBuild8).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this MlResult<T>             source,
                                                                                                                                   Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                                   Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                                   Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                                   Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                                   Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                                   Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                                   Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                                   Func<T, MlResult<TR8>>  funcBuild8,
                                                                                                                                   Func<Exception, string> errorMessageBuilder)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, funcBuild8, errorMessageBuilder: errorMessageBuilder).ToAsync();

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this MlResult<T>            source,
                                                                                                                                   Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                                   Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                                   Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                                   Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                                   Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                                   Func<T, MlResult<TR6>> funcBuild6,
                                                                                                                                   Func<T, MlResult<TR7>> funcBuild7,
                                                                                                                                   Func<T, MlResult<TR8>> funcBuild8,
                                                                                                                                   string                 exceptionAditionalMessage)
        => source.TryBindBuild(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, funcBuild8, errorMessageBuilder: ex => exceptionAditionalMessage).ToAsync();

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this Task<MlResult<T>>      sourceAsync,
                                                                                                                                          Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                                          Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                                          Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                                          Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                                          Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                                          Func<T, MlResult<TR6>> funcBuild6,
                                                                                                                                          Func<T, MlResult<TR7>> funcBuild7,
                                                                                                                                          Func<T, MlResult<TR8>> funcBuild8)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, funcBuild8);

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this Task<MlResult<T>>       sourceAsync,
                                                                                                                                          Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                                          Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                                          Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                                          Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                                          Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                                          Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                                          Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                                          Func<T, MlResult<TR8>>  funcBuild8,
                                                                                                                                          Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, funcBuild8, errorMessageBuilder: errorMessageBuilder);

    public static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this Task<MlResult<T>>      sourceAsync,
                                                                                                                                          Func<T, MlResult<TR1>> funcBuild1,
                                                                                                                                          Func<T, MlResult<TR2>> funcBuild2,
                                                                                                                                          Func<T, MlResult<TR3>> funcBuild3,
                                                                                                                                          Func<T, MlResult<TR4>> funcBuild4,
                                                                                                                                          Func<T, MlResult<TR5>> funcBuild5,
                                                                                                                                          Func<T, MlResult<TR6>> funcBuild6,
                                                                                                                                          Func<T, MlResult<TR7>> funcBuild7,
                                                                                                                                          Func<T, MlResult<TR8>> funcBuild8,
                                                                                                                                          string                 exceptionAditionalMessage)
        => await (await sourceAsync).TryBindBuildAsync(funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, funcBuild8, errorMessageBuilder: ex => exceptionAditionalMessage);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this Task<MlResult<T>>            sourceAsync,
                                                                                                                                   Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                                   Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                                   Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                                   Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                                   Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                                   Func<T, Task<MlResult<TR6>>> funcBuild6Async,
                                                                                                                                   Func<T, Task<MlResult<TR7>>> funcBuild7Async,
                                                                                                                                   Func<T, Task<MlResult<TR8>>> funcBuild8Async)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, funcBuild7Async, funcBuild8Async, errorMessageBuilder: null!);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this Task<MlResult<T>>            sourceAsync,
                                                                                                                                   Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                                   Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                                   Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                                   Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                                   Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                                   Func<T, Task<MlResult<TR6>>> funcBuild6Async,
                                                                                                                                   Func<T, Task<MlResult<TR7>>> funcBuild7Async,
                                                                                                                                   Func<T, Task<MlResult<TR8>>> funcBuild8Async,
                                                                                                                                   Func<Exception, string>      errorMessageBuilder)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, funcBuild7Async, funcBuild8Async, errorMessageBuilder: errorMessageBuilder);

    public static Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)>> TryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this Task<MlResult<T>>            sourceAsync,
                                                                                                                                   Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                                   Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                                   Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                                   Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                                   Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                                   Func<T, Task<MlResult<TR6>>> funcBuild6Async,
                                                                                                                                   Func<T, Task<MlResult<TR7>>> funcBuild7Async,
                                                                                                                                   Func<T, Task<MlResult<TR8>>> funcBuild8Async,
                                                                                                                                   string                        exceptionAditionalMessage)
        => sourceAsync.InternalTryBindBuildAsync(funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, funcBuild7Async, funcBuild8Async, errorMessageBuilder: ex => exceptionAditionalMessage);





        #region private Tuple MlResult<T>


    private static MlResult<(TR1, TR2)> InternalTryBindBuild<T, TR1, TR2>(this MlResult<T>             source,
                                                                                Func<T, MlResult<TR1>>  funcBuild1,
                                                                                Func<T, MlResult<TR2>>  funcBuild2,
                                                                                Func<Exception, string> errorMessageBuilder = null!)
    {
        var result =                EnsureFp.NotNull(funcBuild1, $"The parameter {nameof(funcBuild1)}, can't be empty.")
                        .Bind( _ => EnsureFp.NotNull(funcBuild2, $"The parameter {nameof(funcBuild2)}, can't be empty."))
                        .Bind( _ => source)
                        .Bind( x => InternalTryBindBuild(source: x, funcBuild1, funcBuild2, errorMessageBuilder));
        return result;
    }

    private static MlResult<(TR1, TR2, TR3)> InternalTryBindBuild<T, TR1, TR2, TR3>(this MlResult<T>             source,
                                                                                            Func<T, MlResult<TR1>>  funcBuild1,
                                                                                            Func<T, MlResult<TR2>>  funcBuild2,
                                                                                            Func<T, MlResult<TR3>>  funcBuild3,
                                                                                            Func<Exception, string> errorMessageBuilder = null!)
    {
        var result =                EnsureFp.NotNull(funcBuild1, $"The parameter {nameof(funcBuild1)}, can't be empty.")
                        .Bind( _ => EnsureFp.NotNull(funcBuild2, $"The parameter {nameof(funcBuild2)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild3, $"The parameter {nameof(funcBuild3)}, can't be empty."))
                        .Bind( _ => source)
                        .Bind( x => InternalTryBindBuild(source: x, funcBuild1, funcBuild2, funcBuild3, errorMessageBuilder));
        return result;
    }

    private static MlResult<(TR1, TR2, TR3, TR4)> InternalTryBindBuild<T, TR1, TR2, TR3, TR4>(this MlResult<T>             source,
                                                                                                    Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                    Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                    Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                    Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                    Func<Exception, string> errorMessageBuilder = null!)
    {
        var result =                EnsureFp.NotNull(funcBuild1, $"The parameter {nameof(funcBuild1)}, can't be empty.")
                        .Bind( _ => EnsureFp.NotNull(funcBuild2, $"The parameter {nameof(funcBuild2)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild3, $"The parameter {nameof(funcBuild3)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild4, $"The parameter {nameof(funcBuild4)}, can't be empty."))
                        .Bind( _ => source)
                        .Bind( x => InternalTryBindBuild(source: x, funcBuild1, funcBuild2, funcBuild3, funcBuild4, errorMessageBuilder));
        return result;
    }

    private static MlResult<(TR1, TR2, TR3, TR4, TR5)> InternalTryBindBuild<T, TR1, TR2, TR3, TR4, TR5>(this MlResult<T>             source,
                                                                                                                Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                Func<Exception, string> errorMessageBuilder = null!)
    {
        var result =                EnsureFp.NotNull(funcBuild1, $"The parameter {nameof(funcBuild1)}, can't be empty.")
                        .Bind( _ => EnsureFp.NotNull(funcBuild2, $"The parameter {nameof(funcBuild2)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild3, $"The parameter {nameof(funcBuild3)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild4, $"The parameter {nameof(funcBuild4)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild5, $"The parameter {nameof(funcBuild5)}, can't be empty."))
                        .Bind( _ => source)
                        .Bind( x => InternalTryBindBuild(source: x, funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, errorMessageBuilder));
        return result;
    }

    private static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)> InternalTryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6>(this MlResult<T>             source,
                                                                                                                        Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                        Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                        Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                        Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                        Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                        Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                        Func<Exception, string> errorMessageBuilder = null!)
    {
        var result =                EnsureFp.NotNull(funcBuild1, $"The parameter {nameof(funcBuild1)}, can't be empty.")
                        .Bind( _ => EnsureFp.NotNull(funcBuild2, $"The parameter {nameof(funcBuild2)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild3, $"The parameter {nameof(funcBuild3)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild4, $"The parameter {nameof(funcBuild4)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild5, $"The parameter {nameof(funcBuild5)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild6, $"The parameter {nameof(funcBuild6)}, can't be empty."))
                        .Bind( _ => source)
                        .Bind( x => InternalTryBindBuild(source: x, funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, errorMessageBuilder));
        return result;
    }

    private static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)> InternalTryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this MlResult<T>             source,
                                                                                                                                    Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                                    Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                                    Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                                    Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                                    Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                                    Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                                    Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                                    Func<Exception, string> errorMessageBuilder = null!)
    {
        var result =                EnsureFp.NotNull(funcBuild1, $"The parameter {nameof(funcBuild1)}, can't be empty.")
                        .Bind( _ => EnsureFp.NotNull(funcBuild2, $"The parameter {nameof(funcBuild2)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild3, $"The parameter {nameof(funcBuild3)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild4, $"The parameter {nameof(funcBuild4)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild5, $"The parameter {nameof(funcBuild5)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild6, $"The parameter {nameof(funcBuild6)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild7, $"The parameter {nameof(funcBuild7)}, can't be empty."))
                        .Bind( _ => source)
                        .Bind( x => InternalTryBindBuild(source: x, funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, errorMessageBuilder));
        return result;
    }

    private static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)> InternalTryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this MlResult<T>             source,
                                                                                                                                            Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                                            Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                                            Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                                            Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                                            Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                                            Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                                            Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                                            Func<T, MlResult<TR8>>  funcBuild8,
                                                                                                                                            Func<Exception, string> errorMessageBuilder = null!)
    {
        var result =                EnsureFp.NotNull(funcBuild1, $"The parameter {nameof(funcBuild1)}, can't be empty.")
                        .Bind( _ => EnsureFp.NotNull(funcBuild2, $"The parameter {nameof(funcBuild2)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild3, $"The parameter {nameof(funcBuild3)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild4, $"The parameter {nameof(funcBuild4)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild5, $"The parameter {nameof(funcBuild5)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild6, $"The parameter {nameof(funcBuild6)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild7, $"The parameter {nameof(funcBuild7)}, can't be empty."))
                        .Bind( _ => EnsureFp.NotNull(funcBuild8, $"The parameter {nameof(funcBuild8)}, can't be empty."))
                        .Bind( _ => source)
                        .Bind( x => InternalTryBindBuild(source: x, funcBuild1, funcBuild2, funcBuild3, funcBuild4, funcBuild5, funcBuild6, funcBuild7, funcBuild8, errorMessageBuilder));
        return result;
    }




    private static async Task<MlResult<(TR1, TR2)>> InternalTryBindBuildAsync<T, TR1, TR2>(this Task<MlResult<T>>             sourceAsync,
                                                                                                Func<T, Task<MlResult<TR1>>>  funcBuild1Async,
                                                                                                Func<T, Task<MlResult<TR2>>>  funcBuild2Async,
                                                                                                Func<Exception, string>       errorMessageBuilder = null!)
    {
        var result = await               EnsureFp.NotNullAsync(funcBuild1Async, $"The parameter {nameof(funcBuild1Async)}, can't be empty.")
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild2Async, $"The parameter {nameof(funcBuild2Async)}, can't be empty."))
                        .BindAsync( _ => sourceAsync)
                        .BindAsync( x => InternalTryBindBuildAsync(source: x, funcBuild1Async, funcBuild2Async, errorMessageBuilder));
        return result;
    }

    private static async Task<MlResult<(TR1, TR2, TR3)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3>(this Task<MlResult<T>>             sourceAsync,
                                                                                                          Func<T, Task<MlResult<TR1>>>  funcBuild1Async,
                                                                                                          Func<T, Task<MlResult<TR2>>>  funcBuild2Async,
                                                                                                          Func<T, Task<MlResult<TR3>>>  funcBuild3Async,
                                                                                                          Func<Exception, string>       errorMessageBuilder = null!)
    {
        var result = await               EnsureFp.NotNullAsync(funcBuild1Async, $"The parameter {nameof(funcBuild1Async)}, can't be empty.")
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild2Async, $"The parameter {nameof(funcBuild2Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild3Async, $"The parameter {nameof(funcBuild3Async)}, can't be empty."))
                        .BindAsync( _ => sourceAsync)
                        .BindAsync( x => InternalTryBindBuildAsync(source: x, funcBuild1Async, funcBuild2Async, funcBuild3Async, errorMessageBuilder));
        return result;
    }

    private static async Task<MlResult<(TR1, TR2, TR3, TR4)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3, TR4>(this Task<MlResult<T>>             sourceAsync,
                                                                                                                    Func<T, Task<MlResult<TR1>>>  funcBuild1Async,
                                                                                                                    Func<T, Task<MlResult<TR2>>>  funcBuild2Async,
                                                                                                                    Func<T, Task<MlResult<TR3>>>  funcBuild3Async,
                                                                                                                    Func<T, Task<MlResult<TR4>>>  funcBuild4Async,
                                                                                                                    Func<Exception, string>       errorMessageBuilder = null!)
    {
        var result = await               EnsureFp.NotNullAsync(funcBuild1Async, $"The parameter {nameof(funcBuild1Async)}, can't be empty.")
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild2Async, $"The parameter {nameof(funcBuild2Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild3Async, $"The parameter {nameof(funcBuild3Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild4Async, $"The parameter {nameof(funcBuild4Async)}, can't be empty."))
                        .BindAsync( _ => sourceAsync)
                        .BindAsync( x => InternalTryBindBuildAsync(source: x, funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, errorMessageBuilder));
        return result;
    }

    private static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5>(this Task<MlResult<T>>             sourceAsync,
                                                                                                                              Func<T, Task<MlResult<TR1>>>  funcBuild1Async,
                                                                                                                              Func<T, Task<MlResult<TR2>>>  funcBuild2Async,
                                                                                                                              Func<T, Task<MlResult<TR3>>>  funcBuild3Async,
                                                                                                                              Func<T, Task<MlResult<TR4>>>  funcBuild4Async,
                                                                                                                              Func<T, Task<MlResult<TR5>>>  funcBuild5Async,
                                                                                                                              Func<Exception, string>       errorMessageBuilder = null!)
    {
        var result = await               EnsureFp.NotNullAsync(funcBuild1Async, $"The parameter {nameof(funcBuild1Async)}, can't be empty.")
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild2Async, $"The parameter {nameof(funcBuild2Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild3Async, $"The parameter {nameof(funcBuild3Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild4Async, $"The parameter {nameof(funcBuild4Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild5Async, $"The parameter {nameof(funcBuild5Async)}, can't be empty."))
                        .BindAsync( _ => sourceAsync)
                        .BindAsync( x => InternalTryBindBuildAsync(source: x, funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, errorMessageBuilder));
        return result;
    }

    private static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6>(this Task<MlResult<T>>             sourceAsync,
                                                                                                                                        Func<T, Task<MlResult<TR1>>>  funcBuild1Async,
                                                                                                                                        Func<T, Task<MlResult<TR2>>>  funcBuild2Async,
                                                                                                                                        Func<T, Task<MlResult<TR3>>>  funcBuild3Async,
                                                                                                                                        Func<T, Task<MlResult<TR4>>>  funcBuild4Async,
                                                                                                                                        Func<T, Task<MlResult<TR5>>>  funcBuild5Async,
                                                                                                                                        Func<T, Task<MlResult<TR6>>>  funcBuild6Async,
                                                                                                                                        Func<Exception, string>       errorMessageBuilder = null!)
    {
        var result = await               EnsureFp.NotNullAsync(funcBuild1Async, $"The parameter {nameof(funcBuild1Async)}, can't be empty.")
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild2Async, $"The parameter {nameof(funcBuild2Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild3Async, $"The parameter {nameof(funcBuild3Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild4Async, $"The parameter {nameof(funcBuild4Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild5Async, $"The parameter {nameof(funcBuild5Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild6Async, $"The parameter {nameof(funcBuild6Async)}, can't be empty."))
                        .BindAsync( _ => sourceAsync)
                        .BindAsync( x => InternalTryBindBuildAsync(source: x, funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, errorMessageBuilder));
        return result;
    }

    private static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(this Task<MlResult<T>>             sourceAsync,
                                                                                                                                                  Func<T, Task<MlResult<TR1>>>  funcBuild1Async,
                                                                                                                                                  Func<T, Task<MlResult<TR2>>>  funcBuild2Async,
                                                                                                                                                  Func<T, Task<MlResult<TR3>>>  funcBuild3Async,
                                                                                                                                                  Func<T, Task<MlResult<TR4>>>  funcBuild4Async,
                                                                                                                                                  Func<T, Task<MlResult<TR5>>>  funcBuild5Async,
                                                                                                                                                  Func<T, Task<MlResult<TR6>>>  funcBuild6Async,
                                                                                                                                                  Func<T, Task<MlResult<TR7>>>  funcBuild7Async,
                                                                                                                                                  Func<Exception, string>       errorMessageBuilder = null!)
    {
        var result = await               EnsureFp.NotNullAsync(funcBuild1Async, $"The parameter {nameof(funcBuild1Async)}, can't be empty.")
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild2Async, $"The parameter {nameof(funcBuild2Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild3Async, $"The parameter {nameof(funcBuild3Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild4Async, $"The parameter {nameof(funcBuild4Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild5Async, $"The parameter {nameof(funcBuild5Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild6Async, $"The parameter {nameof(funcBuild6Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild7Async, $"The parameter {nameof(funcBuild7Async)}, can't be empty."))
                        .BindAsync( _ => sourceAsync)
                        .BindAsync( x => InternalTryBindBuildAsync(source: x, funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, funcBuild7Async, errorMessageBuilder));
        return result;
    }

    private static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(this Task<MlResult<T>>             sourceAsync,
                                                                                                                                                            Func<T, Task<MlResult<TR1>>>  funcBuild1Async,
                                                                                                                                                            Func<T, Task<MlResult<TR2>>>  funcBuild2Async,
                                                                                                                                                            Func<T, Task<MlResult<TR3>>>  funcBuild3Async,
                                                                                                                                                            Func<T, Task<MlResult<TR4>>>  funcBuild4Async,
                                                                                                                                                            Func<T, Task<MlResult<TR5>>>  funcBuild5Async,
                                                                                                                                                            Func<T, Task<MlResult<TR6>>>  funcBuild6Async,
                                                                                                                                                            Func<T, Task<MlResult<TR7>>>  funcBuild7Async,
                                                                                                                                                            Func<T, Task<MlResult<TR8>>>  funcBuild8Async,
                                                                                                                                                            Func<Exception, string>       errorMessageBuilder = null!)
    {
        var result = await               EnsureFp.NotNullAsync(funcBuild1Async, $"The parameter {nameof(funcBuild1Async)}, can't be empty.")
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild2Async, $"The parameter {nameof(funcBuild2Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild3Async, $"The parameter {nameof(funcBuild3Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild4Async, $"The parameter {nameof(funcBuild4Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild5Async, $"The parameter {nameof(funcBuild5Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild6Async, $"The parameter {nameof(funcBuild6Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild7Async, $"The parameter {nameof(funcBuild7Async)}, can't be empty."))
                        .BindAsync( _ => EnsureFp.NotNullAsync(funcBuild8Async, $"The parameter {nameof(funcBuild8Async)}, can't be empty."))
                        .BindAsync( _ => sourceAsync)
                        .BindAsync( x => InternalTryBindBuildAsync(source: x, funcBuild1Async, funcBuild2Async, funcBuild3Async, funcBuild4Async, funcBuild5Async, funcBuild6Async, funcBuild7Async, funcBuild8Async, errorMessageBuilder));
        return result;
    }






    #endregion


        #region private Tuple T


    private static MlResult<(TR1, TR2)> InternalTryBindBuild<T, TR1, TR2>(T                       source,
                                                                          Func<T, MlResult<TR1>>  funcBuild1,
                                                                          Func<T, MlResult<TR2>>  funcBuild2,
                                                                          Func<Exception, string> errorMessageBuilder = null!)
        {
            var result = MlResult.Empty()
                                    .TryBind(func               : _           => funcBuild1(source), 
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild2(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }

        private static MlResult<(TR1, TR2, TR3)> InternalTryBindBuild<T, TR1, TR2, TR3>(T                       source,
                                                                                        Func<T, MlResult<TR1>>  funcBuild1,
                                                                                        Func<T, MlResult<TR2>>  funcBuild2,
                                                                                        Func<T, MlResult<TR3>>  funcBuild3,
                                                                                        Func<Exception, string> errorMessageBuilder = null!)
        {
            var result = MlResult.Empty()
                                    .TryBind(func               : _           => funcBuild1(source), 
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild2(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild3(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }

        private static MlResult<(TR1, TR2, TR3, TR4)> InternalTryBindBuild<T, TR1, TR2, TR3, TR4>(T                       source,
                                                                                                  Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                  Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                  Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                  Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                  Func<Exception, string> errorMessageBuilder = null!)
        {
            var result = MlResult.Empty()
                                    .TryBind(func               : _           => funcBuild1(source), 
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild2(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild3(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild4(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild4)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }

        private static MlResult<(TR1, TR2, TR3, TR4, TR5)> InternalTryBindBuild<T, TR1, TR2, TR3, TR4, TR5>(T                       source,
                                                                                                            Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                            Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                            Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                            Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                            Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                            Func<Exception, string> errorMessageBuilder = null!)
        {
            var result = MlResult.Empty()
                                    .TryBind(func               : _           => funcBuild1(source), 
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild2(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild3(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild4(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild4)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild5(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild5)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }

        private static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)> InternalTryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6>(T                       source,
                                                                                                                      Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                      Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                      Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                      Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                      Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                      Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                      Func<Exception, string> errorMessageBuilder = null!)
        {
            var result = MlResult.Empty()
                                    .TryBind(func               : _           => funcBuild1(source), 
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild2(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild3(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild4(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild4)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild5(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild5)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild6(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild6)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }

        private static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)> InternalTryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(T                       source,
                                                                                                                                Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                                Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                                Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                                Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                                Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                                Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                                Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                                Func<Exception, string> errorMessageBuilder = null!)
        {
            var result = MlResult.Empty()
                                    .TryBind(func               : _           => funcBuild1(source), 
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild2(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild3(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild4(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild4)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild5(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild5)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild6(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild6)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild7(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild7)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }

        private static MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)> InternalTryBindBuild<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(T                       source,
                                                                                                                                          Func<T, MlResult<TR1>>  funcBuild1,
                                                                                                                                          Func<T, MlResult<TR2>>  funcBuild2,
                                                                                                                                          Func<T, MlResult<TR3>>  funcBuild3,
                                                                                                                                          Func<T, MlResult<TR4>>  funcBuild4,
                                                                                                                                          Func<T, MlResult<TR5>>  funcBuild5,
                                                                                                                                          Func<T, MlResult<TR6>>  funcBuild6,
                                                                                                                                          Func<T, MlResult<TR7>>  funcBuild7,
                                                                                                                                          Func<T, MlResult<TR8>>  funcBuild8,
                                                                                                                                          Func<Exception, string> errorMessageBuilder = null!)
        {
            var result = MlResult.Empty()
                                    .TryBind(func               : _           => funcBuild1(source), 
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild2(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild3(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild4(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild4)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild5(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild5)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild6(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild6)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild7(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild7)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBind(func               : previewData => previewData.Combine(funcBuild8(source)),
                                             errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild8)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }


        private static async Task<MlResult<(TR1, TR2)>> InternalTryBindBuildAsync<T, TR1, TR2>(T                      source,
                                                                                         Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                         Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                         Func<Exception, string>      errorMessageBuilder = null!)
        {
            var result = await MlResult.EmptyAsync()
                                    .TryBindAsync(funcAsync          : _           => funcBuild1Async(source), 
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild2Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;

        }

        private static async Task<MlResult<(TR1, TR2, TR3)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3>(T                             source,
                                                                                                          Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                          Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                          Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                          Func<Exception, string>      errorMessageBuilder = null!)
        {
            var result = await MlResult.EmptyAsync()
                                    .TryBindAsync(funcAsync          : _           => funcBuild1Async(source), 
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild2Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild3Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }

        private static async Task<MlResult<(TR1, TR2, TR3, TR4)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3, TR4>(T                             source,
                                                                                                                    Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                    Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                    Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                    Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                    Func<Exception, string>      errorMessageBuilder = null!)
        {
            var result = await MlResult.EmptyAsync()
                                    .TryBindAsync(funcAsync          : _           => funcBuild1Async(source), 
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild2Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild3Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild4Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild4Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }

        private static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5>(T                             source,
                                                                                                                              Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                              Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                              Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                              Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                              Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                              Func<Exception, string>      errorMessageBuilder = null!)
        {
            var result = await MlResult.EmptyAsync()
                                    .TryBindAsync(funcAsync          : _           => funcBuild1Async(source), 
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild2Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild3Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild4Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild4Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild5Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild5Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }

        private static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6>(T                             source,
                                                                                                                                        Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                                        Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                                        Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                                        Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                                        Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                                        Func<T, Task<MlResult<TR6>>> funcBuild6Async,
                                                                                                                                        Func<Exception, string>      errorMessageBuilder = null!)
        {
            var result = await MlResult.EmptyAsync()
                                    .TryBindAsync(funcAsync          : _           => funcBuild1Async(source), 
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild2Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild3Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild4Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild4Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild5Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild5Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild6Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild6Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }

        private static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>(T                             source,
                                                                                                                                                  Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                                                  Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                                                  Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                                                  Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                                                  Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                                                  Func<T, Task<MlResult<TR6>>> funcBuild6Async,
                                                                                                                                                  Func<T, Task<MlResult<TR7>>> funcBuild7Async,
                                                                                                                                                  Func<Exception, string>      errorMessageBuilder = null!)
        {
            var result = await MlResult.EmptyAsync()
                                    .TryBindAsync(funcAsync          : _           => funcBuild1Async(source), 
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild2Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild3Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild4Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild4Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild5Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild5Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild6Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild6Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild7Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild7Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }

        private static async Task<MlResult<(TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8)>> InternalTryBindBuildAsync<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>(T                             source,
                                                                                                                                                            Func<T, Task<MlResult<TR1>>> funcBuild1Async,
                                                                                                                                                            Func<T, Task<MlResult<TR2>>> funcBuild2Async,
                                                                                                                                                            Func<T, Task<MlResult<TR3>>> funcBuild3Async,
                                                                                                                                                            Func<T, Task<MlResult<TR4>>> funcBuild4Async,
                                                                                                                                                            Func<T, Task<MlResult<TR5>>> funcBuild5Async,
                                                                                                                                                            Func<T, Task<MlResult<TR6>>> funcBuild6Async,
                                                                                                                                                            Func<T, Task<MlResult<TR7>>> funcBuild7Async,
                                                                                                                                                            Func<T, Task<MlResult<TR8>>> funcBuild8Async,
                                                                                                                                                            Func<Exception, string>      errorMessageBuilder = null!)
        {
            var result = await MlResult.EmptyAsync()
                                    .TryBindAsync(funcAsync          : _           => funcBuild1Async(source), 
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild1Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild2Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild2Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild3Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild3Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild4Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild4Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild5Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild5Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild6Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild6Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild7Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild7Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder))
                                    .TryBindAsync(funcAsync          : previewData => previewData.CombineAsync(funcBuild8Async(source)),
                                                  errorMessageBuilder: ex          => CreatePartialErrorMessage(ex, $"Unexpected error applying the {nameof(funcBuild8Async)} in {nameof(TryBindBuild)}: {ex.Message}", errorMessageBuilder));
            return result;
        }


    #endregion








    #endregion


    #region TryBindBuildWhile


    public static MlResult<TResult> TryBindBuildWhile<T, TResult>(this   MlResult<T>                 source,
                                                                      params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuild<T, TResult>(true, errorMessageBuilder: null!, funcArgs: funcArgs);

        public static MlResult<TResult> TryBindBuildWhile<T, TResult>(this   MlResult<T>                 source,
                                                                             Func<Exception, string>     errorMessageBuilder,
                                                                      params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuild<T, TResult>(true, errorMessageBuilder, funcArgs);

        public static MlResult<TResult> TryBindBuildWhile<T, TResult>(this   MlResult<T>                 source,
                                                                             string                      exceptionAditionalMessage,
                                                                      params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuild<T, TResult>(true, exceptionAditionalMessage, funcArgs);


    public static Task<MlResult<TResult>> TryBindBuildWhileAsync<T, TResult>(this   MlResult<T>                 source,
                                                                             params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuildSyncAsync<T, TResult>(true, errorMessageBuilder: null!, funcArgs: funcArgs);

    public static Task<MlResult<TResult>> TryBindBuildWhileAsync<T, TResult>(this   MlResult<T>                 source,
                                                                                    Func<Exception, string>     errorMessageBuilder,
                                                                             params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuildSyncAsync<T, TResult>(true, errorMessageBuilder, funcArgs);

    public static Task<MlResult<TResult>> TryBindBuildWhileAsync<T, TResult>(this   MlResult<T>                 source,
                                                                                    string                      exceptionAditionalMessage,
                                                                             params Func<T, MlResult<object>>[] funcArgs)
        => source.InternalTryBindBuildSyncAsync<T, TResult>(true, ex => exceptionAditionalMessage, funcArgs);


    public static async Task<MlResult<TResult>> TryBindBuildWhileAsync<T, TResult>(this   Task<MlResult<T>>           sourceAsync,
                                                                                   params Func<T, MlResult<object>>[] funcArgs)
        => await sourceAsync.InternalTryBindBuildSyncAsync<T, TResult>(true, errorMessageBuilder: null!, funcArgs: funcArgs);

    public static async Task<MlResult<TResult>> TryBindBuildWhileAsync<T, TResult>(this   Task<MlResult<T>>           sourceAsync,
                                                                                          Func<Exception, string>     errorMessageBuilder,
                                                                                   params Func<T, MlResult<object>>[] funcArgs)
        => await sourceAsync.InternalTryBindBuildSyncAsync<T, TResult>(true, errorMessageBuilder, funcArgs);

    public static async Task<MlResult<TResult>> TryBindBuildWhileAsync<T, TResult>(this   Task<MlResult<T>>           sourceAsync,
                                                                                          string                      exceptionAditionalMessage,
                                                                                   params Func<T, MlResult<object>>[] funcArgs)
        => await sourceAsync.InternalTryBindBuildSyncAsync<T, TResult>(true, exceptionAditionalMessage, funcArgs);

    public static async Task<MlResult<TResult>> TryBindBuildWhileAsync<T, TResult>(this   Task<MlResult<T>>                 sourceAsync,
                                                                                   params Func<T, Task<MlResult<object>>>[] funcArgsAsync)
        => await sourceAsync.InternalTryBindBuildAsync<T, TResult>(true, errorMessageBuilder: null!, funcArgsAsync: funcArgsAsync);

    public static async Task<MlResult<TResult>> TryBindBuildWhileAsync<T, TResult>(this   Task<MlResult<T>>                 sourceAsync,
                                                                                          Func<Exception, string>           errorMessageBuilder,
                                                                                   params Func<T, Task<MlResult<object>>>[] funcArgsAsync)
        => await sourceAsync.InternalTryBindBuildAsync<T, TResult>(true, errorMessageBuilder, funcArgsAsync);

    public static async Task<MlResult<TResult>> TryBindBuildWhileAsync<T, TResult>(this   Task<MlResult<T>>                 sourceAsync,
                                                                                          string                            exceptionAditionalMessage,
                                                                                   params Func<T, Task<MlResult<object>>>[] funcArgsAsync)
        => await sourceAsync.InternalTryBindBuildAsync<T, TResult>(true, ex => exceptionAditionalMessage, funcArgsAsync);


    #endregion


    #region Private Methods


    private static MlResult<IEnumerable<object>> ApplyValues<T>(       MlResult<T>                 source,
                                                                       bool                        breakInError,
                                                                params Func<T, MlResult<object>>[] funcTransforms)
    {
        var result = source.Bind(value => {
                                                var partialResult = new List<MlResult<Object>>();

                                                foreach (var func in funcTransforms)
                                                {
                                                    var mlResultTyped = func(value);

                                                    var mlResult = mlResultTyped.IsValid 
                                                                        ? ApplyValue(func(value)) 
                                                                        : mlResultTyped.ErrorsDetails.ToMlResultFail<object>();
                                                    partialResult.Add(mlResult);

                                                    if (breakInError)
                                                    {
                                                        if (mlResult.IsFail) break;
                                                    }
                                                       
                                                }
                                                return partialResult.VerifiedEnumerableResultData();
                                            });
        return result;
    }

    private static async Task<MlResult<IEnumerable<object>>> ApplyValuesAsync<T>(       MlResult<T>                       source,
                                                                                        bool                              breakInError,
                                                                                 params Func<T, Task<MlResult<object>>>[] funcAsyncTransforms)
    {
        var result = await source.BindAsync(async value => {
                                                                var partialResult = new List<MlResult<Object>>();

                                                                foreach (var func in funcAsyncTransforms)
                                                                {
                                                                    var mlResultTyped = await func(value);

                                                                    var mlResult = mlResultTyped.IsValid
                                                                                        ? await ApplyValueAsync(func(value))
                                                                                        : await mlResultTyped.ErrorsDetails.ToMlResultFailAsync<object>();
                                                                    partialResult.Add(mlResult);

                                                                    if (breakInError)
                                                                    {
                                                                        if (mlResult.IsFail) break;
                                                                    }
                                                                        
                                                                }
                                                                return partialResult.VerifiedEnumerableResultData();
                                                            });
        return result;
    }

    private static MlResult<object> ApplyValue(MlResult<object> source)
    {
        var result = source.BindIf<object, object>(condition: value => value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(MlResult<>),
                                                   funcTrue : value => value.SecureGetValueFromMlResultBoxed(),
                                                   funcFalse: value => value);

        return result;
    }  
    
    private static async Task<MlResult<object>> ApplyValueAsync(Task<MlResult<object>> sourceAsync)
        => ApplyValue(await sourceAsync);


    private static string CreatePartialErrorMessage(Exception ex, string principalMessage, Func<Exception, string> errorMessageBuilder = null!)
    {
        string result = errorMessageBuilder is not null && !string.IsNullOrWhiteSpace(errorMessageBuilder(ex))
                            ? $"{errorMessageBuilder(ex)}{Environment.NewLine}{principalMessage}"
                            : principalMessage;

        return result;
    }




    #endregion


}
