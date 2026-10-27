namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public class SimplePruebaComplexClient(ILogger<SimplePruebaComplexClient> _logger,
                                       IHttpClientFactoryManager    _httpClientFactoryManager,
                                       Key                          _httpClientFactoryKey) : GenClientFp<PruebaComplexDto>(_logger, _httpClientFactoryManager, _httpClientFactoryKey), ISimplePruebaComplexClient
{

}

