using Microsoft.EntityFrameworkCore;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EFCore.Repos;

public class EFRepoUpdaterFp<T, TContext>(TContext dbContext) : EFRepoBaseFp(dbContext), IEFRepoUpdaterFp<T>
        where T        : class
        where TContext : DbContext
{

    internal readonly IEFRepoUpdater<T>?  _internalRepoUpdater = RegisterServices.ResolveRepoFp<IEFRepoUpdater <T>>();
    internal readonly IEFRepoReaderFp<T>? _repoReaderFp        = RegisterServices.ResolveRepoFp<IEFRepoReaderFp<T>>();





    public MlResult<T> TryUpdate(T item)
    {
        var result = EnsureFp.NotNull(item, "The entity item to update cannot be null")
                            .TryMap(x => _internalRepoUpdater!.Update(item));
        return result;
    }


    public MlResult<T> TryUpdate(T item, params object[] pk) => TryUpdate(item, null!, pk);

    public MlResult<T> TryUpdate(T item, MlErrorsDetails notFoundErrorDetails, params object[] pk)
    {
        var result = EnsureFp.NotNull  (item, "The entity item to update cannot be null")
                                .Bind  ( _  => EnsureFp.NotEmpty(pk, "The object array pk cannot be empty"))
                                .Bind  ( pk => notFoundErrorDetails is null 
                                                ? _repoReaderFp!.TryFind(pk) 
                                                : _repoReaderFp!.TryFind(notFoundErrorDetails, pk))
                                .TryMap( x  => _internalRepoUpdater!.Update(item));
        return result;
    }


    public async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(item, "The entity item to update cannot be null")
                            .TryMapAsync(x => _internalRepoUpdater!.UpdateAsync(item, token));
        return result;
    }


    public async Task<MlResult<T>> TryUpdateAsync(T item, CancellationToken token = default, params object[] pk)
    {
        var result = await EnsureFp.NotNullAsync( item, "The entity item to update cannot be null")
                                    .BindAsync  ( _  => EnsureFp.NotEmptyAsync(pk, "The object array pk cannot be empty"))
                                    .BindAsync  ( _ => _repoReaderFp!.TryFindAsync(token, pk))
                                    .TryMapAsync( x  => _internalRepoUpdater!.UpdateAsync(item, token));
        return result;
    }

    public async Task<MlResult<T>> TryUpdateAsync(T item, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk)
    {
        var result = await EnsureFp.NotNullAsync( item, "The entity item to update cannot be null")
                                    .BindAsync  ( _  => EnsureFp.NotEmptyAsync(pk, "The object array pk cannot be empty"))
                                    .BindAsync  ( _ => notFoundErrorDetails is null 
                                                        ? _repoReaderFp!.TryFindAsync(token, pk) 
                                                        : _repoReaderFp!.TryFindAsync(notFoundErrorDetails, token, pk))
                                    .TryMapAsync( x  => _internalRepoUpdater!.UpdateAsync(item, token));
        return result;
    }


    public MlResult<IEnumerable<T>> TryUpdateRange(IEnumerable<T> items)
    {
        var result = EnsureFp.NotNull(items, "The entity items to update cannot be null")
                            .TryMap(x => _internalRepoUpdater!.UpdateRange(items));
        return result;
    }

    public async Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(items, "The entity items to update cannot be null")
                            .TryMapAsync(x => _internalRepoUpdater!.UpdateRangeAsync(items, token));
        return result;
    }



}

