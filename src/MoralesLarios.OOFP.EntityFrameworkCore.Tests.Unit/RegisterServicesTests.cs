namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Unit;

public class RegisterServicesTests
{
    [Fact]
    public void AddOofpRepositories_ReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddOofpRepositories<TestEntity, DbContext>();

        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddOofpRepositories_RegistersRepositoryServicesAsScoped()
    {
        var services = new ServiceCollection();
        services.AddOofpRepositories<TestEntity, DbContext>();
        var expected = new[]
        {
            typeof(IMlUnitOfWork),
            typeof(IMlUnitOfWork<DbContext>),
            typeof(IEfRepository<TestEntity>),
            typeof(IEfPagedRepository<TestEntity>),
            typeof(IEfCreateRepository<TestEntity>),
            typeof(IEfDeleteRepository<TestEntity>),
            typeof(IEfReadRepository<TestEntity>),
            typeof(IEfOrderedReadRepository<TestEntity>),
            typeof(IEfUpdateRepository<TestEntity>),
            typeof(IEfWriteRepository<TestEntity>),
            typeof(IEfPagedReadRepository<TestEntity>)
        };

        var registrations = services
            .Where(descriptor => expected.Contains(descriptor.ServiceType))
            .ToArray();

        registrations.Select(descriptor => descriptor.ServiceType)
            .Should().BeEquivalentTo(expected);
        registrations.Should().OnlyContain(descriptor => descriptor.Lifetime == ServiceLifetime.Scoped);
    }
}