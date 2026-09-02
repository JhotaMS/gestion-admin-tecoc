using gestionAdminTECOCApi.Domain.ProgramasAcademicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Configurations;

internal sealed class ProgramaAcademicoConfiguration
    : IEntityTypeConfiguration<ProgramaAcademico> {
    public void Configure( EntityTypeBuilder<ProgramaAcademico> builder ) {
        builder.ToTable( "ProgramasAcademicos" );
        builder.HasKey( key => key.Id );

        builder
            .Property( property => property.Name )
            .HasMaxLength( 150 )
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
