

namespace MoralesLarios.OOFP.HttpClients.Tests.Integration;

public class Startup
{

    private readonly IConfiguration _configuration;

    public Startup()
    {
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
    }


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClientsFp();

        services.AddHttpClient("default", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7240/api/Customer/");
        });
    }
}
