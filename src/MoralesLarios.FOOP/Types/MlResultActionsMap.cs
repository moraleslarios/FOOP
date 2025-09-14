using System;

namespace MoralesLarios.OOFP.Types;
public static class MlResultActionsMap
{




    #region Map



    public static MlResult<TReturn> Map<T, TReturn>(this MlResult<T>      source, 
                                                         Func<T, TReturn> func)
        => source.Match
        (
            fail : MlResult<TReturn>.Fail,
            valid: value => func(value)
        );

    public static Task<MlResult<TReturn>> MapAsync<T, TReturn>(this MlResult<T>      source, 
                                                                    Func<T, TReturn> func)
        => source.Map(func).ToAsync();

    public static async Task<MlResult<TReturn>> MapAsync<T, TReturn>(this MlResult<T>            source,
                                                                          Func<T, Task<TReturn>> funcAsync)
        => await source.MatchAsync
        (
            failAsync : errorsDetails => Task.FromResult(MlResult<TReturn>.Fail(errorsDetails)),
            validAsync: value         => funcAsync.ToMlResultAsync(value)
        );


    public static async Task<MlResult<TReturn>> MapAsync<T, TReturn>(this Task<MlResult<T>>      sourceAsync,
                                                                          Func<T, Task<TReturn>> funcAsync)
    {
        var partialMlResult = await sourceAsync;

        var result = await partialMlResult.MatchAsync
        (
            failAsync : errorsDetails => Task.FromResult(MlResult<TReturn>.Fail(errorsDetails)),
            validAsync: value         => funcAsync.ToMlResultAsync(value)
        );

        return result;
    }

