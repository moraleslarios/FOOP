
namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public class PruebaDuplexClient(ILogger<PruebaDuplexClient> _logger,
                                IHttpApiInvoker   _httpClientFactoryManager,
                                ServiceKey                         _httpClientFactoryKey)
    : RestClientFp<PruebaRequestDto, PruebaResponseDto>(_logger, _httpClientFactoryManager, _httpClientFactoryKey), IPruebaDuplexClient
{

}
