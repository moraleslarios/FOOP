using MoralesLarios.OOFP.EFCore;

namespace MoralesLarios.OOFP.EFCore.OopRepos;

internal class EFRepo<T, TContext> : EFRepoBase, IEFRepo<T>
    where T : class
    where TContext : DbContext
{
    internal readonly IEFRepoReader<T>? _repoReader;
    internal readonly IEFRepoWriter<T>? _repoWriter;

    public EFRepo(TContext dbContext) : base(dbContext)
    {

        _repoReader = RegisterServices.ServiceProvider.GetService<IEFRepoReader<T>>();
        _repoWriter = RegisterServices.ServiceProvider.GetService<IEFRepoWriter<T>>();
    }

    public T Add(T item) => _repoWriter!.Add(item);
    public Task<T> AddAsync(T item, CancellationToken token = default) => _repoWriter!.AddAsync(item, token);

    public IEnumerable<T> AddRange(IEnumerable<T> items) => _repoWriter!.AddRange(items);

    public Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items, CancellationToken token = default) => _repoWriter!.AddRangeAsync(items, token);

    public T Find(params object[] keys) => _repoReader!.Find(keys);

    public Task<T> FindAsync(CancellationToken token = default, params object[] keys) => _repoReader!.FindAsync(token, keys);

    public T FirstOrDefault(Expression<Func<T, bool>> filter) => _repoReader!.FirstOrDefault(filter);

    public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default) => _repoReader!.FirstOrDefaultAsync(filter, token);

    public IEnumerable<T> All() => _repoReader!.All();

    public Task<IEnumerable<T>> AllAsync => _repoReader!.AllAsync();


    public IEnumerable<T> GetData(Expression<Func<T, bool>> filter) => _repoReader!.GetData(filter);

    public Task<IEnumerable<T>> GetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default) => _repoReader!.GetDataAsync(filter, token);

    public T Remove(T item) => _repoWriter!.Remove(item);

    public Task<T> RemoveAsync(T item, CancellationToken token = default) => _repoWriter!.RemoveAsync(item, token);

    public IEnumerable<T> RemoveRange(IEnumerable<T> items) => _repoWriter!.RemoveRange(items);

    public Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default) => _repoWriter!.RemoveRangeAsync(items, token);

    public T Update(T item) => _repoWriter!.Update(item);

    public Task<T> UpdateAsync(T item, CancellationToken token = default) => _repoWriter!.UpdateAsync(item, token);

    public IEnumerable<T> UpdateRange(IEnumerable<T> items) => _repoWriter!.UpdateRange(items);

    public Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default) => _repoWriter!.UpdateRangeAsync(items, token);

}

