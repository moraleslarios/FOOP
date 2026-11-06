namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public class PruebaComplexClient(IRestClientFp<PruebaComplexDto> _genClientFp) : CompositeKeyRestClientFp<PruebaComplexDto>(_genClientFp), IPruebaComplexClient
{

}
