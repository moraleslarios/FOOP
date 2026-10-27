// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types;

public static partial class MlResultBucles
{

    //public static MlResult<IEnumerable<T>> Projection<T>(this IEnumerable<T> source,
    //                                                            Func<T, MlResult<T>> completeFuncTransform)
    //{
    //    var result = source.ToMlResultValid()
    //                        .Bind(x => completeFuncTransform.GestionCollection(x));
    //    return result;
    //}

    //public static MlResult<IEnumerable<T>> GestionCollection<T>(this Func<T, MlResult<T>> completeFuncTransform, IEnumerable<T> x)
    //{
    //    var partialData = x.Select(completeFuncTransform).ToList();

    //    var result = partialData.Any(x => x.IsFailure) ?
    //                 MergeFailureErrors(partialData) :
    //                 MlResult<IEnumerable<T>>.Valid(partialData.Select(x => x.SuccessValue));

    //    return result;
    //}

    //public static Task<MlResult<IEnumerable<T>>> ProjectionAsync<T>(this IEnumerable<T> source,
    //                                                                       Func<T, MlResult<T>> completeFuncTransform)
    //    => source.Projection<T>(completeFuncTransform).ToAsync();

    //public static async Task<MlResult<IEnumerable<T>>> ProjectionAsync<T>(this Task<IEnumerable<T>>       sourceAsync,
    //                                                                             Func<T, Task<MlResult<T>>> completeFuncTransformAsync) //
    //{
    //    var result = await sourceAsync.ToMlResultValidAsync()
    //                        .BindAsync(async x =>
    //                        {
    //                            var colec = await x;

    //                            var partialData = new List<MlResult<T>>();

    //                            foreach (var item in colec)
    //                            {
    //                                var funcResult = await completeFuncTransformAsync(item);
    //                                partialData.Add(funcResult);
    //                            }

    //                            var result = partialData.Any(x => x.IsFailure) ?
    //                                         MergeFailureErrors(partialData)   :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.SuccessValue));

    //                            return result;
    //                        });
    //    return result;
    //}


    //public static async Task<MlResult<IEnumerable<T>>> ProjectionAsync<T>(this IEnumerable<T> source,
    //                                                                             Func<T, Task<MlResult<T>>> completeFuncTransformAsync) //
    //{
    //    var result = await source.ToMlResultValidAsync()
    //                        .BindAsync(async colec =>
    //                        {
    //                            var partialData = new List<MlResult<T>>();

    //                            foreach (var item in colec)
    //                            {
    //                                var funcResult = await completeFuncTransformAsync(item);
    //                                partialData.Add(funcResult);
    //                            }

    //                            var result = partialData.Any(x => x.IsFailure) ?
    //                                         MergeFailureErrors(partialData)   :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.SuccessValue));

    //                            return result;
    //                        });
    //    return result;
    //}



    public static MlResult<IEnumerable<TResult>> Projection<T, TResult>(this IEnumerable<T>             source,
                                                                             Func<T, MlResult<TResult>> completeFuncTransform)
    {
        var result = source.ToMlResultValid()
                            .Bind(x =>
                            {
                                var partialData = x.Select(completeFuncTransform).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    public static MlResult<IEnumerable<TResult>> Projection<T, TResult>(this IEnumerable<T>                  source,
                                                                             Func<T, int, MlResult<TResult>> completeFuncTransform)
    {
        var result = source.ToMlResultValid()
                            .Bind(x =>
                            {
                                var partialData = x.Select(completeFuncTransform).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }





    public static Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                        Func<T, MlResult<TResult>> completeFuncTransform)
        => source.Projection(completeFuncTransform).ToAsync();

    public static Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this IEnumerable<T>                  source,
                                                                                        Func<T, int, MlResult<TResult>> completeFuncTransform)
        => source.Projection(completeFuncTransform).ToAsync();


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                              Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                var partialData = new List<MlResult<TResult>>();
                                
                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add(funcResult);
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this Task<IEnumerable<T>>                  sourceAsync,
                                                                                              Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                var partialData = new List<MlResult<TResult>>();
                                
                                foreach (var item in colec.Select((value, index) => (value, index)))
                                {
                                    var funcResult = await completeFuncTransformAsync(item.value, item.index);
                                    partialData.Add(funcResult);
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }



    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                              Func<T, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).ProjectionAsync(completeFuncTransform);


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                              Func<T, int, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).ProjectionAsync(completeFuncTransform);


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                              Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {
                                var partialData = new List<MlResult<TResult>>();

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add(funcResult);
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                              Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {
                                var partialData = new List<MlResult<TResult>>();
                                
                                int index = 0;
                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item, index);
                                    partialData.Add(funcResult);
                                    index++;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData)   :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }



