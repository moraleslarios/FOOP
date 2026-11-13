using System.Linq.Expressions;
using MoralesLarios.OOFP.EntityFrameworkCore.Enums;
using MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration.Repositories;

public class EfReadRepositoryIntegrationTests
{
    [Fact]
    public async Task TryFind_NullNotFoundDetails_UsesKeyFallback()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfReadRepository<Vino>>();

        var result = repository.TryFind(null!, int.MinValue);

        AssertNotFoundFallback(result, "-2147483648 values not found data in Vino");
    }

    [Fact]
    public async Task TryFindAsync_NullNotFoundDetails_UsesKeyFallback()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfReadRepository<Vino>>();

        var result = await repository.TryFindAsync(null!, default, int.MinValue);

        AssertNotFoundFallback(result, "-2147483648 values not found data in Vino");
    }

    [Fact]
    public async Task TryFirst_NullNotFoundDetails_UsesQueryFallback()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfReadRepository<Vino>>();

        var result = repository.TryFirst(vino => vino.Nombre == "no encontrado", null!);

        AssertNotFoundFallback(result, "The query did not return any elements");
    }

    [Fact]
    public async Task TryFirstAsync_NullNotFoundDetails_UsesQueryFallback()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfReadRepository<Vino>>();

        var result = await repository.TryFirstAsync(vino => vino.Nombre == "no encontrado", null!);

        AssertNotFoundFallback(result, "The query did not return any elements");
    }

    [Fact]
    public async Task TryGetLast_NullNotFoundDetails_UsesQueryFallback()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfReadRepository<Vino>>();

        var result = repository.TryGetLast(vino => vino.Nombre == "no encontrado", null!);

        AssertNotFoundFallback(result, "The query did not return any elements");
    }

    [Fact]
    public async Task TryGetLastAsync_NullNotFoundDetails_UsesQueryFallback()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfReadRepository<Vino>>();

        var result = await repository.TryGetLastAsync(
            vino => vino.Nombre == "no encontrado",
            notFoundErrorDetails: null!);

        AssertNotFoundFallback(result, "The query did not return any elements");
    }

    [Fact]
    public async Task TryGetLastBy_NullNotFoundDetails_UsesQueryFallback()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfOrderedReadRepository<Vino>>();

        var result = repository.TryGetLastBy(
            vino => vino.Nombre == "no encontrado",
            vino => vino.Año,
            null!);

        AssertNotFoundFallback(result, "The query did not return any elements");
    }

    [Fact]
    public async Task TryGetLastByAsync_NullNotFoundDetails_UsesQueryFallback()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfOrderedReadRepository<Vino>>();

        var result = await repository.TryGetLastByAsync(
            vino => vino.Nombre == "no encontrado",
            vino => vino.Año,
            null!);

        AssertNotFoundFallback(result, "The query did not return any elements");
    }

    [Fact]
    public async Task TryGetLastBy_TiedSelector_ReturnsLastPrimaryKey()
    {
        var interceptor = new UnitOfWorkTransactionTests.CommandCaptureInterceptor();
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync(commandInterceptor: interceptor);
        var wines = await AddWinesAsync(database, "last sync");
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfOrderedReadRepository<Vino>>();

        var result = repository.TryGetLastBy(
            vino => vino.Nombre.StartsWith("last sync"), vino => vino.Año, SortDirection.Ascending);

        result.GetValueOrThrow().Id.Should().Be(wines.Max(vino => vino.Id));
        interceptor.Commands.Last().Should().Contain("LIMIT 1");
    }

    [Fact]
    public async Task TryGetLastByAsync_TiedSelector_ReturnsLastPrimaryKey()
    {
        var interceptor = new UnitOfWorkTransactionTests.CommandCaptureInterceptor();
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync(commandInterceptor: interceptor);
        var wines = await AddWinesAsync(database, "last async");
        interceptor.Commands.Clear();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfOrderedReadRepository<Vino>>();

        var result = await repository.TryGetLastByAsync(
            vino => vino.Nombre.StartsWith("last async"), vino => vino.Año, SortDirection.Ascending);

        result.GetValueOrThrow().Id.Should().Be(wines.Max(vino => vino.Id));
        interceptor.Commands.Last().Should().Contain("LIMIT 1");
    }

    [Fact]
    public async Task TryGetLastByAsync_DescendingSelector_ReturnsFirstPrimaryKey()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var wines = await AddWinesAsync(database, "last descending");
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfOrderedReadRepository<Vino>>();

        var result = await repository.TryGetLastByAsync(
            vino => vino.Nombre.StartsWith("last descending"), vino => vino.Año, SortDirection.Descending);

        result.GetValueOrThrow().Id.Should().Be(wines.Min(vino => vino.Id));
    }


    [Fact]
    public async Task TryGetLast_SynchronousQueryHasNoLimitClause()
    {
        var interceptor = new UnitOfWorkTransactionTests.CommandCaptureInterceptor();
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync(
            commandInterceptor: interceptor);
        var prefix = $"legacy last {Guid.NewGuid():N}";
        await AddWinesAsync(database, prefix);
        interceptor.Commands.Clear();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfReadRepository<Vino>>();

        var result = repository.TryGetLast(vino => vino.Nombre.StartsWith(prefix));

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Nombre.Should().StartWith(prefix);
        var select = interceptor.Commands.Should().ContainSingle().Which;
        select.Should().Contain("SELECT");
        select.Should().NotContain("LIMIT");
    }


    [Fact]
    public async Task TryGetLastAsync_QueryHasNoLimitClause()
    {
        var interceptor = new UnitOfWorkTransactionTests.CommandCaptureInterceptor();
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync(
            commandInterceptor: interceptor);
        var prefix = $"legacy async last {Guid.NewGuid():N}";
        await AddWinesAsync(database, prefix);
        interceptor.Commands.Clear();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfReadRepository<Vino>>();

        var result = await repository.TryGetLastAsync(vino => vino.Nombre.StartsWith(prefix));

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Nombre.Should().StartWith(prefix);
        var select = interceptor.Commands.Should().ContainSingle().Which;
        select.Should().Contain("SELECT");
        select.Should().NotContain("LIMIT");
    }

    [Fact]
    public async Task TryGetLastByAsync_CanceledToken_ReturnsFailure()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var repository = database.Scope.ServiceProvider.GetRequiredService<IEfOrderedReadRepository<Vino>>();

        var result = await repository.TryGetLastByAsync(
            vino => vino.Nombre == "not found", vino => vino.Año,
            token: new CancellationToken(canceled: true));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task TryGetLastByAsync_CompositeKeyTies_UsesEveryKeyPart()
    {
        await using var database = await UnitOfWorkTransactionTests.TestDatabase.CreateAsync();
        var tag = $"Último clave compuesta {Guid.NewGuid():N}";
        var rows = CreateCompositeRows(tag);
        database.Context.Set<UnitOfWorkTransactionTests.CompositeKeyVino>().AddRange(rows);
        await database.Context.SaveChangesAsync();
        var repository = database.Scope.ServiceProvider
            .GetRequiredService<IEfOrderedReadRepository<UnitOfWorkTransactionTests.CompositeKeyVino>>();
        Expression<Func<UnitOfWorkTransactionTests.CompositeKeyVino, bool>> filter = row => row.Tag == tag;

        var result = await repository.TryGetLastByAsync(filter, row => row.SortValue);

        result.GetValueOrThrow().Should().BeEquivalentTo(rows[0]);
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

    private static void AssertNotFoundFallback(MlResult<Vino> result, string expectedMessage)
    {
        result.IsFailure.Should().BeTrue();
        result.TryGetErrors(out var errorsDetails).Should().BeTrue();
        errorsDetails.Should().NotBeNull();
        errorsDetails.Errors.Should().ContainSingle()
            .Which.Message.Should().Be(expectedMessage);
    }
}