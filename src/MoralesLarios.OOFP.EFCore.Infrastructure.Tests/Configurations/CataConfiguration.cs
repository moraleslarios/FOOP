using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Configurations;

public class CataConfiguration : IEntityTypeConfiguration<Cata>
{
    public void Configure(EntityTypeBuilder<Cata> builder)
    {
        // Unique index for Nombre property
        builder.HasIndex(c => c.Nombre)
            .IsUnique()
            .HasDatabaseName("IX_CATAS_Nombre_Unique");

        // Relationships
        builder.HasMany(c => c.VinosCatas)
            .WithOne(vc => vc.Cata)
            .HasForeignKey(vc => vc.IdCata)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.VinosCatasPuntuaciones)
            .WithOne(vcp => vcp.Cata)
            .HasForeignKey(vcp => vcp.IdCata)
            .OnDelete(DeleteBehavior.Cascade);
    }
}