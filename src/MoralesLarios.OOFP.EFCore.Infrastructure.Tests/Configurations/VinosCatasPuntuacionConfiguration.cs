
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Configurations;

public class VinosCatasPuntuacionConfiguration : IEntityTypeConfiguration<VinosCatasPuntuacion>
{
    public void Configure(EntityTypeBuilder<VinosCatasPuntuacion> builder)
    {
        // Composite primary key
        builder.HasKey(vcp => new { vcp.IdVino, vcp.IdCata, vcp.IdUsuario });

        // Relationship with Identity User

    }
}