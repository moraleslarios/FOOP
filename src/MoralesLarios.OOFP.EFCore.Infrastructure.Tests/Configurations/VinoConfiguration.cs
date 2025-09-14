
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Configurations;

public class VinoConfiguration : IEntityTypeConfiguration<Vino>
{
    public void Configure(EntityTypeBuilder<Vino> builder)
    {
        // Unique index for Nombre property
        builder.HasIndex(v => v.Nombre)
            .IsUnique()
            .HasDatabaseName("IX_VINOS_Nombre_Unique");

        // Relationships
        builder.HasMany(v => v.VinosCatas)
            .WithOne(vc => vc.Vino)
            .HasForeignKey(vc => vc.IdVino)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.VinosCatasPuntuaciones)
            .WithOne(vcp => vcp.Vino)
            .HasForeignKey(vcp => vcp.IdVino)
            .OnDelete(DeleteBehavior.Cascade);
    }
}