namespace MoralesLarios.OOFP.EFCore.OopRepos;

internal class EFRepoDeleter<T, TContext> : EFRepoBase, IEFRepoDeleter<T>
    where T : class
    where TContext : DbContext
{
    public EFRepoDeleter(TContext dbContext) : base(dbContext)
    {
    }

    public T Remove(T item)
    {
        internalDbContext.Set<T>().Remove(item);

        internalDbContext.SaveChanges();

        return item;
    }

    public async Task<T> RemoveAsync(T item, CancellationToken token = default)
    {
        internalDbContext.Set<T>().Remove(item);

        await internalDbContext.SaveChangesAsync(token);

        return item;
    }

    public IEnumerable<T> RemoveRange(IEnumerable<T> items)
    {
        internalDbContext.Set<T>().RemoveRange(items);

        internalDbContext.SaveChanges();

        return items;
    }

    public async Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        internalDbContext.Set<T>().RemoveRange(items);

        await internalDbContext.SaveChangesAsync(token);

        return items;
    }
}


