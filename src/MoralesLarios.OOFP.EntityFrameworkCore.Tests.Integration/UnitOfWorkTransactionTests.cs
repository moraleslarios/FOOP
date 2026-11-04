using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using MoralesLarios.OOFP.EntityFrameworkCore;
using MoralesLarios.OOFP.EntityFrameworkCore.Repositories;
using MoralesLarios.OOFP.EntityFrameworkCore.TestSupport.Configurations;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration;

public class UnitOfWorkTransactionTests
{
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
                                                           WarningCaptureLoggerProvider? loggerProvider = null)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<CountingTestDbContext>()
                          .UseSqlite(connection)
                          .Options;
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