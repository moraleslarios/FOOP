// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Reflection.PortableExecutable;

namespace MoralesLarios.OOFP.HttpClients.Tests.Integration.Clients;

public class PruebasClient(ILogger<PruebasClient>    _logger,
                           IHttpClientFactoryManager _httpClientFactoryManager,
                           Key                       _httpClientFactoryKey) : GenClientFp<PruebasDto>(_logger, _httpClientFactoryManager, _httpClientFactoryKey), IPruebasClient
{


    public async Task<MlResult<PruebasDto>> MyGetAsync(NotEmptyString data)
    {
        var result = await _httpClientFactoryManager.GetAsync<PruebasDto>(_httpClientFactoryKey, 
                                                                           $"with-header",
                                                                           new Dictionary<string, string> { {"data", data } });
        return result;
    }


    public async Task<MlResult<IEnumerable<PruebasDto>>> MyGetAllwithCacheAsync()
    {
        var result = await GetAllAsync();

        return result;
    }


    public async Task<MlResult<IEnumerable<PruebasDto>>> MyGetAllwithoutCacheAsync()
    {
        var result = await GetAllAsync(new Dictionary<string, string> { { "X-Bypass-Cache", "no-cache" } });

        return result;
    }



    public async Task<MlResult<IEnumerable<PruebasDto>>> MyGetAllwithCache2Async()
    {
        var result = await _httpClientFactoryManager.GetAsync<IEnumerable<PruebasDto>>(_httpClientFactoryKey, "with-cache1");

        //var result = await _httpClientFactoryManager.GetAsync<IEnumerable<PruebasDto>>(_httpClientFactoryKey);

        return result;
    }

    public async Task<MlResult<IEnumerable<PruebasDto>>> MyGetAllwithoutCache2Async()
    {
        var result = await _httpClientFactoryManager.GetAsync<IEnumerable<PruebasDto>>(_httpClientFactoryKey, "with-cache1", new Dictionary<string, string> { { "X-Bypass-Cache", "no-cache" } });

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

