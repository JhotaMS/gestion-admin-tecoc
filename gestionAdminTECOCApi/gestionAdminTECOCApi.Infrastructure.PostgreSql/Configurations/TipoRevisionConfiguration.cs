using gestionAdminTECOCApi.Domain.Loans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class TipoRevisionConfiguration
    : IEntityTypeConfiguration<TipoRevision> {
    public void Configure( EntityTypeBuilder<TipoRevision> builder ) {
        builder.ToTable( "TiposRevision" );
        builder.HasKey( key => key.Id );

        builder
            .Property( property => property.Id )
            .ValueGeneratedNever();

        builder
            .Property( property => property.Nombre )
            .HasMaxLength( 50 )
            .IsRequired( true );

        builder
            .Property( property => property.Descripcion )
            .HasMaxLength( 200 )
            .IsRequired( false );

        builder.Property( property => property.Enabled );

        builder.HasData(
            TipoRevision.Create( 1, "Inicio Préstamo", "Revisión al inicio del préstamo del implemento" ),
            TipoRevision.Create( 2, "Fin Préstamo", "Revisión al finalizar el préstamo del implemento" )
        );
    }
}

