using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using System.Collections;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Adapters;

internal sealed record class UnitOfWorkService : IUnitOfWork {
    private Hashtable? _repositories;

    public UnitOfWorkService( ApplicationDbContext context ) => DbContext = context;

    public ApplicationDbContext DbContext { get; }

    public async ValueTask<int> SaveChangesAsync() {
        var test = await DbContext.SaveChangesAsync( new CancellationToken() );
        return test;
    }

    public void Dispose() {
        DbContext.Dispose();
    }

    public IAsyncRepository<TEntity> Repository<TEntity>() where TEntity : class {
        _repositories ??= [];
        var type = typeof( TEntity ).Name;

        if (!_repositories.ContainsKey( type )) {
            Type reporitoryType = typeof( RepositoryBaseService<> );

            var repositoryInstance = Activator
                .CreateInstance( reporitoryType.MakeGenericType( typeof( TEntity ) ), DbContext );

            _repositories.Add( type, repositoryInstance );
        }

        return (IAsyncRepository<TEntity>)_repositories[type]!;
    }
}
