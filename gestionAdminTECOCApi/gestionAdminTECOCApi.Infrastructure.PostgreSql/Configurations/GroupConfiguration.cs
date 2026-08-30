using gestionAdminTECOCApi.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class GroupConfiguration
    : IEntityTypeConfiguration<Group> {
    public void Configure( EntityTypeBuilder<Group> builder ) {
        builder.ToTable( "Groups" );
        builder.HasKey( key => key.Id );

        builder
            .Property( property => property.Name )
            .HasMaxLength( 100 )
            .IsRequired( true );

        builder
            .Property( property => property.Code )
            .HasMaxLength( 30 )
            .IsRequired( true );

        builder
            .HasIndex( index => index.Code )
            .IsUnique();
    }
}
