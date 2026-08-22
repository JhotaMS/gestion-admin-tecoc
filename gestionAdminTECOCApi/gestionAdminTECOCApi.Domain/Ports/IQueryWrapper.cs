namespace gestionAdminTECOCApi.Domain.Ports;

public interface IQueryWrapper {
    public Task<IEnumerable<T>> QueryAsync<T>( string resourceItemDescription )
        where T : class;

    public Task<IEnumerable<T>> QueryAsync<T>( string resourceItemDescription, object parameters )
        where T : class;

    public Task<IEnumerable<T>> QueryAsync<T>( string resourceItemDescription, object parameters, object[]? args )
        where T : class;

    public Task<IEnumerable<dynamic>> QueryAsync( string resourceItemDescription, object[]? args );

    public Task<T> QuerySingleAsync<T>( string resourceItemDescription, object parameters );

    public Task<T> QuerySingleAsync<T>( string resourceItemDescription, object parameters, object[]? args );

    public Task ExecuteAsync( string resourceItemDescription, object parameters );

    public Task ExecuteAsync( string resourceItemDescription, object parameters, object[]? args );
}