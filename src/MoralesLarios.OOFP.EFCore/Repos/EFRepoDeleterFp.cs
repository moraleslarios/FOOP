
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EFCore.Repos;

public class EFRepoDeleterFp<T, TContext>(TContext dbContext) : EFRepoBaseFp(dbContext), IEFRepoDeleterFp<T>
    where T        : class
    where TContext : DbContext
{

    internal readonly IEFRepoDeleter <T>? _internalRepoDeleter = RegisterServices.ResolveRepoFp<IEFRepoDeleter <T>>();
    internal readonly IEFRepoReaderFp<T>? _repoReaderFp        = RegisterServices.ResolveRepoFp<IEFRepoReaderFp<T>>();




    public MlResult<T> TryRemove(T item)
    {
        var result = EnsureFp.NotNull(item, "The entity item to remove cannot be null")
                            .TryMap(x => _internalRepoDeleter!.Remove(item));
        return result;
    }

    public MlResult<T> TryRemove(params object[] pk)
        => TryRemove(null!, pk);

    public MlResult<T> TryRemove(MlErrorsDetails notFoundErrorDetails, params object[] pk)
    {
        var result = EnsureFp.NotEmpty(pk, "The object array pk cannot be empty")
                                .Bind  ( pk      => notFoundErrorDetails is null        
                                                        ? _repoReaderFp!.TryFind(pk)
                                                        : _repoReaderFp!.TryFind(notFoundErrorDetails, pk))
                                .TryMap( bdItem  => _internalRepoDeleter!.Remove(bdItem));
        return result;
    }


    public async Task<MlResult<T>> TryRemoveAsync(T item, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(item, "The entity item to remove cannot be null")
                            .TryMapAsync(x => _internalRepoDeleter!.RemoveAsync(item, token));
        return result;
    }


    public async Task<MlResult<T>> TryRemoveAsync(CancellationToken token = default, params object[] pk)
    {
        var result = await EnsureFp.NotEmptyAsync(pk, "The object array pk cannot be empty")
                                    .BindAsync  ( _       => _repoReaderFp!.TryFindAsync(token, pk))
                                    .TryMapAsync( bdItem  => _internalRepoDeleter!.RemoveAsync(bdItem, token));
        return result;
    }

    public async Task<MlResult<T>> TryRemoveAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk)
    {
        var result = await EnsureFp.NotEmptyAsync(pk, "The object array pk cannot be empty")
                                    .BindAsync  ( _       => notFoundErrorDetails is null
                                                                ? _repoReaderFp!.TryFindAsync(token, pk)
                                                                : _repoReaderFp!.TryFindAsync(notFoundErrorDetails, token, pk))
                                    .TryMapAsync( bdItem  => _internalRepoDeleter!.RemoveAsync(bdItem, token));
        return result;
    }


    public MlResult<IEnumerable<T>> TryRemoveRange(IEnumerable<T> items)
    {
        var result = EnsureFp.NotNull(items, "The entity items to remove cannot be null")
                            .TryMap(x => _internalRepoDeleter!.RemoveRange(items));
        return result;
    }

    public Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        var result = EnsureFp.NotNull(items, "The entity items to remove cannot be null")
                            .TryMapAsync(x => _internalRepoDeleter!.RemoveRangeAsync(items, token));
        return result;
    }

}