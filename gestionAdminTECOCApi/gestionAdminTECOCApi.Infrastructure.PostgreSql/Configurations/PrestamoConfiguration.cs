using gestionAdminTECOCApi.Domain.Prestamos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class PrestamoConfiguration
    : IEntityTypeConfiguration<Prestamo> {
    public void Configure( EntityTypeBuilder<Prestamo> builder ) {
        builder.ToTable( "Prestamos" );
        builder.HasKey( key => key.Id );
    }
}
