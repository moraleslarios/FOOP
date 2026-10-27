// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MoralesLarios.OOFP.EntityFrameworkCore.TestSupport.Configurations;

public class VinosCataConfiguration : IEntityTypeConfiguration<VinosCata>
{
    public void Configure(EntityTypeBuilder<VinosCata> builder)
    {
        // Composite primary key
        builder.HasKey(vc => new { vc.IdVino, vc.IdCata });
    }
}
