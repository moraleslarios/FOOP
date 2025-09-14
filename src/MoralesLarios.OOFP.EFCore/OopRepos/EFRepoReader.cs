namespace MoralesLarios.OOFP.EFCore.OopRepos;


internal class EFRepoReader<T, TContext> : EFRepoBase, IEFRepoReader<T>
    where T        : class
    where TContext : DbContext
{

    public EFRepoReader(TContext dbContext) : base(dbContext)
    {

    }




    public T Find(params object[] keys) => internalDbContext.Set<T>().Find(keys)!;

    public async Task<T> FindAsync(CancellationToken token = default, params object[] keys) => (await internalDbContext.Set<T>().FindAsync(keys, token))!;

    public T FirstOrDefault(Expression<Func<T, bool>> filter) => internalDbContext.Set<T>().FirstOrDefault(filter)!;

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default) => (await internalDbContext.Set<T>().FirstOrDefaultAsync(filter, token))!;

    public T First(Expression<Func<T, bool>> filter) => internalDbContext.Set<T>().First(filter)!;

    public async Task<T> FirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default) => (await internalDbContext.Set<T>().FirstAsync(filter, token))!;


    public IEnumerable<T> All() => internalDbContext.Set<T>().AsNoTracking();

    public async Task<IEnumerable<T>> AllAsync(CancellationToken token = default) => await internalDbContext.Set<T>().AsNoTracking().ToListAsync(token);

    public IEnumerable<T> GetData(Expression<Func<T, bool>> filter) => internalDbContext.Set<T>().AsNoTracking().Where(filter);

    public async Task<IEnumerable<T>> GetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default)
        => await internalDbContext.Set<T>().AsNoTracking().Where(filter).ToListAsync(token);



}