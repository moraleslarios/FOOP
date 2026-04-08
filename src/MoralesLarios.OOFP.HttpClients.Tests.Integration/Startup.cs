// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



using MoralesLarios.OOFP.HttpClients.Tests.Integration.Clients;

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


        //services.AddHttpClient("pruebas-api", client =>
        //{
        //    client.BaseAddress = new Uri("https://localhost:7197/api/Pruebas/");
        //});


        //services.AddHttpClient("vinos-api", client =>
        //{
        //    client.BaseAddress = new Uri("https://localhost:7197/api/Vinos/");
        //});


        services.AddGenClientFp<IPruebasClient, PruebasClient>(configureClient: client =>
        {
            client.BaseAddress = new Uri("https://localhost:7197/api/Pruebas/");
        });


    }
}

