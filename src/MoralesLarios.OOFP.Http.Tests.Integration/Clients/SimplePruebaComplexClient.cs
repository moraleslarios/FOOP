namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public class SimplePruebaComplexClient(ILogger<SimplePruebaComplexClient> _logger,
                                       IHttpClientFactoryManager    _httpClientFactoryManager,
                                       ServiceKey                          _httpClientFactoryKey) : GenClientFp<PruebaComplexDto>(_logger, _httpClientFactoryManager, _httpClientFactoryKey), ISimplePruebaComplexClient
{

}

