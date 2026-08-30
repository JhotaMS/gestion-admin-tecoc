using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Application.Features.Implementos.GetImplementosDisponibles;
using gestionAdminTECOCApi.Domain.Implementos;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class ImplementosDisponiblesTests : IClassFixture<WebApplicationFactory<Program>> {
    private const string Url = "/api/implementos/disponibles";

    private readonly WebApplicationFactory<Program> _factory;

    public ImplementosDisponiblesTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof( DbContextOptions<ApplicationDbContext> ) );
                if (descriptor is not null) services.Remove( descriptor );
                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-implementos-disponibles-tests" ) );
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
    public async Task CA03_retorna_la_informacion_del_implemento_disponible() {
        Seed( Implemento.Create( "MT-014", "Multímetro digital", "Multímetro digital de gama media", 8, 3, "Bueno", true ) );

        var response = await Client().GetAsync( Url );

        Assert.Equal( HttpStatusCode.OK, response.StatusCode );

        var body = await response.Content.ReadFromJsonAsync<GetImplementosDisponiblesResponse>();
        Assert.NotNull( body );

        var implemento = Assert.Single( body.Implementos );
        Assert.NotEqual( Guid.Empty, implemento.Id );
        Assert.Equal( "Multímetro digital", implemento.Nombre );
        Assert.Equal( "MT-014", implemento.Codigo );
        Assert.Equal( "Multímetro digital de gama media", implemento.Descripcion );
        Assert.Equal( 8, implemento.CantidadTotal );
        Assert.Equal( 3, implemento.CantidadDisponible );
        Assert.Equal( "Bueno", implemento.Estado );
        Assert.Null( body.Mensaje );
    }

    [Fact]
    public async Task CA04_no_incluye_los_implementos_sin_unidades_disponibles() {
        Seed(
            Implemento.Create( "MC-006", "Microscopio óptico", "Microscopio de laboratorio", 5, 0, "Bueno", true ),
            Implemento.Create( "PR-005", "Proyector portátil", "Proyector HDMI para aulas", 6, 5, "Bueno", true ) );

        var body = await Client().GetFromJsonAsync<GetImplementosDisponiblesResponse>( Url );

        Assert.NotNull( body );
        var implemento = Assert.Single( body.Implementos );
        Assert.Equal( "PR-005", implemento.Codigo );
    }

    [Fact]
    public async Task CA05_no_retorna_los_implementos_inactivos_aunque_tengan_unidades() {
        Seed( Implemento.Create( "TL-002", "Taladro inalámbrico", "Taladro con batería de repuesto", 4, 4, "Bueno", false ) );

        var body = await Client().GetFromJsonAsync<GetImplementosDisponiblesResponse>( Url );

        Assert.NotNull( body );
        Assert.Empty( body.Implementos );
    }

    [Fact]
    public async Task CA06_sin_resultados_retorna_lista_vacia_e_informa() {
        Seed( Implemento.Create( "KD-031", "Kit de disección", "Kit completo de disección", 2, 0, "Regular", true ) );

        var response = await Client().GetAsync( Url );

        Assert.Equal( HttpStatusCode.OK, response.StatusCode );

        var body = await response.Content.ReadFromJsonAsync<GetImplementosDisponiblesResponse>();
        Assert.NotNull( body );
        Assert.Empty( body.Implementos );
        Assert.Equal( "No hay implementos disponibles", body.Mensaje );
    }

    [Fact]
    public async Task Los_implementos_disponibles_se_devuelven_ordenados_por_nombre() {
        Seed(
            Implemento.Create( "TL-002", "Taladro inalámbrico", "Taladro con batería de repuesto", 4, 1, "Bueno", true ),
            Implemento.Create( "CM-017", "Cámara réflex", "Cámara con lente 18-55", 3, 3, "Regular", true ),
            Implemento.Create( "PR-005", "Proyector portátil", "Proyector HDMI para aulas", 6, 5, "Bueno", true ) );

        var body = await Client().GetFromJsonAsync<GetImplementosDisponiblesResponse>( Url );

        Assert.NotNull( body );
        Assert.Equal(
            new[] { "Cámara réflex", "Proyector portátil", "Taladro inalámbrico" },
            body.Implementos.Select( i => i.Nombre ) );
    }
}
