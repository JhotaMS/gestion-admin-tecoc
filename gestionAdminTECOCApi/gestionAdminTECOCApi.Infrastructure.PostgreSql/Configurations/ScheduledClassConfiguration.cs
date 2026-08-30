using gestionAdminTECOCApi.Domain.ScheduledClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class ScheduledClassConfiguration
    : IEntityTypeConfiguration<ScheduledClass> {
    public void Configure( EntityTypeBuilder<ScheduledClass> builder ) {
        builder.ToTable( "ScheduledClasses" );
        builder.HasKey( key => key.Id );

        builder
            .Property( property => property.ScheduledDate )
            .IsRequired( true );

        builder
            .Property( property => property.ScheduledTime )
            .IsRequired( true );

        builder
            .Property( property => property.Topic )
            .HasMaxLength( 200 )
            .IsRequired( true );

        builder
            .Property( property => property.CourseLevel )
            .HasMaxLength( 100 )
            .IsRequired( true );

        builder.Property( property => property.Enabled );

        builder
            .HasIndex( index => new { index.ScheduledDate, index.ScheduledTime } )
            .IsUnique();
    }
}
