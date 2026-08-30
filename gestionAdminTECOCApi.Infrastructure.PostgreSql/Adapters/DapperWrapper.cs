using Dapper;
using gestionAdminTECOCApi.Domain.Constants;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Exceptions;
using Npgsql;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Adapters;

internal class DapperWrapper : IQueryWrapper {
    private readonly IDbConnection _connection;
    private readonly ComponentResourceManager _componentResourceManager;

    public DapperWrapper( IDbConnection connection ) {
        _connection = connection;
        _componentResourceManager = new( typeof( SqlQueriesConstants ) );
    }

    public async Task<IEnumerable<T>> QueryAsync<T>( string resourceItemDescription )
        where T : class {
        string query = GetQuery( resourceItemDescription );

        return await _connection.QueryAsync<T>( query );
    }

    public async Task<IEnumerable<T>> QueryAsync<T>( string resourceItemDescription, object parameters )
        where T : class {
        string query = GetQuery( resourceItemDescription );

        return await _connection.QueryAsync<T>( query, parameters );
    }

    public async Task<IEnumerable<T>> QueryAsync<T>( string resourceItemDescription, object parameters, object[]? args )
        where T : class {
        string query = GetQuery( resourceItemDescription, args );

        return await _connection.QueryAsync<T>( query, parameters );
    }

    public Task<IEnumerable<dynamic>> QueryAsync( string resourceItemDescription, object[]? args ) {
        string query = GetQuery( resourceItemDescription, args );

        return _connection.QueryAsync( query );
    }

    public async Task<T> QuerySingleAsync<T>( string resourceItemDescription, object parameters ) {
        string query = GetQuery( resourceItemDescription );

        return await _connection.QuerySingleOrDefaultAsync<T>( query, parameters ) ?? default!;
    }

    public async Task<T> QuerySingleAsync<T>( string resourceItemDescription, object parameters, object[]? args ) {
        try {
            string query = GetQuery( resourceItemDescription, args );

            return await _connection.QuerySingleOrDefaultAsync<T>( query, parameters ) ?? default!;
        }
        catch (PostgresException ex) {
            throw new TimeoutErrorException( ex.Message, ex.InnerException! );
        }
    }

    public async Task ExecuteAsync( string resourceItemDescription, object parameters ) {
        string query = GetQuery( resourceItemDescription );

        await _connection.ExecuteAsync( query, parameters );
    }

    public async Task ExecuteAsync( string resourceItemDescription, object parameters, object[]? args ) {
        string query = GetQuery( resourceItemDescription, args );

        await _connection.ExecuteAsync( query, parameters );
    }

    private string GetQuery( string resourceItemDescription, object[]? args = null ) {
        if (args is null) {
            return string.Format(
                CultureInfo.InvariantCulture,
                _componentResourceManager.GetString( resourceItemDescription )!
            );
        }

        return string.Format(
            CultureInfo.InvariantCulture,
            _componentResourceManager.GetString( resourceItemDescription )!,
            args!
        );
    }
}