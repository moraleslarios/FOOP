using System.Linq.Expressions;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using MoralesLarios.OOFP.EntityFrameworkCore;
using MoralesLarios.OOFP.EntityFrameworkCore.Enums;
using MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
using MoralesLarios.OOFP.EntityFrameworkCore.TestSupport.Configurations;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration;

public class UnitOfWorkTransactionTests
{
    [Fact]
    public void ContratosGenericosDeEntidad_ExponenNombreTEntityEnMetadatos()
    {
        var entityTypes = new[]
        {
            typeof(IEfCreateRepository<>),
            typeof(IEfDeleteRepository<>),
            typeof(IEfReadRepository<>),
            typeof(IEfOrderedReadRepository<>),
            typeof(IEfUpdateRepository<>),
            typeof(IEfWriteRepository<>),
            typeof(IEfPagedReadRepository<>),
            typeof(IEfPagedRepository<>),
            typeof(IEfRepository<>),
            typeof(IEfPaginator<>),
            typeof(EfCreateRepository<,>),
            typeof(EfDeleteRepository<,>),
            typeof(EfReadRepository<,>),
            typeof(EfUpdateRepository<,>),
            typeof(EfWriteRepository<,>),
            typeof(EfCrudRepository<,>),
            typeof(EfPagedReadRepository<,>),
            typeof(EfPagedRepository<,>)
        };

        entityTypes.Select(type => type.GetGenericArguments()[0].Name)
            .Should().OnlyContain(name => name == "TEntity");
        typeof(RegisterServices).GetMethod(nameof(RegisterServices.AddOofpRepositories))!
            .GetGenericArguments().Select(parameter => parameter.Name)
            .Should().Equal("TEntity", "TContext");
        typeof(MoralesLarios.OOFP.EntityFrameworkCore.Helpers.Extensions)
            .GetMethod(nameof(MoralesLarios.OOFP.EntityFrameworkCore.Helpers.Extensions.PrivateOrderBy))!
            .GetGenericArguments()[0].Name.Should().Be("TEntity");
    }

    [Fact]
    public async Task Repository_ExponeLaUnidadDeTrabajoScopedYEspecificaPorContexto()
    {
        await using var database = await TestDatabase.CreateAsync();
        var specific = database.Scope.ServiceProvider
                                .GetRequiredService<IMlUnitOfWork<CountingTestDbContext>>();

        typeof(IMlUnitOfWork).IsAssignableFrom(typeof(IMlUnitOfWork<CountingTestDbContext>))
                             .Should().BeTrue();
        database.Repository.UnitOfWork.Should().BeSameAs(specific);
        database.Scope.ServiceProvider.GetRequiredService<IMlUnitOfWork>()
                .Should().BeSameAs(specific);
    }

    [Fact]
    public async Task RepositoriosDelMismoScope_CompartenLaMismaReferenciaDeDbContext()
    {
        await using var database = await TestDatabase.CreateAsync();
        var createRepository = database.Scope.ServiceProvider.GetRequiredService<IEfCreateRepository<Vino>>();
        var updateRepository = database.Scope.ServiceProvider.GetRequiredService<IEfUpdateRepository<Vino>>();

        ((IGetContextable)createRepository).GetContext().Should().BeSameAs(database.Context);
        ((IGetContextable)updateRepository).GetContext().Should().BeSameAs(database.Context);
        ((IGetContextable)createRepository).GetContext()
            .Should().BeSameAs(((IGetContextable)updateRepository).GetContext());
    }

