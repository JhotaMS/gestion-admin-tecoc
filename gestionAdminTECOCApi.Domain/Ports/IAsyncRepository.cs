using System.Linq.Expressions;

namespace gestionAdminTECOCApi.Domain.Ports;

public interface IAsyncRepository<T> where T : class {
    public void Add( T entity );
    public Task AddAsync( T entity, CancellationToken cancellationToken = default );
    public Task AddAsync( IEnumerable<T> entity, CancellationToken cancellationToken = default );
    public void Delete( T entity );
    public Task DeleteAsync( T entity );
    public Task<bool> Exitst( Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default );
    public Task<IReadOnlyList<T>> GetAllAsync( CancellationToken cancellationToken = default );
    public Task<IReadOnlyList<T>> GetAllIgnoreQueryFiltersAsync( CancellationToken cancellationToken = default );
    public Task<IReadOnlyList<T>> GetAsync( Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken = default );
    public Task<IReadOnlyList<T>> GetAsync( Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeString = null, bool? disableTracking = true, CancellationToken cancellationToken = default );
    public Task<IReadOnlyList<T>> GetAsync( Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? includes = null, bool? disableTracking = true, CancellationToken cancellationToken = default );
    public Task<T> GetByAsync( Expression<Func<T, bool>>? predicate = null, bool? disableTracking = true, CancellationToken cancellationToken = default );
    public void Update( T entity );
    public Task UpdateAsync( T entity );
}