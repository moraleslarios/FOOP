

namespace MoralesLarios.OOFP.HttpClients.Tests.Integration.Clients;

public interface IPruebasClient : IGenClientFp<PruebasDto>
{
    Task<MlResult<IEnumerable<PruebasDto>>> MyGetAllwithCache2Async();
    Task<MlResult<IEnumerable<PruebasDto>>> MyGetAllwithCacheAsync();
    Task<MlResult<IEnumerable<PruebasDto>>> MyGetAllwithoutCache2Async();
    Task<MlResult<IEnumerable<PruebasDto>>> MyGetAllwithoutCacheAsync();
    Task<MlResult<PruebasDto>> MyGetAsync(NotEmptyString data);
}