namespace MoralesLarios.OOFP.EFCore.OopRepos;

internal interface IEFRepo<T>
{
    Task<IEnumerable<T>> AllAsync { get; }

    T Add(T item);
    Task<T> AddAsync(T item, CancellationToken token = default);

    IEnumerable<T> AddRange(IEnumerable<T> items);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
    IEnumerable<T> All();
    T Find(params object[] keys);
    Task<T> FindAsync(CancellationToken token = default, params object[] keys);

    T FirstOrDefault(Expression<Func<T, bool>> filter);
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);

    IEnumerable<T> GetData(Expression<Func<T, bool>> filter);
    Task<IEnumerable<T>> GetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);

    T Remove(T item);
    Task<T> RemoveAsync(T item, CancellationToken token = default);

    IEnumerable<T> RemoveRange(IEnumerable<T> items);
    Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default);

    T Update(T item);
    Task<T> UpdateAsync(T item, CancellationToken token = default);

    IEnumerable<T> UpdateRange(IEnumerable<T> items);
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}