    //public static MlResult<IEnumerable<T>> ProjectionWhile<T>(this IEnumerable<T>       source,
    //                                                                 Func<T, MlResult<T>> completeFuncTransform)
    //{
    //    var result = source.ToMlResultValid()
    //                        .Bind(x =>
    //                        {
    //                            List<MlResult<T>> partialData = [];
                                
    //                            foreach (var item in x)
    //                            {
    //                                var funcResult = completeFuncTransform(item);
    //                                partialData.Add(funcResult);
    //                                if (funcResult.IsFailure) break;
    //                            }

    //                            var result = partialData.Any(x => x.IsFailure) ?
    //                                         MergeFailureErrors(partialData) :
    //                                         MlResult<IEnumerable<T>>.Valid(partialData.Select(x => x.SuccessValue));

    //                            return result;
    //                        });
    //    return result;
    //}

    //public static Task<MlResult<IEnumerable<T>>> ProjectionWhileAsync<T>(this IEnumerable<T>       source,
    //                                                                            Func<T, MlResult<T>> completeFuncTransform)
    //    => source.ProjectionWhile<T>(completeFuncTransform).ToAsync();

    //public static async Task<MlResult<IEnumerable<T>>> ProjectionWhileAsync<T>(this Task<IEnumerable<T>> sourceAsync,
    //                                                                                  Func<T, Task<MlResult<T>>> completeFuncTransformAsync)
    //{
    //    var result = await sourceAsync.ToMlResultValidAsync()
    //                        .BindAsync(async x =>
    //                        {
    //                            var colec = await x;

    //                            var partialData = new List<MlResult<T>>();

    //                            foreach (var item in colec)
    //                            {
    //                                var funcResult = await completeFuncTransformAsync(item);
    //                                partialData.Add(funcResult);
    //                                if (funcResult.IsFailure) break;
    //                            }

    //                            var result = partialData.Any(x => x.IsFailure) ?
    //                                         MergeFailureErrors(partialData) :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.SuccessValue));

    //                            return result;
    //                        });
    //    return result;
    //}

    //public static async Task<MlResult<IEnumerable<T>>> ProjectionWhileAsync<T>(this IEnumerable<T>             source,
    //                                                                                  Func<T, Task<MlResult<T>>> completeFuncTransformAsync)
    //{
    //    var result = await source.ToMlResultValidAsync()
    //                        .BindAsync(async colec =>
    //                        {
    //                            var partialData = new List<MlResult<T>>();

    //                            foreach (var item in colec)
    //                            {
    //                                var funcResult = await completeFuncTransformAsync(item);
    //                                partialData.Add(funcResult);
    //                                if (funcResult.IsFailure) break;
    //                            }

    //                            var result = partialData.Any(x => x.IsFailure) ?
    //                                         MergeFailureErrors(partialData) :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.SuccessValue));

    //                            return result;
    //                        });
    //    return result;
    //}

