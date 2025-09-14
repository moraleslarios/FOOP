namespace MoralesLarios.OOFP.Types;
public static class MlResultActionsMatch
{

    #region Match


    public static TReturn Match<T, TReturn>(this MlResult<T>                    source,
                                                 Func<T              , TReturn> valid,
                                                 Func<MlErrorsDetails, TReturn> fail)
        => source.IsValid ? valid(source.Value) : fail(source.ErrorsDetails);

    public static async Task<TReturn> MatchAsync<T, TReturn>(this MlResult<T>                          source,
                                                                  Func<T              , Task<TReturn>> validAsync,
                                                                  Func<MlErrorsDetails, Task<TReturn>> failAsync)
        => source.IsValid ? await validAsync(source.Value) : await failAsync(source.ErrorsDetails);

    public static async Task<TReturn> MatchAsync<T, TReturn>(this MlResult<T>                          source,
                                                                  Func<T              ,      TReturn>  valid,
                                                                  Func<MlErrorsDetails, Task<TReturn>> failAsync)
        => source.IsValid ? valid(source.Value) : await failAsync(source.ErrorsDetails);

    public static async Task<TReturn> MatchAsync<T, TReturn>(this MlResult<T>                          source,
                                                                  Func<T              , Task<TReturn>> validAsync,
                                                                  Func<MlErrorsDetails,      TReturn>  fail)
        => source.IsValid ? await validAsync(source.Value) : fail(source.ErrorsDetails);



