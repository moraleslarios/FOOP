
namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public class PruebaDuplexClient(ILogger<PruebaDuplexClient> _logger,
                                IHttpClientFactoryManager   _httpClientFactoryManager,
                                ServiceKey                         _httpClientFactoryKey)
    : GenClientFp<PruebaRequestDto, PruebaResponseDto>(_logger, _httpClientFactoryManager, _httpClientFactoryKey), IPruebaDuplexClient
{

}
