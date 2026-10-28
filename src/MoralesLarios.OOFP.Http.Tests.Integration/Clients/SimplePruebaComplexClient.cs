namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public class SimplePruebaComplexClient(ILogger<SimplePruebaComplexClient> _logger,
                                       IHttpApiInvoker    _apiInvoker,
                                       ServiceKey                          _clientName) : RestClientFp<PruebaComplexDto>(_logger, _apiInvoker, _clientName), ISimplePruebaComplexClient
{

}

