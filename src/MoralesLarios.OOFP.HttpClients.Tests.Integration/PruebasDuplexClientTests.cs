using MoralesLarios.OOFP.HttpClients.Tests.Integration.Clients;

namespace MoralesLarios.OOFP.HttpClients.Tests.Integration;

public class PruebasDuplexClientTests(IPruebaDuplexClient _sut)
{



    [Fact]
    public async Task AllAsync_Integration()
    {
        var data = await _sut.GetAllAsync(ct: CancellationToken.None);


    }


    [Fact]
    public async Task GetByIdAsync_Integration()
    {
        var data = await _sut.GetByIdAsync("2", ct: CancellationToken.None);
    }


    [Fact]
    public async Task PostAsync_Integration()
    {
        var data = await _sut.PostAsync(new PruebaRequestDto
        {
            Id = 0,
            Nombre = "Prueba 1",
            Comentarios = "Comentarios de la prueba 1",
            Fecha = DateTime.UtcNow
        }, ct: CancellationToken.None);

    }



    [Fact]
    public async Task PutAsync_Integration()
    {
        var data = await _sut.PutAsync(new PruebaRequestDto
        {
            Id = 18,
            Nombre = "Prueba 1 actualizada",
            Comentarios = "Comentarios de la prueba 1 actualizada",
            Fecha = DateTime.UtcNow
        }, ct: CancellationToken.None);

    }


    [Fact]
    public async Task PutByIdAsync_Integration()
    {
        var data = await _sut.PutByIdAsync("18", new PruebaRequestDto
        {
            Id = 18,
            Nombre = "Prueba 1 actualizada por id",
            Comentarios = "Comentarios de la prueba 1 actualizada por id",
            Fecha = DateTime.UtcNow
        }, ct: CancellationToken.None);
    }

    [Fact]
    public async Task DeleteAsync_Integration()
    {
        var data = await _sut.DeleteAsync(new PruebaRequestDto
        {
            Id = 18,
            Nombre = "Prueba 1 actualizada por id",
            Comentarios = "Comentarios de la prueba 1 actualizada por id",
            Fecha = DateTime.UtcNow
        }, ct: CancellationToken.None);
    }

    [Fact]
    public async Task DeleteByIdAsync_Integration()
    {
        var data = await _sut.DeleteByIdAsync("17", ct: CancellationToken.None);
    }

}
