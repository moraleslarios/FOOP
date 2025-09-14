


using MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Repos;

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests;

public static class RegisterServices
{

    public static IServiceCollection AddInfrastructureTests(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            EnsureDatabaseDirectoryExists(connectionString);
        }

        // Configure SQLite Database
        services.AddDbContext<JfCatasDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddTransientOOFPRepos<Vino, JfCatasDbContext>();
        services.AddTransientOOFPRepos<Cata                , JfCatasDbContext>();
        services.AddTransientOOFPRepos<VinosCata, JfCatasDbContext>();
        services.AddTransientOOFPRepos<VinosCatasPuntuacion, JfCatasDbContext>();

        // Register specific repository implementations
        services.AddTransient<IVinosRepo, VinosRepo>();
        services.AddTransient<ICatasRepo, CatasRepo>();
        services.AddTransient<IVinosCatasRepo, VinosCatasRepo>();
        services.AddTransient<IVinosCatasPuntuacionesRepo, VinosCatasPuntuacionesRepo>();

        return services;
    }

    private static void EnsureDatabaseDirectoryExists(string connectionString)
    {
        try
        {
            // Extract the Data Source path from the connection string
            var dataSourceStart = connectionString.IndexOf("Data Source=", StringComparison.OrdinalIgnoreCase);
            if (dataSourceStart >= 0)
            {
                var pathStart = dataSourceStart + "Data Source=".Length;
                var pathEnd = connectionString.IndexOf(';', pathStart);
                var dbPath = pathEnd > 0 ? connectionString.Substring(pathStart, pathEnd - pathStart) : connectionString.Substring(pathStart);

                // Get the directory from the database file path
                var directory = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }
        catch (Exception)
        {
            // Silently continue if directory creation fails
            // Azure might have different permissions or directory structure
        }
    }

}
