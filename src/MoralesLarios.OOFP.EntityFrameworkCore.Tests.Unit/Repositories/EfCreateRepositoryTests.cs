namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Unit.Repositories;

public class EfCreateRepositoryTests
{
    [Fact]
    public void TryAdd_NullEntity_ReturnsFailureWithValidationMessage()
    {
        using var context = TestDbContextFactory.Create();
        var repository = new EfCreateRepository<TestEntity, DbContext>(context);

        var result = repository.TryAdd(null!);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Errors.Single().Message
            .Should().Be("The entity item to add cannot be null");
    }

    [Fact]
    public async Task TryAddRange_NullEntities_ReturnsFailureWithValidationMessage()
    {
        using var context = TestDbContextFactory.Create();
        var repository = new EfCreateRepository<TestEntity, DbContext>(context);

        var result = await repository.TryAddRangeAsync(null!);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Errors.Single().Message
            .Should().Be("The entity items to addRange cannot be null");
    }
}