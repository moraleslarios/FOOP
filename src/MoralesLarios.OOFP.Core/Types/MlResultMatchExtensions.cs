// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types;
public static class MlResultMatchExtensions
{

    #region Match


    public static TResult Match<T, TResult>(this MlResult<T>                    source,
                                                 Func<T              , TResult> valid,
                                                 Func<MlErrorDetails, TResult> fail)
        => source.IsSuccess ? valid(source.SuccessValue) : fail(source.FailureDetails);

    public static async Task<TResult> MatchAsync<T, TResult>(this MlResult<T>                          source,
                                                                  Func<T              , Task<TResult>> validAsync,
                                                                  Func<MlErrorDetails, Task<TResult>> failAsync)
        => source.IsSuccess ? await validAsync(source.SuccessValue) : await failAsync(source.FailureDetails);

    public static async Task<TResult> MatchAsync<T, TResult>(this MlResult<T>                          source,
                                                                  Func<T              ,      TResult>  valid,
                                                                  Func<MlErrorDetails, Task<TResult>> failAsync)
        => source.IsSuccess ? valid(source.SuccessValue) : await failAsync(source.FailureDetails);

    public static async Task<TResult> MatchAsync<T, TResult>(this MlResult<T>                          source,
                                                                  Func<T              , Task<TResult>> validAsync,
                                                                  Func<MlErrorDetails,      TResult>  fail)
        => source.IsSuccess ? await validAsync(source.SuccessValue) : fail(source.FailureDetails);

    public static Task<TResult> MatchAsync<T, TResult>(this MlResult<T>                    source,
                                                            Func<T              , TResult> valid,
                                                            Func<MlErrorDetails, TResult> fail)
        => source.Match(valid, fail).ToAsync();



