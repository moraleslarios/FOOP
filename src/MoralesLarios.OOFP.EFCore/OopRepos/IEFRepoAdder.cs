

namespace MoralesLarios.OOFP.EFCore.OopRepos;

public interface IEFRepoAdder<T> where T : class
{
    T Add(T item);
    Task<T> AddAsync(T item, CancellationToken token = default);
    IEnumerable<T> AddRange(IEnumerable<T> items);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}