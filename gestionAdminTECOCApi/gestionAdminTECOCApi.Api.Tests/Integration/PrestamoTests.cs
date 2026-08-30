using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Application.Features.Prestamos.GetPagedPrestamos;
using gestionAdminTECOCApi.Domain.Prestamos;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class PrestamoTests : IClassFixture<WebApplicationFactory<Program>> {
    private readonly WebApplicationFactory<Program> _factory;

    public PrestamoTests( WebApplicationFactory<Program> factory ) {
        // Cada clase de test usa su propia base InMemory (nombre distinto) para no
        // interferir con los datos que sembraron otras clases de test.
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof( DbContextOptions<ApplicationDbContext> ) );
                if (descriptor is not null) services.Remove( descriptor );
                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-prestamo-tests" ) );
            } );
        } );
    }

    private HttpClient Client() => _factory.CreateClient();

    private void ResetDatabase() {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    private void SeedPrestamos( int count ) {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        for (var i = 0; i < count; i++) {
            db.Prestamo.Add( new Prestamo {
                Id = Guid.NewGuid(),
                UuserId = Guid.NewGuid(),
                ImplementoId = Guid.NewGuid(),
                TipoRevisionId = Guid.NewGuid(),
                EstadoTipo = "reservado",
                // Cada préstamo con una fecha de inicio distinta para poder comprobar el orden.
                Inicio = new DateTime( 2026, 1, 1, 0, 0, 0, DateTimeKind.Utc ).AddDays( i ),
                Fin = new DateTime( 2026, 1, 2, 0, 0, 0, DateTimeKind.Utc ).AddDays( i ),
                Observacion = $"Préstamo de prueba {i}"
            } );
        }

        db.SaveChanges();
    }

    [Fact]
    public async Task Sin_prestamos_retorna_lista_vacia_y_totalcount_cero() {
        ResetDatabase();
        var client = Client();

        var response = await client.GetAsync( "/api/v1/Prestamo" );
        Assert.Equal( HttpStatusCode.OK, response.StatusCode );

        var body = await response.Content.ReadFromJsonAsync<GetPagedPrestamosResponse>();
        Assert.NotNull( body );
        Assert.Empty( body.Items );
        Assert.Equal( 0, body.TotalCount );
        Assert.Equal( 0, body.TotalPages );
    }

    [Fact]
    public async Task Pagina_solicitada_trae_solo_el_tamano_pedido_y_el_total_correcto() {
        ResetDatabase();
        SeedPrestamos( 25 );
        var client = Client();

        var page1 = await client.GetFromJsonAsync<GetPagedPrestamosResponse>( "/api/v1/Prestamo?pageNumber=1&pageSize=10" );
        Assert.NotNull( page1 );
        Assert.Equal( 10, page1.Items.Count );
        Assert.Equal( 25, page1.TotalCount );
        Assert.Equal( 3, page1.TotalPages );

        var page3 = await client.GetFromJsonAsync<GetPagedPrestamosResponse>( "/api/v1/Prestamo?pageNumber=3&pageSize=10" );
        Assert.NotNull( page3 );
        // La última página solo trae los 5 registros que sobran (25 - 10 - 10).
        Assert.Equal( 5, page3.Items.Count );
    }

    [Fact]
    public async Task Sin_parametros_usa_pagina_1_y_tamano_10_por_defecto() {
        ResetDatabase();
        SeedPrestamos( 15 );
        var client = Client();

        var response = await client.GetFromJsonAsync<GetPagedPrestamosResponse>( "/api/v1/Prestamo" );

        Assert.NotNull( response );
        Assert.Equal( 1, response.PageNumber );
        Assert.Equal( 10, response.PageSize );
        Assert.Equal( 10, response.Items.Count );
    }

    [Theory]
    [InlineData( "pageNumber=0&pageSize=10" )]
    [InlineData( "pageNumber=1&pageSize=0" )]
    [InlineData( "pageNumber=1&pageSize=101" )]
    public async Task Parametros_de_paginacion_invalidos_retornan_badrequest( string queryString ) {
        ResetDatabase();
        var client = Client();

        var response = await client.GetAsync( $"/api/v1/Prestamo?{queryString}" );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }
}
