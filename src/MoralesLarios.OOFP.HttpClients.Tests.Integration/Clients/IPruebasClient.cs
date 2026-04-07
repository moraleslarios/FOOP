
namespace MoralesLarios.OOFP.HttpClients.Tests.Integration.Clients;

public interface IPruebasClient : IGenClientFp<PruebasDto>
{
    Task<MlResult<PruebasDto>> MyGetAsync(NotEmptyString data);
}