    public static MlResult<IEnumerable<TResult>> ProjectionWhile<T, TResult>(this IEnumerable<T>             source,
                                                                                  Func<T, MlResult<TResult>> completeFuncTransform)
    {
        var result = source.ToMlResultValid()
                            .Bind(x =>
                            {
                                List<MlResult<TResult>> partialData = [];

                                foreach (var item in x)
                                {
                                    var funcResult = completeFuncTransform(item);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    public static MlResult<IEnumerable<TResult>> ProjectionWhile<T, TResult>(this IEnumerable<T>                  source,
                                                                                  Func<T, int, MlResult<TResult>> completeFuncTransform)
    {
        var result = source.ToMlResultValid()
                            .Bind(x =>
                            {
                                List<MlResult<TResult>> partialData = [];

                                int index = 0;
                                foreach (var item in x)
                                {
                                    var funcResult = completeFuncTransform(item, index);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                    index++;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    public static Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                             Func<T, MlResult<TResult>> completeFuncTransform)
        => source.ProjectionWhile(completeFuncTransform).ToAsync();


    public static Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                             Func<T, int, MlResult<TResult>> completeFuncTransform)
        => source.ProjectionWhile(completeFuncTransform).ToAsync();



    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                   Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                var partialData = new List<MlResult<TResult>>();

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this Task<IEnumerable<T>>                  sourceAsync,
                                                                                                   Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                var partialData = new List<MlResult<TResult>>();

                                int index = 0;

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item, index);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                    index++;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                   Func<T, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).ProjectionWhileAsync(completeFuncTransform);


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                   Func<T, int, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).ProjectionWhileAsync(completeFuncTransform);


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                                   Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {
                                var partialData = new List<MlResult<TResult>>();

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionWhileAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                                   Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {
                                var partialData = new List<MlResult<TResult>>();
                                int index = 0;  

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item, index);
                                    partialData.Add(funcResult);
                                    if (funcResult.IsFailure) break;
                                    index++;
                                }

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }




    //public static async Task<MlResult<IEnumerable<T>>> ProjectionParallelAsync<T>(this Task<IEnumerable<T>>       sourceAsync,
    //                                                                                     Func<T, Task<MlResult<T>>> completeFuncTransformAsync)
    //{
    //    var result = await sourceAsync.ToMlResultValidAsync()
    //                        .BindAsync(async x =>
    //                        {
    //                            var colec = await x;

    //                            List<Task<MlResult<T>>> tasks = colec.Select(item => completeFuncTransformAsync(item)).ToList();

    //                            await Task.WhenAll(tasks);

    //                            List<MlResult<T>> partialData = tasks.Select(t => t.Result).ToList();

    //                            var result = partialData.Any(x => x.IsFailure) ?
    //                                         MergeFailureErrors(partialData) :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.SuccessValue));

    //                            return result;
    //                        });
    //    return result;
    //}

    //public static async Task<MlResult<IEnumerable<T>>> ProjectionParallelAsync<T>(this IEnumerable<T>             source,
    //                                                                                     Func<T, Task<MlResult<T>>> completeFuncTransformAsync)
    //{
    //    var result = await source.ToMlResultValidAsync()
    //                        .BindAsync(async colec =>
    //                        {
    //                            List<Task<MlResult<T>>> tasks = colec.Select(item => completeFuncTransformAsync(item)).ToList();

    //                            await Task.WhenAll(tasks);

    //                            List<MlResult<T>> partialData = tasks.Select(t => t.Result).ToList();

    //                            var result = partialData.Any(x => x.IsFailure) ?
    //                                         MergeFailureErrors(partialData) :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.SuccessValue));

    //                            return result;
    //                        });
    //    return result;
    //}

    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionParallelAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                      Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                List<Task<MlResult<TResult>>> tasks = colec.Select(item => completeFuncTransformAsync(item)).ToList();

                                await Task.WhenAll(tasks);

                                List<MlResult<TResult>> partialData = tasks.Select(t => t.Result).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }

    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionParallelAsync<T, TResult>(this Task<IEnumerable<T>>                  sourceAsync,
                                                                                                      Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                List<Task<MlResult<TResult>>> tasks = colec.Select((item, index) => completeFuncTransformAsync(item, index)).ToList();

                                await Task.WhenAll(tasks);

                                List<MlResult<TResult>> partialData = tasks.Select(t => t.Result).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionParallelAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                                      Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {

                                List<Task<MlResult<TResult>>> tasks = colec.Select(item => completeFuncTransformAsync(item)).ToList();

                                await Task.WhenAll(tasks);

                                List<MlResult<TResult>> partialData = tasks.Select(t => t.Result).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectionParallelAsync<T, TResult>(this IEnumerable<T>                        source,
                                                                                                      Func<T, int, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {

                                List<Task<MlResult<TResult>>> tasks = colec.Select((item, index) => completeFuncTransformAsync(item, index)).ToList();

                                await Task.WhenAll(tasks);

                                List<MlResult<TResult>> partialData = tasks.Select(t => t.Result).ToList();

                                var result = partialData.Any(x => x.IsFailure) ?
                                             MergeFailureErrors(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.SuccessValue));

                                return result;
                            });
        return result;
    }



    public static MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorsDetails> fails)> ProjectionSplit<T, TResult>(this IEnumerable<T>             source,
                                                                                                                                   Func<T, MlResult<TResult>> completeFuncTransform)
        where T : notnull
    {
        var result = EnsureFp.NotNull(completeFuncTransform, "completeFuncTransform cannot be null")
                            .Map (x =>
                            {
                                var partialData = source.Where(z => z is not null)
                                                        .Select(z => (z, completeFuncTransform(z))).ToList();

                                var partialResult = (valids: partialData.Where(x => x.Item2.IsSuccess).ToDictionary(x => x.Item1, x => x.Item2.GetValueOrThrow()),
                                                     fails : partialData.Where(x => x.Item2.IsFailure ).ToDictionary(x => x.Item1, x => x.Item2.GetFailureDetailsOrThrow()));
                                return partialResult;
                            });
        return result;
    }


