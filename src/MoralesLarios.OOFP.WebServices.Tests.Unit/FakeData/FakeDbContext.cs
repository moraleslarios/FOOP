using Microsoft.EntityFrameworkCore;

namespace MoralesLarios.OOFP.WebServices.Tests.Unit.FakeData;
public class FakeDbContext : DbContext
{

    public DbSet<MyTable> MyTables { get; set; } = null!;


}