            [Fact]
            public async Task Paginacion_SinSelectorYConEmpates_OrdenaPorClavePrimaria()
            {
                await using var database = await TestDatabase.CreateAsync();
                var tag = $"Paginacion {Guid.NewGuid():N}";
                var wines = Enumerable.Range(0, 3).Select(index => CreateVino($"{tag} {index}")).ToArray();
                database.Context.Vinos.AddRange(wines);
                await database.Context.SaveChangesAsync();
                var repository = database.Scope.ServiceProvider.GetRequiredService<IEfPagedRepository<Vino>>();

                var withoutSelector = await repository.TryGetDataPaginationAsync(
                    new PageRequest(1, 2),
                    SortDirection.Descending,
                    filter: vino => vino.Nombre.StartsWith(tag));
                var nullSelectorsAndFilter = repository.TryGetDataPagination(
                    new PageRequest(1, 2),
                    orderByField: null,
                    filter: null);
                var withoutSelectorNextPage = await repository.TryGetDataPaginationAsync(
                    new PageRequest(2, 2),
                    SortDirection.Descending,
                    filter: vino => vino.Nombre.StartsWith(tag));

                withoutSelector.IsFailure.Should().BeFalse();
                nullSelectorsAndFilter.IsFailure.Should().BeFalse();
                withoutSelectorNextPage.IsFailure.Should().BeFalse();
                withoutSelector.GetValueOrThrow().Items.Select(vino => vino.Id)
                    .Concat(withoutSelectorNextPage.GetValueOrThrow().Items.Select(vino => vino.Id))
                    .Should().Equal(wines.Select(vino => vino.Id).OrderDescending());

                var tiedSelector = await repository.TryGetDataPaginationAsync(
                    new PageRequest(1, 2),
                    SortDirection.Descending,
                    orderByField: vino => vino.Año,
                    filter: vino => vino.Nombre.StartsWith(tag));
                var tiedSelectorNextPage = await repository.TryGetDataPaginationAsync(
                    new PageRequest(2, 2),
                    SortDirection.Descending,
                    orderByField: vino => vino.Año,
                    filter: vino => vino.Nombre.StartsWith(tag));

                tiedSelector.IsFailure.Should().BeFalse();
                tiedSelectorNextPage.IsFailure.Should().BeFalse();
                tiedSelector.GetValueOrThrow().Items.Select(vino => vino.Id)
                    .Concat(tiedSelectorNextPage.GetValueOrThrow().Items.Select(vino => vino.Id))
                    .Should().Equal(wines.Select(vino => vino.Id).OrderDescending());

                var ascendingPage = repository.TryGetDataPagination(
                    new PageRequest(1, 2),
                    SortDirection.Ascending,
                    orderByField: vino => vino.Año,
                    filter: vino => vino.Nombre.StartsWith(tag));
                var ascendingNextPage = repository.TryGetDataPagination(
                    new PageRequest(2, 2),
                    SortDirection.Ascending,
                    orderByField: vino => vino.Año,
                    filter: vino => vino.Nombre.StartsWith(tag));

                ascendingPage.GetValueOrThrow().Items.Select(vino => vino.Id)
                    .Concat(ascendingNextPage.GetValueOrThrow().Items.Select(vino => vino.Id))
                    .Should().Equal(wines.Select(vino => vino.Id).Order());
            }

            [Fact]
            public async Task TryGetLastBy_SelectorEmpatado_UsaClavePrimariaYLimitaResultado()
            {
                var interceptor = new CommandCaptureInterceptor();
                await using var database = await TestDatabase.CreateAsync(commandInterceptor: interceptor);
                var tag = $"Último ordenado {Guid.NewGuid():N}";
                var wines = Enumerable.Range(0, 3).Select(index => CreateVino($"{tag} {index}")).ToArray();
                database.Context.Vinos.AddRange(wines);
                await database.Context.SaveChangesAsync();
                interceptor.Commands.Clear();
                var repository = database.Scope.ServiceProvider.GetRequiredService<IEfOrderedReadRepository<Vino>>();
                var filter = (Expression<Func<Vino, bool>>)(vino => vino.Nombre.StartsWith(tag));
                Expression<Func<Vino, object>> tiedSelector = vino => vino.Año;

                var last = repository.TryGetLastBy(filter, tiedSelector, SortDirection.Ascending);
                var syncCommand = interceptor.Commands.Last();
                var lastAsync = await repository.TryGetLastByAsync(filter, tiedSelector, SortDirection.Ascending);
                var asyncCommand = interceptor.Commands.Last();
                var firstByDescending = await repository.TryGetLastByAsync(filter, tiedSelector, SortDirection.Descending);
                var descendingCommand = interceptor.Commands.Last();
                var canceled = await repository.TryGetLastByAsync(
                    filter,
                    tiedSelector,
                    SortDirection.Ascending,
                    new CancellationToken(canceled: true));
                var notFound = await repository.TryGetLastByAsync(
                    vino => vino.Nombre == "no encontrado",
                    tiedSelector,
                    SortDirection.Ascending);

                last.IsFailure.Should().BeFalse();
                last.GetValueOrThrow().Id.Should().Be(wines.Max(vino => vino.Id));
                lastAsync.IsFailure.Should().BeFalse();
                lastAsync.GetValueOrThrow().Id.Should().Be(wines.Max(vino => vino.Id));
                firstByDescending.IsFailure.Should().BeFalse();
                firstByDescending.GetValueOrThrow().Id.Should().Be(wines.Min(vino => vino.Id));
                canceled.IsFailure.Should().BeTrue();
                notFound.IsFailure.Should().BeTrue();
                syncCommand.Should().Contain("LIMIT 1");
                asyncCommand.Should().Contain("LIMIT 1");
                descendingCommand.Should().Contain("LIMIT 1");
            }

