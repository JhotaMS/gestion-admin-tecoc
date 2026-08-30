using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Extensions;
using Newtonsoft.Json;

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Adapters;

internal sealed class JsonConfiguration : IJsonConfiguration {
    private readonly JsonSerializerSettings _settings;
    public JsonConfiguration() {
        _settings = new JsonSerializerSettings {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            ,
            ContractResolver = new PrivateResolver()
        };
    }
    public TEntity DeserializeObject<TEntity>( string source ) =>
        JsonConvert.DeserializeObject<TEntity>( source, _settings )!;
    public string SerializeObject<TEntity>( TEntity source ) =>
        JsonConvert.SerializeObject( source, _settings );
}
