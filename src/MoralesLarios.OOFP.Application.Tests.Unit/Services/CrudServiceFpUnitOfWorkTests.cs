using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.Application.Tests.Unit.Services;

public class CrudServiceFpUnitOfWorkTests
{
    [Fact]
    public void CrudServiceFp_ConservaLosConstructoresPublicosDeAmbasAridades()
    {
        typeof(CrudServiceFp<MyTable, MyTableDto>)
            .GetConstructor(new[]
            {
                typeof(IEfRepository<MyTable>),
                typeof(ILogger<CrudServiceFp<MyTable, MyTableDto>>)
            })
            .Should().NotBeNull();
        typeof(CrudServiceFp<MyTable, MyTableDto, MyTableDto>)
            .GetConstructor(new[]
            {
                typeof(IEfRepository<MyTable>),
                typeof(ILogger<CrudServiceFp<MyTable, MyTableDto, MyTableDto>>)
            })
            .Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_CuandoElRepositorioTieneExito_ConfirmaUnaVezYPropagaToken()
    {
        var token = new CancellationTokenSource().Token;
        var dto = new MyTableDto(0, "nuevo", "detalle");
        var stored = new MyTable { Id = 37, Name = dto.Name, Description = dto.Description };
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryAddAsync(It.IsAny<MyTable>(), token))
                  .Returns((MyTable entity, CancellationToken _) =>
                      Task.FromResult(stored.ToMlResultValid()));
        unitOfWork.Setup(x => x.TryCommitAsync(token)).ReturnsAsync(1.ToMlResultValid());
        var service = CreateSingleService(repository.Object);

        var result = await service.CreateAsync(dto, token);

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be(new MyTableDto(37, "nuevo", "detalle"));
        repository.Verify(x => x.TryAddAsync(It.Is<MyTable>(entity => entity.Name == dto.Name &&
                                                                        entity.Description == dto.Description), token),
                          Times.Once);
        unitOfWork.Verify(x => x.TryCommitAsync(token), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CuandoElRepositorioFalla_NoConfirma()
    {
        var token = CancellationToken.None;
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryAddAsync(It.IsAny<MyTable>(), token))
                  .ReturnsAsync("fallo del repositorio".ToMlResultFail<MyTable>());
        var service = CreateSingleService(repository.Object);

        var result = await service.CreateAsync(new MyTableDto(0, "nuevo", "detalle"), token);

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.TryCommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_CuandoElCommitFalla_PropagaElFallo()
    {
        var token = CancellationToken.None;
        var commitErrors = MlErrorDetails.FromErrorMessage("fallo al confirmar");
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryAddAsync(It.IsAny<MyTable>(), token))
                  .Returns((MyTable entity, CancellationToken _) =>
                      Task.FromResult(entity.ToMlResultValid()));
        unitOfWork.Setup(x => x.TryCommitAsync(token))
                  .ReturnsAsync(commitErrors.ToMlResultFail<int>());
        var service = CreateSingleService(repository.Object);

        var result = await service.CreateAsync(new MyTableDto(0, "nuevo", "detalle"), token);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Should().BeSameAs(commitErrors);
        unitOfWork.Verify(x => x.TryCommitAsync(token), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CuandoElRepositorioTieneExito_ConfirmaUnaVez()
    {
        var token = CancellationToken.None;
        var dto = new MyTableDto(37, "actualizado", "detalle");
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryUpdateAsync(It.IsAny<MyTable>(), token))
                  .Returns((MyTable entity, CancellationToken _) =>
                      Task.FromResult(entity.ToMlResultValid()));
        unitOfWork.Setup(x => x.TryCommitAsync(token)).ReturnsAsync(1.ToMlResultValid());
        var service = CreateSingleService(repository.Object);

        var result = await service.UpdateAsync(dto, token);

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be(dto);
        repository.Verify(x => x.TryUpdateAsync(It.Is<MyTable>(entity => entity.Id == dto.Id &&
                                                                           entity.Name == dto.Name), token),
                          Times.Once);
        unitOfWork.Verify(x => x.TryCommitAsync(token), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CuandoElRepositorioFalla_NoConfirma()
    {
        var token = CancellationToken.None;
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryUpdateAsync(It.IsAny<MyTable>(), token))
                  .ReturnsAsync("fallo de actualización".ToMlResultFail<MyTable>());
        var service = CreateSingleService(repository.Object);

        var result = await service.UpdateAsync(new MyTableDto(37, "nombre", "detalle"), token);

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.TryCommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_PorClave_CuandoElRepositorioTieneExito_ConfirmaUnaVez()
    {
        var token = CancellationToken.None;
        var dto = new MyTableDto(37, "actualizado", "detalle");
        MlErrorDetails notFound = "no existe";
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryFindAsync(notFound, token, It.IsAny<object[]>()))
                  .ReturnsAsync(new MyTable { Id = dto.Id, Name = "anterior" }.ToMlResultValid());
        repository.Setup(x => x.TryUpdateAsync(It.IsAny<MyTable>(), notFound, token, It.IsAny<object[]>()))
                  .Returns((MyTable entity, MlErrorDetails _, CancellationToken _, object[] _) =>
                      Task.FromResult(entity.ToMlResultValid()));
        unitOfWork.Setup(x => x.TryCommitAsync(token)).ReturnsAsync(1.ToMlResultValid());
        var service = CreateSingleService(repository.Object);

        var result = await service.UpdateAsync(dto, notFound, token, pk: new object[] { dto.Id });

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be(dto);
        unitOfWork.Verify(x => x.TryCommitAsync(token), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_PorClave_CuandoLaBusquedaFalla_NoConfirma()
    {
        var token = CancellationToken.None;
        MlErrorDetails notFound = "no existe";
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryFindAsync(notFound, token, It.IsAny<object[]>()))
                  .ReturnsAsync("fallo de búsqueda".ToMlResultFail<MyTable>());
        var service = CreateSingleService(repository.Object);

        var result = await service.UpdateAsync(new MyTableDto(37, "nombre", "detalle"),
                                               notFound,
                                               token,
                                               pk: new object[] { 37 });

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.TryCommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_PorEntidad_CuandoElRepositorioTieneExito_ConfirmaUnaVez()
    {
        var token = CancellationToken.None;
        var dto = new MyTableDto(37, "eliminar", "detalle");
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryRemoveAsync(It.IsAny<MyTable>(), token))
                  .Returns((MyTable entity, CancellationToken _) =>
                      Task.FromResult(entity.ToMlResultValid()));
        unitOfWork.Setup(x => x.TryCommitAsync(token)).ReturnsAsync(1.ToMlResultValid());
        var service = CreateSingleService(repository.Object);

        var result = await service.DeleteAsync(dto, token);

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be(dto);
        unitOfWork.Verify(x => x.TryCommitAsync(token), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_PorEntidad_CuandoElRepositorioFalla_NoConfirma()
    {
        var token = CancellationToken.None;
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryRemoveAsync(It.IsAny<MyTable>(), token))
                  .ReturnsAsync("fallo de eliminación".ToMlResultFail<MyTable>());
        var service = CreateSingleService(repository.Object);

        var result = await service.DeleteAsync(new MyTableDto(37, "nombre", "detalle"), token);

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.TryCommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProblemDetailsAsync_PorClave_CuandoElRepositorioTieneExito_ConfirmaUnaVez()
    {
        var token = CancellationToken.None;
        var dto = new MyTableDto(37, "eliminar", "detalle");
        MlErrorDetails notFound = "no existe";
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryFindAsync(notFound, token, It.IsAny<object[]>()))
                  .ReturnsAsync(new MyTable { Id = dto.Id, Name = dto.Name, Description = dto.Description }
                      .ToMlResultValid());
        repository.Setup(x => x.TryRemoveAsync(It.IsAny<MyTable>(), token))
                  .Returns((MyTable entity, CancellationToken _) =>
                      Task.FromResult(entity.ToMlResultValid()));
        unitOfWork.Setup(x => x.TryCommitAsync(token)).ReturnsAsync(1.ToMlResultValid());
        var service = CreateSingleService(repository.Object);

        var result = await service.DeleteProblemDetailsAsync(notFound, token, pk: new object[] { dto.Id });

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be(dto);
        unitOfWork.Verify(x => x.TryCommitAsync(token), Times.Once);
    }

    [Fact]
    public async Task DeleteProblemDetailsAsync_PorClave_CuandoLaBusquedaFalla_NoConfirma()
    {
        var token = CancellationToken.None;
        MlErrorDetails notFound = "no existe";
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryFindAsync(notFound, token, It.IsAny<object[]>()))
                  .ReturnsAsync("fallo de búsqueda".ToMlResultFail<MyTable>());
        var service = CreateSingleService(repository.Object);

        var result = await service.DeleteProblemDetailsAsync(notFound, token, pk: new object[] { 37 });

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.TryCommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_Duplex_CuandoElRepositorioTieneExito_ConfirmaUnaVez()
    {
        var token = CancellationToken.None;
        var dto = new MyTableDto(0, "nuevo", "detalle");
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryAddAsync(It.IsAny<MyTable>(), token))
                  .Returns((MyTable entity, CancellationToken _) =>
                  {
                      entity.Id = 41;
                      return Task.FromResult(entity.ToMlResultValid());
                  });
        unitOfWork.Setup(x => x.TryCommitAsync(token)).ReturnsAsync(1.ToMlResultValid());
        var service = CreateDuplexService(repository.Object);

        var result = await service.CreateAsync(dto, token);

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be(new MyTableDto(41, "nuevo", "detalle"));
        unitOfWork.Verify(x => x.TryCommitAsync(token), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Duplex_CuandoElRepositorioTieneExito_ConfirmaUnaVez()
    {
        var token = CancellationToken.None;
        var dto = new MyTableDto(41, "actualizado", "detalle");
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryUpdateAsync(It.IsAny<MyTable>(), token))
                  .Returns((MyTable entity, CancellationToken _) =>
                      Task.FromResult(entity.ToMlResultValid()));
        unitOfWork.Setup(x => x.TryCommitAsync(token)).ReturnsAsync(1.ToMlResultValid());
        var service = CreateDuplexService(repository.Object);

        var result = await service.UpdateAsync(dto, token);

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be(dto);
        unitOfWork.Verify(x => x.TryCommitAsync(token), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Duplex_CuandoElRepositorioFalla_NoConfirma()
    {
        var token = CancellationToken.None;
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryUpdateAsync(It.IsAny<MyTable>(), token))
                  .ReturnsAsync("fallo de actualización".ToMlResultFail<MyTable>());
        var service = CreateDuplexService(repository.Object);

        var result = await service.UpdateAsync(new MyTableDto(41, "nombre", "detalle"), token);

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.TryCommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_Duplex_CuandoElRepositorioTieneExito_ConfirmaUnaVez()
    {
        var token = CancellationToken.None;
        var dto = new MyTableDto(41, "eliminar", "detalle");
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryRemoveAsync(It.IsAny<MyTable>(), token))
                  .Returns((MyTable entity, CancellationToken _) =>
                      Task.FromResult(entity.ToMlResultValid()));
        unitOfWork.Setup(x => x.TryCommitAsync(token)).ReturnsAsync(1.ToMlResultValid());
        var service = CreateDuplexService(repository.Object);

        var result = await service.DeleteAsync(dto, token);

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be(dto);
        unitOfWork.Verify(x => x.TryCommitAsync(token), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Duplex_CuandoElRepositorioFalla_NoConfirma()
    {
        var token = CancellationToken.None;
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryRemoveAsync(It.IsAny<MyTable>(), token))
                  .ReturnsAsync("fallo de eliminación".ToMlResultFail<MyTable>());
        var service = CreateDuplexService(repository.Object);

        var result = await service.DeleteAsync(new MyTableDto(41, "nombre", "detalle"), token);

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.TryCommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_Duplex_CuandoElRepositorioFalla_NoConfirma()
    {
        var token = CancellationToken.None;
        var (repository, unitOfWork) = CreateRepository();
        repository.Setup(x => x.TryAddAsync(It.IsAny<MyTable>(), token))
                  .ReturnsAsync("fallo del repositorio".ToMlResultFail<MyTable>());
        var service = CreateDuplexService(repository.Object);

        var result = await service.CreateAsync(new MyTableDto(0, "nuevo", "detalle"), token);

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.TryCommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static (Mock<IEfRepository<MyTable>> Repository, Mock<IMlUnitOfWork> UnitOfWork)
        CreateRepository()
    {
        var repository = new Mock<IEfRepository<MyTable>>(MockBehavior.Strict);
        var unitOfWork = new Mock<IMlUnitOfWork>(MockBehavior.Strict);
        repository.SetupGet(x => x.UnitOfWork).Returns(unitOfWork.Object);
        return (repository, unitOfWork);
    }

    private static CrudServiceFp<MyTable, MyTableDto> CreateSingleService(
        IEfRepository<MyTable> repository)
        => new(repository, NullLogger<CrudServiceFp<MyTable, MyTableDto>>.Instance);

    private static CrudServiceFp<MyTable, MyTableDto, MyTableDto> CreateDuplexService(
        IEfRepository<MyTable> repository)
        => new(repository, NullLogger<CrudServiceFp<MyTable, MyTableDto, MyTableDto>>.Instance);
}