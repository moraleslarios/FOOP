namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Unit.Repositories;

public class EfRepositoryTests
{
    [Fact]
    public void GetContext_ReturnsTheContextProvidedToTheRepository()
    {
        using var context = TestDbContextFactory.Create();
        var repository = new EfRepository(context);

        repository.GetContext().Should().BeSameAs(context);
        repository.Should().NotBeAssignableTo<IDisposable>();
    }
}