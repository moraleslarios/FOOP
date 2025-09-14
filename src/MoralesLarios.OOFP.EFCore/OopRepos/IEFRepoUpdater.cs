namespace MoralesLarios.OOFP.EFCore.OopRepos;

public interface IEFRepoUpdater<T> where T : class
{
    T Update(T item);
    Task<T> UpdateAsync(T item, CancellationToken token = default);
    IEnumerable<T> UpdateRange(IEnumerable<T> items);
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}