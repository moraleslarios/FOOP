namespace MoralesLarios.OOFP.EFCore.OopRepos;

internal interface IEFRepoReader<T>
    where T : class
{
    T Find(params object[] keys);
    Task<T> FindAsync(CancellationToken token = default, params object[] keys);
    T First(Expression<Func<T, bool>> filter);
    T FirstOrDefault(Expression<Func<T, bool>> filter);
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    Task<T> FirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    IEnumerable<T> GetData(Expression<Func<T, bool>> filter);
    Task<IEnumerable<T>> GetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    IEnumerable<T> All();
    Task<IEnumerable<T>> AllAsync(CancellationToken token = default);
}