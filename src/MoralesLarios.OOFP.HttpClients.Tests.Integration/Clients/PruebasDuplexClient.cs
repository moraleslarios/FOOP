
namespace MoralesLarios.OOFP.HttpClients.Tests.Integration.Clients;

public class PruebaDuplexClient(ILogger<PruebaDuplexClient> _logger,
                                IHttpClientFactoryManager   _httpClientFactoryManager,
                                Key                         _httpClientFactoryKey)
    : GenClientFp<PruebaRequestDto, PruebaResponseDto>(_logger, _httpClientFactoryManager, _httpClientFactoryKey), IPruebaDuplexClient
{

}
