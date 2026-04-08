// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.HttpClients.Tests.Integration.Clients;

public class PruebasClient(ILogger<PruebasClient>    _logger,
                           IHttpClientFactoryManager _httpClientFactoryManager,
                           Key                       _clientHttpFactoryKey) : GenClientFp<PruebasDto>(_logger, _httpClientFactoryManager, _clientHttpFactoryKey), IPruebasClient
{


    public async Task<MlResult<PruebasDto>> MyGetAsync(NotEmptyString data)
    {
        var result = await _httpClientFactoryManager.GetAsync<PruebasDto>(_clientHttpFactoryKey, 
                                                                           $"with-header",
                                                                           new Dictionary<string, string> { {"data", data } });
        return result;
    }





    //[Fact]
    //public async Task GetAllAsync_integracion()
    //{
    //    var result = await GetAllAsync();
    //}


    //[Fact]
    //public async Task GetByIdAsync_integracion()
    //{
    //    var result = await GetByIdAsync("1");
    //}


    //[Fact]
    //public async Task PostAsync_integracion()
    //{
    //    var item = new PruebasDto
    //    {
    //        Id = 0,
    //        Nombre = "Ábalos",
    //        Comentarios = "El de Jésica"
    //    };
    //    var result = await PostAsync(item);
    //}

    //[Fact]
    //public async Task PutAsync_integracion()
    //{
    //    var item = new PruebasDto
    //    {
    //        Id = 14,
    //        Nombre = "Ábalos 2",
    //        Comentarios = "El de Jésica - Modificado"
    //    };

    //    var result = await PutAsync(item);
    //}

    //[Fact]
    //public async Task DeleteByIdAsync_integracion()
    //{
    //    var result = await DeleteByIdAsync("14");
    //}

    //[Fact]
    //public async Task DeleteAsync_integracion()
    //{
    //    var item = new PruebasDto
    //    {
    //        Id = 11,
    //        Nombre = "Ábalos 2",
    //        Comentarios = "El de Jésica - Modificado"
    //    };
    //    var result = await DeleteAsync(item);
    //}

}

