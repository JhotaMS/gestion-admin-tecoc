using gestionAdminTECOCApi.Domain.Loans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class ImplementoPrestadoConfiguration
    : IEntityTypeConfiguration<ImplementoPrestado> {
    public void Configure( EntityTypeBuilder<ImplementoPrestado> builder ) {
        builder.ToTable( "ImplementosPrestados" );
        builder.HasKey( key => key.Id );

        builder
            .Property( property => property.UserId )
            .IsRequired( true );

        builder
            .Property( property => property.ImplementoId )
            .IsRequired( true );

        builder
            .Property( property => property.TipoRevisionId )
            .IsRequired( true );

        builder
            .Property( property => property.EstadoTipo )
            .HasConversion<string>()
            .HasMaxLength( 20 )
            .IsRequired( true );

        builder
            .Property( property => property.FechaInicio )
            .IsRequired( true );

        builder
            .Property( property => property.FechaFin )
            .IsRequired( true );

        builder
            .Property( property => property.Observacion )
            .HasMaxLength( 500 )
            .IsRequired( false );

        builder.Property( property => property.Enabled );

        builder
            .HasIndex( index => index.UserId );

        builder
            .HasIndex( index => index.ImplementoId );

        builder
            .HasIndex( index => index.FechaInicio );
    }
}

