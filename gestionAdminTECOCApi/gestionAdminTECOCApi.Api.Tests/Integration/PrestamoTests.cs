using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Application.Features.Prestamos.GetPagedPrestamos;
using gestionAdminTECOCApi.Application.Features.Prestamos.GetPrestamoById;
using gestionAdminTECOCApi.Domain.Loans;
using gestionAdminTECOCApi.Domain.Prestamos;
using gestionAdminTECOCApi.Domain.Users;
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
                // TipoRevision 1 y 2 vienen precargados por seed data del modelo (HasData);
                // se usa un Id fuera de ese rango para poder probar el caso "no encontrado".
                TipoRevisionId = 9999,
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

    [Fact]
    public async Task Prestamo_con_relaciones_existentes_trae_id_y_nombre_de_cada_fk() {
        ResetDatabase();

        // Id de TipoRevision fuera del rango 1/2 que usan los seeds reales (y otros tests de
        // esta clase), para no chocar con datos que otro test haya insertado en la misma base.
        var user = User.Create( "Camila Restrepo", DocumentType.CedulaCiudadania, "1000200030", "camilar", "camila@tecoc.edu", "hash" );
        var implemento = Implemento.Create( "MT-014", "Multímetro digital", "Uso en laboratorio" );
        var tipoRevision = TipoRevision.Create( 501, "Inicio Préstamo", "Revisión al inicio del préstamo" );
        var prestamoId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Users.Add( user );
            db.Set<Implemento>().Add( implemento );
            db.Set<TipoRevision>().Add( tipoRevision );
            db.Prestamo.Add( new Prestamo {
                Id = prestamoId,
                UuserId = user.Id,
                ImplementoId = implemento.Id,
                TipoRevisionId = tipoRevision.Id,
                EstadoTipo = "reservado",
                Inicio = DateTime.UtcNow,
                Fin = DateTime.UtcNow.AddDays( 1 ),
                Observacion = "Con relaciones reales"
            } );
            db.SaveChanges();
        }

        var client = Client();
        var response = await client.GetFromJsonAsync<GetPagedPrestamosResponse>( "/api/v1/Prestamo" );

        var dto = response!.Items.Single();
        Assert.Equal( user.Id, dto.UuserId );
        Assert.Equal( "Camila Restrepo", dto.RequesterName );
        Assert.Equal( implemento.Id, dto.ImplementoId );
        Assert.Equal( "Multímetro digital", dto.ImplementoNombre );
        Assert.Equal( tipoRevision.Id, dto.TipoRevisionId );
        Assert.Equal( "Inicio Préstamo", dto.TipoRevisionNombre );
    }

    [Fact]
    public async Task Prestamo_con_fk_sin_coincidencia_no_falla_y_usa_texto_por_defecto() {
        ResetDatabase();
        SeedPrestamos( 1 );

        var client = Client();
        var response = await client.GetFromJsonAsync<GetPagedPrestamosResponse>( "/api/v1/Prestamo" );

        var dto = response!.Items.Single();
        Assert.Equal( "(no encontrado)", dto.RequesterName );
        Assert.Equal( "(no encontrado)", dto.ImplementoNombre );
        Assert.Equal( "(no encontrado)", dto.TipoRevisionNombre );
    }

    [Fact]
    public async Task Detalle_de_prestamo_existente_trae_id_y_nombre_de_cada_fk() {
        ResetDatabase();

        var user = User.Create( "Camila Restrepo", DocumentType.CedulaCiudadania, "1000200031", "camilar2", "camila2@tecoc.edu", "hash" );
        var implemento = Implemento.Create( "MT-015", "Multímetro digital", "Uso en laboratorio", estado: "Disponible" );
        var tipoRevision = TipoRevision.Create( 502, "Inicio Préstamo", "Revisión al inicio del préstamo" );
        var prestamoId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Users.Add( user );
            db.Set<Implemento>().Add( implemento );
            db.Set<TipoRevision>().Add( tipoRevision );
            db.Prestamo.Add( new Prestamo {
                Id = prestamoId,
                UuserId = user.Id,
                ImplementoId = implemento.Id,
                TipoRevisionId = tipoRevision.Id,
                EstadoTipo = "reservado",
                Inicio = DateTime.UtcNow,
                Fin = DateTime.UtcNow.AddDays( 1 ),
                Observacion = "Con relaciones reales"
            } );
            db.SaveChanges();
        }

        var client = Client();
        var response = await client.GetFromJsonAsync<GetPrestamoByIdResponse>( $"/api/v1/Prestamo/{prestamoId}" );

        Assert.NotNull( response );
        Assert.Equal( prestamoId, response.Id );
        Assert.Equal( "Camila Restrepo", response.RequesterName );
        Assert.Equal( "camila2@tecoc.edu", response.RequesterEmail );
        Assert.Equal( "Cédula de ciudadanía", response.RequesterDocumentType );
        Assert.Equal( "1000200031", response.RequesterDocumentNumber );
        Assert.Equal( "Multímetro digital", response.ImplementoNombre );
        Assert.Equal( "Disponible", response.ImplementoEstado );
        Assert.Equal( "Inicio Préstamo", response.TipoRevisionNombre );
    }

    [Fact]
    public async Task Detalle_de_prestamo_inexistente_retorna_notfound() {
        ResetDatabase();
        var client = Client();

        var response = await client.GetAsync( $"/api/v1/Prestamo/{Guid.NewGuid()}" );

        Assert.Equal( HttpStatusCode.NotFound, response.StatusCode );
    }
}
