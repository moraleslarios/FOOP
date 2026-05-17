using MoralesLarios.OOFP.HttpClients.Tests.Integration.Clients;

namespace MoralesLarios.OOFP.HttpClients.Tests.Integration;

public class GenClientFpPruebaComplexTests(ISimplePruebaComplexClient _sut)
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
                                    .MapAsync(data => data.First().With(d => d.Comentarios = "Cambiados por mi"))
                                    .BindAsync(item => _sut.PutAsync(item));

    }




    [Fact]
    public async Task Add_Tests()
    {
        PruebaComplexDto pruebaComplexDto = new PruebaComplexDto
        {
            Nombre = "Prueba",
            Lugar = "Lugar de prueba",
            Precio = 100,
            Fecha = new DateTime(2020, 1, 1),
        };

        var result = await _sut.PostAsync(pruebaComplexDto);
    }




    [Fact]
    public async Task Find_and_get_by_id_Tests()
    {
        PruebaComplexDto pruebaComplexDto = new PruebaComplexDto
        {
            Nombre = "Prueba",
            Lugar = "Lugar de prueba",
            Precio = 100,
            Fecha = new DateTime(2020, 1, 1),
        };

        var ids = $"{pruebaComplexDto.Nombre},{pruebaComplexDto.Lugar},{pruebaComplexDto.Precio},{pruebaComplexDto.Fecha.ToString(("yyyy-MM-ddTHH:mm:ss.fff"))}";

        var result = await _sut.GetByIdAsync(ids);
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
            Comentarios = "Comentarios de prueba ñññññññññ"
        };
        var ids = $"{pruebaComplexDto.Nombre},{pruebaComplexDto.Lugar},{pruebaComplexDto.Precio},{pruebaComplexDto.Fecha.ToString(("yyyy-MM-ddTHH:mm:ss.fff"))}";

        var result = await _sut.PutByIdAsync(ids, pruebaComplexDto);
    }


}
