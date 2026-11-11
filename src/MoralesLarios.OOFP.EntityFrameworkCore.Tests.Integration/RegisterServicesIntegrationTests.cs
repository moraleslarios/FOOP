using MoralesLarios.OOFP.EntityFrameworkCore;
using MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration;

public class RegisterServicesIntegrationTests
{
    [Fact]
    public void RepositoryGenericParameters_ExposeStableNames()
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
    }

    [Fact]
    public async Task AddOofpRepositories_SharesTypedAndUntypedUnitOfWorkWithinScope()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var specific = database.Scope.ServiceProvider
            .GetRequiredService<IMlUnitOfWork<UnitOfWorkTransactionTests.CountingTestDbContext>>();

        database.Repository.UnitOfWork.Should().BeSameAs(specific);
        database.Scope.ServiceProvider.GetRequiredService<IMlUnitOfWork>()
            .Should().BeSameAs(specific);
    }

    [Fact]
    public async Task AddOofpRepositories_SharesDbContextAcrossRepositoriesInScope()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var createRepository = database.Scope.ServiceProvider.GetRequiredService<IEfCreateRepository<Vino>>();
        var updateRepository = database.Scope.ServiceProvider.GetRequiredService<IEfUpdateRepository<Vino>>();

        ((IGetContextable)createRepository).GetContext().Should().BeSameAs(database.Context);
        ((IGetContextable)updateRepository).GetContext().Should().BeSameAs(database.Context);
    }

    [Fact]
    public void AddOofpRepositories_SeparatesUnitOfWorkByContextType()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        services.AddScoped<UnitOfWorkTransactionTests.CountingTestDbContext>(_ =>
            new UnitOfWorkTransactionTests.CountingTestDbContext(
                new DbContextOptionsBuilder<UnitOfWorkTransactionTests.CountingTestDbContext>().Options));
        services.AddScoped<UnitOfWorkTransactionTests.AlternateTestDbContext>(_ =>
            new UnitOfWorkTransactionTests.AlternateTestDbContext(
                new DbContextOptionsBuilder<UnitOfWorkTransactionTests.AlternateTestDbContext>().Options));
        services.AddOofpRepositories<Vino, UnitOfWorkTransactionTests.CountingTestDbContext>();
        services.AddOofpRepositories<Cata, UnitOfWorkTransactionTests.AlternateTestDbContext>();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var repositoryA = scope.ServiceProvider.GetRequiredService<IEfRepository<Vino>>();
        var repositoryB = scope.ServiceProvider.GetRequiredService<IEfRepository<Cata>>();
        var contextA = scope.ServiceProvider.GetRequiredService<UnitOfWorkTransactionTests.CountingTestDbContext>();
        var contextB = scope.ServiceProvider.GetRequiredService<UnitOfWorkTransactionTests.AlternateTestDbContext>();
        var unitOfWorkA = scope.ServiceProvider
            .GetRequiredService<IMlUnitOfWork<UnitOfWorkTransactionTests.CountingTestDbContext>>();
        var unitOfWorkB = scope.ServiceProvider
            .GetRequiredService<IMlUnitOfWork<UnitOfWorkTransactionTests.AlternateTestDbContext>>();

        repositoryA.UnitOfWork.Should().BeSameAs(unitOfWorkA);
        repositoryB.UnitOfWork.Should().BeSameAs(unitOfWorkB);
        ((IGetContextable)repositoryA).GetContext().Should().BeSameAs(contextA);
        ((IGetContextable)repositoryB).GetContext().Should().BeSameAs(contextB);
        unitOfWorkA.Should().NotBeSameAs(unitOfWorkB);
    }

    [Fact]
    public async Task AddOofpRepositories_ResolvesSameRepositoryInstanceWithinScope()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var first = database.Scope.ServiceProvider.GetRequiredService<IEfRepository<Vino>>();

        for (var index = 0; index < 1_000; index++)
        {
            database.Scope.ServiceProvider.GetRequiredService<IEfRepository<Vino>>()
                .Should().BeSameAs(first);
        }
    }
}