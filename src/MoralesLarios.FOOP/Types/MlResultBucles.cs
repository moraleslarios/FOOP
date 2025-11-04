namespace MoralesLarios.OOFP.Types;

public static class MlResultBucles
{

    //public static MlResult<IEnumerable<T>> Project<T>(this IEnumerable<T> source,
    //                                                            Func<T, MlResult<T>> completeFuncTransform)
    //{
    //    var result = source.ToMlResultValid()
    //                        .Bind(x => completeFuncTransform.GestionCollection(x));
    //    return result;
    //}

    //public static MlResult<IEnumerable<T>> GestionCollection<T>(this Func<T, MlResult<T>> completeFuncTransform, IEnumerable<T> x)
    //{
    //    var partialData = x.Select(completeFuncTransform).ToList();

    //    var result = partialData.Any(x => x.IsFail) ?
    //                 FusionFailErros(partialData) :
    //                 MlResult<IEnumerable<T>>.Valid(partialData.Select(x => x.Value));

    //    return result;
    //}

    //public static Task<MlResult<IEnumerable<T>>> ProjectAsync<T>(this IEnumerable<T> source,
    //                                                                       Func<T, MlResult<T>> completeFuncTransform)
    //    => source.Project<T>(completeFuncTransform).ToAsync();

    //public static async Task<MlResult<IEnumerable<T>>> ProjectAsync<T>(this Task<IEnumerable<T>>       sourceAsync,
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

    //                            var result = partialData.Any(x => x.IsFail) ?
    //                                         FusionFailErros(partialData)   :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.Value));

    //                            return result;
    //                        });
    //    return result;
    //}


    //public static async Task<MlResult<IEnumerable<T>>> ProjectAsync<T>(this IEnumerable<T> source,
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

    //                            var result = partialData.Any(x => x.IsFail) ?
    //                                         FusionFailErros(partialData)   :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.Value));

    //                            return result;
    //                        });
    //    return result;
    //}



