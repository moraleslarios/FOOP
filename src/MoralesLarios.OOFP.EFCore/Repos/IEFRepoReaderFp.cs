using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EFCore.Repos;

public interface IEFRepoReaderFp<T> where T : class
{
    MlResult<T> TryFind(params object[] pk);
    Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] pk);
    Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    MlResult<T> TryFirstOrDefault(Expression<Func<T, bool>> filter);
    Task<MlResult<T>> TryFirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    MlResult<T> TryFirst(Expression<Func<T, bool>> filter);
    MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter);
    Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
    MlResult<IEnumerable<T>> TryAll();
    Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default);
    MlResult<T> TryFind(MlErrorsDetails notFoundErrorDetails, params object[] pk);
    Task<MlResult<T>> TryFindAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk);
    MlResult<T> TryFirstOrDefault(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails);
    Task<MlResult<T>> TryFirstOrDefaultAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default);
    MlResult<T> TryFirst(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails);
    Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default);
}