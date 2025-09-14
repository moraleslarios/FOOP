namespace MoralesLarios.OOFP.EFCore.OopRepos;

internal class EFRepoWriter<T, TContext> : EFRepoBase, IEFRepoWriter<T>
    where T        : class
    where TContext : DbContext
{
    private readonly IEFRepoAdder<T>?   _repoAdder;
    private readonly IEFRepoDeleter<T>? _repoDeleter;
    private readonly IEFRepoUpdater<T>? _repoUpdater;

    public EFRepoWriter(TContext dbContext) : base(dbContext)
    {
        _repoAdder   = RegisterServices.ServiceProvider.GetService<IEFRepoAdder  <T>>();
        _repoDeleter = RegisterServices.ServiceProvider.GetService<IEFRepoDeleter<T>>();
        _repoUpdater = RegisterServices.ServiceProvider.GetService<IEFRepoUpdater<T>>();
    }

    public T Add(T item) => _repoAdder!.Add(item);
    public async Task<T> AddAsync(T item, CancellationToken token = default) => await _repoAdder!.AddAsync(item, token);

    public IEnumerable<T> AddRange(IEnumerable<T> items) => _repoAdder!.AddRange(items);

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoAdder!.AddRangeAsync(items, token);

    public T Remove(T item) => _repoDeleter!.Remove(item);

    public async Task<T> RemoveAsync(T item, CancellationToken token = default) => await _repoDeleter!.RemoveAsync(item, token);

    public IEnumerable<T> RemoveRange(IEnumerable<T> items) => _repoDeleter!.RemoveRange(items);

    public async Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoDeleter!.RemoveRangeAsync(items, token);

    public T Update(T item) => _repoUpdater!.Update(item);

    public async Task<T> UpdateAsync(T item, CancellationToken token = default) => await _repoUpdater!.UpdateAsync(item, token);

    public IEnumerable<T> UpdateRange(IEnumerable<T> items) => _repoUpdater!.UpdateRange(items);

    public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default) => await _repoUpdater!.UpdateRangeAsync(items, token);
}