    public static Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorsDetails> fails)>> ProjectionSplitAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                                                                              Func<T, MlResult<TResult>> completeFuncTransform)
        where T : notnull
        => source.ProjectionSplit(completeFuncTransform).ToAsync();


    public static async Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorsDetails> fails)>> ProjectionSplitAsync<T, TResult>(this Task<IEnumerable<T>>       sourceAsync,
                                                                                                                                                    Func<T, MlResult<TResult>> completeFuncTransform)
        where T : notnull
        => await (await sourceAsync).ProjectionSplitAsync(completeFuncTransform);


    public static Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorsDetails> fails)>> ProjectionSplitAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                                                                              Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where T : notnull
    {
        var result = EnsureFp.NotNull(completeFuncTransformAsync, "completeFuncTransformAsync cannot be null")
                            .MapAsync(async x =>
                            {
                                var partialData = new List<(T item, MlResult<TResult> result)>();

                                foreach (var item in source.Where(z => z is not null))
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add((item, funcResult));
                                }

                                var partialResult = (valids: partialData.Where(x => x.result.IsSuccess).ToDictionary(x => x.item, x => x.result.GetValueOrThrow()),
                                                     fails : partialData.Where(x => x.result.IsFailure ).ToDictionary(x => x.item, x => x.result.GetFailureDetailsOrThrow()));

                                return partialResult;
                            });
        return result;
    }


    public static async Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorsDetails> fails)>> ProjectionSplitAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                                                                    Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where T : notnull
        => await (await sourceAsync).ProjectionSplitAsync(completeFuncTransformAsync);




    public static MlResult<(IEnumerable<ProyectSplitValidInfo<TKey, TResult>> valids, IEnumerable<ProyectSplitFailInfo<TKey>> fails)> ProjectionSplitPlus<TKey, TResult>(this IEnumerable<TKey> source,
                                                                                                                                                                              Func<TKey, MlResult<TResult>> completeFuncTransform)
        where TKey : notnull
    {
        var result = EnsureFp.NotNull(completeFuncTransform, "completeFuncTransform cannot be null")
                                .Map(x =>
                                {
                                    var partialData = source.Where(z => z is not null)
                                                            .Select(z => (z, completeFuncTransform(z))).ToList();
                                    var partialResult = (valids: partialData.Where(x => x.Item2.IsSuccess)
                                                                            .Select(x => new ProyectSplitValidInfo<TKey, TResult>(x.Item1, x.Item2.GetValueOrThrow())),
                                                         fails: partialData.Where(x => x.Item2.IsFailure)
                                                                            .Select(x => new ProyectSplitFailInfo<TKey>(x.Item1, x.Item2.GetFailureDetailsOrThrow())));
                                    return partialResult;
                                });
        return result;
    }


    public static Task<MlResult<(IEnumerable<ProyectSplitValidInfo<TKey, TResult>> valids, IEnumerable<ProyectSplitFailInfo<TKey>> fails)>> ProjectionSplitPlusAsync<TKey, TResult>(this IEnumerable<TKey>             source,
                                                                                                                                                                                         Func<TKey, MlResult<TResult>> completeFuncTransform)
        where TKey : notnull
        => source.ProjectionSplitPlus(completeFuncTransform).ToAsync();


    public static async Task<MlResult<(IEnumerable<ProyectSplitValidInfo<TKey, TResult>> valids, IEnumerable<ProyectSplitFailInfo<TKey>> fails)>> ProjectionSplitPlusAsync<TKey, TResult>(this Task<IEnumerable<TKey>>       sourceAsync,
                                                                                                                                                                                               Func<TKey, MlResult<TResult>> completeFuncTransform)
        where TKey : notnull
        => await (await sourceAsync).ProjectionSplitPlusAsync(completeFuncTransform);


    public static Task<MlResult<(IEnumerable<ProyectSplitValidInfo<TKey, TResult>> valids, IEnumerable<ProyectSplitFailInfo<TKey>> fails)>> ProjectionSplitPlusAsync<TKey, TResult>(this IEnumerable<TKey>                   source,
                                                                                                                                                                                         Func<TKey, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where TKey : notnull
    {
        var result = EnsureFp.NotNull(completeFuncTransformAsync, "completeFuncTransformAsync cannot be null")
                            .MapAsync(async x =>
                            {
                                var partialData = new List<(TKey item, MlResult<TResult> result)>();

                                foreach (var item in source.Where(z => z is not null))
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add((item, funcResult));
                                }

                                var partialResult = (valids: partialData.Where(x => x.result.IsSuccess)
                                                                        .Select(x => new ProyectSplitValidInfo<TKey, TResult>(x.item, x.result.GetValueOrThrow())),
                                                     fails: partialData.Where(x => x.result.IsFailure)
                                                                        .Select(x => new ProyectSplitFailInfo<TKey>(x.item, x.result.GetFailureDetailsOrThrow())));

                                return partialResult;
                            });
        return result;
    }


    public static async Task<MlResult<(IEnumerable<ProyectSplitValidInfo<TKey, TResult>> valids, IEnumerable<ProyectSplitFailInfo<TKey>> fails)>> ProjectionSplitPlusAsync<TKey, TResult>(this Task<IEnumerable<TKey>>             sourceAsync,
                                                                                                                                                                                               Func<TKey, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where TKey : notnull
        => await (await sourceAsync).ProjectionSplitPlusAsync(completeFuncTransformAsync);






    public static MlResult<IEnumerable<T>> MergeFailureErrors<T>(this IEnumerable<MlResult<T>> source)
    {
        var partialResult = source.Where(x => x.IsFailure).ToList();

        if ( ! partialResult.Any()) return MlResult<IEnumerable<T>>.Fail("No elements found in failed state to merge");

        MlErrorsDetails result = partialResult.First().FailureDetails;

        foreach (var item in partialResult.Skip(1))
        {
            result = result.Merge(item.FailureDetails);
        }

        return result;
    }


    public static Task<MlResult<IEnumerable<T>>> MergeFailureErrorsAsync<T>(this IEnumerable<MlResult<T>> source)
        => source.MergeFailureErrors().ToAsync();

    public static async Task<MlResult<IEnumerable<T>>> MergeFailureErrorsAsync<T>(this Task<IEnumerable<MlResult<T>>> sourceAsync)
        => await (await sourceAsync).MergeFailureErrorsAsync();




    public static MlResult<IEnumerable<T>> MergeErrorsIfExists<T>(this IEnumerable<MlResult<T>> source)
    {
        var partialResult = source.Where(x => x.IsFailure).ToList();

        if ( ! partialResult.Any())
        {
            return MlResult<IEnumerable<T>>.Valid(source.Select(x => x.GetValueOrThrow()));
        }

        MlErrorsDetails result = partialResult.First().FailureDetails;

        foreach (var item in partialResult.Skip(1))
        {
            result = result.Merge(item.FailureDetails);
        }

        return result;

    }

    public static Task<MlResult<IEnumerable<T>>> MergeErrorsIfExistsAsync<T>(this IEnumerable<MlResult<T>> source)
        => source.MergeErrorsIfExists().ToAsync();

    public static async Task<MlResult<IEnumerable<T>>> MergeErrorsIfExistsAsync<T>(this Task<IEnumerable<MlResult<T>>> sourceAsync)
        => await (await sourceAsync).MergeErrorsIfExistsAsync();





    public static MlResult<IEnumerable<T>> VerifiedEnumerableResultData<T>(this IEnumerable<MlResult<T>> source)
        => source.Any(x => x.IsFailure) ?
           MergeFailureErrors(source)   :
           MlResult<IEnumerable<T>>.Valid(source.Select(x => x.SuccessValue));




}


public record ProyectSplitValidInfo<TKey, TResult>(TKey Key, TResult Value);

public record ProyectSplitFailInfo<TKey>(TKey Key, MlErrorsDetails Errors);