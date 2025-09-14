using MoralesLarios.OOFP.EFCore.OopRepos;

namespace MoralesLarios.OOFP.EFCore.Repos;

public interface IEFRepoPaginationFp<T> : IEFRepoFp<T>, IEFPaginatorFp<T>
    where T : class
{
}