using gestionAdminTECOCApi.Domain.Implementos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class ImplementoConfiguration
    : IEntityTypeConfiguration<Implemento> {
    public void Configure( EntityTypeBuilder<Implemento> builder ) {
        builder.ToTable( "Implementos" );
        builder.HasKey( property => property.Id );

        builder
            .Property( property => property.Nombre )
            .IsRequired()
            .HasMaxLength( 150 );

        builder
            .Property( property => property.Codigo )
            .IsRequired()
            .HasMaxLength( 50 );

        builder
            .Property( property => property.Descripcion )
            .HasMaxLength( 500 );

        builder.Property( property => property.CantidadTotal );

        builder.Property( property => property.CantidadDisponible );

        builder
            .Property( property => property.Estado )
            .IsRequired()
            .HasMaxLength( 40 );

        builder.Property( property => property.Enabled );

        builder
            .HasIndex( property => property.Codigo )
            .IsUnique();
    }
}
