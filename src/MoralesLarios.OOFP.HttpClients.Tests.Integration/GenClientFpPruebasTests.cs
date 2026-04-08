// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.HttpClients.Tests.Integration.Clients;

namespace MoralesLarios.OOFP.HttpClients.Tests.Integration;

public class GenClientFpPruebasTests(IPruebasClient _sut)
{
    [Fact]
    public async Task GetAllAsync_integracion()
    {
        var result = await _sut.GetAllAsync();
    }


    [Fact]
    public async Task MyGetAsync_integracion()
    {
        var result = await _sut.MyGetAsync("Test-Header - jajajaajajaja");
    }



    [Fact]
    public async Task GetByIdAsync_integracion()
    {
        var result = await _sut.GetByIdAsync("1");
    }


    [Fact]
    public async Task PostAsync_integracion()
    {
        var item = new PruebasDto
        {
            Id = 0,
            Nombre = "Ábalos",
            Comentarios = "El de Jésica"
        };
        var result = await _sut.PostAsync(item);
    }

    [Fact]
    public async Task PutAsync_integracion()
    {
        var item = new PruebasDto
        {
            Id = 14,
            Nombre = "Ábalos 2",
            Comentarios = "El de Jésica - Modificado"
        };

        var result = await _sut.PutAsync(item);
    }

    [Fact]
    public async Task DeleteByIdAsync_integracion()
    {
        var result = await _sut.DeleteByIdAsync("14");
    }

    [Fact]
    public async Task DeleteAsync_integracion()
    {
        var item = new PruebasDto
        {
            Id = 11,
            Nombre = "Ábalos 2",
            Comentarios = "El de Jésica - Modificado"
        };
        var result = await _sut.DeleteAsync(item);
    }
}

