using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace gestionAdminTECOCApi.Api;

public class PersistenceContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext> {
    public ApplicationDbContext CreateDbContext( string[] args ) {
        IConfigurationRoot Config = new ConfigurationBuilder()
           .SetBasePath( Directory.GetCurrentDirectory() )
           .AddJsonFile( "appsettings.json" )
           .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        string database = Config.GetConnectionString( "ConnectionString" ) ?? string.Empty;
        optionsBuilder.UseNpgsql( database, sqlopts => {
            sqlopts.MigrationsHistoryTable( "_MigrationHistory" );
        } );

        return new ApplicationDbContext( optionsBuilder.Options, Config );
    }
}