            [Fact]
            public async Task Paginacion_EntidadSinClavePrimaria_DevuelveFallo()
            {
                await using var database = await TestDatabase.CreateAsync();
                var repository = database.Scope.ServiceProvider.GetRequiredService<IEfPagedRepository<KeylessVino>>();

                var result = repository.TryGetDataPagination(
                    new PageRequest(1, 10),
                    orderByField: vino => vino.Nombre);

                result.IsFailure.Should().BeTrue();
            }

            [Fact]
            public async Task OrdenEstable_ClavePrimariaCompuesta_DesempataTodasLasPropiedades()
            {
                await using var database = await TestDatabase.CreateAsync();
                var tag = $"Clave compuesta {Guid.NewGuid():N}";
                var rows = new[]
                {
                    new CompositeKeyVino(2, 1, tag, 10),
                    new CompositeKeyVino(1, 2, tag, 10),
                    new CompositeKeyVino(1, 1, tag, 10)
                };
                database.Context.Set<CompositeKeyVino>().AddRange(rows);
                await database.Context.SaveChangesAsync();
                var pagedRepository = database.Scope.ServiceProvider
                    .GetRequiredService<IEfPagedRepository<CompositeKeyVino>>();
                var orderedRepository = database.Scope.ServiceProvider
                    .GetRequiredService<IEfOrderedReadRepository<CompositeKeyVino>>();
                var filter = (Expression<Func<CompositeKeyVino, bool>>)(row => row.Tag == tag);

                var pageOne = pagedRepository.TryGetDataPagination(
                    new PageRequest(1, 2),
                    SortDirection.Ascending,
                    row => row.SortValue,
                    filter);
                var pageTwo = pagedRepository.TryGetDataPagination(
                    new PageRequest(2, 2),
                    SortDirection.Ascending,
                    row => row.SortValue,
                    filter);
                var last = await orderedRepository.TryGetLastByAsync(filter, row => row.SortValue);
                var pageKeys = pageOne.GetValueOrThrow().Items.Concat(pageTwo.GetValueOrThrow().Items)
                    .Select(row => (row.BodegaId, row.VinoId));

                pageKeys.Should().Equal((1, 1), (1, 2), (2, 1));
                last.GetValueOrThrow().Should().BeEquivalentTo(rows[0]);
            }

