
namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public class PruebaDuplexClient(IHttpApiInvoker _apiInvoker,
                                ServiceKey _clientName)
    : RestClientFp<PruebaRequestDto, PruebaResponseDto>(_apiInvoker, _clientName), IPruebaDuplexClient
{

}
