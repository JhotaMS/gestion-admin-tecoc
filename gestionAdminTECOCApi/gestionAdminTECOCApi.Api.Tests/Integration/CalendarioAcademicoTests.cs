using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.CalendarioAcademico;
using gestionAdminTECOCApi.Application.Features.CalendarioAcademico.CreateEventoAcademico;
using gestionAdminTECOCApi.Application.Features.CalendarioAcademico.GetAllEventosAcademicos;
using gestionAdminTECOCApi.Application.Features.CalendarioAcademico.UpdateEventoAcademico;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class CalendarioAcademicoTests : IClassFixture<WebApplicationFactory<Program>> {
    private readonly WebApplicationFactory<Program> _factory;

    public CalendarioAcademicoTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    service => service.ServiceType == typeof( DbContextOptions<ApplicationDbContext> )
                );

                if (descriptor is not null)
                    services.Remove( descriptor );

                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-calendario-academico-tests" )
                );
            } );
        } );

        ResetDatabase();
    }

    private void ResetDatabase() {
        using IServiceScope scope = _factory.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        database.Database.EnsureDeleted();
        database.Database.EnsureCreated();
    }

    private HttpClient Client() => _factory.CreateClient();

    [Fact]
    public async Task Crear_evento_normaliza_datos_y_retorna_created() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/CalendarioAcademico",
            new CreateEventoAcademicoCommand(
                "  Inicio de clases  ",
                "  Primer semestre  ",
                new DateOnly( 2026, 1, 26 ),
                null
            )
        );

        Assert.Equal( HttpStatusCode.Created, response.StatusCode );

        EventoAcademicoResponse? evento = await response.Content.ReadFromJsonAsync<EventoAcademicoResponse>();
        Assert.NotNull( evento );
        Assert.NotEqual( Guid.Empty, evento.Id );
        Assert.Equal( "Inicio de clases", evento.Titulo );
        Assert.Equal( "Primer semestre", evento.Descripcion );
        Assert.Equal( new DateOnly( 2026, 1, 26 ), evento.FechaInicio );
        Assert.Null( evento.FechaFin );
        Assert.True( evento.Enabled );
    }

    [Fact]
    public async Task Crear_evento_sin_descripcion_es_valido() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/CalendarioAcademico",
            new CreateEventoAcademicoCommand( "Examen final", null, new DateOnly( 2026, 5, 20 ), null )
        );

        Assert.Equal( HttpStatusCode.Created, response.StatusCode );
    }

    [Fact]
    public async Task Crear_evento_con_titulo_vacio_retorna_badrequest() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/CalendarioAcademico",
            new CreateEventoAcademicoCommand( "", null, new DateOnly( 2026, 5, 20 ), null )
        );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }

    [Fact]
    public async Task Crear_evento_con_fecha_fin_anterior_a_inicio_retorna_badrequest() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/CalendarioAcademico",
            new CreateEventoAcademicoCommand(
                "Matrículas",
                null,
                new DateOnly( 2026, 1, 20 ),
                new DateOnly( 2026, 1, 10 )
            )
        );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
        CodeError? error = await response.Content.ReadFromJsonAsync<CodeError>();
        Assert.Equal( "La fecha de fin no puede ser anterior a la fecha de inicio", error?.Message );
    }

    [Fact]
    public async Task Listar_eventos_retorna_ordenados_por_fecha_de_inicio() {
        HttpClient client = Client();
        await client.PostAsJsonAsync( "/api/v1/CalendarioAcademico",
            new CreateEventoAcademicoCommand( "Exámenes finales", null, new DateOnly( 2026, 5, 20 ), null ) );
        await client.PostAsJsonAsync( "/api/v1/CalendarioAcademico",
            new CreateEventoAcademicoCommand( "Inicio de clases", null, new DateOnly( 2026, 1, 26 ), null ) );

        GetAllEventosAcademicosResponse? response = await client.GetFromJsonAsync<GetAllEventosAcademicosResponse>(
            "/api/v1/CalendarioAcademico"
        );

        Assert.NotNull( response );
        Assert.Equal( 2, response.Eventos.Count );
        Assert.Equal( ["Inicio de clases", "Exámenes finales"], response.Eventos.Select( evento => evento.Titulo ) );
    }

    [Fact]
    public async Task Listar_eventos_sin_registros_retorna_arreglo_vacio() {
        HttpClient client = Client();

        GetAllEventosAcademicosResponse? response = await client.GetFromJsonAsync<GetAllEventosAcademicosResponse>(
            "/api/v1/CalendarioAcademico"
        );

        Assert.NotNull( response );
        Assert.Empty( response.Eventos );
    }

    [Fact]
    public async Task Actualizar_evento_retorna_datos_actualizados() {
        HttpClient client = Client();
        HttpResponseMessage create = await client.PostAsJsonAsync(
            "/api/v1/CalendarioAcademico",
            new CreateEventoAcademicoCommand( "Inicio de clases", null, new DateOnly( 2026, 1, 26 ), null )
        );
        EventoAcademicoResponse? created = await create.Content.ReadFromJsonAsync<EventoAcademicoResponse>();
        Assert.NotNull( created );

        HttpResponseMessage update = await client.PutAsJsonAsync(
            $"/api/v1/CalendarioAcademico/{created.Id}",
            new UpdateEventoAcademicoCommand(
                created.Id,
                "  Inicio de clases (actualizado)  ",
                "Nueva fecha",
                new DateOnly( 2026, 2, 2 ),
                new DateOnly( 2026, 2, 6 )
            )
        );

        Assert.Equal( HttpStatusCode.OK, update.StatusCode );
        EventoAcademicoResponse? updated = await update.Content.ReadFromJsonAsync<EventoAcademicoResponse>();
        Assert.Equal( "Inicio de clases (actualizado)", updated?.Titulo );
        Assert.Equal( new DateOnly( 2026, 2, 2 ), updated?.FechaInicio );
        Assert.Equal( new DateOnly( 2026, 2, 6 ), updated?.FechaFin );
    }

    [Fact]
    public async Task Actualizar_evento_inexistente_retorna_notfound() {
        HttpClient client = Client();
        Guid eventoId = Guid.NewGuid();

        HttpResponseMessage response = await client.PutAsJsonAsync(
            $"/api/v1/CalendarioAcademico/{eventoId}",
            new UpdateEventoAcademicoCommand( eventoId, "Inicio de clases", null, new DateOnly( 2026, 1, 26 ), null )
        );

        Assert.Equal( HttpStatusCode.NotFound, response.StatusCode );
    }

    [Fact]
    public async Task Actualizar_con_id_distinto_retorna_badrequest() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PutAsJsonAsync(
            $"/api/v1/CalendarioAcademico/{Guid.NewGuid()}",
            new UpdateEventoAcademicoCommand( Guid.NewGuid(), "Inicio de clases", null, new DateOnly( 2026, 1, 26 ), null )
        );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }

    [Fact]
    public async Task Eliminar_evento_lo_retira_del_listado() {
        HttpClient client = Client();
        HttpResponseMessage create = await client.PostAsJsonAsync(
            "/api/v1/CalendarioAcademico",
            new CreateEventoAcademicoCommand( "Inicio de clases", null, new DateOnly( 2026, 1, 26 ), null )
        );
        EventoAcademicoResponse? created = await create.Content.ReadFromJsonAsync<EventoAcademicoResponse>();
        Assert.NotNull( created );

        HttpResponseMessage delete = await client.DeleteAsync( $"/api/v1/CalendarioAcademico/{created.Id}" );

        Assert.Equal( HttpStatusCode.NoContent, delete.StatusCode );
        GetAllEventosAcademicosResponse? list = await client.GetFromJsonAsync<GetAllEventosAcademicosResponse>(
            "/api/v1/CalendarioAcademico"
        );
        Assert.Empty( list!.Eventos );
    }

    [Fact]
    public async Task Eliminar_evento_inexistente_retorna_notfound() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.DeleteAsync(
            $"/api/v1/CalendarioAcademico/{Guid.NewGuid()}"
        );

        Assert.Equal( HttpStatusCode.NotFound, response.StatusCode );
    }
}
