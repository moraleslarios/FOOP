using MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Configurations;

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests;

public class JfCatasDbContext : DbContext
{
    public JfCatasDbContext(DbContextOptions<JfCatasDbContext> options) : base(options)
    {
    }

    // DbSets for the entities
    public DbSet<Vino> Vinos { get; set; }
    public DbSet<Cata> Catas { get; set; }
    public DbSet<VinosCata> VinosCatas { get; set; }
    public DbSet<VinosCatasPuntuacion> VinosCatasPuntuaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Call base method to configure Identity tables
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new VinoConfiguration());
        modelBuilder.ApplyConfiguration(new CataConfiguration());
        modelBuilder.ApplyConfiguration(new VinosCataConfiguration());
        modelBuilder.ApplyConfiguration(new VinosCatasPuntuacionConfiguration());

        // Configure Identity table names to use Pascal case
        //modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers");
        //modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
        //modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
        //modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
        //modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
        //modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");
        //modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
    }
}