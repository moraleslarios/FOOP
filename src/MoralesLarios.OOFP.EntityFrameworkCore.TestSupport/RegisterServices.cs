// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0




using Microsoft.Data.Sqlite;
using MoralesLarios.OOFP.EntityFrameworkCore.TestSupport.Repos;

namespace MoralesLarios.OOFP.EntityFrameworkCore.TestSupport;

public static class RegisterServices
{

    public static IServiceCollection AddInfrastructureTests(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required for integration tests.");
        }

        var connection = new SqliteConnection(connectionString);
        connection.Open();

        var seedPath = Path.Combine(AppContext.BaseDirectory, "App_Data", "jfcatas.db");
        if (!File.Exists(seedPath))
        {
            throw new FileNotFoundException("The SQLite integration seed database was not copied to the test output.", seedPath);
        }

        using (var seedConnection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = seedPath }.ToString()))
        {
            seedConnection.Open();
            seedConnection.BackupDatabase(connection);
        }

        services.AddSingleton(connection);
        services.AddDbContext<TestDbContext>(options =>
            options.UseSqlite(connection));

        services.AddOofpRepositories<Vino, TestDbContext>();
        services.AddOofpRepositories<Cata                , TestDbContext>();
        services.AddOofpRepositories<VinosCata, TestDbContext>();
        services.AddOofpRepositories<VinosCatasPuntuacion, TestDbContext>();

        // Register specific repository implementations
        services.AddScoped<IVinosRepo, VinosRepo>();
        services.AddScoped<ICatasRepo, CatasRepo>();
        services.AddScoped<IVinosCatasRepo, VinosCatasRepo>();
        services.AddScoped<IVinosCatasPuntuacionesRepo, VinosCatasPuntuacionesRepo>();

        return services;
    }
}

