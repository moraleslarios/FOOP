namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Unit.Repositories;

public class EfDeleteRepositoryTests
{
    [Fact]
    public void TryRemove_NullEntity_ReturnsFailureWithValidationMessage()
    {
        using var context = TestDbContextFactory.Create();
        var repository = new EfDeleteRepository<TestEntity, DbContext>(context);

        var result = repository.TryRemove((TestEntity)null!);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Errors.Single().Message
            .Should().Be("The entity item to remove cannot be null");
    }

    [Fact]
    public async Task TryRemoveRange_NullEntities_ReturnsFailureWithValidationMessage()
    {
        using var context = TestDbContextFactory.Create();
        var repository = new EfDeleteRepository<TestEntity, DbContext>(context);

        var result = await repository.TryRemoveRangeAsync(null!);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Errors.Single().Message
            .Should().Be("The entity items to remove cannot be null");
    }
}