namespace MoralesLarios.OOFP.Types;
public static class MlResultBucles
{

    public static MlResult<IEnumerable<T>> CompleteData<T>(this IEnumerable<T> source,
                                                                Func<T, MlResult<T>> completeFuncTransform)
    {
        var result = source.ToMlResultValid()
                            .Bind(x =>
                            {
                                var partialData = x.Select(completeFuncTransform).ToList();

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData)   :
                                             MlResult<IEnumerable<T>>.Valid(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }


    public static Task<MlResult<IEnumerable<T>>> CompleteDataAsync<T>(this IEnumerable<T> source,
                                                                           Func<T, MlResult<T>> completeFuncTransform)
        => source.CompleteData<T>(completeFuncTransform).ToAsync();

    public static async Task<MlResult<IEnumerable<T>>> CompleteDataAsync<T>(this Task<IEnumerable<T>>       sourceAsync,
                                                                                 Func<T, Task<MlResult<T>>> completeFuncTransformAsync)
    {
        var result = await sourceAsync.ToMlResultValidAsync()
                            .BindAsync(async x =>
                            {
                                var colec = await x;

                                var partialData = new List<MlResult<T>>();

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add(funcResult);
                                }

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData)   :
                                             await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }


    public static async Task<MlResult<IEnumerable<T>>> CompleteDataAsync<T>(this IEnumerable<T> source,
                                                                                 Func<T, Task<MlResult<T>>> completeFuncTransformAsync)
    {
        var result = await source.ToMlResultValidAsync()
                            .BindAsync(async colec =>
                            {
                                var partialData = new List<MlResult<T>>();

                                foreach (var item in colec)
                                {
                                    var funcResult = await completeFuncTransformAsync(item);
                                    partialData.Add(funcResult);
                                }

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData)   :
                                             await MlResult<IEnumerable<T>>.ValidAsync(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }



    public static MlResult<IEnumerable<TResult>> CompleteData<T, TResult>(this IEnumerable<T>             source,
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


    public static Task<MlResult<IEnumerable<TResult>>> CompleteDataAsync<T, TResult>(this IEnumerable<T>             source,
                                                                                          Func<T, MlResult<TResult>> completeFuncTransform)
        => source.CompleteData<T, TResult>(completeFuncTransform).ToAsync();


    public static async Task<MlResult<IEnumerable<TResult>>> CompleteDataAsync<T, TResult>(this Task<IEnumerable<T>>             sourceAsync,
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

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData)   :
                                             await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.Value));

                                return result;
                            });
        return result;
    }


    public static async Task<MlResult<IEnumerable<TResult>>> CompleteDataAsync<T, TResult>(this IEnumerable<T>                   source,
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

                                var result = partialData.Any(x => x.IsFail) ?
                                             FusionFailErros(partialData)   :
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










}
