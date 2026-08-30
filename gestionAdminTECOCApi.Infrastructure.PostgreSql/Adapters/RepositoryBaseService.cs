using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Adapters;

internal sealed record class RepositoryBaseService<T>
    : IAsyncRepository<T> where T : DomainEntity {
    private readonly ApplicationDbContext _context;

    public RepositoryBaseService(
        ApplicationDbContext context
    ) {
        _context = context ?? throw new ArgumentNullException( nameof( context ) );
    }

    public void Add(
        T entity
    ) {
        _context
            .Set<T>()
            .Add( entity );
    }

    public async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default
    ) {
        await _context
            .Set<T>()
            .AddAsync( entity, cancellationToken );
    }

    public async Task AddAsync(
        IEnumerable<T> entity,
        CancellationToken cancellationToken = default
    ) {
        await _context
            .Set<T>()
            .AddRangeAsync( entity, cancellationToken );
    }

    public void Delete( T entity ) {
        _context
            .Set<T>()
            .Remove( entity );
    }

    public async Task DeleteAsync( T entity ) {
        _context
            .Set<T>()
            .Remove( entity );

        await Task.CompletedTask;
    }

    public async Task<bool> Exitst(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default
    ) {
        var result = await _context.Set<T>()
            .AnyAsync( predicate, cancellationToken );

        return result;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken cancellationToken = default
    ) {
        return await _context.Set<T>()
            .ToListAsync( cancellationToken );
    }

    public async Task<IReadOnlyList<T>> GetAllIgnoreQueryFiltersAsync(
            CancellationToken cancellationToken = default
    ) {
        return await _context.Set<T>()
            .IgnoreQueryFilters()
            .ToListAsync( cancellationToken );
    }

    public async Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>>? predicate,
        CancellationToken cancellationToken = default
    ) {
        return await _context
            .Set<T>()
            .Where( predicate! )
            .ToListAsync( cancellationToken );
    }

    public async Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeString = null,
        bool? disableTracking = true,
        CancellationToken cancellationToken = default
    ) {
        IQueryable<T> query = _context.Set<T>();

        if ((bool)disableTracking!)
            query = query.AsNoTracking();

        if (!string.IsNullOrWhiteSpace( includeString ))
            query = query.Include( includeString );

        if (predicate != null)
            query = query.Where( predicate );

        if (orderBy != null)
            return await orderBy( query ).ToListAsync( cancellationToken );

        return await query.ToListAsync( cancellationToken );
    }

    public async Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<Expression<Func<T, object>>>? includes = null,
        bool? disableTracking = true,
        CancellationToken cancellationToken = default
    ) {
        IQueryable<T> query = _context.Set<T>();

        if ((bool)disableTracking!)
            query = query.AsNoTracking();

        if (includes != null)
            query = includes.Aggregate( query, ( current, include ) => current.Include( include ) );

        if (predicate != null)
            query = query.Where( predicate );

        if (orderBy != null)
            return await orderBy( query ).ToListAsync( cancellationToken );

        return await query.ToListAsync( cancellationToken );
    }

    public async Task<T> GetByAsync(
        Expression<Func<T, bool>>? predicate = null,
        bool? disableTracking = true,
        CancellationToken cancellationToken = default
    ) {
        IQueryable<T> query = _context.Set<T>();

        if ((bool)disableTracking!)
            query = query.AsNoTracking();

        if (predicate != null)
            query = query.Where( predicate );

        return await query.FirstOrDefaultAsync( cancellationToken ) ?? default!;
    }

    public void Update( T entity ) {
        _context.Set<T>()
            .Attach( entity );

        _context.Entry( entity )
            .State = EntityState.Modified;
    }

    public async Task UpdateAsync( T entity ) {
        _context.Set<T>().Update( entity );
        _context.Entry( entity ).State = EntityState.Modified;
        await Task.CompletedTask;
    }
}