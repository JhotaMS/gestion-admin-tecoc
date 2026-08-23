using gestionAdminTECOCApi.Domain.DocumentTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class DocumentTypeEntityConfiguration
    : IEntityTypeConfiguration<DocumentTypeEntity> {
    public void Configure( EntityTypeBuilder<DocumentTypeEntity> builder ) {
        builder.ToTable( "DocumentTypes" );
        builder.HasKey( key => key.Id );

        builder
            .Property( property => property.Code )
            .HasMaxLength( 20 )
            .IsRequired( true );

        builder
            .Property( property => property.Description )
            .HasMaxLength( 100 )
            .IsRequired( true );

        builder
            .HasIndex( index => index.Code )
            .IsUnique();
    }
}
