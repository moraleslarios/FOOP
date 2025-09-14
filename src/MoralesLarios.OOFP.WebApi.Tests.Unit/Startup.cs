

namespace MoralesLarios.OOFP.WebApi.Tests.Unit;

public class Startup
{

    private IConfiguration _configuration;

    public Startup()
    {
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
    }


    public void ConfigureServices(IServiceCollection services)
    {

    }
}