    public static async Task<MlResult<TReturn>> MapAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                          Func<T, TReturn>  func)
        => (await sourceAsync).Map(func);


    public static MlResult<TReturn> TryMap<T, TReturn>(this MlResult<T>      source, 
                                                            Func<T, TReturn> func,
                                                            string           exceptionAditionalMessage = null!)
        => TryMap(source, func, _ => exceptionAditionalMessage!);

    public static Task<MlResult<TReturn>> TryMapAsync<T, TReturn>(this MlResult<T>      source, 
                                                                       Func<T, TReturn> func,
                                                                       string           exceptionAditionalMessage = null!)
        => source.TryMap(func, exceptionAditionalMessage).ToAsync();

    public static MlResult<TReturn> TryMap<T, TReturn>(this MlResult<T>             source, 
                                                            Func<T, TReturn>        func,
                                                            Func<Exception, string> errorMessageBuilder)
        => source.Match
        (
            fail : MlResult<TReturn>.Fail,
            valid: value => func.TryToMlResult(source.Value, errorMessageBuilder)
        );

    public static Task<MlResult<TReturn>> TryMapAsync<T, TReturn>(this MlResult<T>             source, 
                                                                       Func<T, TReturn>        func,
                                                                       Func<Exception, string> errorMessageBuilder)
        => source.TryMap(func, errorMessageBuilder).ToAsync();

    public static async Task<MlResult<TReturn>> TryMapAsync<T, TReturn>(this MlResult<T>            source,
                                                                             Func<T, Task<TReturn>> funcAsync,
                                                                             string                 exceptionAditionalMessage = null!)
        => await TryMapAsync(source, funcAsync, _ => exceptionAditionalMessage!);

    public static async Task<MlResult<TReturn>> TryMapAsync<T, TReturn>(this MlResult<T>             source,
                                                                             Func<T, Task<TReturn>>  funcAsync,
                                                                             Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync
        (
            failAsync :       errorsDetails => Task.FromResult(MlResult<TReturn>.Fail(errorsDetails)),
            validAsync: async value         => await funcAsync.TryToMlResultAsync(source.Value, errorMessageBuilder)
        );


    public static async Task<MlResult<TReturn>> TryMapAsync<T, TReturn>(this Task<MlResult<T>>      sourceAsync,
                                                                             Func<T, Task<TReturn>> funcAsync,
                                                                             string                 exceptionAditionalMessage = null!)
        => await TryMapAsync(sourceAsync, funcAsync, _ => exceptionAditionalMessage!);

    public static async Task<MlResult<TReturn>> TryMapAsync<T, TReturn>(this Task<MlResult<T>>       sourceAsync,
                                                                             Func<T, Task<TReturn>>  funcAsync,
                                                                             Func<Exception, string> errorMessageBuilder)
    {
        var partialMlResult = await sourceAsync;

        var result = await partialMlResult.MatchAsync
        (
            failAsync :       errorsDetails => Task.FromResult(MlResult<TReturn>.Fail(errorsDetails)),
            validAsync: async value         => await funcAsync.TryToMlResultAsync((await sourceAsync).Value, errorMessageBuilder)
        );

        return result;
    }


    public static async Task<MlResult<TReturn>> TryMapAsync<T, TReturn>(this Task<MlResult<T>>       sourceAsync,
                                                                             Func<T, TReturn>        func,
                                                                             Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapAsync(func, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                             Func<T, TReturn>  func,
                                                                             string            exceptionAditionalMessage = null!)
        => await (await sourceAsync).TryMapAsync(func, exceptionAditionalMessage);


    #endregion


    #region MapEnsure


    //public static MlResult<T> MapEquals<T>(this MlResult<T>   source,
    //                                            Func<T, bool> predicate,
    //                                            string        errorMessage = null!)
    //{
    //    var result = source.Match(
    //                                valid: x      => predicate(x) ? x : (errorMessage ?? "The Mapp comparation is false, return Result Error.").ToMlResultFail<T>(),
    //                                fail : errors => errors.ToMlResultFail<T>()
    //                             );

    //    return result;
    //}



    /// <summary>
    /// Convierte un MlResult en Fail, si no cumple con la condición de EnsureFUnc
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="ensureFunc"></param>
    /// <param name="errorDetailsResult"></param>
    /// <returns></returns>
    public static MlResult<T> MapEnsure<T>(this MlResult<T>     source,
                                                Func<T, bool>   ensureFunc,
                                                MlErrorsDetails errorDetailsResult)
        => source.MapEnsure(ensureFunc, _ => errorDetailsResult);

    public static MlResult<T> MapEnsure<T>(this MlResult<T>              source,
                                                Func<T, bool>            ensureFunc,
                                                Func<T, MlErrorsDetails> errorDetailsResultBuilder)
    {
        var result = source.Match(
                                       valid: x      => ensureFunc(x) ? x : errorDetailsResultBuilder(x),
                                       fail : errors => errors.ToMlResultFail<T>()
                                 );

        return result;
    }

    public static MlResult<T> MapEnsure<T>(this MlResult<T>   source,
                                                Func<T, bool> ensureFunc,
                                                string        errorMessageResult)
        => source.MapEnsure(ensureFunc, errorMessageResult.ToMlErrorsDetails());

    public static MlResult<T> MapEnsure<T>(this MlResult<T>               source,
                                                Func<T, bool>             ensureFunc,
                                                Func<T, string>           errorMessageResultBuilder)
        => source.MapEnsure(ensureFunc, x => errorMessageResultBuilder(x).ToMlErrorsDetails());



    public static Task<MlResult<T>> MapEnsureAsync<T>(this MlResult<T>     source,
                                                           Func<T, bool>   ensureFunc,
                                                           MlErrorsDetails errorDetailsResult)
        => source.MapEnsure(ensureFunc, errorDetailsResult).ToAsync();

    public static Task<MlResult<T>> MapEnsureAsync<T>(this MlResult<T>              source,
                                                           Func<T, bool>            ensureFunc,
                                                           Func<T, MlErrorsDetails> errorDetailsResultBuilder)
        => source.MapEnsure(ensureFunc, errorDetailsResultBuilder).ToAsync();


    public static Task<MlResult<T>> MapEnsureAsync<T>(this MlResult<T>   source,
                                                           Func<T, bool> ensureFunc,
                                                           string        errorMessageResult)
        => source.MapEnsure(ensureFunc, errorMessageResult).ToAsync();


    public static Task<MlResult<T>> MapEnsureAsync<T>(this MlResult<T>      source,
                                                           Func<T, bool>   ensureFunc,
                                                           Func<T, string> errorMessageResultBuilder)
        => source.MapEnsure(ensureFunc, errorMessageResultBuilder).ToAsync();



    public static async Task<MlResult<T>> MapEnsureAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                 Func<T, bool>     ensureFunc,
                                                                 MlErrorsDetails   errorDetailsResult)
        => await (await sourceAsync).MapEnsureAsync(ensureFunc, errorDetailsResult);

    public static async Task<MlResult<T>> MapEnsureAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                 Func<T, bool>            ensureFunc,
                                                                 Func<T, MlErrorsDetails> errorDetailsResultBuilder)
        => await (await sourceAsync).MapEnsureAsync(ensureFunc, errorDetailsResultBuilder);


    public static async Task<MlResult<T>> MapEnsureAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                 Func<T, bool>     ensureFunc,
                                                                 string            errorMessageResult)
        => await (await sourceAsync).MapEnsureAsync(ensureFunc, errorMessageResult);

    public static async Task<MlResult<T>> MapEnsureAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                 Func<T, bool>     ensureFunc,
                                                                 Func<T, string>   errorMessageResultBuilder)
        => await (await sourceAsync).MapEnsureAsync(ensureFunc, errorMessageResultBuilder);




    #endregion


    #region MapIf

    public static MlResult<TReturn> MapIf<T, TReturn>(this MlResult<T>      source,
                                                           Func<T, bool>    condition,
                                                           Func<T, TReturn> funcTrue,
                                                           Func<T, TReturn> funcFalse)
    {
        var result = source.Match(
                                       valid: x => condition(x) ? funcTrue(x) : funcFalse(x),
                                       fail :      MlResult<TReturn>.Fail
                                 );
        return result;
    }


    public static Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this MlResult<T>      source,
                                                                      Func<T, bool>    condition,
                                                                      Func<T, TReturn> funcTrue,
                                                                      Func<T, TReturn> funcFalse)
        => source.MapIf(condition, funcTrue, funcFalse).ToAsync();




    public static async Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this MlResult<T>            source,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, Task<TReturn>> funcTrueAsync,
                                                                            Func<T, TReturn>       funcFalse)
    {
        var result = await source.MatchAsync<T, MlResult<TReturn>>(
                                           validAsync: async x => condition(x) ? await funcTrueAsync(x) : await funcFalse(x).ToAsync(),
                                           fail      :            MlResult<TReturn>.Fail
                                      );
        return result;
    }

    public static async Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this MlResult<T>            source,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, TReturn>       funcTrue,
                                                                            Func<T, Task<TReturn>> funcFalseAsync)
    {
        var result = await source.MatchAsync<T, MlResult<TReturn>>(
                                           validAsync: async x => condition(x) ? await funcTrue(x).ToAsync() : await funcFalseAsync(x),
                                           fail      :            MlResult<TReturn>.Fail
                                      );
        return result;
    }

   public static async Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this MlResult<T>             source,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, Task<TReturn>> funcTrueAsync,
                                                                            Func<T, Task<TReturn>> funcFalseAsync)
    {
        var result = await source.MatchAsync<T, MlResult<TReturn>>(
                                           validAsync: async x => condition(x) ? await funcTrueAsync(x) : await funcFalseAsync(x),
                                           fail      :            MlResult<TReturn>.Fail
                                      );
        return result;
    }
    public static async Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                            Func<T, bool>     condition,
                                                                            Func<T, TReturn>  funcTrue,
                                                                            Func<T, TReturn>  funcFalse)
        => await (await sourceAsync).MapIfAsync(condition, funcTrue, funcFalse);

    public static async Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this Task<MlResult<T>>      sourceAsync,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, Task<TReturn>> funcTrueAsync,
                                                                            Func<T, TReturn>       funcFalse)
        => await (await sourceAsync).MapIfAsync(condition, funcTrueAsync, funcFalse);

    public static async Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this Task<MlResult<T>>      sourceAsync,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, TReturn>       funcTrue,
                                                                            Func<T, Task<TReturn>> funcFalseAsync)
        => await (await sourceAsync).MapIfAsync(condition, funcTrue, funcFalseAsync);

    public static async Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this Task<MlResult<T>>      sourceAsync,
                                                                            Func<T, bool>          condition,
                                                                            Func<T, Task<TReturn>> funcTrueAsync,
                                                                            Func<T, Task<TReturn>> funcFalseAsync)
        => await (await sourceAsync).MapIfAsync(condition, funcTrueAsync, funcFalseAsync);


    // // En este grupo, no añado funcion de parámetro para cuando no se cumple la comprobación, ya que al ser el mismo tipo no seria necesario.
    // // Si quisiera hacer algo en caso de no cumplirse, me valdrían las sobrecargas de arriba

    public static MlResult<T> MapIf<T>(this MlResult<T>   source,
                                            Func<T, bool> condition,
                                            Func<T, T>    func)
    {
        var result = source.Match(
                                       valid: x => condition(x) ? func(x) : x,
                                       fail :      MlResult<T>.Fail
                                 );
        return result;
    }

    public static Task<MlResult<T>> MapIfAsync<T>(this MlResult<T>   source,
                                                       Func<T, bool> condition,
                                                       Func<T, T>    func)
        => source.MapIf(condition, func).ToAsync();

    public static async Task<MlResult<T>> MapIfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                             Func<T, bool>     condition,
                                                             Func<T, T>        func)
        => await (await sourceAsync).MapIfAsync(condition, func);

    public static async Task<MlResult<T>> MapIfAsync<T>(this MlResult<T>      source,
                                                             Func<T, bool>    condition,
                                                             Func<T, Task<T>> funcAsync)
    {
        var result = await source.MatchAsync<T, MlResult<T>>(
                                                               validAsync: async x => condition(x) ? await funcAsync(x) : await x.ToAsync(),
                                                               fail :      MlResult<T>.Fail
                                                            );
        return result;
    }

    public static async Task<MlResult<T>> MapIfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                             Func<T, bool>     condition,
                                                             Func<T, Task<T>>  funcAsync)
        => await (await sourceAsync).MapIfAsync(condition, funcAsync);






    public static MlResult<TReturn> TryMapIf<T, TReturn>(this MlResult<T>             source,
                                                              Func<T, bool>           condition,
                                                              Func<T, TReturn>        funcTrue,
                                                              Func<T, TReturn>        funcFalse,
                                                              Func<Exception, string> errorMessageBuilder)
    {
        var result = source.Match(
                                       valid: x => condition(x) 
                                                        ? funcTrue .TryToMlResult(x, errorMessageBuilder)
                                                        : funcFalse.TryToMlResult(x, errorMessageBuilder),
                                       fail :      MlResult<TReturn>.Fail
                                 );
        return result;
    }

    public static MlResult<TReturn> TryMapIf<T, TReturn>(this MlResult<T>      source,
                                                              Func<T, bool>    condition,
                                                              Func<T, TReturn> funcTrue,
                                                              Func<T, TReturn> funcFalse,
                                                              string           exceptionAditionalMessage = null!)
        => source.TryMapIf(condition, funcTrue, funcFalse, _ => exceptionAditionalMessage!);


    public static Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this MlResult<T>             source,
                                                                         Func<T, bool>           condition,
                                                                         Func<T, TReturn>        funcTrue,
                                                                         Func<T, TReturn>        funcFalse,
                                                                         Func<Exception, string> errorMessageBuilder)
        => source.TryMapIf(condition, funcTrue, funcFalse, errorMessageBuilder).ToAsync();

    public static Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this MlResult<T>      source,
                                                                         Func<T, bool>    condition,
                                                                         Func<T, TReturn> funcTrue,
                                                                         Func<T, TReturn> funcFalse,
                                                                         string           exceptionAditionalMessage = null!)
        => source.TryMapIf(condition, funcTrue, funcFalse, exceptionAditionalMessage!).ToAsync();


    public static async Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this MlResult<T>       source,
                                                                         Func<T, bool>           condition,
                                                                         Func<T, Task<TReturn>>  funcTrueAsync,
                                                                         Func<T, TReturn>        funcFalse,
                                                                         Func<Exception, string> errorMessageBuilder)
    {
        var result = await source.MatchAsync(
                                                 validAsync: async x => condition(x) 
                                                                          ? await funcTrueAsync.TryToMlResultAsync(x, errorMessageBuilder)
                                                                          : await funcFalse    .TryToMlResult     (x, errorMessageBuilder).ToAsync(),
                                                 fail: MlResult<TReturn>.Fail
                                            );
        return result;
    }

    public static async Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this MlResult<T>            source,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, Task<TReturn>> funcTrueAsync,
                                                                               Func<T, TReturn>       funcFalse,
                                                                               string                 exceptionAditionalMessage = null!)
        => await source.TryMapIfAsync(condition, funcTrueAsync, funcFalse, _ => exceptionAditionalMessage!);


    public static async Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this MlResult<T>       source,
                                                                         Func<T, bool>           condition,
                                                                         Func<T, TReturn>        funcTrue,
                                                                         Func<T, Task<TReturn>>  funcFalseAsync,
                                                                         Func<Exception, string> errorMessageBuilder)
    {
        var result = await source.MatchAsync(
                                                 validAsync: async x => condition(x) 
                                                                          ? await funcTrue      .TryToMlResult     (x, errorMessageBuilder).ToAsync()
                                                                          : await funcFalseAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                                 fail: MlResult<TReturn>.Fail
                                            );
        return result;
    }

    public static async Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this MlResult<T>            source,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, TReturn>       funcTrue,
                                                                               Func<T, Task<TReturn>> funcFalseAsync,
                                                                               string                 exceptionAditionalMessage = null!)
        => await source.TryMapIfAsync(condition, funcTrue, funcFalseAsync, _ => exceptionAditionalMessage!);


    public static async Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this MlResult<T>             source,
                                                                               Func<T, bool>           condition,
                                                                               Func<T, Task<TReturn>>  funcTrueAsync,
                                                                               Func<T, Task<TReturn>>  funcFalseAsync,
                                                                               Func<Exception, string> errorMessageBuilder)
    {
        var result = await source.MatchAsync(
                                                 validAsync: async x => condition(x) 
                                                                          ? await funcTrueAsync .TryToMlResultAsync(x, errorMessageBuilder)
                                                                          : await funcFalseAsync.TryToMlResultAsync(x, errorMessageBuilder),
                                                 fail: MlResult<TReturn>.Fail
                                            );
        return result;
    }

    public static async Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this MlResult<T>            source,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, Task<TReturn>> funcTrueAsync,
                                                                               Func<T, Task<TReturn>> funcFalseAsync,
                                                                               string                 exceptionAditionalMessage = null!)
        => await source.TryMapIfAsync(condition, funcTrueAsync, funcFalseAsync, _ => exceptionAditionalMessage!);



    public static async Task<MlResult<TReturn>> TryMapIAsyncf<T, TReturn>(this Task<MlResult<T>>       sourceAsync,
                                                                               Func<T, bool>           condition,
                                                                               Func<T, TReturn>        funcTrue,
                                                                               Func<T, TReturn>        funcFalse,
                                                                               Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrue, funcFalse, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                               Func<T, bool>     condition,
                                                                               Func<T, TReturn>  funcTrue,
                                                                               Func<T, TReturn>  funcFalse,
                                                                               string            exceptionAditionalMessage = null!)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrue, funcFalse, exceptionAditionalMessage!);


    public static async Task<MlResult<TReturn>> TryMapIAsyncf<T, TReturn>(this Task<MlResult<T>>       sourceAsync,
                                                                               Func<T, bool>           condition,
                                                                               Func<T, Task<TReturn>>  funcTrueAsync,
                                                                               Func<T, TReturn>        funcFalse,
                                                                               Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrueAsync, funcFalse, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIAsyncf<T, TReturn>(this Task<MlResult<T>>      sourceAsync,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, Task<TReturn>> funcTrueAsync,
                                                                               Func<T, TReturn>       funcFalse,
                                                                               string                 exceptionAditionalMessage = null!)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrueAsync, funcFalse, _ => exceptionAditionalMessage!);

    public static async Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this Task<MlResult<T>>       sourceAsync,
                                                                               Func<T, bool>           condition,
                                                                               Func<T, TReturn>        funcTrue,
                                                                               Func<T, Task<TReturn>>  funcFalseAsync,
                                                                               Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrue, funcFalseAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this Task<MlResult<T>>      sourceAsync,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, TReturn>       funcTrue,
                                                                               Func<T, Task<TReturn>> funcFalseAsync,
                                                                               string                 exceptionAditionalMessage = null!)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrue, funcFalseAsync, _ => exceptionAditionalMessage!);

    public static async Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this Task<MlResult<T>>       sourceAsync,
                                                                               Func<T, bool>           condition,
                                                                               Func<T, Task<TReturn>>  funcTrueAsync,
                                                                               Func<T, Task<TReturn>>  funcFalseAsync,
                                                                               Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrueAsync, funcFalseAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfAsync<T, TReturn>(this Task<MlResult<T>>      sourceAsync,
                                                                               Func<T, bool>          condition,
                                                                               Func<T, Task<TReturn>> funcTrueAsync,
                                                                               Func<T, Task<TReturn>> funcFalseAsync,
                                                                               string                 exceptionAditionalMessage = null!)
        => await (await sourceAsync).TryMapIfAsync(condition, funcTrueAsync, funcFalseAsync, _ => exceptionAditionalMessage!);



    public static MlResult<T> TryMapIf<T>(this MlResult<T>             source,
                                               Func<T, bool>           condition,
                                               Func<T, T>              func,
                                               Func<Exception, string> errorMessageBuilder)
         => source.Match
                        (
                            valid: x => condition(x)
                                            ? func.TryToMlResult(x, errorMessageBuilder)
                                            : x.ToMlResultValid(),
                            fail: MlResult<T>.Fail
                        );

    public static MlResult<T> TryMapIf<T>(this MlResult<T>   source,
                                               Func<T, bool> condition,
                                               Func<T, T>    func,
                                               string        exceptionAditionalMessage = null!)
        => source.TryMapIf(condition, func, _ => exceptionAditionalMessage!);

    public static Task<MlResult<T>> TryMapIfAsync<T>(this MlResult<T>             source,
                                                          Func<T, bool>           condition,
                                                          Func<T, T>              func,
                                                          Func<Exception, string> errorMessageBuilder)
        => source.TryMapIf(condition, func, errorMessageBuilder).ToAsync();

    public static Task<MlResult<T>> TryMapIfAsync<T>(this MlResult<T>   source,
                                                          Func<T, bool> condition,
                                                          Func<T, T>    func,
                                                          string        exceptionAditionalMessage = null!)
        => source.TryMapIf(condition, func, _ => exceptionAditionalMessage!).ToAsync();

    public static async Task<MlResult<T>> TryMapIfAsync<T>(this MlResult<T>             source,
                                                                Func<T, bool>           condition,
                                                                Func<T, Task<T>>        funcAsync,
                                                                Func<Exception, string> errorMessageBuilder)
         => await source.MatchAsync(
                                        validAsync: async x => condition(x)
                                                                    ? await funcAsync.TryToMlResultAsync(x, errorMessageBuilder)
                                                                    : await x.ToMlResultValidAsync(),
                                        fail: MlResult<T>.Fail
                                    );

    public static async Task<MlResult<T>> TryMapIfAsync<T>(this MlResult<T>      source,
                                                                Func<T, bool>    condition,
                                                                Func<T, Task<T>> funcAsync,
                                                                string           exceptionAditionalMessage = null!)
        => await source.TryMapIfAsync(condition, funcAsync, _ => exceptionAditionalMessage!);

    public static async Task<MlResult<T>> TryMapIfAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                Func<T, bool>           condition,
                                                                Func<T, Task<T>>        funcAsync,
                                                                Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfAsync(condition, funcAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> TryMapIfAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                Func<T, bool>     condition,
                                                                Func<T, Task<T>>  funcAsync,
                                                                string            exceptionAditionalMessage = null!)
        => await (await sourceAsync).TryMapIfAsync(condition, funcAsync, _ => exceptionAditionalMessage!);




    #endregion







    #region MapIfFail





    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static MlResult<T> MapIfFail<T>(this MlResult<T>              source, 
                                                Func<MlErrorsDetails, T> func)
        => source.Match
                        (
                            fail : func,
                            valid: value => value
                        );

    public static Task<MlResult<T>> MapIfFailAsync<T>(this MlResult<T>              source, 
                                                           Func<MlErrorsDetails, T> func)
        => source.MapIfFail(func).ToAsync();

    public static async Task<MlResult<T>> MapIfFailAsync<T>(this MlResult<T>                    source, 
                                                                 Func<MlErrorsDetails, Task<T>> funcAsync)
        => await source.MatchAsync<T, MlResult<T>>
                        (
                            failAsync : async errorDetails => await funcAsync(errorDetails),
                            validAsync:       value        =>       value.ToMlResultValidAsync()
                        );

    public static async Task<MlResult<T>> MapIfFailAsync<T>(this Task<MlResult<T>>              sourceAsync, 
                                                                 Func<MlErrorsDetails, Task<T>> funcAsync)
        => await (await sourceAsync).MapIfFailAsync(funcAsync);

    public static async Task<MlResult<T>> MapIfFailAsync<T>(this Task<MlResult<T>>        sourceAsync, 
                                                                 Func<MlErrorsDetails, T> func)
        => (await sourceAsync).MapIfFail(func);



    public static MlResult<T> TryMapIfFail<T>(this MlResult<T>              source, 
                                                   Func<MlErrorsDetails, T> func,
                                                   Func<Exception, string>  errorMessageBuilder)
        => source.Match
                        (
                            fail : errorDetails => func.TryToMlResult(errorDetails, errorMessageBuilder),
                            valid: value => value
                        );

    public static Task<MlResult<T>> TryMapIfFailAsync<T>(this MlResult<T>              source, 
                                                              Func<MlErrorsDetails, T> func,
                                                              Func<Exception, string>  errorMessageBuilder)
        => source.TryMapIfFail(func, errorMessageBuilder).ToAsync();

    public static MlResult<T> TryMapIfFail<T>(this MlResult<T>              source, 
                                                   Func<MlErrorsDetails, T> func,
                                                   string                   errorMessage = null!)
        => source.TryMapIfFail(func, _ => errorMessage!);

    public static Task<MlResult<T>> TryMapIfFailAsync<T>(this MlResult<T>              source, 
                                                              Func<MlErrorsDetails, T> func,
                                                              string                   errorMessage = null!)
        => source.TryMapIfFail(func, errorMessage).ToAsync();

    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this MlResult<T>                    source, 
                                                                    Func<MlErrorsDetails, Task<T>> funcAsync,
                                                                    Func<Exception, string>        errorMessageBuilder)
        => await source.MatchAsync
                        (
                            failAsync : errorDetails => funcAsync.TryToMlResultAsync(errorDetails, errorMessageBuilder),
                            validAsync: value        => value.ToMlResultValidAsync()
                        );

    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this MlResult<T>                    source, 
                                                                    Func<MlErrorsDetails, Task<T>> funcAsync,
                                                                    string                         errorMessage = null!)
        => await source.TryMapIfFailAsync(funcAsync, _ => errorMessage!);



    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this Task<MlResult<T>>              sourceAsync, 
                                                                    Func<MlErrorsDetails, Task<T>> funcAsync,
                                                                    Func<Exception, string>        errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailAsync(funcAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this Task<MlResult<T>>              sourceAsync, 
                                                                    Func<MlErrorsDetails, Task<T>> funcAsync,
                                                                    string                         errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailAsync(funcAsync, errorMessage);


    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this Task<MlResult<T>>        sourceAsync, 
                                                                    Func<MlErrorsDetails, T> func,
                                                                    Func<Exception, string>  errorMessageBuilder)
        => (await sourceAsync).TryMapIfFail(func, errorMessageBuilder);

    public static async Task<MlResult<T>> TryMapIfFailAsync<T>(this Task<MlResult<T>>        sourceAsync, 
                                                                    Func<MlErrorsDetails, T> func,
                                                                    string                   errorMessage = null!)
        => (await sourceAsync).TryMapIfFail(func, errorMessage);


    public static MlResult<TReturn> MapIfFail<T, TReturn>(this MlResult<T>                    source, 
                                                               Func<T              , TReturn> funcValid,
                                                               Func<MlErrorsDetails, TReturn> funcFail)
        => source.Match
                        (
                            fail : funcFail,
                            valid: funcValid
                        );

    public static Task<MlResult<TReturn>> MapIfFailAsync<T, TReturn>(this MlResult<T>                    source, 
                                                                          Func<T              , TReturn> funcValid,
                                                                          Func<MlErrorsDetails, TReturn> funcFail)
        => source.MapIfFail(funcValid, funcFail).ToAsync();

    public static async Task<MlResult<TReturn>> MapIfFailAsync<T, TReturn>(this MlResult<T>                          source, 
                                                                                Func<T              , Task<TReturn>> funcValidAsync,
                                                                                Func<MlErrorsDetails, Task<TReturn>> funcFailAsync)
        => await source.MatchAsync
                        (
                            failAsync : funcFailAsync,
                            validAsync: funcValidAsync
                        );



    public static async Task<MlResult<TReturn>> MapIfFailAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , Task<TReturn>> funcValidAsync,
                                                                                Func<MlErrorsDetails, Task<TReturn>> funcFailAsync)
        => await (await sourceAsync).MapIfFailAsync(funcValidAsync, funcFailAsync);

    public static async Task<MlResult<TReturn>> MapIfFailAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , TReturn>       funcValid,
                                                                                Func<MlErrorsDetails, Task<TReturn>> funcFailAsync)
        => await (await sourceAsync).MapIfFailAsync(funcValid.ToFuncTask(), funcFailAsync);

    public static async Task<MlResult<TReturn>> MapIfFailAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , Task<TReturn>> funcValidAsync,
                                                                                Func<MlErrorsDetails, TReturn>       funcFail)
        => await (await sourceAsync).MapIfFailAsync(funcValidAsync, funcFail.ToFuncTask());

    public static async Task<MlResult<TReturn>> MapIfFailAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync, 
                                                                                Func<T              , TReturn> funcValid,
                                                                                Func<MlErrorsDetails, TReturn> funcFail)
        => await sourceAsync.MapIfFailAsync(funcValid, funcFail);



    public static MlResult<TReturn> TryMapIfFail<T, TReturn>(this MlResult<T>                    source, 
                                                                  Func<T              , TReturn> funcValid,
                                                                  Func<MlErrorsDetails, TReturn> funcFail,
                                                                  Func<Exception, string>        errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => funcFail .TryToMlResult(errorsDetails, errorMessageBuilder),
                            valid: x             => funcValid.TryToMlResult(x            , errorMessageBuilder)
                        );

    public static Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this MlResult<T>                    source, 
                                                                             Func<T              , TReturn> funcValid,
                                                                             Func<MlErrorsDetails, TReturn> funcFail,
                                                                             Func<Exception, string>        errorMessageBuilder)
        => source.TryMapIfFail(funcValid, funcFail, errorMessageBuilder).ToAsync();

    public static MlResult<TReturn> TryMapIfFail<T, TReturn>(this MlResult<T>                    source, 
                                                                  Func<T              , TReturn> funcValid,
                                                                  Func<MlErrorsDetails, TReturn> funcFail,
                                                                  string                         errorMessage = null!)
        => source.TryMapIfFail(funcValid, funcFail, _ => errorMessage!);

    public static Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this MlResult<T>                    source, 
                                                                             Func<T              , TReturn> funcValid,
                                                                             Func<MlErrorsDetails, TReturn> funcFail,
                                                                             string                         errorMessage = null!)
        => source.TryMapIfFail(funcValid, funcFail, errorMessage).ToAsync();

    public static async Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this MlResult<T>                          source, 
                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFailAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await source.MatchAsync(
                            failAsync : errorsDetails => funcFailAsync .TryToMlResultAsync(errorsDetails, errorMessageBuilder),
                            validAsync: x             => funcValidAsync.TryToMlResultAsync(x            , errorMessageBuilder)
                        );

    public static async Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this MlResult<T>                          source, 
                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFailAsync,
                                                                                   string                               errorMessage = null!)
        => await source.TryMapIfFailAsync(funcValidAsync, funcFailAsync, _ => errorMessage!);

    public static async Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFailAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailAsync(funcValidAsync, funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFailAsync,
                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailAsync(funcValidAsync, funcFailAsync, errorMessage);


    public static async Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                   Func<T              , TReturn>       funcValid,
                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFailAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                   Func<T              , TReturn>       funcValid,
                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFailAsync,
                                                                                   string errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                   Func<MlErrorsDetails, TReturn>       funcFail,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                   Func<MlErrorsDetails, TReturn>       funcFail,
                                                                                   string errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    public static async Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync, 
                                                                                   Func<T              , TReturn> funcValid,
                                                                                   Func<MlErrorsDetails, TReturn> funcFail,
                                                                                   Func<Exception, string>        errorMessageBuilder)
        => await sourceAsync.TryMapIfFailAsync(funcValid, funcFail, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                   Func<T, TReturn>               funcValid,
                                                                                   Func<MlErrorsDetails, TReturn> funcFail,
                                                                                   string                         errorMessage = null!)
        => await sourceAsync.TryMapIfFailAsync(funcValid, funcFail, errorMessage);





    





    #endregion


    #region MapDefault


    public static MlResult<T> MapDefault<T>(this object source) => "Warning, MapDefault method is only valid tu debug code".ToMlResultFail<T>();

    public static async Task<MlResult<T>> MapDefaultAsync<T>(this object source) => await (source ?? new object()).MapDefault<T>().ToAsync();



    #endregion


    #region MapIfFailWithValue




    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// source parameter has a prevous 'Value' execution
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static MlResult<T> MapIfFailWithValue<T>(this MlResult<T> source,
                                                          Func<T, T> func)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetDetailValue<T>().Map(func),
                            valid: value         => value
                        );



    public static async Task<MlResult<T>> MapIfFailWithValueAsync<T>(this MlResult<T>       source,
                                                                          Func<T, Task<T>> funcAsync)
        => await source.MatchAsync
                        (
                            failAsync : errorsDetails => errorsDetails.GetDetailValue<T>().MapAsync(funcAsync),
                            validAsync: value         => value.ToMlResultValidAsync()
                        );


    public static async Task<MlResult<T>> MapIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                           Func<T, Task<T>> funcAsync)
        => await (await sourceAsync).MapIfFailWithValueAsync(funcAsync);

    public static async Task<MlResult<T>> MapIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                           Func<T, T>       func)
        => (await sourceAsync).MapIfFailWithValue(func);



    public static MlResult<T> TryMapIfFailWithValue<T>(this MlResult<T>             source,
                                                            Func<T, T>              funcValue,
                                                            Func<Exception, string> errorMessageBuilder)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetDetailValue<T>().Bind(x => funcValue.TryToMlResult(x, errorMessageBuilder)),
                            valid: value         => value
                        );

    public static MlResult<T> TryMapIfFailWithValue<T>(this MlResult<T> source,
                                                            Func<T, T>  funcValue,
                                                            string      errorMessage = null!)
        => source.TryMapIfFailWithValue(funcValue, _ => errorMessage!);

    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this MlResult<T>             source,
                                                                             Func<T, Task<T>>        funcValueAsync,
                                                                             Func<Exception, string> errorMessageBuilder)
        => await source.MatchAsync
                        (
                            failAsync : errorsDetails => errorsDetails.GetDetailValue<T>().BindAsync(x => funcValueAsync.TryToMlResultAsync(x, errorMessageBuilder)),
                            validAsync: value         => value.ToMlResultValidAsync()
                        );

    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this MlResult<T>      source,
                                                                             Func<T, Task<T>> funcValueAsync,
                                                                             string           errorMessage = null!)
        => await source.TryMapIfFailWithValueAsync(funcValueAsync, _ => errorMessage!);


    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                             Func<T, Task<T>>        funcValueAsync,
                                                                             Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValueAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                             Func<T, Task<T>>  funcValueAsync,
                                                                             string            errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValueAsync, errorMessage);

    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                             Func<T, T>              funcValue,
                                                                             Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithValue(funcValue, errorMessageBuilder);

    public static async Task<MlResult<T>> TryMapIfFailWithValueAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                             Func<T, T>        funcValue,
                                                                             string            errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithValue(funcValue, errorMessage);




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
    public static MlResult<TReturn> MapIfFailWithValue<T, TValue, TReturn>(this MlResult<T>            source,
                                                                                 Func<T     , TReturn> funcValid,
                                                                                 Func<TValue, TReturn> funcFail)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetDetailValue<TValue>().Map(value => funcFail(value)),
                            valid: value         => funcValid(value)
                        );


    public static async Task<MlResult<TReturn>> MapIfFailWithValueAsync<T, TValue, TReturn>(this MlResult<T>                 source,
                                                                                                 Func<T     , Task<TReturn>> funcValidAsync,
                                                                                                 Func<TValue, Task<TReturn>> funcFailAlways)
        => await source.MatchAsync<T, MlResult<TReturn>>
                        (
                            failAsync :       errorsDetails =>       errorsDetails.GetDetailValueAsync<TValue>().MapAsync(value => funcFailAlways(value)),
                            validAsync: async value         => await funcValidAsync(value)
                        );


    public static async Task<MlResult<TReturn>> MapIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>            sourceAsync,
                                                                                                  Func<T     , Task<TReturn>> funcValidAsync,
                                                                                                  Func<TValue, Task<TReturn>> funcFailAlways)
        => await (await sourceAsync).MapIfFailWithValueAsync(funcValidAsync, funcFailAlways);







    public static MlResult<TReturn> TryMapIfFailWithValue<T, TValue, TReturn>(this MlResult<T>             source,
                                                                                   Func<T     , TReturn>   funcValid,
                                                                                   Func<TValue, TReturn>   funcFail,
                                                                                   Func<Exception, string> errorMessageBuilder)
        => source.Match
                        (
                            fail : errorsDetails => errorsDetails.GetDetailValue<TValue>().Bind(value => funcFail.TryToMlResult(value, errorMessageBuilder)),  
                            valid: value         => funcValid.TryToMlResult(source.Value, errorMessageBuilder)
                        );

    public static MlResult<TReturn> TryMapIfFailWithValue<T, TValue, TReturn>(this MlResult<T>           source,
                                                                                   Func<T     , TReturn> funcValid,
                                                                                   Func<TValue, TReturn> funcFail,
                                                                                   string                errorMessage = null!)
        => source.TryMapIfFailWithValue(funcValid, funcFail, _ => errorMessage!);


    public static async Task<MlResult<TReturn>> TryMapIfFailWithValueAsync<T, TValue, TReturn>(this MlResult<T>                 source,
                                                                                                    Func<T     , Task<TReturn>> funcValidAsync,
                                                                                                    Func<TValue, Task<TReturn>> funcFailAsync,
                                                                                                    Func<Exception, string>     errorMessageBuilder)
        => await source.MatchAsync
                                 (
                                     failAsync : errorsDetails => errorsDetails.GetDetailValueAsync<TValue>().BindAsync(value => funcFailAsync.TryToMlResultAsync(value, errorMessageBuilder)),
                                     validAsync: value         => funcValidAsync.TryToMlResultAsync(source.Value, errorMessageBuilder)
                                 );


    public static async Task<MlResult<TReturn>> TryMapIfFailWithValueAsync<T, TValue, TReturn>(this MlResult<T>                 source,
                                                                                                    Func<T     , Task<TReturn>> funcValidAsync,
                                                                                                    Func<TValue, Task<TReturn>> funcFailAsync,
                                                                                                    string                      errorMessage = null!)
        => await source.TryMapIfFailWithValueAsync(funcValidAsync, funcFailAsync, _ => errorMessage!);


    public static async Task<MlResult<TReturn>> TryMapIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>           sourceAsync,
                                                                                                    Func<T     , Task<TReturn>> funcValidAsync,
                                                                                                    Func<TValue, Task<TReturn>> funcFailAsync,
                                                                                                    Func<Exception, string>     errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValidAsync, funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>            sourceAsync,
                                                                                                    Func<T     , Task<TReturn>> funcValidAsync,
                                                                                                    Func<TValue, Task<TReturn>> funcFailAsync,
                                                                                                    string                      errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValidAsync, funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                                                    Func<T, TReturn> funcValid,
                                                                                                    Func<TValue, Task<TReturn>> funcFailAsync,
                                                                                                    Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                                                    Func<T, TReturn> funcValid,
                                                                                                    Func<TValue, Task<TReturn>> funcFailAsync,
                                                                                                    string errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                                                    Func<T, Task<TReturn>> funcValidAsync,
                                                                                                    Func<TValue, TReturn> funcFail,
                                                                                                    Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                                                    Func<T, Task<TReturn>> funcValidAsync,
                                                                                                    Func<TValue, TReturn> funcFail,
                                                                                                    string errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithValueAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>       sourceAsync,
                                                                                                    Func<T     , TReturn>   funcValid,
                                                                                                    Func<TValue, TReturn>   funcFail,
                                                                                                    Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithValue(funcValid, funcFail, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithValueAsync<T, TValue, TReturn>(this Task<MlResult<T>>       sourceAsync,
                                                                                                    Func<T     , TReturn>   funcValid,
                                                                                                    Func<TValue, TReturn>   funcFail,
                                                                                                    string                  errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithValue(funcValid, funcFail, errorMessage);



    #endregion


    #region MapIfFailWithException


    /// En el caso de MapIfFailWithException es diferente al MapIfFailWithValue. 
    /// 
    ///     1.- MapIfFailWithValue: Si recibe un MlResult Fail sin ValueDetail, añadira´un nuevo Error al que le viene de la ejecución anterior
    ///     
    ///     2.- MapIfFailWithException: Si recibe un MlResult Fail sin ExceptionDetail, Devolvera el MlResult Fail, igual que le vino


    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// source parameter has a prevous Exception execution or 'ex' ErrorDetail
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static MlResult<T> MapIfFailWithException<T>(this MlResult<T>        source,
                                                             Func<Exception, T> funcException)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(),
                                                                                                valid: ex              => funcException(ex).ToMlResultValid()
                                                                                            ),
                            valid: value         => value
                        );

    public static async Task<MlResult<T>> MapIfFailWithExceptionAsync<T>(this MlResult<T>              source,
                                                                              Func<Exception, Task<T>> funcExceptionAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                                            failAsync :       exErrorsDetails =>              exErrorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                            validAsync: async ex              => await (await funcExceptionAsync(ex)).ToMlResultValidAsync<T>()
                                                                                                                        ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );


    public static async Task<MlResult<T>> MapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                              Func<Exception, Task<T>> funcExceptionAsync)
        => await (await sourceAsync).MapIfFailWithExceptionAsync(funcExceptionAsync);

    public static async Task<MlResult<T>> MapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>  sourceAsync,
                                                                              Func<Exception, T> funcException)
        => (await sourceAsync).MapIfFailWithException(funcException);




    public static MlResult<T> TryMapIfFailWithException<T>(this MlResult<T>             source,
                                                                Func<Exception, T>      funcException,
                                                                Func<Exception, string> errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : _  => errorsDetails.ToMlResultFail<T>(),
                                                                                                valid: ex => funcException.TryToMlResult(ex, errorMessageBuilder)
                                                                                                                            .MergeErrorsDetailsIfFail(source)
                                                                                            ),
                            valid: value         => value
                        );

    public static MlResult<T> TryMapIfFailWithException<T>(this MlResult<T>        source,
                                                                Func<Exception, T> funcException,
                                                                string             errorMessage = null!)
        => source.TryMapIfFailWithException(funcException, _ => errorMessage!);


    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this MlResult<T>               source,
                                                                                 Func<Exception, Task<T>> funcExceptionAsync,
                                                                                 Func<Exception, string>  errorMessageBuilder)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                                            failAsync : _  => errorsDetails.ToMlResultFailAsync<T>(),
                                                                                                                            validAsync: ex => funcExceptionAsync.TryToMlResultAsync(ex, errorMessageBuilder)
                                                                                                                                                                .MergeErrorsDetailsIfFailAsync(source)
                                                                                                                        ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );

    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this MlResult<T>              source,
                                                                                 Func<Exception, Task<T>> funcExceptionAsync,
                                                                                 string                   errorMessage = null!)
        => await source.TryMapIfFailWithExceptionAsync(funcExceptionAsync, _ => errorMessage!);


    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                 Func<Exception, Task<T>> funcExceptionAsync,
                                                                                 Func<Exception, string>  errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithExceptionAsync(funcExceptionAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                 Func<Exception, Task<T>> funcExceptionAsync,
                                                                                 string                   errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithExceptionAsync(funcExceptionAsync, errorMessage);

    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>       sourceAsync,
                                                                                 Func<Exception, T>      funcException,
                                                                                 Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithException(funcException, errorMessageBuilder);

    public static async Task<MlResult<T>> TryMapIfFailWithExceptionAsync<T>(this Task<MlResult<T>>  sourceAsync,
                                                                                 Func<Exception, T> funcException,
                                                                                 string             errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithException(funcException, errorMessage);







    public static MlResult<TReturn> MapIfFailWithException<T, TReturn>(this MlResult<T>              source,
                                                                            Func<T        , TReturn> funcValid,
                                                                            Func<Exception, TReturn> funcFail)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<TReturn>(),
                                                                                                valid: ex => funcFail(ex).ToMlResultValid()
                                                                                            ),
                            valid: x => funcValid(x).ToMlResultValid()
                        );

    public static async Task<MlResult<TReturn>> MapIfFailWithExceptionAsync<T, TReturn>(this MlResult<T>                    source,
                                                                                             Func<T        , Task<TReturn>> funcValidAsync,
                                                                                             Func<Exception, Task<TReturn>> funcFailAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailException().MatchAsync(
                                                                                                            failAsync : exErrorsDetails => exErrorsDetails.ToMlResultFailAsync<TReturn>(),
                                                                                                            validAsync:  async ex       => await (await funcFailAsync(ex)).ToMlResultValidAsync()
                                                                                                        ),
                                        validAsync: async x => await (await funcValidAsync(x)).ToMlResultValidAsync<TReturn>()
                                    );

    public static async Task<MlResult<TReturn>> MapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                             Func<T        , Task<TReturn>> funcValidAsync,
                                                                                             Func<Exception, Task<TReturn>> funcFailAsync)
        => await (await sourceAsync).MapIfFailWithExceptionAsync(funcValidAsync, funcFailAsync);


    public static async Task<MlResult<TReturn>> MapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                             Func<T        , TReturn>       funcValid,
                                                                                             Func<Exception, Task<TReturn>> funcFailAsync)
        => await (await sourceAsync).MapIfFailWithExceptionAsync(funcValid.ToFuncTask(), funcFailAsync);

    public static async Task<MlResult<TReturn>> MapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                             Func<T        , Task<TReturn>> funcValidAsync,
                                                                                             Func<Exception, TReturn>       funcFail)
        => await (await sourceAsync).MapIfFailWithExceptionAsync(funcValidAsync, funcFail.ToFuncTask());

    public static async Task<MlResult<TReturn>> MapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>        sourceAsync,
                                                                                             Func<T        , TReturn> funcValid,
                                                                                             Func<Exception, TReturn> funcFail)
        => await sourceAsync.MapIfFailWithExceptionAsync(funcValid, funcFail);


    public static MlResult<TReturn> TryMapIfFailWithException<T, TReturn>(this MlResult<T>              source,
                                                                               Func<T        , TReturn> funcValid,
                                                                               Func<Exception, TReturn> funcFail,
                                                                               Func<Exception, string>  errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<TReturn>(),
                                                                                                valid: ex              => funcFail.TryToMlResult(ex, errorMessageBuilder)
                                                                                                                                  .MergeErrorsDetailsIfFailDiferentTypes(source)
                                                                                            ),
                            valid: x => funcValid.TryToMlResult(x, errorMessageBuilder)
                        );

    public static MlResult<TReturn> TryMapIfFailWithException<T, TReturn>(this MlResult<T>              source,
                                                                               Func<T        , TReturn> funcValid,
                                                                               Func<Exception, TReturn> funcFail,
                                                                               string                   errorMessage = null!)
        => source.TryMapIfFailWithException(funcValid, funcFail, _ => errorMessage!);


    public static async Task<MlResult<TReturn>> TryMapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>        sourceAsync,
                                                                                                Func<T        , TReturn> funcValid,
                                                                                                Func<Exception, TReturn> funcFail,
                                                                                                Func<Exception, string>  errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithException(funcValid, funcFail, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>        sourceAsync,
                                                                                                Func<T        , TReturn> funcValid,
                                                                                                Func<Exception, TReturn> funcFail,
                                                                                                string                   errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValid, funcFail, _ => errorMessage);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , Task<TReturn>> funcValidAsync,
                                                                                                Func<Exception, Task<TReturn>> funcFailAsync,
                                                                                                Func<Exception, string>        errorMessageBuilder)
        => await sourceAsync.MatchAsync(
                            failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                failAsync : exErrorsDetails => exErrorsDetails.ToMlResultFailAsync<TReturn>(),
                                                                                                validAsync: async ex        => await (await funcFailAsync(ex)).ToMlResultValidAsync()
                                                                                                                                                                    .MergeErrorsDetailsIfFailDiferentTypesAsync(sourceAsync)
                                                                                            ),
                            validAsync:  x => funcValidAsync.TryToMlResultAsync(x, errorMessageBuilder)
                        );

    public static async Task<MlResult<TReturn>> TryMapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , Task<TReturn>> funcValidAsync,
                                                                                                Func<Exception, Task<TReturn>> funcFailAsync,
                                                                                                string                         errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValidAsync, funcFailAsync, _ => errorMessage!);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , TReturn>       funcValid,
                                                                                                Func<Exception, Task<TReturn>> funcFailAsync,
                                                                                                Func<Exception, string>        errorMessageBuilder)
        => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValid, funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , TReturn>       funcValid,
                                                                                                Func<Exception, Task<TReturn>> funcFailAsync,
                                                                                                string                         errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValid, funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , Task<TReturn>> funcValidAsync,
                                                                                                Func<Exception, TReturn>       funcFail,
                                                                                                Func<Exception, string>        errorMessageBuilder)
        => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValidAsync, funcFail, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T        , Task<TReturn>> funcValidAsync,
                                                                                                Func<Exception, TReturn>       funcFail,
                                                                                                string                         errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValidAsync, funcFail, errorMessage);

    //public static async Task<MlResult<TReturn>> TryMapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
    //                                                                                            Func<T, TReturn> funcValid,
    //                                                                                            Func<Exception, TReturn> funcFail,
    //                                                                                            Func<Exception, string> errorMessageBuilder)
    //    => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValid, funcFail, errorMessageBuilder);

    //public static async Task<MlResult<TReturn>> TryMapIfFailWithExceptionAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
    //                                                                                            Func<T, TReturn> funcValid,
    //                                                                                            Func<Exception, TReturn> funcFail,
    //                                                                                            string errorMessage = null!)
    //    => await sourceAsync.TryMapIfFailWithExceptionAsync(funcValid, funcFail, errorMessage);


    #endregion


    #region MapIfFailWithoutException


    /// <summary>
    /// Execute the function if the source is fail, otherwise return the source.
    /// The previous run cannot have raised an Exception. If this case occurs, the 'func' will not be executed
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static MlResult<T> MapIfFailWithoutException<T>(this MlResult<T>              source,
                                                                Func<MlErrorsDetails, T> func)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : errorsDetails => func(errorsDetails).ToMlResultValid<T>(),
                                                                                                valid: _             => errorsDetails
                                                                                            ),
                            valid: x             => x
                        );


    public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T>(this MlResult<T>                    source,
                                                                                 Func<MlErrorsDetails, Task<T>> funcAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                                            failAsync : async errorDetails2 => (await funcAsync(errorDetails2)).ToMlResultValid(),
                                                                                                                            validAsync: async _             => await errorsDetails.ToMlResultFailAsync<T>()
                                                                                                                        ),
                                        validAsync: value         => value.ToMlResultValidAsync()
                                    );

    public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                 Func<MlErrorsDetails, Task<T>> funcAsync)
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync(funcAsync);

    public static async Task<MlResult<T>> MapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                 Func<MlErrorsDetails, T> func)
        => (await sourceAsync).MapIfFailWithoutException(func);




    public static MlResult<T> TryMapIfFailWithoutException<T>(this MlResult<T>              source,
                                                                   Func<MlErrorsDetails, T> func,
                                                                   Func<Exception, string>  errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : _ => func.TryToMlResult(errorsDetails, errorMessageBuilder),
                                                                                                valid: _ => errorsDetails
                                                                                            ),
                            valid: x             => x
                        );

    public static MlResult<T> TryMapIfFailWithoutException<T>(this MlResult<T>              source,
                                                                   Func<MlErrorsDetails, T> func,
                                                                   string                   errorMessage = null!)
        => source.TryMapIfFailWithoutException(func, _ => errorMessage!);

    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this MlResult<T>                    source,
                                                                                    Func<MlErrorsDetails, Task<T>> funcAsync,
                                                                                    Func<Exception, string>        errorMessageBuilder)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                                            failAsync : _ => funcAsync.TryToMlResultAsync(errorsDetails, errorMessageBuilder),
                                                                                                                            validAsync: _ => errorsDetails.ToMlResultFailAsync<T>()
                                                                                                                        ),
                                        validAsync: x             => x.ToMlResultValidAsync()
                                    );

    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this MlResult<T>                    source,
                                                                                    Func<MlErrorsDetails, Task<T>> funcAsync,
                                                                                    string                         errorMessage = null!)
        => await source.TryMapIfFailWithoutExceptionAsync(funcAsync, _ => errorMessage!);


    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                    Func<MlErrorsDetails, Task<T>> funcAsync,
                                                                                    Func<Exception, string>        errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcAsync, errorMessageBuilder);

    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>              sourceAsync,
                                                                                    Func<MlErrorsDetails, Task<T>> funcAsync,
                                                                                    string                         errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcAsync, errorMessage);

    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                    Func<MlErrorsDetails, T> func,
                                                                                    Func<Exception, string>  errorMessageBuilder)
        => (await sourceAsync).TryMapIfFailWithoutException(func, errorMessageBuilder);

    public static async Task<MlResult<T>> TryMapIfFailWithoutExceptionAsync<T>(this Task<MlResult<T>>        sourceAsync,
                                                                                    Func<MlErrorsDetails, T> func,
                                                                                    string                   errorMessage = null!)
        => (await sourceAsync).TryMapIfFailWithoutException(func, errorMessage);





    public static MlResult<TReturn> MapIfFailWithoutException<T, TReturn>(this MlResult<T>                    source,
                                                                               Func<T              , TReturn> funcValid,
                                                                               Func<MlErrorsDetails, TReturn> funcFail)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : _ => funcFail(errorsDetails).ToMlResultValid(),
                                                                                                valid: _ => errorsDetails
                                                                                            ),
                            valid: x => funcValid(x).ToMlResultValid()
                        );

    public static async Task<MlResult<TReturn>> MapIfFailWithoutExceptionAsync<T, TReturn>(this MlResult<T>                          source,
                                                                                                Func<T              , Task<TReturn>> funcValidAsync,
                                                                                                Func<MlErrorsDetails, Task<TReturn>> funcFailAsync)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                                            failAsync : async _ => (await funcFailAsync(errorsDetails)).ToMlResultValid(),
                                                                                                                            validAsync: async _ => await errorsDetails.ToMlResultFailAsync<TReturn>()
                                                                                                                        ),
                                        validAsync: async x => (await funcValidAsync(x)).ToMlResultValid()
                                    );

    public static async Task<MlResult<TReturn>> MapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , Task<TReturn>> funcValidAsync,
                                                                                                Func<MlErrorsDetails, Task<TReturn>> funcFailAsync)
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync);

    public static async Task<MlResult<TReturn>> MapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , TReturn>       funcValid,
                                                                                                Func<MlErrorsDetails, Task<TReturn>> funcFailAsync)
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync(funcValid.ToFuncTask(), funcFailAsync);

    public static async Task<MlResult<TReturn>> MapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                Func<T              , Task<TReturn>> funcValidAsync,
                                                                                                Func<MlErrorsDetails, TReturn>       funcFail)
        => await (await sourceAsync).MapIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask());

    public static async Task<MlResult<TReturn>> MapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                                Func<T              , TReturn> funcValid,
                                                                                                Func<MlErrorsDetails, TReturn> funcFail)
        => await sourceAsync.MapIfFailWithoutExceptionAsync(funcValid, funcFail);



    public static MlResult<TReturn> TryMapIfFailWithoutException<T, TReturn>(this MlResult<T>                    source,
                                                                                  Func<T              , TReturn> funcValid,
                                                                                  Func<MlErrorsDetails, TReturn> funcFail,
                                                                                  Func<Exception, string>        errorMessageBuilder)
        => source.Match(
                            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                                                                fail : _ => funcFail.TryToMlResult(errorsDetails, errorMessageBuilder),
                                                                                                valid: _ => errorsDetails
                                                                                            ),
                            valid: x             => funcValid.TryToMlResult(x, errorMessageBuilder)
                        );

    public static MlResult<TReturn> TryMapIfFailWithoutException<T, TReturn>(this MlResult<T>                    source,
                                                                                  Func<T              , TReturn> funcValid,
                                                                                  Func<MlErrorsDetails, TReturn> funcFail,
                                                                                  string                         errorMessageBuilder)
        => source.TryMapIfFailWithoutException(funcValid, funcFail, _ => errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithoutExceptionAsync<T, TReturn>(this MlResult<T>                          source,
                                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFailAsync,
                                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await source.MatchAsync(
                                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                                                                                                            failAsync : _ => funcFailAsync.TryToMlResultAsync(errorsDetails, errorMessageBuilder),
                                                                                                                            validAsync: _ => errorsDetails.ToMlResultFailAsync<TReturn>()
                                                                                                                        ),
                                        validAsync: x             => funcValidAsync.TryToMlResultAsync(x, errorMessageBuilder)
                                    );

    public static async Task<MlResult<TReturn>> TryMapIfFailWithoutExceptionAsync<T, TReturn>(this MlResult<T>                          source,
                                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFailAsync,
                                                                                                   string                               errorMessage = null!)
        => await source.TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync, _ => errorMessage);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFailAsync,
                                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await sourceAsync.TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFailAsync,
                                                                                                   string                               errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , TReturn>       funcValid,
                                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFail,
                                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await sourceAsync.TryMapIfFailWithoutExceptionAsync(funcValid, funcFail, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , TReturn>       funcValid,
                                                                                                   Func<MlErrorsDetails, Task<TReturn>> funcFailAsync,
                                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcValid.ToFuncTask(), funcFailAsync, errorMessage);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                                   Func<MlErrorsDetails, TReturn>       funcFail,
                                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>                    sourceAsync,
                                                                                                   Func<T              , Task<TReturn>> funcValidAsync,
                                                                                                   Func<MlErrorsDetails, TReturn>       funcFail,
                                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapIfFailWithoutExceptionAsync(funcValidAsync, funcFail.ToFuncTask(), errorMessage);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                                   Func<T              , TReturn> funcValid,
                                                                                                   Func<MlErrorsDetails, TReturn> funcFail,
                                                                                                   Func<Exception, string>        errorMessageBuilder)
        => await sourceAsync.TryMapIfFailWithoutExceptionAsync(funcValid, funcFail, errorMessageBuilder);

    public static async Task<MlResult<TReturn>> TryMapIfFailWithoutExceptionAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                                                   Func<T              , TReturn> funcValid,
                                                                                                   Func<MlErrorsDetails, TReturn> funcFail,
                                                                                                   string                         errorMessage = null!)
        => await sourceAsync.TryMapIfFailWithoutExceptionAsync(funcValid, funcFail, errorMessage);



    #endregion


    #region MapAlways



    public static MlResult<TReturn> MapAlways<T, TReturn>(this MlResult<T>   source, 
                                                               Func<TReturn> funcAlways)
        => funcAlways();

    public static async Task<MlResult<TReturn>> MapAlwaysAsync<T, TReturn>(this MlResult<T>         source, 
                                                                                Func<Task<TReturn>> funcAlwaysAsync)
        => await funcAlwaysAsync();


    public static async Task<MlResult<TReturn>> MapAlwaysAsync<T, TReturn>(this Task<MlResult<T>>   sourceAsync, 
                                                                                Func<Task<TReturn>> funcAlwaysAsync)
        => await funcAlwaysAsync();

    public static async Task<MlResult<TReturn>> MapAlwaysAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync, 
                                                                                Func<TReturn>     funcAlways)
        => (await sourceAsync).MapAlways(funcAlways);




    public static MlResult<TReturn> TryMapAlways<T, TReturn>(this MlResult<T>             source, 
                                                                  Func<TReturn>           funcAlways,
                                                                  Func<Exception, string> errorMessageBuilder)
        => source.Match(valid: _            => funcAlways.TryToMlResult(errorMessageBuilder),
                        fail : errorDetails => funcAlways.TryToMlResult(errorDetails, errorMessageBuilder));

    public static MlResult<TReturn> TryMapAlways<T, TReturn>(this MlResult<T>   source, 
                                                                  Func<TReturn> funcAlways,
                                                                  string        errorMessage = null!)
        => TryMapAlways(source, funcAlways, _ => errorMessage);


    public static Task<MlResult<TReturn>> TryMapAlwaysAsync<T, TReturn>(this MlResult<T>             source, 
                                                                             Func<Task<TReturn>>     funcAlwaysAsync,
                                                                             Func<Exception, string> errorMessageBuilder)
        => source.MatchAsync(validAsync: _            => funcAlwaysAsync.TryToMlResultAsync(errorMessageBuilder),
                             failAsync : errorDetails => funcAlwaysAsync.TryToMlResultAsync(errorDetails, errorMessageBuilder));

    public static Task<MlResult<TReturn>> TryMapAlwaysAsync<T, TReturn>(this MlResult<T>         source, 
                                                                             Func<Task<TReturn>> funcAlwaysAsync,
                                                                             string              errorMessage = null!)
        => TryMapAlwaysAsync(source, funcAlwaysAsync, _ => errorMessage);

    public async static Task<MlResult<TReturn>> TryMapAlwaysAsync<T, TReturn>(this Task<MlResult<T>>       sourceAsync, 
                                                                                   Func<Task<TReturn>>     funcAlwaysAsync,
                                                                                   Func<Exception, string> errorMessageBuilder)
        => await (await sourceAsync).TryMapAlwaysAsync(funcAlwaysAsync, errorMessageBuilder);

    public async static Task<MlResult<TReturn>> TryMapAlwaysAsync<T, TReturn>(this Task<MlResult<T>>   sourceAsync, 
                                                                                   Func<Task<TReturn>> funcAlwaysAsync,
                                                                                   string              errorMessage = null!)
        => await (await sourceAsync).TryMapAlwaysAsync(funcAlwaysAsync, errorMessage);

    public async static Task<MlResult<TReturn>> TryMapAlwaysAsync<T, TReturn>(this Task<MlResult<T>>       sourceAsync, 
                                                                                   Func<TReturn>           funcAlways,
                                                                                   Func<Exception, string> errorMessageBuilder)
        => (await sourceAsync).TryMapAlways(funcAlways, errorMessageBuilder);

    public async static Task<MlResult<TReturn>> TryMapAlwaysAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync, 
                                                                                   Func<TReturn>     funcAlways,
                                                                                   string            errorMessage = null!)
        => (await sourceAsync).TryMapAlways(funcAlways, errorMessage);



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
    public static MlResult<TResult> MapAlways<T, TResult>(this MlResult<T>                    source, 
                                                               Func<T              , TResult> funcValidAlways,
                                                               Func<MlErrorsDetails, TResult> funcFailAlways)
        => source.Match(valid: funcValidAlways,
                        fail : funcFailAlways);

    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this MlResult<T>                          source, 
                                                                                Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                Func<MlErrorsDetails, Task<TResult>> funcFailAlwaysAsync)
        => await source.MatchAsync(validAsync: funcValidAlwaysAsync,
                                   failAsync : funcFailAlwaysAsync);

    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                Func<MlErrorsDetails, Task<TResult>> funcFailAlwaysAsync)
        => await (await sourceAsync).MapAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync);

    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , TResult>       funcValidAlways,
                                                                                Func<MlErrorsDetails, Task<TResult>> funcFailAlwaysAsync)
        => await (await sourceAsync).MapAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync);

    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                Func<MlErrorsDetails, TResult>       funcFailAlways)
        => await (await sourceAsync).MapAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask());

    public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                                Func<T              , TResult> funcValidAlways,
                                                                                Func<MlErrorsDetails, TResult> funcFailAlways)
        => await sourceAsync.MapAlwaysAsync(funcValidAlways, funcFailAlways);

    public static MlResult<TResult> TryMapAlways<T, TResult>(this MlResult<T>                    source, 
                                                                  Func<T              , TResult> funcValidAlways,
                                                                  Func<MlErrorsDetails, TResult> funcFailAlways,
                                                                  Func<Exception, string>        errorMessageBuilder)
        => source.Match(valid: value        => funcValidAlways.TryToMlResult(value       , errorMessageBuilder),
                        fail : errorDetails => funcFailAlways .TryToMlResult(errorDetails, errorMessageBuilder));

    public static MlResult<TResult> TryMapAlways<T, TResult>(this MlResult<T>                    source, 
                                                                  Func<T              , TResult> funcValidAlways,
                                                                  Func<MlErrorsDetails, TResult> funcFailAlways,
                                                                  string                         errorMessage = null!)
        => source.TryMapAlways(funcValidAlways, funcFailAlways, _ => errorMessage);

    public static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>                          source, 
                                                                             Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                             Func<MlErrorsDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                             Func<Exception, string>              errorMessageBuilder)
        => source.MatchAsync(validAsync: value        => funcValidAlwaysAsync.TryToMlResultAsync(value       , errorMessageBuilder),
                             failAsync : errorDetails => funcFailAlwaysAsync .TryToMlResultAsync(errorDetails, errorMessageBuilder));

    public static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this MlResult<T>                          source, 
                                                                             Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                             Func<MlErrorsDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                             string                               errorMessage = null!)
        => source.TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync, _ => errorMessage);


    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                   Func<MlErrorsDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                   Func<MlErrorsDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlwaysAsync, errorMessage);

    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , TResult>       funcValidAlways,
                                                                                   Func<MlErrorsDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , TResult>       funcValidAlways,
                                                                                   Func<MlErrorsDetails, Task<TResult>> funcFailAlwaysAsync,
                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlways.ToFuncTask(), funcFailAlwaysAsync, errorMessage);

    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                   Func<MlErrorsDetails, TResult>       funcFailAlways,
                                                                                   Func<Exception, string>              errorMessageBuilder)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask(), errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>                    sourceAsync, 
                                                                                   Func<T              , Task<TResult>> funcValidAlwaysAsync,
                                                                                   Func<MlErrorsDetails, TResult>       funcFailAlways,
                                                                                   string                               errorMessage = null!)
        => await (await sourceAsync).TryMapAlwaysAsync(funcValidAlwaysAsync, funcFailAlways.ToFuncTask(), errorMessage);

    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                                   Func<T              , TResult> funcValidAlways,
                                                                                   Func<MlErrorsDetails, TResult> funcFailAlways,
                                                                                   Func<Exception, string>        errorMessageBuilder)
        => await sourceAsync.TryMapAlwaysAsync(funcValidAlways, funcFailAlways, errorMessageBuilder);

    public async static Task<MlResult<TResult>> TryMapAlwaysAsync<T, TResult>(this Task<MlResult<T>>              sourceAsync, 
                                                                                   Func<T              , TResult> funcValidAlways,
                                                                                   Func<MlErrorsDetails, TResult> funcFailAlways,
                                                                                   string                         errorMessage = null!)
        => await sourceAsync.TryMapAlwaysAsync(funcValidAlways, funcFailAlways, errorMessage);



    #endregion






}
