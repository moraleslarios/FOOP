
namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public class PruebaDuplexClient(ILogger<PruebaDuplexClient> _logger,
                                IHttpApiInvoker   _apiInvoker,
                                ServiceKey                         _clientName)
    : RestClientFp<PruebaRequestDto, PruebaResponseDto>(_logger, _apiInvoker, _clientName), IPruebaDuplexClient
{

}