    public static async Task<TResult> MatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                  Func<T              , Task<TResult>> validAsync,
                                                                  Func<MlErrorDetails, Task<TResult>> failAsync)
    {
        var partialResult = (await sourceAsync);

        var result = partialResult.IsSuccess ? (await validAsync(partialResult.SuccessValue)) : (await failAsync(partialResult.FailureDetails));

        return result;
    }

    public static async Task<TResult> MatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                  Func<T              ,      TResult>  valid,
                                                                  Func<MlErrorDetails, Task<TResult>> failAsync)
    {
        var partialResult = (await sourceAsync);

        var result = partialResult.IsSuccess ? valid(partialResult.SuccessValue) : await failAsync(partialResult.FailureDetails);

        return result;
    }

    public static async Task<TResult> MatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                  Func<T              , Task<TResult>> validAsync,
                                                                  Func<MlErrorDetails,      TResult>  fail)
    {
        var partialResult = (await sourceAsync);

        var result = partialResult.IsSuccess ? await validAsync(partialResult.SuccessValue) : fail(partialResult.FailureDetails);

        return result;
    }

    public static async Task<TResult> MatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                  Func<T              , TResult> valid,
                                                                  Func<MlErrorDetails, TResult> fail)
        => (await sourceAsync).Match(valid, fail);














    public static MlResult<TResult> TryMatch<T, TResult>(this MlResult<T>                    source, 
                                                              Func<T,TResult>                valid,
                                                              Func<MlErrorDetails, TResult> fail,
                                                              Func<Exception, string>        errorMessageBuilder)
        => source.Match(valid: value        => valid.TryToMlResult(value       , errorMessageBuilder),
                        fail : errorDetails => fail .TryToMlResult(errorDetails, errorMessageBuilder));


    public static MlResult<TResult> TryMatch<T, TResult>(this MlResult<T>                    source, 
                                                              Func<T,TResult>                valid,
                                                              Func<MlErrorDetails, TResult> fail,
                                                              string                         errorMessage = null!)
        => source.TryMatch(valid, fail, _ => errorMessage);



    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,Task<TResult>>                validAsync,
                                                                         Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                         Func<Exception, string>              errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => validAsync.TryToMlResultAsync(value       , errorMessageBuilder),
                             failAsync : errorDetails => failAsync .TryToMlResultAsync(errorDetails, errorMessageBuilder));

    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,Task<TResult>>                validAsync,
                                                                         Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                         string                               errorMessage = null!)
        => source.TryMatchAsync(validAsync, failAsync, _ => errorMessage);




    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                    source, 
                                                                         Func<T,Task<TResult>>          validAsync,
                                                                         Func<MlErrorDetails, TResult> fail,
                                                                         Func<Exception, string>        errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => validAsync.TryToMlResultAsync(value       , errorMessageBuilder),
                             failAsync : errorDetails => fail      .TryToMlResult     (errorDetails, errorMessageBuilder).ToAsync());


    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                    source, 
                                                                         Func<T,Task<TResult>>          validAsync,
                                                                         Func<MlErrorDetails, TResult> fail,
                                                                         string                         errorMessage = null!)
        => source.TryMatchAsync(validAsync, fail, _ => errorMessage);


    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,TResult>                      valid,
                                                                         Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                         Func<Exception, string>              errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => valid     .TryToMlResult     (value       , errorMessageBuilder).ToAsync(),
                             failAsync : errorDetails => failAsync .TryToMlResultAsync(errorDetails, errorMessageBuilder));

    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>                          source, 
                                                                         Func<T,TResult>                      valid,
                                                                         Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                         string                               errorMessage = null!)
        => source.TryMatchAsync(valid, failAsync, _ => errorMessage);

    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>              source,
                                                                         Func<T, TResult>         valid,
                                                                         Func<MlErrorDetails, TResult> fail,
                                                                         Func<Exception, string>  errorMessageBuilder)
        => source.TryMatch(valid, fail, errorMessageBuilder).ToAsync();

    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>              source,
                                                                         Func<T, TResult>         valid,
                                                                         Func<MlErrorDetails, TResult> fail,
                                                                         string                    errorMessage = null!)
        => source.TryMatchAsync(valid, fail, _ => errorMessage);




    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                               Func<T,Task<TResult>>                validAsync,
                                                                               Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                               Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMatchAsync(validAsync, failAsync, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                               Func<T,Task<TResult>>                validAsync,
                                                                               Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                               string                               errorMessage = null!)
        => await (await sourceAsync).TryMatchAsync(validAsync, failAsync, errorMessage);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                               Func<T,Task<TResult>>          validAsync,
                                                                               Func<MlErrorDetails, TResult> fail,
                                                                               Func<Exception, string>        errorMessageBuilder)
        => await (await sourceAsync).TryMatchAsync(validAsync, fail, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                               Func<T,Task<TResult>>          validAsync,
                                                                               Func<MlErrorDetails, TResult> fail,
                                                                               string                         errorMessage = null!)
        => await (await sourceAsync).TryMatchAsync(validAsync, fail, errorMessage);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                               Func<T, TResult>                     valid,
                                                                               Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                               Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMatchAsync(valid, failAsync, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync,
                                                                               Func<T, TResult>                     valid,
                                                                               Func<MlErrorDetails, Task<TResult>> failAsync,
                                                                               string                               errorMessage = null!)
        => await (await sourceAsync).TryMatchAsync(valid, failAsync, errorMessage);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                               Func<T, TResult>               valid,
                                                                               Func<MlErrorDetails, TResult> fail,
                                                                               Func<Exception, string>        errorMessageBuilder)
        => (await sourceAsync).TryMatch(valid, fail,errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync,
                                                                               Func<T, TResult>               valid,
                                                                               Func<MlErrorDetails, TResult> fail,
                                                                               string                         errorMessage = null!)
        => (await sourceAsync).TryMatch(valid, fail, errorMessage);



    #endregion Match




    #region MatchAll



    public static MlResult<TResult> Match<T, TResult>(this MlResult<T>   source, 
                                                           Func<TResult> funcAll)
        => funcAll();

    public static async Task<MlResult<TResult>> MatchAsync<T, TResult>(this MlResult<T>         source, 
                                                                            Func<Task<TResult>> funcAllAsync)
        => await funcAllAsync();

    public static Task<MlResult<TResult>> MatchAsync<T, TResult>(this MlResult<T>   source,
                                                                      Func<TResult> funcAll)
        => source.Match(funcAll).ToAsync();


    public static async Task<MlResult<TResult>> MatchAsync<T, TResult>(this Task<MlResult<T>>   sourceAsync, 
                                                                            Func<Task<TResult>> funcAllAsync)
        => await funcAllAsync();

    public static async Task<MlResult<TResult>> MatchAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, 
                                                                            Func<TResult>     funcAll)
        => (await sourceAsync).Match(funcAll);





    public static MlResult<TResult> TryMatch<T, TResult>(this MlResult<T>             source, 
                                                              Func<TResult>           funcAll,
                                                              Func<Exception, string> errorMessageBuilder)
        => source.Match(valid: _            => funcAll.TryToMlResult(errorMessageBuilder),
                        fail : errorDetails => funcAll.TryToMlResult(errorDetails, errorMessageBuilder));

    public static MlResult<TResult> TryMatch<T, TResult>(this MlResult<T>   source, 
                                                              Func<TResult> funcAll,
                                                              string        errorMessage = null!)
        => TryMatch(source, funcAll, _ => errorMessage);


    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>             source, 
                                                                         Func<Task<TResult>>     funcAllAsync,
                                                                         Func<Exception, string> errorMessageBuilder)
        => source.MatchAsync(validAsync: _            => funcAllAsync.TryToMlResultAsync(errorMessageBuilder),
                             failAsync : errorDetails => funcAllAsync.TryToMlResultAsync(errorDetails, errorMessageBuilder));

    public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this MlResult<T>         source, 
                                                                         Func<Task<TResult>> funcAllAsync,
                                                                         string              errorMessage = null!)
        => TryMatchAsync(source, funcAllAsync, _ => errorMessage);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync, 
                                                                               Func<Task<TResult>>     funcAllAsync,
                                                                               Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMatchAsync(funcAllAsync, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>   sourceAsync, 
                                                                               Func<Task<TResult>> funcAllAsync,
                                                                               string              errorMessage = null!)
        => await (await sourceAsync).TryMatchAsync(funcAllAsync, errorMessage);


    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>>       sourceAsync, 
                                                                               Func<TResult>           funcAll,
                                                                               Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryMatch(funcAll, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(this Task<MlResult<T>> sourceAsync, 
                                                                               Func<TResult>     funcAll,
                                                                               string            errorMessage = null!)
        => (await sourceAsync).TryMatch(funcAll, errorMessage);



    #endregion MatchAll



}

