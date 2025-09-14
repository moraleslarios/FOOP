

namespace MoralesLarios.OOFP.EFCore.Repos;

public interface IEFRepoAdderFp<T>
    where T : class
{
    MlResult<T> TryAdd(T item);
    Task<MlResult<T>> TryAddAsync(T item, CancellationToken token = default);
    MlResult<IEnumerable<T>> TryAddRange(IEnumerable<T> items);
    Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
}