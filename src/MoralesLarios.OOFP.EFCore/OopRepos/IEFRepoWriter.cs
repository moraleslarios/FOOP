namespace MoralesLarios.OOFP.EFCore.OopRepos;

internal interface IEFRepoWriter<T> where T : class
{
    T Add(T item);
    Task<T> AddAsync(T item, CancellationToken token = default);
    IEnumerable<T> AddRange(IEnumerable<T> items);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
    T Remove(T item);
    Task<T> RemoveAsync(T item, CancellationToken token = default);
    IEnumerable<T> RemoveRange(IEnumerable<T> items);
    Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default);
    T Update(T item);
    Task<T> UpdateAsync(T item, CancellationToken token = default);
    IEnumerable<T> UpdateRange(IEnumerable<T> items);
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}