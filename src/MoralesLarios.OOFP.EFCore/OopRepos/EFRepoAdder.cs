namespace MoralesLarios.OOFP.EFCore.OopRepos;

internal class EFRepoAdder<T, TContext> : EFRepoBase, IEFRepoAdder<T>
    where T : class
    where TContext : DbContext
{
    public EFRepoAdder(TContext dbContext) : base(dbContext)
    {
    }

    public T Add(T item)
    {
        internalDbContext.Set<T>().Add(item);

        internalDbContext.SaveChanges();

        return item;
    }

    public async Task<T> AddAsync(T item, CancellationToken token = default)
    {
        await internalDbContext.Set<T>().AddAsync(item, token);

        await internalDbContext.SaveChangesAsync(token);

        return item;
    }

    public IEnumerable<T> AddRange(IEnumerable<T> items)
    {
        internalDbContext.Set<T>().AddRange(items);

        internalDbContext.SaveChanges();

        return items;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        await internalDbContext.Set<T>().AddRangeAsync(items, token);

        await internalDbContext.SaveChangesAsync(token);

        return items;
    }
}

