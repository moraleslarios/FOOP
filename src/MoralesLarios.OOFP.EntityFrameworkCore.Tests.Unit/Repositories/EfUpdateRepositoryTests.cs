namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Unit.Repositories;

public class EfUpdateRepositoryTests
{
    [Fact]
    public void TryUpdate_NullEntity_ReturnsFailureWithValidationMessage()
    {
        using var context = TestDbContextFactory.Create();
        var repository = new EfUpdateRepository<TestEntity, DbContext>(context);

        var result = repository.TryUpdate(null!);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Errors.Single().Message
            .Should().Be("The entity item to update cannot be null");
    }

    [Fact]
    public async Task TryUpdateRange_NullEntities_ReturnsFailureWithValidationMessage()
    {
        using var context = TestDbContextFactory.Create();
        var repository = new EfUpdateRepository<TestEntity, DbContext>(context);

        var result = await repository.TryUpdateRangeAsync(null!);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Errors.Single().Message
            .Should().Be("The entity items to update cannot be null");
    }
}