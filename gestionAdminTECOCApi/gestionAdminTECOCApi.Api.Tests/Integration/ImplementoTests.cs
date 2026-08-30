using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Application.Features.Implementos.GetAllImplementos;
using gestionAdminTECOCApi.Domain.Implementos;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class ImplementoTests : IClassFixture<WebApplicationFactory<Program>> {
    private readonly WebApplicationFactory<Program> _factory;

    public ImplementoTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof( DbContextOptions<ApplicationDbContext> ) );
                if (descriptor is not null) services.Remove( descriptor );
                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-implemento-tests" ) );
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

    private void Seed( params Implemento[] implementos ) {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Implementos.AddRange( implementos );
        db.SaveChanges();
    }

    private HttpClient Client() => _factory.CreateClient();

    [Fact]
    public async Task Listar_implementos_retorna_200_con_los_implementos_registrados() {
        Seed( Implemento.Create( "MT-014", "Multímetro digital", "Multímetro digital de gama media", 8, 3, "Bueno", true ) );

        var response = await Client().GetAsync( "/api/v1/Implemento" );

        Assert.Equal( HttpStatusCode.OK, response.StatusCode );

        var body = await response.Content.ReadFromJsonAsync<GetAllImplementosResponse>();
        Assert.NotNull( body );

        var implemento = Assert.Single( body.Implementos );
        Assert.Equal( "MT-014", implemento.Codigo );
        Assert.Equal( "Multímetro digital", implemento.Nombre );
        Assert.Equal( "Multímetro digital de gama media", implemento.Descripcion );
        Assert.Equal( 8, implemento.CantidadTotal );
        Assert.Equal( 3, implemento.CantidadDisponible );
        Assert.Equal( "Bueno", implemento.Estado );
        Assert.True( implemento.Activo );
        Assert.NotEqual( Guid.Empty, implemento.ImplementoId );
    }

    [Fact]
    public async Task Listar_implementos_sin_registros_retorna_lista_vacia() {
        var body = await Client().GetFromJsonAsync<GetAllImplementosResponse>( "/api/v1/Implemento" );

        Assert.NotNull( body );
        Assert.Empty( body.Implementos );
    }

    [Fact]
    public async Task Listar_implementos_los_devuelve_ordenados_por_nombre() {
        Seed(
            Implemento.Create( "TL-002", "Taladro inalámbrico", "Taladro con batería de repuesto", 4, 1, "Bueno", true ),
            Implemento.Create( "CM-017", "Cámara réflex", "Cámara con lente 18-55", 3, 3, "Regular", true ),
            Implemento.Create( "PR-005", "Proyector portátil", "Proyector HDMI para aulas", 6, 5, "Bueno", true ) );

        var body = await Client().GetFromJsonAsync<GetAllImplementosResponse>( "/api/v1/Implemento" );

        Assert.NotNull( body );
        Assert.Equal(
            new[] { "Cámara réflex", "Proyector portátil", "Taladro inalámbrico" },
            body.Implementos.Select( i => i.Nombre ) );
    }

    [Fact]
    public async Task Listar_implementos_incluye_los_marcados_como_no_activos() {
        Seed( Implemento.Create( "MC-006", "Microscopio óptico", "Microscopio dado de baja", 5, 0, "Dañado", false ) );

        var body = await Client().GetFromJsonAsync<GetAllImplementosResponse>( "/api/v1/Implemento" );

        Assert.NotNull( body );
        var implemento = Assert.Single( body.Implementos );
        Assert.False( implemento.Activo );
    }
}
