using MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration.Repositories;

public class EfDeleteRepositoryIntegrationTests
{
    [Fact]
    public async Task RemoveAndRemoveRange_SyncAndAsync_MarkChangesWithoutSaving()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var existing = await AddWinesAsync(database, 6);
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfDeleteRepository<Vino>>();

        var single = repository.TryRemove(existing[0]);
        var singleByKey = repository.TryRemove(existing[1].Id);
        var singleAsync = await repository.TryRemoveAsync(existing[2]);
        var singleByKeyAsync = await repository.TryRemoveAsync(CancellationToken.None, existing[3].Id);
        var range = repository.TryRemoveRange(existing[4..6]);
        var rangeAsync = await repository.TryRemoveRangeAsync(existing[4..6]);

        new[] { single.IsSuccess, singleByKey.IsSuccess, singleAsync.IsSuccess,
                singleByKeyAsync.IsSuccess, range.IsSuccess, rangeAsync.IsSuccess }
            .Should().OnlyContain(isSuccess => isSuccess);
        database.Context.SaveChangesCallCount.Should().Be(0);
        database.UnitOfWork.HasPendingChanges.Should().BeTrue();
        (await database.Context.Vinos.AsNoTracking().CountAsync()).Should().Be(6);
    }

    private static async Task<Vino[]> AddWinesAsync(
        UnitOfWorkTransactionTests.TestDatabase database,
        int count)
    {
        var wines = Enumerable.Range(0, count)
            .Select(_ => UnitOfWorkTransactionTests.CreateVino())
            .ToArray();
        database.Context.Vinos.AddRange(wines);
        await database.Context.SaveChangesAsync();
        database.Context.SaveChangesCallCount = 0;
        return wines;
    }
}