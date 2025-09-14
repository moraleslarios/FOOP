namespace MoralesLarios.OOFP.EFCore.OopRepos;

public interface IEFRepoDeleter<T> where T : class
{
    T Remove(T item);
    Task<T> RemoveAsync(T item, CancellationToken token = default);
    IEnumerable<T> RemoveRange(IEnumerable<T> items);
    Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}