using MoralesLarios.OOFP.HttpClients.Tests.Integration.Clients;

namespace MoralesLarios.OOFP.HttpClients.Tests.Integration;

public class GenComplexClientTests(IPruebaComplexClient _sut)
{


    [Fact]
    public async Task GetAllAsync_integracion()
    {
        var result = await _sut.GetAllAsync();

    }


    [Fact]
    public async Task GetAll_and_update_integracion()
    {
        var result1 = await _sut.GetAllAsync()
                                    .MapAsync(data => data.ElementAt(2).With(d => d.Comentarios = "Cambiados por mi ahora mismo !!!"))
                                    .BindAsync(item => _sut.PutAsync(item));

    }


    [Fact]
    public async Task Add_Tests()
    {
        PruebaComplexDto pruebaComplexDto = new PruebaComplexDto
        {
            Nombre = "YYYY",
            Lugar = "Lugar de YYYY",
            Precio = 899,
            Fecha = new DateTime(2020, 1, 1)
        };

        var result = await _sut.PostAsync(pruebaComplexDto);
    }


    [Fact]
    public async Task Find_and_get_by_id_Tests()
    {
        PruebaComplexDto pruebaComplexDto = new PruebaComplexDto
        {
            Nombre = "YYYY",
            Lugar = "Lugar de YYYY",
            Precio = 899,
            Fecha = new DateTime(2020, 1, 1)
        };

        var result = await _sut.GetByIdAsync(null!, default!, pruebaComplexDto.Nombre, pruebaComplexDto.Lugar, pruebaComplexDto.Precio, pruebaComplexDto.Fecha);
    }


    [Fact]
    public async Task Update_Tests()
    {
        PruebaComplexDto pruebaComplexDto = new PruebaComplexDto
        {
            Nombre = "Prueba-",
            Lugar = "Lugar de prueba",
            Precio = 100,
            Fecha = new DateTime(2020, 1, 1),
            Comentarios = "---------------------"
        };

        var result = await _sut.PutByIdAsync(pruebaComplexDto, null!, default!, pruebaComplexDto.Nombre, pruebaComplexDto.Lugar, pruebaComplexDto.Precio, pruebaComplexDto.Fecha);
    }

    [Fact]
    public async Task Delete_Tests()
    {
        PruebaComplexDto pruebaComplexDto = new PruebaComplexDto
        {
            Nombre = "Prueba",
            Lugar = "Lugar de prueba",
            Precio = 100,
            Fecha = new DateTime(2020, 1, 1)
        };
        var result = await _sut.DeleteByIdAsync(null!, default!, pruebaComplexDto.Nombre, pruebaComplexDto.Lugar, pruebaComplexDto.Precio, pruebaComplexDto.Fecha);
    }

}
