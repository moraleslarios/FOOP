using MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration.Repositories;

public class EfCreateRepositoryIntegrationTests
{
    [Fact]
    public async Task TryAddAsync_MarksChangesWithoutSaving()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfCreateRepository<Vino>>();

        var result = await repository.TryAddAsync(UnitOfWorkTransactionTests.CreateVino());

        result.IsSuccess.Should().BeTrue();
        database.Context.SaveChangesCallCount.Should().Be(0);
        database.UnitOfWork.HasPendingChanges.Should().BeTrue();
        (await database.Context.Vinos.AsNoTracking().CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task AddAndAddRange_SyncAndAsync_MarkChangesWithoutSaving()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfCreateRepository<Vino>>();

        var single = repository.TryAdd(UnitOfWorkTransactionTests.CreateVino());
        var singleAsync = await repository.TryAddAsync(UnitOfWorkTransactionTests.CreateVino());
        var range = repository.TryAddRange(CreateWines(2));
        var rangeAsync = await repository.TryAddRangeAsync(CreateWines(2));

        new[] { single.IsSuccess, singleAsync.IsSuccess, range.IsSuccess, rangeAsync.IsSuccess }
            .Should().OnlyContain(isSuccess => isSuccess);
        database.Context.SaveChangesCallCount.Should().Be(0);
        database.UnitOfWork.HasPendingChanges.Should().BeTrue();
        (await database.Context.Vinos.AsNoTracking().CountAsync()).Should().Be(0);
    }

    private static Vino[] CreateWines(int count)
        => Enumerable.Range(0, count)
            .Select(_ => UnitOfWorkTransactionTests.CreateVino())
            .ToArray();
}