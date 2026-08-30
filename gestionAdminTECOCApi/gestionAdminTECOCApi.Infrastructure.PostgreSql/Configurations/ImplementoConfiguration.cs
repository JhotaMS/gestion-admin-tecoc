using gestionAdminTECOCApi.Domain.Implementos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class ImplementoConfiguration
    : IEntityTypeConfiguration<Implemento> {
    public void Configure( EntityTypeBuilder<Implemento> builder ) {
        builder.ToTable( "Implementos" );
        builder.HasKey( key => key.Id );

        builder
            .Property( property => property.Codigo )
            .HasMaxLength( 20 )
            .IsRequired( true );

        builder
            .Property( property => property.Nombre )
            .HasMaxLength( 100 )
            .IsRequired( true );

        builder
            .Property( property => property.Descripcion )
            .HasMaxLength( 250 )
            .IsRequired( true );

        builder
            .Property( property => property.CantidadTotal )
            .IsRequired( true );

        builder
            .Property( property => property.CantidadDisponible )
            .IsRequired( true );

        builder
            .Property( property => property.Estado )
            .HasMaxLength( 50 )
            .IsRequired( true );

        builder
            .HasIndex( index => index.Codigo )
            .IsUnique();
    }
}
