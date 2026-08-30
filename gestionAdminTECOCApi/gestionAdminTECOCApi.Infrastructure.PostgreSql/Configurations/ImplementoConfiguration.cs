using gestionAdminTECOCApi.Domain.Loans;
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
            .HasMaxLength( 30 )
            .IsRequired( true );

        builder
            .Property( property => property.Nombre )
            .HasMaxLength( 150 )
            .IsRequired( true );

        builder
            .Property( property => property.Descripcion )
            .HasMaxLength( 500 )
            .IsRequired( false );

        builder.Property( property => property.Enabled );

        builder
            .HasIndex( index => index.Codigo )
            .IsUnique();
    }
}

