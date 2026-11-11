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

    internal static Vino CreateVino(string? nombre = null)
        => new()
        {
            Nombre = nombre ?? $"Vino UoW {Guid.NewGuid():N}",
            Bodega = "Bodega UoW",
            Año = 2026,
            FechaEntrada = DateTime.UtcNow,
            Comentarios = "Prueba de unidad de trabajo"
        };

    internal sealed class CountingTestDbContext(DbContextOptions<CountingTestDbContext> options)
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

    internal sealed class AlternateTestDbContext(DbContextOptions<AlternateTestDbContext> options)
        : DbContext(options)
    {
    }

    internal sealed class KeylessVino
    {
        public string Nombre { get; set; } = string.Empty;
    }

    internal sealed record CompositeKeyVino(int BodegaId, int VinoId, string Tag, int SortValue);

    internal sealed class TestDatabase(SqliteConnection connection,
                                       ServiceProvider provider,
                                       AsyncServiceScope scope,
                                       DbContextOptions<CountingTestDbContext> options,
                                       CountingTestDbContext context,
                                       IEfRepository<Vino> repository) : IAsyncDisposable
    {
        internal AsyncServiceScope Scope { get; } = scope;
        internal DbContextOptions<CountingTestDbContext> Options { get; } = options;
        internal CountingTestDbContext Context { get; } = context;
        internal IEfRepository<Vino> Repository { get; } = repository;
        internal IMlUnitOfWork UnitOfWork => Repository.UnitOfWork;

        internal static async Task<TestDatabase> CreateAsync(bool enableDiagnostics = false,
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

    internal sealed class WarningCaptureLoggerProvider : ILoggerProvider
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public ILogger CreateLogger(string categoryName) => new WarningCaptureLogger(Entries);

        public void Dispose()
        {
        }
    }

    internal sealed class CommandCaptureInterceptor : DbCommandInterceptor
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