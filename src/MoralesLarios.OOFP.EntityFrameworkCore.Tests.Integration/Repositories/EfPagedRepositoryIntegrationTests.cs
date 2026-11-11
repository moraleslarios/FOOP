using System.Linq.Expressions;
using MoralesLarios.OOFP.EntityFrameworkCore.Enums;
using MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration;
using MoralesLarios.OOFP.Shared.Pagination;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration.Repositories;

public class EfPagedRepositoryIntegrationTests
{
    [Fact]
    public async Task TryGetDataPaginationAsync_NoSelector_OrdersPagesByPrimaryKeyDescending()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var wines = await AddWinesAsync(database, "sin selector");
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfPagedRepository<Vino>>();
        var filter = (Expression<Func<Vino, bool>>)(vino => vino.Nombre.StartsWith("sin selector"));

        var firstPage = await repository.TryGetDataPaginationAsync(
            new PageRequest(1, 2), SortDirection.Descending, filter: filter);
        var secondPage = await repository.TryGetDataPaginationAsync(
            new PageRequest(2, 2), SortDirection.Descending, filter: filter);

        firstPage.GetValueOrThrow().Items.Select(vino => vino.Id)
            .Concat(secondPage.GetValueOrThrow().Items.Select(vino => vino.Id))
            .Should().Equal(wines.Select(vino => vino.Id).OrderDescending());
    }

    [Fact]
    public async Task TryGetDataPaginationAsync_TiedSelector_UsesPrimaryKeyAcrossPages()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var wines = await AddWinesAsync(database, "selector empatado");
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfPagedRepository<Vino>>();
        var filter = (Expression<Func<Vino, bool>>)(vino => vino.Nombre.StartsWith("selector empatado"));

        var firstPage = await repository.TryGetDataPaginationAsync(
            new PageRequest(1, 2), SortDirection.Ascending, vino => vino.Año, filter);
        var secondPage = await repository.TryGetDataPaginationAsync(
            new PageRequest(2, 2), SortDirection.Ascending, vino => vino.Año, filter);

        firstPage.GetValueOrThrow().Items.Select(vino => vino.Id)
            .Concat(secondPage.GetValueOrThrow().Items.Select(vino => vino.Id))
            .Should().Equal(wines.Select(vino => vino.Id).Order());
    }

    [Fact]
    public async Task TryGetDataPagination_NoSelectorOrFilter_ReturnsFirstPageAndTotal()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var wines = await AddWinesAsync(database, "sin filtro");
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfPagedRepository<Vino>>();

        var result = repository.TryGetDataPagination(new PageRequest(1, 2));

        result.GetValueOrThrow().TotalCount.Should().Be(3);
        result.GetValueOrThrow().Items.Select(vino => vino.Id)
            .Should().Equal(wines.Take(2).Select(vino => vino.Id));
    }

    [Fact]
    public async Task TryGetDataPagination_KeylessEntity_ReturnsMissingPrimaryKeyFailure()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfPagedRepository<UnitOfWorkTransactionTests.KeylessVino>>();

        var result = repository.TryGetDataPagination(
            new PageRequest(1, 10), orderByField: vino => vino.Nombre);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().ToString().Should().Contain("has no primary key");
    }

    [Fact]
    public async Task TryGetDataPagination_CompositeKeyTies_OrdersEveryKeyPart()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var tag = $"Clave compuesta {Guid.NewGuid():N}";
        var rows = CreateCompositeRows(tag);
        database.Context.Set<UnitOfWorkTransactionTests.CompositeKeyVino>().AddRange(rows);
        await database.Context.SaveChangesAsync();
        var repository = database.Scope.ServiceProvider
            .GetRequiredService<IEfPagedRepository<UnitOfWorkTransactionTests.CompositeKeyVino>>();
        Expression<Func<UnitOfWorkTransactionTests.CompositeKeyVino, bool>> filter = row => row.Tag == tag;

        var firstPage = repository.TryGetDataPagination(new PageRequest(1, 2), filter: filter);
        var secondPage = repository.TryGetDataPagination(new PageRequest(2, 2), filter: filter);

        firstPage.GetValueOrThrow().Items.Concat(secondPage.GetValueOrThrow().Items)
            .Select(row => (row.BodegaId, row.VinoId))
            .Should().Equal((1, 1), (1, 2), (2, 1));
    }

    private static async Task<Vino[]> AddWinesAsync(
        UnitOfWorkTransactionTests.TestDatabase database,
        string prefix)
    {
        var wines = Enumerable.Range(0, 3)
            .Select(index => UnitOfWorkTransactionTests.CreateVino($"{prefix} {Guid.NewGuid():N} {index}"))
            .ToArray();
        database.Context.Vinos.AddRange(wines);
        await database.Context.SaveChangesAsync();
        return wines;
    }

    private static UnitOfWorkTransactionTests.CompositeKeyVino[] CreateCompositeRows(string tag)
        =>
        [
            new(2, 1, tag, 10),
            new(1, 2, tag, 10),
            new(1, 1, tag, 10)
        ];

}