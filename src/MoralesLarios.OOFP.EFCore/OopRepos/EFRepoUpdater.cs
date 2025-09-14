namespace MoralesLarios.OOFP.EFCore.OopRepos;

internal class EFRepoUpdater<T, TContext> : EFRepoBase, IEFRepoUpdater<T>
    where T : class
    where TContext : DbContext
{
    public EFRepoUpdater(TContext dbContext) : base(dbContext)
    {
    }

    public T Update(T item)
    {
        internalDbContext.Update(item);

        internalDbContext.SaveChanges();

        return item;
    }

    public async Task<T> UpdateAsync(T item, CancellationToken token = default)
    {
        internalDbContext.Update(item);

        await internalDbContext.SaveChangesAsync(token);

        return item;
    }

    public IEnumerable<T> UpdateRange(IEnumerable<T> items)
    {
        internalDbContext.UpdateRange(items);

        internalDbContext.SaveChanges();

        return items;
    }

    public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        internalDbContext.UpdateRange(items);

        await internalDbContext.SaveChangesAsync(token);

        return items;
    }
}
