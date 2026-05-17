namespace MoralesLarios.OOFP.HttpClients.Tests.Integration.Clients;

public class PruebaComplexClient(ILogger<PruebaComplexClient>   _logger,
                                 IGenClientFp<PruebaComplexDto> _genClientFp) : GenComplexClientFp<PruebaComplexDto>(_logger, _genClientFp), IPruebaComplexClient
{

}
