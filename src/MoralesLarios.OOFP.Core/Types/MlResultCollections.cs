// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types;

public static partial class MlResultCollections
{

    //public static MlResult<IEnumerable<T>> Traverse<T>(this IEnumerable<T> source,
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

    //public static Task<MlResult<IEnumerable<T>>> TraverseAsync<T>(this IEnumerable<T> source,
    //                                                                       Func<T, MlResult<T>> completeFuncTransform)
    //    => source.Traverse<T>(completeFuncTransform).ToAsync();

    //public static async Task<MlResult<IEnumerable<T>>> TraverseAsync<T>(this Task<IEnumerable<T>>       sourceAsync,
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


    //public static async Task<MlResult<IEnumerable<T>>> TraverseAsync<T>(this IEnumerable<T> source,
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



    public static MlResult<IEnumerable<TResult>> Traverse<T, TResult>(this IEnumerable<T>             source,
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


    public static MlResult<IEnumerable<TResult>> Traverse<T, TResult>(this IEnumerable<T>                  source,
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





    public static Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                        Func<T, MlResult<TResult>> completeFuncTransform)
        => source.Traverse(completeFuncTransform).ToAsync();

    public static Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this IEnumerable<T>                  source,
                                                                                        Func<T, int, MlResult<TResult>> completeFuncTransform)
        => source.Traverse(completeFuncTransform).ToAsync();


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
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


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this Task<IEnumerable<T>>                  sourceAsync,
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



    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                              Func<T, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).TraverseAsync(completeFuncTransform);


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                              Func<T, int, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).TraverseAsync(completeFuncTransform);


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this IEnumerable<T>                   source,
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


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseAsync<T, TResult>(this IEnumerable<T>                   source,
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



    //public static MlResult<IEnumerable<T>> TraverseWhile<T>(this IEnumerable<T>       source,
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

    //public static Task<MlResult<IEnumerable<T>>> TraverseWhileAsync<T>(this IEnumerable<T>       source,
    //                                                                            Func<T, MlResult<T>> completeFuncTransform)
    //    => source.TraverseWhile<T>(completeFuncTransform).ToAsync();

    //public static async Task<MlResult<IEnumerable<T>>> TraverseWhileAsync<T>(this Task<IEnumerable<T>> sourceAsync,
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

    //public static async Task<MlResult<IEnumerable<T>>> TraverseWhileAsync<T>(this IEnumerable<T>             source,
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

    public static MlResult<IEnumerable<TResult>> TraverseWhile<T, TResult>(this IEnumerable<T>             source,
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


    public static MlResult<IEnumerable<TResult>> TraverseWhile<T, TResult>(this IEnumerable<T>                  source,
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


    public static Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                             Func<T, MlResult<TResult>> completeFuncTransform)
        => source.TraverseWhile(completeFuncTransform).ToAsync();


    public static Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                             Func<T, int, MlResult<TResult>> completeFuncTransform)
        => source.TraverseWhile(completeFuncTransform).ToAsync();



    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
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


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this Task<IEnumerable<T>>                  sourceAsync,
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


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                   Func<T, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).TraverseWhileAsync(completeFuncTransform);


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                   Func<T, int, MlResult<TResult>> completeFuncTransform)
        => await (await sourceAsync).TraverseWhileAsync(completeFuncTransform);


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this IEnumerable<T>                   source,
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


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseWhileAsync<T, TResult>(this IEnumerable<T>                   source,
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




    //public static async Task<MlResult<IEnumerable<T>>> TraverseParallelAsync<T>(this Task<IEnumerable<T>>       sourceAsync,
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

    //public static async Task<MlResult<IEnumerable<T>>> TraverseParallelAsync<T>(this IEnumerable<T>             source,
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

    public static async Task<MlResult<IEnumerable<TResult>>> TraverseParallelAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
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

    public static async Task<MlResult<IEnumerable<TResult>>> TraverseParallelAsync<T, TResult>(this Task<IEnumerable<T>>                  sourceAsync,
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


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseParallelAsync<T, TResult>(this IEnumerable<T>                   source,
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


    public static async Task<MlResult<IEnumerable<TResult>>> TraverseParallelAsync<T, TResult>(this IEnumerable<T>                        source,
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



    public static MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)> TraverseSplit<T, TResult>(this IEnumerable<T>             source,
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


    public static Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> TraverseSplitAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                                                                              Func<T, MlResult<TResult>> completeFuncTransform)
        where T : notnull
        => source.TraverseSplit(completeFuncTransform).ToAsync();


    public static async Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> TraverseSplitAsync<T, TResult>(this Task<IEnumerable<T>>       sourceAsync,
                                                                                                                                                    Func<T, MlResult<TResult>> completeFuncTransform)
        where T : notnull
        => await (await sourceAsync).TraverseSplitAsync(completeFuncTransform);


    public static Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> TraverseSplitAsync<T, TResult>(this IEnumerable<T>                   source,
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


    public static async Task<MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorDetails> fails)>> TraverseSplitAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                                                                    Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where T : notnull
        => await (await sourceAsync).TraverseSplitAsync(completeFuncTransformAsync);




    public static MlResult<(IEnumerable<TraverseSplitSuccess<TKey, TResult>> valids, IEnumerable<TraverseSplitFailure<TKey>> fails)> TraverseSplitWithKeys<TKey, TResult>(this IEnumerable<TKey> source,
                                                                                                                                                                              Func<TKey, MlResult<TResult>> completeFuncTransform)
        where TKey : notnull
    {
        var result = EnsureFp.NotNull(completeFuncTransform, "completeFuncTransform cannot be null")
                                .Map(x =>
                                {
                                    var partialData = source.Where(z => z is not null)
                                                            .Select(z => (z, completeFuncTransform(z))).ToList();
                                    var partialResult = (valids: partialData.Where(x => x.Item2.IsSuccess)
                                                                            .Select(x => new TraverseSplitSuccess<TKey, TResult>(x.Item1, x.Item2.GetValueOrThrow())),
                                                         fails: partialData.Where(x => x.Item2.IsFailure)
                                                                            .Select(x => new TraverseSplitFailure<TKey>(x.Item1, x.Item2.GetFailureDetailsOrThrow())));
                                    return partialResult;
                                });
        return result;
    }


    public static Task<MlResult<(IEnumerable<TraverseSplitSuccess<TKey, TResult>> valids, IEnumerable<TraverseSplitFailure<TKey>> fails)>> TraverseSplitWithKeysAsync<TKey, TResult>(this IEnumerable<TKey>             source,
                                                                                                                                                                                         Func<TKey, MlResult<TResult>> completeFuncTransform)
        where TKey : notnull
        => source.TraverseSplitWithKeys(completeFuncTransform).ToAsync();


    public static async Task<MlResult<(IEnumerable<TraverseSplitSuccess<TKey, TResult>> valids, IEnumerable<TraverseSplitFailure<TKey>> fails)>> TraverseSplitWithKeysAsync<TKey, TResult>(this Task<IEnumerable<TKey>>       sourceAsync,
                                                                                                                                                                                               Func<TKey, MlResult<TResult>> completeFuncTransform)
        where TKey : notnull
        => await (await sourceAsync).TraverseSplitWithKeysAsync(completeFuncTransform);


    public static Task<MlResult<(IEnumerable<TraverseSplitSuccess<TKey, TResult>> valids, IEnumerable<TraverseSplitFailure<TKey>> fails)>> TraverseSplitWithKeysAsync<TKey, TResult>(this IEnumerable<TKey>                   source,
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
                                                                        .Select(x => new TraverseSplitSuccess<TKey, TResult>(x.item, x.result.GetValueOrThrow())),
                                                     fails: partialData.Where(x => x.result.IsFailure)
                                                                        .Select(x => new TraverseSplitFailure<TKey>(x.item, x.result.GetFailureDetailsOrThrow())));

                                return partialResult;
                            });
        return result;
    }


    public static async Task<MlResult<(IEnumerable<TraverseSplitSuccess<TKey, TResult>> valids, IEnumerable<TraverseSplitFailure<TKey>> fails)>> TraverseSplitWithKeysAsync<TKey, TResult>(this Task<IEnumerable<TKey>>             sourceAsync,
                                                                                                                                                                                               Func<TKey, Task<MlResult<TResult>>> completeFuncTransformAsync)
        where TKey : notnull
        => await (await sourceAsync).TraverseSplitWithKeysAsync(completeFuncTransformAsync);






    public static MlResult<IEnumerable<T>> MergeFailureErrors<T>(this IEnumerable<MlResult<T>> source)
    {
        var partialResult = source.Where(x => x.IsFailure).ToList();

        if ( ! partialResult.Any()) return MlResult<IEnumerable<T>>.Fail("No elements found in failed state to merge");

        MlErrorDetails result = partialResult.First().FailureDetails;

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

        MlErrorDetails result = partialResult.First().FailureDetails;

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


public record TraverseSplitSuccess<TKey, TResult>(TKey Key, TResult Value);

public record TraverseSplitFailure<TKey>(TKey Key, MlErrorDetails Errors);