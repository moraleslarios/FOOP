namespace MoralesLarios.OOFP.EFCore.OopRepos;

internal class EFRepoReaderPagination<T, TContext> : EFRepoBase, IEFRepoReaderPagination<T>
    where T : class
    where TContext : DbContext
{
    public EFRepoReaderPagination(TContext dbContext) : base(dbContext)
    {
    }

    public IEnumerable<T> GetData(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null!) => GetInternalData(pageNumber, pageSize, filter).ToList();

    public async Task<IEnumerable<T>> GetDataAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null!, CancellationToken token = default)
        => await GetInternalData(pageNumber, pageSize, filter).ToListAsync(token);

    public IEnumerable<T> GetDataOrderby<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> filter = null!)
        => GetInternalData(pageNumber, pageSize, filter).OrderBy(orderBy).ToList();

    public async Task<IEnumerable<T>> GetDataOrderbyAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> filter = null!, CancellationToken token = default)
        => await GetInternalData(pageNumber, pageSize, filter).OrderBy(orderBy).ToListAsync(token);

    public IEnumerable<T> GetDataOrderbyDescending<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderByDescending, Expression<Func<T, bool>> filter = null!)
        => GetInternalData(pageNumber, pageSize, filter).OrderByDescending(orderByDescending).ToList();


    public async Task<IEnumerable<T>> GetDataOrderbyDescendingAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderByDescending, Expression<Func<T, bool>> filter = null!, CancellationToken token = default)
        => await GetInternalData(pageNumber, pageSize, filter).OrderByDescending(orderByDescending).ToListAsync(token);

    private IQueryable<T> GetInternalData(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null!)
    {
        var internalFilter = filter ?? (x => true);

        var from = (pageNumber - 1) * pageSize;

        var result = internalDbContext.Set<T>()
                        .AsNoTracking()
                        .Where(internalFilter)
                        .Skip(from)
                        .Take(pageSize);
                        

        return result;
    }



}
