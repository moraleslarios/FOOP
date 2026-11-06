// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.EntityFrameworkCore.Enums;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration.Repos;

/// <summary>
/// Pruebas de integración de los repositorios de Entity Framework Core sobre <c>TestDbContext</c>,
/// usando exclusivamente la API pública de las 8 interfaces funcionales (sin <c>RegisterServices.GetRepository</c>).
/// </summary>
public class RepoVinorFpTests(IVinosRepo                    _sut,
                               IEfReadRepository<Vino>       _vinosReadRepo,
                               IEfCreateRepository<Vino>     _vinosCreateRepo,
                               IEfUpdateRepository<Vino>     _vinosUpdateRepo,
                               IEfDeleteRepository<Vino>     _vinosDeleteRepo,
                               IEfWriteRepository<Vino>      _vinosWriteRepo,
                               IEfPagedRepository<Vino>      _vinosPagedRepo,
                               IEfPagedReadRepository<Vino>  _vinosPagedReadRepo,
                               ICatasRepo                    _catasRepo,
                               IVinosCatasRepo                _vinosCatasRepo,
                               IVinosCatasPuntuacionesRepo    _vinosCatasPuntuacionesRepo,
                               TestDbContext                  _dbContext)
{
    private static Vino CreateUniqueVino(string tag, int año = 2023)
        => new Vino
        {
            Nombre        = $"Vino {tag} {año}",
            Bodega        = "Bodega de Prueba",
            Año           = año,
            Patrocinador  = tag,
            DO            = "DO de Prueba",
            FechaEntrada  = DateTime.Now,
            Comentarios   = "Comentarios de Prueba"
        };

    private static Cata CreateUniqueCata(string tag)
        => new Cata
        {
            Fecha        = DateTime.Now,
            Lugar        = "Lugar de Prueba",
            Nombre       = $"Cata {tag}",
            Comentarios  = "Comentarios de Prueba"
        };

    private static bool IsNameIncludedByCustomMethod(string name)
        => name.Contains("incluido", StringComparison.OrdinalIgnoreCase);

    private async Task CommitChangesAsync()
    {
        var result = await _sut.UnitOfWork.TryCommitAsync();
        result.IsFailure.Should().BeFalse();
    }


    [Fact]
    public void RepositorioScoped_UsaElDbContextDelAmbito_SinSerSuPropietario()
    {
        typeof(IDisposable).IsAssignableFrom(_vinosReadRepo.GetType()).Should().BeFalse();
        ((IGetContextable)_vinosReadRepo).GetContext().Should().BeSameAs(_dbContext);
    }


    [Fact]
    public async Task TryAllAsync_Integration_DevuelveListaDeVinos()
    {
        var tag = Guid.NewGuid().ToString("N");
        var expected = CreateUniqueVino(tag);
        var addResult = await _vinosCreateRepo.TryAddAsync(expected);

        addResult.IsFailure.Should().BeFalse();
        await CommitChangesAsync();

        var result = await _sut.TryAllAsync();

        result.IsFailure.Should().BeFalse();

        result.GetValueOrThrow().Should().Contain(vino => vino.Id == expected.Id);
        result.GetValueOrThrow().Should().NotBeAssignableTo<IQueryable<Vino>>();

        var synchronousResult = _sut.TryAll();
        synchronousResult.IsFailure.Should().BeFalse();
        synchronousResult.GetValueOrThrow().Should().NotBeAssignableTo<IQueryable<Vino>>();

        await _vinosDeleteRepo.TryRemoveAsync(expected);
        await CommitChangesAsync();
    }


    [Fact]
    public void TryFind_ArregloDeClavesNulo_DevuelveFalloSinLanzar()
    {
        object[] keyValues = null!;
        MlResult<Vino>? result = null;

        var exception = Record.Exception(() => result = _vinosReadRepo.TryFind(keyValues));

        exception.Should().BeNull();
        result!.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task TryFindAsync_ArregloDeClavesNulo_DevuelveFalloSinLanzar()
    {
        object[] keyValues = null!;
        MlResult<Vino>? result = null;

        var exception = await Record.ExceptionAsync(async () =>
            result = await _vinosReadRepo.TryFindAsync(default, keyValues));

        exception.Should().BeNull();
        result!.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task TryUpdateYTryRemove_ClavesNulas_DevuelvenFalloSinLanzar()
    {
        object[] keyValues = null!;
        var item = CreateUniqueVino(Guid.NewGuid().ToString("N"));
        var results = new List<MlResult<Vino>>();
        var exceptions = new List<Exception?>
        {
            Record.Exception(() => results.Add(_vinosUpdateRepo.TryUpdate(item, keyValues))),
            await Record.ExceptionAsync(async () =>
                results.Add(await _vinosUpdateRepo.TryUpdateAsync(item, CancellationToken.None, keyValues))),
            Record.Exception(() => results.Add(_vinosDeleteRepo.TryRemove(keyValues))),
            await Record.ExceptionAsync(async () =>
                results.Add(await _vinosDeleteRepo.TryRemoveAsync(CancellationToken.None, keyValues)))
        };

        exceptions.Should().OnlyContain(exception => exception == null);
        results.Should().HaveCount(4);
        results.Should().OnlyContain(result => result.IsFailure);
    }

    [Fact]
    public async Task Repositorios_ClavesVacias_DevuelvenFallo()
    {
        var emptyKeys = Array.Empty<object>();
        var item = CreateUniqueVino(Guid.NewGuid().ToString("N"));

        var find = _vinosReadRepo.TryFind(emptyKeys);
        var findAsync = await _vinosReadRepo.TryFindAsync(default, emptyKeys);
        var update = _vinosUpdateRepo.TryUpdate(item, emptyKeys);
        var updateAsync = await _vinosUpdateRepo.TryUpdateAsync(item, CancellationToken.None, emptyKeys);
        var remove = _vinosDeleteRepo.TryRemove(emptyKeys);
        var removeAsync = await _vinosDeleteRepo.TryRemoveAsync(CancellationToken.None, emptyKeys);

        new[] { find, findAsync, update, updateAsync, remove, removeAsync }
            .Should().OnlyContain(result => result.IsFailure);
    }

    [Fact]
    public void TryGetData_FalloDeTraduccionSincrona_SeCapturaEnMlResult()
    {
        var result = _vinosReadRepo.TryGetData(vino => IsNameIncludedByCustomMethod(vino.Nombre));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void TryGetData_SinCoincidencias_DevuelveSecuenciaVacia()
    {
        var tag = Guid.NewGuid().ToString("N");

        var result = _vinosReadRepo.TryGetData(vino => vino.Patrocinador == tag);

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().BeEmpty();
    }


    [Fact]
    public async Task TryAddAsync_Vino_Valido_DevuelveExito()
    {
        var tag  = Guid.NewGuid().ToString("N");
        var vino = CreateUniqueVino(tag);

        var result = await _vinosCreateRepo.TryAddAsync(vino);

        result.IsFailure.Should().BeFalse();
        await CommitChangesAsync();

        var added = result.GetValueOrThrow();

        added.Id.Should().BeGreaterThan(0);
        added.Nombre.Should().Be(vino.Nombre);

        await _vinosDeleteRepo.TryRemoveAsync(added);
        await CommitChangesAsync();
    }


    [Fact]
    public async Task TryAddAsync_Vino_Nulo_DevuelveFallo()
    {
        Vino vinoNulo = null!;

        var result = await _vinosCreateRepo.TryAddAsync(vinoNulo);

        result.IsFailure.Should().BeTrue();

        var errorDetails = result.GetFailureDetailsOrThrow();

        errorDetails.Should().NotBeNull();
    }


    [Fact]
    public async Task TryUpdateAsync_Vino_Valido_DevuelveExito()
    {
        var tag  = Guid.NewGuid().ToString("N");
        var vino = CreateUniqueVino(tag);

        var added = (await _vinosCreateRepo.TryAddAsync(vino)).GetValueOrThrow();
        await CommitChangesAsync();

        // Se reutiliza la MISMA instancia devuelta por TryAdd (ya trackeada por el DbContext) para
        // evitar el riesgo de InvalidOperationException de tracking al actualizar con clave primaria.
        added.Nombre = $"Vino {tag} actualizado";

        var updateResult = await _vinosWriteRepo.TryUpdateAsync(added, default, added.Id);

        updateResult.IsFailure.Should().BeFalse();

        var updated = updateResult.GetValueOrThrow();

        updated.Nombre.Should().Be(added.Nombre);
        await CommitChangesAsync();

        var reloaded = (await _vinosReadRepo.TryFindAsync(default, added.Id)).GetValueOrThrow();

        reloaded.Nombre.Should().Be(added.Nombre);

        await _vinosDeleteRepo.TryRemoveAsync(added);
    await CommitChangesAsync();
    }


    [Fact]
    public async Task TryUpdateAsync_Vino_NoExistente_DevuelveFallo()
    {
        var tag           = Guid.NewGuid().ToString("N");
        var vinoInexistente = CreateUniqueVino(tag);
        MlErrorDetails notFoundDetails = $"No existe el vino {tag} para actualizar";

        var result = await _vinosUpdateRepo.TryUpdateAsync(vinoInexistente, notFoundDetails, default, -999_999);

        result.IsFailure.Should().BeTrue();

        var errorDetails = result.GetFailureDetailsOrThrow();

        errorDetails.Should().NotBeNull();
    }


    [Fact]
    public async Task TryRemoveAsync_Vino_Valido_DevuelveExito()
    {
        var tag  = Guid.NewGuid().ToString("N");
        var vino = CreateUniqueVino(tag);

        var added = (await _vinosCreateRepo.TryAddAsync(vino)).GetValueOrThrow();
        await CommitChangesAsync();

        var removeResult = await _vinosDeleteRepo.TryRemoveAsync(default, added.Id);

        removeResult.IsFailure.Should().BeFalse();
        await CommitChangesAsync();

        var removed = removeResult.GetValueOrThrow();

        removed.Id.Should().Be(added.Id);

        var findAfterRemove = await _vinosReadRepo.TryFindAsync(default, added.Id);

        findAfterRemove.IsFailure.Should().BeTrue();
    }


    [Fact]
    public async Task TryRemoveAsync_Vino_NoExistente_DevuelveFallo()
    {
        MlErrorDetails notFoundDetails = "No existe el vino para eliminar";

        var result = await _vinosDeleteRepo.TryRemoveAsync(notFoundDetails, default, -999_999);

        result.IsFailure.Should().BeTrue();

        var errorDetails = result.GetFailureDetailsOrThrow();

        errorDetails.Should().NotBeNull();
    }


    [Fact]
    public async Task TryFind_ClaveCompuesta_VinosCata_Existente_DevuelveExito()
    {
        var tag = Guid.NewGuid().ToString("N");

        var vino = (await _vinosCreateRepo.TryAddAsync(CreateUniqueVino(tag))).GetValueOrThrow();
        var cata = (await _catasRepo.TryAddAsync(CreateUniqueCata(tag))).GetValueOrThrow();
        await CommitChangesAsync();

        var relacion = new VinosCata { IdVino = vino.Id, IdCata = cata.Id };

        await _vinosCatasRepo.TryAddAsync(relacion);
        await CommitChangesAsync();

        var result = await _vinosCatasRepo.TryFindAsync(default, vino.Id, cata.Id);

        result.IsFailure.Should().BeFalse();

        var encontrada = result.GetValueOrThrow();

        encontrada.IdVino.Should().Be(vino.Id);
        encontrada.IdCata.Should().Be(cata.Id);

        await _vinosCatasRepo.TryRemoveAsync(default, vino.Id, cata.Id);
        await _vinosDeleteRepo.TryRemoveAsync(vino);
        await _catasRepo.TryRemoveAsync(cata);
        await CommitChangesAsync();
    }


    [Fact]
    public async Task TryFind_ClaveCompuesta_VinosCata_NoExistente_DevuelveFallo()
    {
        MlErrorDetails notFoundDetails = "No existe la relación vino-cata solicitada";

        var result = await _vinosCatasRepo.TryFindAsync(notFoundDetails, default, -999_999, -999_999);

        result.IsFailure.Should().BeTrue();

        var errorDetails = result.GetFailureDetailsOrThrow();

        errorDetails.Should().NotBeNull();
    }

    [Fact]
    public void TryFind_ClaveCompuestaNoEncontrada_MensajeConservaElOrdenDeValores()
    {
        var result = _vinosCatasRepo.TryFind(-999_998, -999_997);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().ToString().Should().Contain("(-999998, -999997)");
    }


    [Fact]
    public async Task TryFind_ClaveCompuesta_VinosCatasPuntuacion_Existente_DevuelveExito()
    {
        var tag = Guid.NewGuid().ToString("N");

        var vino = (await _vinosCreateRepo.TryAddAsync(CreateUniqueVino(tag))).GetValueOrThrow();
        var cata = (await _catasRepo.TryAddAsync(CreateUniqueCata(tag))).GetValueOrThrow();
        await CommitChangesAsync();

        // La base de datos física conserva una FK heredada (IdUsuario -> AspNetUsers.Id) de una
        // configuración histórica con ASP.NET Core Identity que ya no existe en el modelo actual
        // de TestDbContext. Para satisfacerla sin tocar el modelo ni la infraestructura compartida,
        // se inserta una fila mínima válida en AspNetUsers y se elimina al finalizar el test.
        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO AspNetUsers (Id, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount) VALUES ({tag}, 0, 0, 0, 0, 0)");

        var puntuacion = new VinosCatasPuntuacion
        {
            IdVino     = vino.Id,
            IdCata     = cata.Id,
            IdUsuario  = tag,
            Puntuacion = 8.50m,
            Fecha      = DateTime.Now
        };

        var addResult = await _vinosCatasPuntuacionesRepo.TryAddAsync(puntuacion);

        addResult.IsFailure.Should().BeFalse();
        await CommitChangesAsync();

        var result = await _vinosCatasPuntuacionesRepo.TryFindAsync(default, vino.Id, cata.Id, tag);

        result.IsFailure.Should().BeFalse();

        var encontrada = result.GetValueOrThrow();

        encontrada.IdVino.Should().Be(vino.Id);
        encontrada.IdCata.Should().Be(cata.Id);
        encontrada.IdUsuario.Should().Be(tag);

        await _vinosCatasPuntuacionesRepo.TryRemoveAsync(default, vino.Id, cata.Id, tag);
        await _vinosDeleteRepo.TryRemoveAsync(vino);
        await _catasRepo.TryRemoveAsync(cata);
        await CommitChangesAsync();
        await _dbContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM AspNetUsers WHERE Id = {tag}");
    }


    [Fact]
    public async Task TryGetLast_ConFiltro_DevuelveElementoEsperado()
    {
        var tag  = Guid.NewGuid().ToString("N");
        var vino = (await _vinosCreateRepo.TryAddAsync(CreateUniqueVino(tag))).GetValueOrThrow();
        await CommitChangesAsync();

        var result = _vinosReadRepo.TryGetLast(v => v.Patrocinador == tag);

        result.IsFailure.Should().BeFalse();

        var encontrado = result.GetValueOrThrow();

        encontrado.Id.Should().Be(vino.Id);

        await _vinosDeleteRepo.TryRemoveAsync(vino);
    await CommitChangesAsync();
    }


    [Fact]
    public async Task TryGetLast_ConFiltro_SinCoincidencias_DevuelveFallo()
    {
        var tag = Guid.NewGuid().ToString("N");
        MlErrorDetails notFoundDetails = "No se encontró ningún vino con ese patrocinador";

        var result = _vinosReadRepo.TryGetLast(v => v.Patrocinador == tag, notFoundDetails);

        result.IsFailure.Should().BeTrue();

        var errorDetails = result.GetFailureDetailsOrThrow();

        errorDetails.Should().NotBeNull();
    }


    [Fact]
    public async Task TryGetDataPaginationAsync_OrdenaYRespetaTotalCount()
    {
        var tag = Guid.NewGuid().ToString("N");

        var v1 = (await _vinosCreateRepo.TryAddAsync(CreateUniqueVino(tag, año: 2001))).GetValueOrThrow();
        var v2 = (await _vinosCreateRepo.TryAddAsync(CreateUniqueVino(tag, año: 2003))).GetValueOrThrow();
        var v3 = (await _vinosCreateRepo.TryAddAsync(CreateUniqueVino(tag, año: 2002))).GetValueOrThrow();
        await CommitChangesAsync();

        PageRequest primeraPagina = new PageRequest(1, 2);

        var primerResultado = await _vinosPagedRepo.TryGetDataPaginationAsync(primeraPagina,
                                                                                SortDirection.Ascending,
                                                                                orderByField: v => v.Año,
                                                                                filter: v => v.Patrocinador == tag);

        primerResultado.IsFailure.Should().BeFalse();

        var primeraPagina_ = primerResultado.GetValueOrThrow();

        primeraPagina_.TotalCount.Should().Be(3);
        primeraPagina_.Items.Should().HaveCount(2);
        primeraPagina_.Items.Select(v => v.Año).Should().BeInAscendingOrder();
        primeraPagina_.Items.First().Año.Should().Be(2001);

        PageRequest segundaPagina = new PageRequest(2, 2);

        var segundoResultado = await _vinosPagedReadRepo.TryGetDataPaginationAsync(segundaPagina,
                                                                                     SortDirection.Ascending,
                                                                                     orderByField: v => v.Año,
                                                                                     filter: v => v.Patrocinador == tag);

        segundoResultado.IsFailure.Should().BeFalse();

        var segundaPagina_ = segundoResultado.GetValueOrThrow();

        segundaPagina_.TotalCount.Should().Be(3);
        segundaPagina_.Items.Should().HaveCount(1);
        segundaPagina_.Items.First().Año.Should().Be(2003);

        await _vinosDeleteRepo.TryRemoveAsync(v1);
        await _vinosDeleteRepo.TryRemoveAsync(v2);
        await _vinosDeleteRepo.TryRemoveAsync(v3);
        await CommitChangesAsync();
    }

    [Fact]
    public async Task TryGetDataPaginationAsync_SinCoincidencias_DevuelveTotalCeroYPaginaVacia()
    {
        var unmatchedTag = Guid.NewGuid().ToString("N");
        var request = new PageRequest(1, 10);

        var result = await _vinosPagedRepo.TryGetDataPaginationAsync(
            request,
            orderByField: vino => vino.Id,
            filter: vino => vino.Patrocinador == unmatchedTag);

        result.IsFailure.Should().BeFalse();
        var page = result.GetValueOrThrow();
        page.TotalCount.Should().Be(0);
        page.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task TryGetDataPaginationAsync_TokenCancelado_DevuelveFallo()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        var result = await _vinosPagedRepo.TryGetDataPaginationAsync(
            new PageRequest(1, 10),
            orderByField: vino => vino.Id,
            ct: cancellation.Token);

        result.IsFailure.Should().BeTrue();
    }
}


