// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



using MoralesLarios.OOFP.Http.Tests.Integration.Clients;

namespace MoralesLarios.OOFP.Http.Tests.Integration;

public class Startup
{

    private readonly IConfiguration _configuration;

    public Startup()
    {
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
    }


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMlHttp();

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


        services.AddRestClient<IPruebasClient, PruebasClient>(configureClient: client =>
        {
            client.BaseAddress = new Uri("https://localhost:7197/api/Pruebas/");
        });

        services.AddRestClient<ISimplePruebaComplexClient, SimplePruebaComplexClient>(configureClient: client =>
        {
            client.BaseAddress = new Uri("https://localhost:7197/api/PruebaComplex/");
        });

        services.AddCompositeKeyRestClient<IPruebaComplexClient, PruebaComplexClient, PruebaComplexDto>(configureClient: client =>
        {
            client.BaseAddress = new Uri("https://localhost:7197/api/PruebaComplex/");
        });

        services.AddCompositeKeyRestClient<IPruebaDuplexClient, PruebaDuplexClient, PruebaRequestDto, PruebaResponseDto>(configureClient: client =>
        {
            client.BaseAddress = new Uri("https://localhost:7197/api/PruebaDuplex/");
        });
    }
}

