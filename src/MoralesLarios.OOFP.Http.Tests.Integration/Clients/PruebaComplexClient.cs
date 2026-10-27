namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public class PruebaComplexClient(ILogger<PruebaComplexClient>   _logger,
                                 IRestClientFp<PruebaComplexDto> _genClientFp) : CompositeKeyRestClientFp<PruebaComplexDto>(_logger, _genClientFp), IPruebaComplexClient
{

}
