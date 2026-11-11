namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Unit;

internal static class TestDbContextFactory
{
    public static DbContext Create()
        => new(new DbContextOptionsBuilder<DbContext>().Options);
}