namespace MoralesLarios.OOFP.WebServices.Tests.Unit;

public class Startup
{

    private readonly IConfiguration _configuration;

    public Startup()
    {
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
    }


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScopedtGenServicesFpWithoutReposGeneral();
    }
}