    [Fact]
    public void DosContextos_ResuelvenLaUnidadDeTrabajoGenericaCorrecta()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        services.AddScoped<CountingTestDbContext>(_ =>
            new CountingTestDbContext(new DbContextOptionsBuilder<CountingTestDbContext>().Options));
        services.AddScoped<AlternateTestDbContext>(_ =>
            new AlternateTestDbContext(new DbContextOptionsBuilder<AlternateTestDbContext>().Options));
        services.AddOofpRepositories<Vino, CountingTestDbContext>();
        services.AddOofpRepositories<Cata, AlternateTestDbContext>();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var repositoryA = scope.ServiceProvider.GetRequiredService<IEfRepository<Vino>>();
        var repositoryB = scope.ServiceProvider.GetRequiredService<IEfRepository<Cata>>();
        var contextA = scope.ServiceProvider.GetRequiredService<CountingTestDbContext>();
        var contextB = scope.ServiceProvider.GetRequiredService<AlternateTestDbContext>();
        var unitOfWorkA = scope.ServiceProvider.GetRequiredService<IMlUnitOfWork<CountingTestDbContext>>();
        var unitOfWorkB = scope.ServiceProvider.GetRequiredService<IMlUnitOfWork<AlternateTestDbContext>>();

        repositoryA.UnitOfWork.Should().BeSameAs(unitOfWorkA);
        repositoryB.UnitOfWork.Should().BeSameAs(unitOfWorkB);
        ((IGetContextable)repositoryA).GetContext().Should().BeSameAs(contextA);
        ((IGetContextable)repositoryB).GetContext().Should().BeSameAs(contextB);
        unitOfWorkA.Should().NotBeSameAs(unitOfWorkB);
    }

    [Fact]
    public async Task ResolverElAgregadoMilVecesEnElMismoScope_DevuelveLaMismaInstancia()
    {
        await using var database = await TestDatabase.CreateAsync();
        var first = database.Scope.ServiceProvider.GetRequiredService<IEfRepository<Vino>>();

        for (var index = 0; index < 1_000; index++)
        {
            database.Scope.ServiceProvider.GetRequiredService<IEfRepository<Vino>>()
                    .Should().BeSameAs(first);
        }
    }

    [Fact]
    public async Task TryAddAsync_SoloMarcaCambiosHastaElCommit()
    {
        await using var database = await TestDatabase.CreateAsync();
        var vino = CreateVino();

        var result = await database.Repository.TryAddAsync(vino);

        result.IsFailure.Should().BeFalse();
        database.Context.SaveChangesCallCount.Should().Be(0);
        database.UnitOfWork.HasPendingChanges.Should().BeTrue();
    }

    [Fact]
    public async Task TodosLosMetodosDeEscritura_SoloMarcanCambiosHastaElCommit()
    {
        await using var database = await TestDatabase.CreateAsync();
        var existing = Enumerable.Range(0, 16).Select(_ => CreateVino()).ToArray();
        database.Context.Vinos.AddRange(existing);
        await database.Context.SaveChangesAsync();
        database.Context.SaveChangesCallCount = 0;

        var create = database.Scope.ServiceProvider.GetRequiredService<IEfCreateRepository<Vino>>();
        var update = database.Scope.ServiceProvider.GetRequiredService<IEfUpdateRepository<Vino>>();
        var delete = database.Scope.ServiceProvider.GetRequiredService<IEfDeleteRepository<Vino>>();

        create.TryAdd(CreateVino()).IsSuccess.Should().BeTrue();
        (await create.TryAddAsync(CreateVino())).IsSuccess.Should().BeTrue();
        create.TryAddRange(new[] { CreateVino(), CreateVino() }).IsSuccess.Should().BeTrue();
        (await create.TryAddRangeAsync(new[] { CreateVino(), CreateVino() })).IsSuccess.Should().BeTrue();

        update.TryUpdate(existing[0]).IsSuccess.Should().BeTrue();
        (await update.TryUpdateAsync(existing[1])).IsSuccess.Should().BeTrue();
        update.TryUpdate(existing[2], existing[2].Id).IsSuccess.Should().BeTrue();
        (await update.TryUpdateAsync(existing[3], CancellationToken.None, existing[3].Id)).IsSuccess.Should().BeTrue();
        update.TryUpdateRange(existing[4..6]).IsSuccess.Should().BeTrue();
        (await update.TryUpdateRangeAsync(existing[6..8])).IsSuccess.Should().BeTrue();

        delete.TryRemove(existing[8]).IsSuccess.Should().BeTrue();
        delete.TryRemove(existing[9].Id).IsSuccess.Should().BeTrue();
        (await delete.TryRemoveAsync(existing[10])).IsSuccess.Should().BeTrue();
        (await delete.TryRemoveAsync(CancellationToken.None, existing[11].Id)).IsSuccess.Should().BeTrue();
        delete.TryRemoveRange(existing[12..14]).IsSuccess.Should().BeTrue();
        (await delete.TryRemoveRangeAsync(existing[14..16])).IsSuccess.Should().BeTrue();

        database.Context.SaveChangesCallCount.Should().Be(0);
        database.UnitOfWork.HasPendingChanges.Should().BeTrue();
        (await database.Context.Vinos.AsNoTracking().CountAsync()).Should().Be(16);
    }

    [Fact]
    public async Task TryCommitAsync_PersisteCambiosYDevuelveFilasAfectadas()
    {
        await using var database = await TestDatabase.CreateAsync();
        var vino = CreateVino();
        database.Context.Vinos.Add(vino);

        var result = await database.UnitOfWork.TryCommitAsync();

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be(1);
        database.Context.SaveChangesCallCount.Should().Be(1);
        database.UnitOfWork.HasPendingChanges.Should().BeFalse();
        (await database.Context.Vinos.AsNoTracking().SingleAsync(x => x.Nombre == vino.Nombre))
            .Nombre.Should().Be(vino.Nombre);
    }

    [Fact]
    public async Task TryCommit_PersisteCambiosYDevuelveFilasAfectadas()
    {
        await using var database = await TestDatabase.CreateAsync();
        var vino = CreateVino();
        database.Context.Vinos.Add(vino);

        var result = database.UnitOfWork.TryCommit();

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be(1);
        database.Context.SaveChangesCallCount.Should().Be(1);
        (await database.Context.Vinos.AsNoTracking().SingleAsync(x => x.Nombre == vino.Nombre))
            .Nombre.Should().Be(vino.Nombre);
    }

    [Fact]
    public async Task HasPendingChanges_EsDeSoloLecturaYDiscardChangesLimpiaElTracker()
    {
        await using var database = await TestDatabase.CreateAsync();
        var vino = CreateVino();
        database.Context.Vinos.Add(vino);

        database.UnitOfWork.HasPendingChanges.Should().BeTrue();
        database.Context.SaveChangesCallCount.Should().Be(0);

        var result = database.UnitOfWork.DiscardChanges();

        result.IsFailure.Should().BeFalse();
        database.UnitOfWork.HasPendingChanges.Should().BeFalse();
        database.Context.SaveChangesCallCount.Should().Be(0);
        (await database.Context.Vinos.AsNoTracking().AnyAsync(x => x.Nombre == vino.Nombre))
            .Should().BeFalse();
    }

    [Fact]
    public async Task TryExecuteInTransactionAsync_CuandoTodasLasOperacionesSonValidas_ConfirmaUnaVez()
    {
        await using var database = await TestDatabase.CreateAsync();
        var first = CreateVino();
        var second = CreateVino();

        var result = await database.UnitOfWork.TryExecuteInTransactionAsync<string>(token =>
        {
            database.Context.Vinos.AddRange(first, second);
            return Task.FromResult("guardado".ToMlResultValid());
        });

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be("guardado");
        database.Context.SaveChangesCallCount.Should().Be(1);
        database.Context.ChangeTracker.Clear();
        (await database.Context.Vinos.AsNoTracking().CountAsync(x =>
            x.Nombre == first.Nombre || x.Nombre == second.Nombre)).Should().Be(2);
    }

    [Fact]
    public async Task TryExecuteInTransactionAsync_CuandoElCallbackDevuelveFallo_ReversaYConservaErrores()
    {
        await using var database = await TestDatabase.CreateAsync();
        var vino = CreateVino();
        var errors = MlErrorDetails.FromErrorMessage("fallo original");
        var callbackResult = errors.ToMlResultFail<string>();

        var result = await database.UnitOfWork.TryExecuteInTransactionAsync<string>(async token =>
        {
            database.Context.Vinos.Add(vino);
            await database.Context.SaveChangesAsync(token);
            return callbackResult;
        });

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Should().BeSameAs(errors);
        database.Context.SaveChangesCallCount.Should().Be(1);
        database.Context.ChangeTracker.Clear();
        (await database.Context.Vinos.AsNoTracking().AnyAsync(x => x.Nombre == vino.Nombre))
            .Should().BeFalse();
    }

    [Fact]
    public async Task TryExecuteInTransactionAsync_CuandoElCallbackLanza_ReversaYDevuelveFallo()
    {
        await using var database = await TestDatabase.CreateAsync();
        var vino = CreateVino();
        var exception = new InvalidOperationException("fallo de callback");

        var result = await database.UnitOfWork.TryExecuteInTransactionAsync<string>(async token =>
        {
            database.Context.Vinos.Add(vino);
            await database.Context.SaveChangesAsync(token);
            throw exception;
        });

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().ToString().Should().Contain(exception.Message);
        database.Context.ChangeTracker.Clear();
        (await database.Context.Vinos.AsNoTracking().AnyAsync(x => x.Nombre == vino.Nombre))
            .Should().BeFalse();
    }

    [Fact]
    public async Task TryExecuteInTransactionAsync_CuandoSeCancela_ReversaYDevuelveFallo()
    {
        await using var database = await TestDatabase.CreateAsync();
        using var cancellation = new CancellationTokenSource();
        var vino = CreateVino();
        CancellationToken callbackToken = default;

        var result = await database.UnitOfWork.TryExecuteInTransactionAsync<string>(async token =>
        {
            callbackToken = token;
            database.Context.Vinos.Add(vino);
            await database.Context.SaveChangesAsync(token);
            cancellation.Cancel();
            throw new OperationCanceledException(token);
        }, cancellation.Token);

        result.IsFailure.Should().BeTrue();
        callbackToken.Should().Be(cancellation.Token);
        database.Context.ChangeTracker.Clear();
        (await database.Context.Vinos.AsNoTracking().AnyAsync(x => x.Nombre == vino.Nombre))
            .Should().BeFalse();
    }

    [Fact]
    public async Task TryExecuteInTransactionAsync_CuandoFallaElCommit_ReversaYDevuelveFallo()
    {
        await using var database = await TestDatabase.CreateAsync();
        var duplicateName = $"Vino duplicado {Guid.NewGuid():N}";
        var first = CreateVino(duplicateName);
        var second = CreateVino(duplicateName);

        var result = await database.UnitOfWork.TryExecuteInTransactionAsync<string>(token =>
        {
            database.Context.Vinos.AddRange(first, second);
            return Task.FromResult("no debe confirmarse".ToMlResultValid());
        });

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().ToString().Should().Contain("confirmar");
        database.Context.SaveChangesCallCount.Should().Be(1);
        database.Context.ChangeTracker.Clear();
        (await database.Context.Vinos.AsNoTracking().AnyAsync(x => x.Nombre == duplicateName))
            .Should().BeFalse();
    }

    [Fact]
    public async Task TryCommitAsync_CuandoLaBaseRechazaCambios_DevuelveFalloConDetalleUtil()
    {
        await using var database = await TestDatabase.CreateAsync();
        var duplicateName = $"duplicado {Guid.NewGuid():N}";
        database.Context.Vinos.AddRange(CreateVino(duplicateName), CreateVino(duplicateName));

        var result = await database.UnitOfWork.TryCommitAsync();

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().ToString().Should().Contain("confirmar");
        database.Context.SaveChangesCallCount.Should().Be(1);
        database.Context.ChangeTracker.Clear();
        (await database.Context.Vinos.AsNoTracking().AnyAsync(x => x.Nombre == duplicateName))
            .Should().BeFalse();
    }

    [Fact]
    public async Task TryCommitAsync_CuandoHayConflictoOptimista_DevuelveFalloConDetalleUtil()
    {
        await using var database = await TestDatabase.CreateAsync();
        var vino = CreateVino();
        database.Context.Vinos.Add(vino);
        await database.Context.SaveChangesAsync();
        database.Context.SaveChangesCallCount = 0;
        var tracked = await database.Context.Vinos.SingleAsync(x => x.Id == vino.Id);

        await using (var competingContext = new CountingTestDbContext(database.Options))
        {
            var competing = await competingContext.Vinos.SingleAsync(x => x.Id == vino.Id);
            competing.Nombre = $"{vino.Nombre} modificado";
            await competingContext.SaveChangesAsync();
        }

        tracked.Comentarios = "edición basada en una versión obsoleta";
        var result = await database.UnitOfWork.TryCommitAsync();

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().ToString().Should().Contain("concurrencia");
        database.Context.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task TryExecuteInTransaction_CuandoEsValido_ConfirmaUnaVez()
    {
        await using var database = await TestDatabase.CreateAsync();
        var vino = CreateVino();

        var result = database.UnitOfWork.TryExecuteInTransaction(() =>
        {
            database.Context.Vinos.Add(vino);
            return "confirmado".ToMlResultValid();
        });

        result.IsFailure.Should().BeFalse();
        result.GetValueOrThrow().Should().Be("confirmado");
        database.Context.SaveChangesCallCount.Should().Be(1);
        database.Context.ChangeTracker.Clear();
        (await database.Context.Vinos.AsNoTracking().AnyAsync(x => x.Nombre == vino.Nombre))
            .Should().BeTrue();
    }

    [Fact]
    public async Task TryExecuteInTransactionAsync_ConTransaccionExterna_ReutilizaSinConfirmarla()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var outerTransaction = await database.Context.Database.BeginTransactionAsync();
        var vino = CreateVino();

        var result = await database.UnitOfWork.TryExecuteInTransactionAsync<string>(token =>
        {
            database.Context.Vinos.Add(vino);
            return Task.FromResult("hecho".ToMlResultValid());
        });

        result.IsFailure.Should().BeFalse();
        database.Context.Database.CurrentTransaction.Should().BeSameAs(outerTransaction);
        database.Context.SaveChangesCallCount.Should().Be(1);
        await outerTransaction.RollbackAsync();
        database.Context.ChangeTracker.Clear();
        (await database.Context.Vinos.AsNoTracking().AnyAsync(x => x.Nombre == vino.Nombre))
            .Should().BeFalse();
    }

    [Fact]
    public async Task Dispose_ConDiagnosticosDesactivados_NoEmiteWarning()
    {
        var loggerProvider = new WarningCaptureLoggerProvider();
        var database = await TestDatabase.CreateAsync(loggerProvider: loggerProvider);
        database.Context.Vinos.Add(CreateVino());

        await database.DisposeAsync();

        loggerProvider.Entries.Should().NotContain(x => x.Level == LogLevel.Warning);
    }

    [Fact]
    public async Task Dispose_ConDiagnosticosActivadosYTrabajoPendiente_EmiteWarning()
    {
        var loggerProvider = new WarningCaptureLoggerProvider();
        var database = await TestDatabase.CreateAsync(enableDiagnostics: true,
                                                      loggerProvider: loggerProvider);
        database.Context.Vinos.Add(CreateVino());

        await database.DisposeAsync();

        loggerProvider.Entries.Should().Contain(x => x.Level == LogLevel.Warning &&
                                                     x.Message.Contains("confirm", StringComparison.OrdinalIgnoreCase));
    }

    private static Vino CreateVino(string? nombre = null)
        => new()
        {
            Nombre = nombre ?? $"Vino UoW {Guid.NewGuid():N}",
            Bodega = "Bodega UoW",
            Año = 2026,
            FechaEntrada = DateTime.UtcNow,
            Comentarios = "Prueba de unidad de trabajo"
        };

    private sealed class CountingTestDbContext(DbContextOptions<CountingTestDbContext> options)
        : DbContext(options)
    {
        public DbSet<Vino> Vinos => Set<Vino>();

        public int SaveChangesCallCount { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new VinoConfiguration());
            modelBuilder.ApplyConfiguration(new CataConfiguration());
            modelBuilder.ApplyConfiguration(new VinosCataConfiguration());
            modelBuilder.ApplyConfiguration(new VinosCatasPuntuacionConfiguration());
            modelBuilder.Entity<Vino>().Property(x => x.Nombre).IsConcurrencyToken();
            modelBuilder.Entity<KeylessVino>().HasNoKey().ToTable("KeylessVinos");
            modelBuilder.Entity<CompositeKeyVino>()
                .HasKey(row => new { row.BodegaId, row.VinoId });
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SaveChangesCallCount++;
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
                                                   CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }

    private sealed class AlternateTestDbContext(DbContextOptions<AlternateTestDbContext> options)
        : DbContext(options)
    {
    }

    private sealed class KeylessVino
    {
        public string Nombre { get; set; } = string.Empty;
    }

    private sealed record CompositeKeyVino(int BodegaId, int VinoId, string Tag, int SortValue);

    private sealed class TestDatabase(SqliteConnection connection,
                                      ServiceProvider provider,
                                      AsyncServiceScope scope,
                                      DbContextOptions<CountingTestDbContext> options,
                                      CountingTestDbContext context,
                                      IEfRepository<Vino> repository) : IAsyncDisposable
    {
        public AsyncServiceScope Scope { get; } = scope;
        public DbContextOptions<CountingTestDbContext> Options { get; } = options;
        public CountingTestDbContext Context { get; } = context;
        public IEfRepository<Vino> Repository { get; } = repository;
        public IMlUnitOfWork UnitOfWork => Repository.UnitOfWork;

        public static async Task<TestDatabase> CreateAsync(bool enableDiagnostics = false,
                                                           WarningCaptureLoggerProvider? loggerProvider = null,
                                                           DbCommandInterceptor? commandInterceptor = null)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var optionsBuilder = new DbContextOptionsBuilder<CountingTestDbContext>().UseSqlite(connection);
            if (commandInterceptor is not null)
                optionsBuilder.AddInterceptors(commandInterceptor);
            var options = optionsBuilder.Options;
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging(builder =>
            {
                if (loggerProvider is not null)
                    builder.AddProvider(loggerProvider);
            });
            if (enableDiagnostics)
                services.Configure<MlUnitOfWorkDiagnostics>(diagnostics =>
                    diagnostics.WarnOnUncommittedChanges = true);
            services.AddScoped<CountingTestDbContext>(_ => new CountingTestDbContext(options));
            services.AddOofpRepositories<Vino, CountingTestDbContext>();
            services.AddOofpRepositories<KeylessVino, CountingTestDbContext>();
            services.AddOofpRepositories<CompositeKeyVino, CountingTestDbContext>();

            var provider = services.BuildServiceProvider();
            var scope = provider.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<CountingTestDbContext>();
            await context.Database.EnsureCreatedAsync();
            context.SaveChangesCallCount = 0;
            var repository = scope.ServiceProvider.GetRequiredService<IEfRepository<Vino>>();

            return new TestDatabase(connection, provider, scope, options, context, repository);
        }

        public async ValueTask DisposeAsync()
        {
            await Scope.DisposeAsync();
            await provider.DisposeAsync();
            await connection.DisposeAsync();
        }
    }

    private sealed class WarningCaptureLoggerProvider : ILoggerProvider
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public ILogger CreateLogger(string categoryName) => new WarningCaptureLogger(Entries);

        public void Dispose()
        {
        }
    }

    private sealed class CommandCaptureInterceptor : DbCommandInterceptor
    {
        public List<string> Commands { get; } = [];

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command,
                                                                          CommandEventData eventData,
                                                                          InterceptionResult<DbDataReader> result)
        {
            Commands.Add(command.CommandText);
            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            Commands.Add(command.CommandText);
            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }
    }

    private sealed class WarningCaptureLogger(List<(LogLevel Level, string Message)> entries) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        void ILogger.Log<TState>(LogLevel logLevel,
                                 EventId eventId,
                                 TState state,
                                 Exception? exception,
                                 Func<TState, Exception?, string> formatter)
            => entries.Add((logLevel, formatter(state, exception)));
    }
}