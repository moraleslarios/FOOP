namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public class SimplePruebaComplexClient(IHttpApiInvoker _apiInvoker,
                                       ServiceKey _clientName) : RestClientFp<PruebaComplexDto>(_apiInvoker, _clientName), ISimplePruebaComplexClient
{

}