    public static MlResult<IEnumerable<TResult>> Project<T, TResult>(this IEnumerable<T>             source,
                                                                               Func<T, MlResult<TResult>> completeFuncTransform)
    {
        var result = source.ToMlResultValid()
                            .Bind(x =>
                            {
                                var partialData = x.Select(completeFuncTransform).ToList();

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData)   :
                                             MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }


    public static Task<MlResult<IEnumerable<TResult>>> ProjectAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                          Func<T, MlResult<TResult>> completeFuncTransform)
        => source.Project(completeFuncTransform).ToAsync();


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync) //
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

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData)   :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                                Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync) //
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

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData)   :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }



    //public static MlResult<IEnumerable<T>> ProjectWhile<T>(this IEnumerable<T>       source,
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
    //                                if (funcResult.IsFail) break;
    //                            }

    //                            var result = partialData.Any(x => x.IsFail) ?
    //                                         FusionFailErros(partialData) :
    //                                         MlResult<IEnumerable<T>>.Valid(partialData.Select(x => x.Value));

    //                            return result;
    //                        });
    //    return result;
    //}

    //public static Task<MlResult<IEnumerable<T>>> ProjectWhileAsync<T>(this IEnumerable<T>       source,
    //                                                                            Func<T, MlResult<T>> completeFuncTransform)
    //    => source.ProjectWhile<T>(completeFuncTransform).ToAsync();

    //public static async Task<MlResult<IEnumerable<T>>> ProjectWhileAsync<T>(this Task<IEnumerable<T>> sourceAsync,
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
    //                                if (funcResult.IsFail) break;
    //                            }

    //                            var result = partialData.Any(x => x.IsFail) ?
    //                                         FusionFailErros(partialData) :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.Value));

    //                            return result;
    //                        });
    //    return result;
    //}

    //public static async Task<MlResult<IEnumerable<T>>> ProjectWhileAsync<T>(this IEnumerable<T>             source,
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
    //                                if (funcResult.IsFail) break;
    //                            }

    //                            var result = partialData.Any(x => x.IsFail) ?
    //                                         FusionFailErros(partialData) :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.Value));

    //                            return result;
    //                        });
    //    return result;
    //}

    public static MlResult<IEnumerable<TResult>> ProjectWhile<T, TResult>(this IEnumerable<T>             source,
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
                                    if (funcResult.IsFail) break;
                                }

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData) :
                                             MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }

    public static Task<MlResult<IEnumerable<TResult>>> ProjectWhileAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                               Func<T, MlResult<TResult>> completeFuncTransform)
        => source.ProjectWhile(completeFuncTransform).ToAsync();

    public static async Task<MlResult<IEnumerable<TResult>>> ProjectWhileAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
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
                                    if (funcResult.IsFail) break;
                                }

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }

    public static async Task<MlResult<IEnumerable<TResult>>> ProjectWhileAsync<T, TResult>(this IEnumerable<T>                   source,
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
                                    if (funcResult.IsFail) break;
                                }

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }


    //public static async Task<MlResult<IEnumerable<T>>> ProjectParallelAsync<T>(this Task<IEnumerable<T>>       sourceAsync,
    //                                                                                     Func<T, Task<MlResult<T>>> completeFuncTransformAsync)
    //{
    //    var result = await sourceAsync.ToMlResultValidAsync()
    //                        .BindAsync(async x =>
    //                        {
    //                            var colec = await x;

    //                            List<Task<MlResult<T>>> tasks = colec.Select(item => completeFuncTransformAsync(item)).ToList();

    //                            await Task.WhenAll(tasks);

    //                            List<MlResult<T>> partialData = tasks.Select(t => t.Result).ToList();

    //                            var result = partialData.Any(x => x.IsFail) ?
    //                                         FusionFailErros(partialData) :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.Value));

    //                            return result;
    //                        });
    //    return result;
    //}

    //public static async Task<MlResult<IEnumerable<T>>> ProjectParallelAsync<T>(this IEnumerable<T>             source,
    //                                                                                     Func<T, Task<MlResult<T>>> completeFuncTransformAsync)
    //{
    //    var result = await source.ToMlResultValidAsync()
    //                        .BindAsync(async colec =>
    //                        {
    //                            List<Task<MlResult<T>>> tasks = colec.Select(item => completeFuncTransformAsync(item)).ToList();

    //                            await Task.WhenAll(tasks);

    //                            List<MlResult<T>> partialData = tasks.Select(t => t.Result).ToList();

    //                            var result = partialData.Any(x => x.IsFail) ?
    //                                         FusionFailErros(partialData) :
    //                                         await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.Value));

    //                            return result;
    //                        });
    //    return result;
    //}

    public static async Task<MlResult<IEnumerable<TResult>>> ProjectParallelAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
                                                                                                        Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                List<Task<MlResult<TResult>>> tasks = colec.Select(item => completeFuncTransformAsync(item)).ToList();

                                await Task.WhenAll(tasks);

                                List<MlResult<TResult>> partialData = tasks.Select(t => t.Result).ToList();

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }


    public static async Task<MlResult<IEnumerable<TResult>>> ProjectParallelAsync<T, TResult>(this IEnumerable<T>                   source,
                                                                                                        Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {

                                List<Task<MlResult<TResult>>> tasks = colec.Select(item => completeFuncTransformAsync(item)).ToList();

                                await Task.WhenAll(tasks);

                                List<MlResult<TResult>> partialData = tasks.Select(t => t.Result).ToList();

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData) :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }







    public static MlResult<IEnumerable<T>> FusionFailErros<T>(this IEnumerable<MlResult<T>> source)
    {
        var partialResult = source.Where(x => x.IsFail).ToList();

        if ( ! partialResult.Any()) MlResult<IEnumerable<T>>.Fail("No elements found in failed state to merge");

        MlErrorsDetails result = partialResult.First().ErrorsDetails;

        foreach (var item in partialResult.Skip(1))
        {
            result = result.Merge(item.ErrorsDetails);
        }

        return result;
    }

    public static Task<MlResult<IEnumerable<T>>> FusionFailErrosAsync<T>(this IEnumerable<MlResult<T>> source)
        => source.FusionFailErros().ToAsync();

    public static async Task<MlResult<IEnumerable<T>>> FusionFailErrosAsync<T>(this Task<IEnumerable<MlResult<T>>> sourceAsync)
        => await (await sourceAsync).FusionFailErrosAsync();




    public static MlResult<IEnumerable<T>> FusionErrosIfExists<T>(this IEnumerable<MlResult<T>> source)
    {
        var partialResult = source.Where(x => x.IsFail).ToList();

        if ( ! partialResult.Any())
        {
            return MlResult<IEnumerable<T>>.Valid(source.Select(x => x.SecureValidValue()));
        }

        MlErrorsDetails result = partialResult.First().ErrorsDetails;

        foreach (var item in partialResult.Skip(1))
        {
            result = result.Merge(item.ErrorsDetails);
        }

        return result;

    }

    public static Task<MlResult<IEnumerable<T>>> FusionErrosIfExistsAsync<T>(this IEnumerable<MlResult<T>> source)
        => source.FusionErrosIfExists().ToAsync();

    public static async Task<MlResult<IEnumerable<T>>> FusionErrosIfExistsAsync<T>(this Task<IEnumerable<MlResult<T>>> sourceAsync)
        => await (await sourceAsync).FusionErrosIfExistsAsync();





    public static MlResult<IEnumerable<T>> VerifiedEnumerableResultData<T>(this IEnumerable<MlResult<T>> source)
        => source.Any(x => x.IsFail) ?
           FusionFailErros(source)   :
           MlResult<IEnumerable<T>>.Valid(source.Select(x => x.Value));




}
