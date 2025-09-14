
namespace MoralesLarios.OOFP.EFCore.Repos;

public class EFRepoAdderFp<T, TContext>(TContext dbContext) : EFRepoBaseFp(dbContext), IEFRepoAdderFp<T>
    where T        : class
    where TContext : DbContext
{

    internal readonly IEFRepoAdder<T>? _internalRepoAdder = RegisterServices.ResolveRepoFp<IEFRepoAdder<T>>();



    public MlResult<T> TryAdd(T item)
    {
        var result = EnsureFp.NotNull(item, "The entity item to add cannot be null")
                            .TryMap(x => _internalRepoAdder!.Add(item));

        return result;
    }

    public async Task<MlResult<T>> TryAddAsync(T item, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(item, "The entity item to add cannot be null")
                            .TryMapAsync(x => _internalRepoAdder!.AddAsync(item, token));

        return result;
    }


    public MlResult<IEnumerable<T>> TryAddRange(IEnumerable<T> items)
    {
        var result = EnsureFp.NotNull(items, "The entity items to addRange cannot be null")
                            .TryMap(x => _internalRepoAdder!.AddRange(items));

        return result;
    }

    public async Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(items, "The entity items to addRange cannot be null")
                            .TryMapAsync(x => _internalRepoAdder!.AddRangeAsync(items, token));

        return result;
    }



}

