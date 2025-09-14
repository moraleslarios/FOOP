
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Configurations;

public class VinosCataConfiguration : IEntityTypeConfiguration<VinosCata>
{
    public void Configure(EntityTypeBuilder<VinosCata> builder)
    {
        // Composite primary key
        builder.HasKey(vc => new { vc.IdVino, vc.IdCata });
    }
}