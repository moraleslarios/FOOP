using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EFCore.Repos;


public class EFRepoReaderFp<T, TContext>(TContext dbContext) : EFRepoBaseFp(dbContext), IEFRepoReaderFp<T>
    where T        : class
    where TContext : DbContext
{

    internal readonly IEFRepoReader<T>? _repoReader = RegisterServices.ResolveRepoFp<IEFRepoReader<T>>();

    private string typeName = typeof(T).Name;




    public MlResult<T> TryFind(MlErrorsDetails notFoundErrorDetails, params object[] pk)
    {
        var result = EnsureFp.NotNull(pk, "The object arrya pk cannot be null")
                                .TryMap (x => EnsureFp.NotEmpty(pk, "The object array pk cannot be empty"))
                                .TryMap (x => _repoReader!.Find(pk))
                                .TryBind(x => x.NullToFailed(notFoundErrorDetails));
        return result;
    }

    public MlResult<T> TryFind(params object[] pk)
        => TryFind($"{pk.GetPkValues()} values not found data in {typeName}", pk);


    public async Task<MlResult<T>> TryFindAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk)
    {
        var result = await EnsureFp.NotNullAsync(pk, "The object arrya pk cannot be null")
                                    .TryMapAsync (x => EnsureFp.NotEmptyAsync(pk, "The object arrya pk cannot be empty"))
                                    .TryMapAsync (x => _repoReader!.FindAsync(token, pk))
                                    .TryBindAsync(x => x.NullToFailedAsync(notFoundErrorDetails));
        return result;
    }

    public Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] pk)
        => TryFindAsync($"{pk.GetPkValues()} values not found data in {typeName}", token, pk);


    public MlResult<T> TryFirstOrDefault(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails)
    {
        var result = EnsureFp.NotNull(filter, "The filter cannot be null")
                                .TryMap (x => _repoReader!.FirstOrDefault(filter))
                                .TryBind(x => x.NullToFailed(notFoundErrorDetails));
        return result;
    }


    public MlResult<T> TryFirstOrDefault(Expression<Func<T, bool>> filter)
        => TryFirstOrDefault(filter, $"The query did not return any elements");


    public async Task<MlResult<T>> TryFirstOrDefaultAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default!)
    {
        var result = await EnsureFp.NotNullAsync(filter, "The filter cannot be null")
                                    .TryMapAsync (x => _repoReader!.FirstOrDefaultAsync(filter, token))
                                    .TryBindAsync(x => x.NullToFailedAsync(notFoundErrorDetails));
        return result;
    }

    public Task<MlResult<T>> TryFirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!)
        => TryFirstOrDefaultAsync(filter, $"The query did not return any elements", token);


    public MlResult<T> TryFirst(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails)
    {
        var result = EnsureFp.NotNull(filter, "The filter cannot be null")
                                .TryMap (x => _repoReader!.First(filter))
                                .TryBind(x => x.NullToFailed(notFoundErrorDetails));
        return result;
    }


    public MlResult<T> TryFirst(Expression<Func<T, bool>> filter) => TryFirst(filter, $"The query did not return any elements");


    public async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default!)
    {
        var result = await EnsureFp.NotNullAsync(filter, "The filter cannot be null")
                                    .TryMapAsync(x => _repoReader!.FirstAsync(filter, token))
                                    .TryBindAsync(x => x.NullToFailedAsync($"The query did not return any elements"));

        return result;
    }

    public Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!)
        => TryFirstAsync(filter, $"The query did not return any elements", token);



    public MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter)
    {
        var result = EnsureFp.NotNull(filter, "The filter cannot be null")
                                .TryMap(x => _repoReader!.GetData(filter));

        return result;
    }


    public async Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!)
    {
        var result = await EnsureFp.NotNullAsync(filter, "The filter cannot be null")
                                    .TryMapAsync(x => _repoReader!.GetDataAsync(filter, token));

        return result;
    }


    public MlResult<IEnumerable<T>> TryAll()
    {
        var result = MlResult<IEnumerable<T>>._
                            .TryMap(x => _repoReader!.All());

        return result;
    }


    public async Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default!)
    {
        var result = await MlResult<IEnumerable<T>>._
                                .TryMapAsync(x => _repoReader!.AllAsync(token: token));

        return result;
    }


}


