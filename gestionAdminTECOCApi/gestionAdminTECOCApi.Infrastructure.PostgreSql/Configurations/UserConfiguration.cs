using gestionAdminTECOCApi.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class UserConfiguration
    : IEntityTypeConfiguration<User> {
    public void Configure( EntityTypeBuilder<User> builder ) {
        builder.ToTable( "Users" );
        builder.HasKey( key => key.Id );

        builder
            .Property( property => property.FullName )
            .HasMaxLength( 150 )
            .IsRequired( true );

        builder
            .Property( property => property.DocumentType )
            .HasConversion<string>()
            .HasMaxLength( 40 )
            .IsRequired( true );

        builder
            .Property( property => property.DocumentNumber )
            .HasMaxLength( 15 )
            .IsRequired( true );

        builder
            .Property( property => property.Position )
            .HasMaxLength( 100 )
            .IsRequired( true );

        builder.Property( property => property.Enabled );

        builder
            .HasIndex( index => new { index.DocumentType, index.DocumentNumber } )
            .IsUnique();
    }
}
