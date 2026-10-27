namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public class SimplePruebaComplexClient(ILogger<SimplePruebaComplexClient> _logger,
                                       IHttpApiInvoker    _httpClientFactoryManager,
                                       ServiceKey                          _httpClientFactoryKey) : RestClientFp<PruebaComplexDto>(_logger, _httpClientFactoryManager, _httpClientFactoryKey), ISimplePruebaComplexClient
{

}

