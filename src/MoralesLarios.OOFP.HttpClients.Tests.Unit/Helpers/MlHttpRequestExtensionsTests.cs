using RichardSzalay.MockHttp;

namespace MoralesLarios.OOFP.HttpClients.Tests.Unit.Helpers;

public class MlHttpRequestExtensionsTests
{

    MockHttpMessageHandler mockHttp = new();

    HttpClient client;

    public MlHttpRequestExtensionsTests()
    {
        
        mockHttp.When("https://api.ejemplo.com")
            .WithHeaders("X-Page-Number", "123")
            .WithHeaders("X-Page-Size", "10");

        client = new HttpClient(mockHttp);
    }


    [Fact]
    public void SetHeaderInfo_ShouldAddHeaderToHttpClient()
    {
       
    }




}
