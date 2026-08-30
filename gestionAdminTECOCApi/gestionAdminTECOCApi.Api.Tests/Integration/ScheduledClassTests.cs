using System.Net;
using System.Text;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class ScheduledClassTests : IClassFixture<WebApplicationFactory<Program>> {
    private const string Url = "/api/v1/ScheduledClass";

    private readonly WebApplicationFactory<Program> _factory;

    public ScheduledClassTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof( DbContextOptions<ApplicationDbContext> ) );
                if (descriptor is not null) services.Remove( descriptor );
                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-scheduledclass-tests" ) );
            } );
        } );

        Reset();
    }

    private void Reset() {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    private HttpClient Client() => _factory.CreateClient();

    private static StringContent Json( string body )
        => new( body, Encoding.UTF8, "application/json" );

    [Fact]
    public async Task Programar_clase_sin_cuerpo_retorna_badrequest() {
        var response = await Client().PostAsync( Url, Json( string.Empty ) );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }

    [Fact]
    public async Task Programar_clase_con_cuerpo_nulo_retorna_badrequest() {
        var response = await Client().PostAsync( Url, Json( "null" ) );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }

    [Fact]
    public async Task Programar_clase_con_campos_vacios_retorna_badrequest() {
        var response = await Client().PostAsync( Url, Json( "{}" ) );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }

    [Fact]
    public async Task Programar_clase_valida_retorna_created() {
        var response = await Client().PostAsync( Url, Json( """
            {
              "scheduledDate": "2026-09-01",
              "scheduledTime": "14:30",
              "topic": "Ecuaciones diferenciales de primer orden",
              "courseLevel": "Unidad 3"
            }
            """ ) );

        Assert.Equal( HttpStatusCode.Created, response.StatusCode );
    }
}
