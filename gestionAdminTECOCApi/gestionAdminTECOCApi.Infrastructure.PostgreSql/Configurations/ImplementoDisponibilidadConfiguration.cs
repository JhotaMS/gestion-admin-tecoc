using gestionAdminTECOCApi.Domain.Loans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class ImplementoDisponibilidadConfiguration
    : IEntityTypeConfiguration<Implemento> {
    public void Configure( EntityTypeBuilder<Implemento> builder ) {
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
    }
}