    public static async Task<TReturn> MatchAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                  Func<T              , Task<TReturn>> validAsync,
                                                                  Func<MlErrorsDetails, Task<TReturn>> failAsync)
    {
        var partialResult = (await sourceAsync);

        var result = partialResult.IsValid ? (await validAsync(partialResult.Value)) : (await failAsync(partialResult.ErrorsDetails));

        return result;
    }

    public static async Task<TReturn> MatchAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                  Func<T              ,      TReturn>  valid,
                                                                  Func<MlErrorsDetails, Task<TReturn>> failAsync)
    {
        var partialResult = (await sourceAsync);

        var result = partialResult.IsValid ? valid(partialResult.Value) : await failAsync(partialResult.ErrorsDetails);

        return result;
    }

    public static async Task<TReturn> MatchAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                  Func<T              , Task<TReturn>> validAsync,
                                                                  Func<MlErrorsDetails,      TReturn>  fail)
    {
        var partialResult = (await sourceAsync);

        var result = partialResult.IsValid ? await validAsync(partialResult.Value) : fail(partialResult.ErrorsDetails);

        return result;
    }

    public static async Task<TReturn> MatchAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                  Func<T              , TReturn> valid,
                                                                  Func<MlErrorsDetails, TReturn> fail)
        => (await sourceAsync).Match(valid, fail);






    public static MlResult<TResult> TryMatch<T, TResult>(this MlResult<T>                    source, 
                                                              Func<T,TResult>                valid,
                                                              Func<MlErrorsDetails, TResult> fail,
                                                              Func<Exception, string>        errorMessageBuilder)
        => source.Match(valid: value        => valid.TryToMlResult(value       , errorMessageBuilder),
                        fail : errorDetails => fail .TryToMlResult(errorDetails, errorMessageBuilder));


    public static MlResult<TResult> TryMatch<T, TResult>(this MlResult<T>                    source, 
                                                              Func<T,TResult>                valid,
                                                              Func<MlErrorsDetails, TResult> fail,
                                                              string                         errorMessage = null!)
        => source.TryMatch(valid, fail, _ => errorMessage);



    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,Task<TResult>>                validAsync,
                                                                         Func<MlErrorsDetails, Task<TResult>> failAsync,
                                                                         Func<Exception, string>              errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => validAsync.TryToMlResultAsync(value       , errorMessageBuilder),
                             failAsync : errorDetails => failAsync .TryToMlResultAsync(errorDetails, errorMessageBuilder));

    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,Task<TResult>>                validAsync,
                                                                         Func<MlErrorsDetails, Task<TResult>> failAsync,
                                                                         string                               errorMessage = null!)
        => source.TryMatchAsync(validAsync, failAsync, _ => errorMessage);




    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                    source, 
                                                                         Func<T,Task<TResult>>          validAsync,
                                                                         Func<MlErrorsDetails, TResult> fail,
                                                                         Func<Exception, string>        errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => validAsync.TryToMlResultAsync(value       , errorMessageBuilder),
                             failAsync : errorDetails => fail      .TryToMlResult     (errorDetails, errorMessageBuilder).ToAsync());


    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                    source, 
                                                                         Func<T,Task<TResult>>          validAsync,
                                                                         Func<MlErrorsDetails, TResult> fail,
                                                                         string                         errorMessage = null!)
        => source.TryMatchAsync(validAsync, fail, _ => errorMessage);


    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,TResult>                      valid,
                                                                         Func<MlErrorsDetails, Task<TResult>> failAsync,
                                                                         Func<Exception, string>              errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => valid     .TryToMlResult     (value       , errorMessageBuilder).ToAsync(),
                             failAsync : errorDetails => failAsync .TryToMlResultAsync(errorDetails, errorMessageBuilder));

    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,TResult>                      valid,
                                                                         Func<MlErrorsDetails, Task<TResult>> failAsync,
                                                                         string                               errorMessage = null!)
        => source.TryMatchAsync(valid, failAsync, _ => errorMessage);




    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                               Func<T,Task<TResult>>                validAsync,
                                                                               Func<MlErrorsDetails, Task<TResult>> failAsync,
                                                                               Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMatchAsync(validAsync, failAsync, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                               Func<T,Task<TResult>>                validAsync,
                                                                               Func<MlErrorsDetails, Task<TResult>> failAsync,
                                                                               string                               errorMessage = null!)
        => await (await sourceAsync).TryMatchAsync(validAsync, failAsync, errorMessage);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                               Func<T,Task<TResult>>          validAsync,
                                                                               Func<MlErrorsDetails, TResult> fail,
                                                                               Func<Exception, string>        errorMessageBuilder)
        => await (await sourceAsync).TryMatchAsync(validAsync, fail, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                               Func<T,Task<TResult>>          validAsync,
                                                                               Func<MlErrorsDetails, TResult> fail,
                                                                               string                         errorMessage = null!)
        => await (await sourceAsync).TryMatchAsync(validAsync, fail, errorMessage);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                               Func<T, TResult>                     valid,
                                                                               Func<MlErrorsDetails, Task<TResult>> failAsync,
                                                                               Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMatchAsync(valid, failAsync, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                               Func<T, TResult>                     valid,
                                                                               Func<MlErrorsDetails, Task<TResult>> failAsync,
                                                                               string                               errorMessage = null!)
        => await (await sourceAsync).TryMatchAsync(valid, failAsync, errorMessage);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                               Func<T, TResult>               valid,
                                                                               Func<MlErrorsDetails, TResult> fail,
                                                                               Func<Exception, string>        errorMessageBuilder)
        => (await sourceAsync).TryMatch(valid, fail,errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                               Func<T, TResult>               valid,
                                                                               Func<MlErrorsDetails, TResult> fail,
                                                                               string                         errorMessage = null!)
        => (await sourceAsync).TryMatch(valid, fail, errorMessage);



    #endregion Match




    #region MatchAll



    public static MlResult<TReturn> MatchAll<T, TReturn>(this MlResult<T>   source, 
                                                              Func<TReturn> funcAll)
        => funcAll();

    public static async Task<MlResult<TReturn>> MatchAllAsync<T, TReturn>(this MlResult<T>         source, 
                                                                               Func<Task<TReturn>> funcAllAsync)
        => await funcAllAsync();


    public static async Task<MlResult<TReturn>> MatchAllAsync<T, TReturn>(this Task<MlResult<T>>   sourceAsync, 
                                                                               Func<Task<TReturn>> funcAllAsync)
        => await funcAllAsync();

    public static async Task<MlResult<TReturn>> MatchAllAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync, 
                                                                               Func<TReturn>     funcAll)
        => (await sourceAsync).MatchAll(funcAll);





    public static MlResult<TReturn> TryMatchAll<T, TReturn>(this MlResult<T>             source, 
                                                                 Func<TReturn>           funcAll,
                                                                 Func<Exception, string> errorMessageBuilder)
        => source.Match(valid: _            => funcAll.TryToMlResult(errorMessageBuilder),
                        fail : errorDetails => funcAll.TryToMlResult(errorDetails, errorMessageBuilder));

    public static MlResult<TReturn> TryMatchAll<T, TReturn>(this MlResult<T>   source, 
                                                                 Func<TReturn> funcAll,
                                                                 string        errorMessage = null!)
        => TryMatchAll(source, funcAll, _ => errorMessage);


    public static Task<MlResult<TReturn>> TryMatchAllAsync<T, TReturn>(this MlResult<T>             source, 
                                                                            Func<Task<TReturn>>     funcAllAsync,
                                                                            Func<Exception, string> errorMessageBuilder)
        => source.MatchAsync(validAsync: _            => funcAllAsync.TryToMlResultAsync(errorMessageBuilder),
                             failAsync : errorDetails => funcAllAsync.TryToMlResultAsync(errorDetails, errorMessageBuilder));

    public static Task<MlResult<TReturn>> TryMatchAllAsync<T, TReturn>(this MlResult<T>         source, 
                                                                            Func<Task<TReturn>> funcAllAsync,
                                                                            string              errorMessage = null!)
        => TryMatchAllAsync(source, funcAllAsync, _ => errorMessage);

    public async static Task<MlResult<TReturn>> TryMatchAllAsync<T, TReturn>(this Task<MlResult<T>>       sourceAsync, 
                                                                                  Func<Task<TReturn>>     funcAllAsync,
                                                                                  Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMatchAllAsync(funcAllAsync, errorMessageBuilder);

    public async static Task<MlResult<TReturn>> TryMatchAllAsync<T, TReturn>(this Task<MlResult<T>>   sourceAsync, 
                                                                                  Func<Task<TReturn>> funcAllAsync,
                                                                                  string              errorMessage = null!)
        => await (await sourceAsync).TryMatchAllAsync(funcAllAsync, errorMessage);


    public async static Task<MlResult<TReturn>> TryMatchAllAsync<T, TReturn>(this Task<MlResult<T>>       sourceAsync, 
                                                                                  Func<TReturn>           funcAll,
                                                                                  Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryMatchAll(funcAll, errorMessageBuilder);

    public async static Task<MlResult<TReturn>> TryMatchAllAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync, 
                                                                                  Func<TReturn>     funcAll,
                                                                                  string            errorMessage = null!)
        => (await sourceAsync).TryMatchAll(funcAll, errorMessage);



    #endregion MatchAll



}
