using Microsoft.Extensions.Logging.Abstractions;
using MoralesLarios.OOFP.Application.Services;
using MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration.Repositories;

public class EfUpdateRepositoryIntegrationTests
{
    [Fact]
    public async Task UpdateAndUpdateRange_SyncAndAsync_MarkChangesWithoutSaving()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var existing = await AddWinesAsync(database, 6);
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfUpdateRepository<Vino>>();
        var originalName = existing[0].Nombre;
        existing[0].Nombre = $"{originalName} actualizado";

        var single = repository.TryUpdate(existing[0]);
        var singleAsync = await repository.TryUpdateAsync(existing[1]);
        var byKey = repository.TryUpdate(existing[2], existing[2].Id);
        var byKeyAsync = await repository.TryUpdateAsync(existing[3], CancellationToken.None, existing[3].Id);
        var range = repository.TryUpdateRange(existing[4..6]);
        var rangeAsync = await repository.TryUpdateRangeAsync(existing[4..6]);

        new[] { single.IsSuccess, singleAsync.IsSuccess, byKey.IsSuccess, byKeyAsync.IsSuccess,
                range.IsSuccess, rangeAsync.IsSuccess }
            .Should().OnlyContain(isSuccess => isSuccess);
        database.Context.SaveChangesCallCount.Should().Be(0);
        database.UnitOfWork.HasPendingChanges.Should().BeTrue();
        (await database.Context.Vinos.AsNoTracking().SingleAsync(vino => vino.Id == existing[0].Id))
            .Nombre.Should().Be(originalName);
    }

    [Fact]
    public async Task TryUpdateAsync_DetachedInstance_ReplacesTrackedEntityAndPersistsChanges()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var stored = UnitOfWorkTransactionTests.CreateVino();
        database.Context.Vinos.Add(stored);
        await database.Context.SaveChangesAsync();
        var detached = UnitOfWorkTransactionTests.CreateVino(stored.Nombre);
        detached.Id = stored.Id;
        detached.Comentarios = "comentarios actualizados";
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfUpdateRepository<Vino>>();

        var result = await repository.TryUpdateAsync(detached, CancellationToken.None, detached.Id);

        result.IsSuccess.Should().BeTrue();
        (await database.UnitOfWork.TryCommitAsync()).IsSuccess.Should().BeTrue();
        database.Context.ChangeTracker.Clear();
        (await database.Context.Vinos.AsNoTracking().SingleAsync(vino => vino.Id == stored.Id))
            .Comentarios.Should().Be(detached.Comentarios);
    }


    [Fact]
    public async Task CrudServiceUpdateAsync_ByKey_ExecutesOneSelectForTrackedEntity()
    {
        var interceptor = new UnitOfWorkTransactionTests.CommandCaptureInterceptor();
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync(
            commandInterceptor: interceptor);
        var stored = UnitOfWorkTransactionTests.CreateVino();
        database.Context.Vinos.Add(stored);
        await database.Context.SaveChangesAsync();
        database.Context.ChangeTracker.Clear();
        interceptor.Commands.Clear();
        var updatedComments = "comentarios actualizados por servicio";
        var dto = UnitOfWorkTransactionTests.CreateVino(stored.Nombre);
        dto.Id = stored.Id;
        dto.Comentarios = updatedComments;
        var service = new CrudServiceFp<Vino, Vino>(
            database.Repository,
            NullLogger<CrudServiceFp<Vino, Vino>>.Instance);

        var result = await service.UpdateAsync(
            dto,
            MlErrorDetails.FromErrorMessage("no encontrado"),
            CancellationToken.None,
            pk: new object[] { dto.Id });

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Comentarios.Should().Be(updatedComments);
        interceptor.Commands
            .Where(command => command.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            .Should().ContainSingle();
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