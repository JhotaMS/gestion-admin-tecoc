using gestionAdminTECOCApi.Domain.CalendarioAcademico;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class EventoAcademicoConfiguration
    : IEntityTypeConfiguration<EventoAcademico> {
    public void Configure( EntityTypeBuilder<EventoAcademico> builder ) {
        builder.ToTable( "EventosAcademicos" );
        builder.HasKey( key => key.Id );

        builder
            .Property( property => property.Titulo )
            .HasMaxLength( 150 )
            .IsRequired( true );

        builder
            .Property( property => property.Descripcion )
            .HasMaxLength( 500 )
            .IsRequired( false );

        builder
            .Property( property => property.FechaInicio )
            .IsRequired( true );

        builder
            .Property( property => property.FechaFin )
            .IsRequired( false );

        builder
            .HasIndex( index => index.FechaInicio );
    }
}
