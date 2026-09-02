using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.ProgramasAcademicos;
using gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.CreateProgramaAcademico;
using gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.GetAllProgramasAcademicos;
using gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.UpdateProgramaAcademico;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class ProgramaAcademicoTests : IClassFixture<WebApplicationFactory<Program>> {
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramaAcademicoTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    service => service.ServiceType == typeof( DbContextOptions<ApplicationDbContext> )
                );

                if (descriptor is not null)
                    services.Remove( descriptor );

                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-programa-academico-tests" )
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
    public async Task Crear_programa_academico_normaliza_datos_y_retorna_created() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/ProgramaAcademico",
            new CreateProgramaAcademicoCommand( "  Ingeniería de Software  ", "  ing-sw  " )
        );

        Assert.Equal( HttpStatusCode.Created, response.StatusCode );

        ProgramaAcademicoResponse? programa = await response.Content.ReadFromJsonAsync<ProgramaAcademicoResponse>();
        Assert.NotNull( programa );
        Assert.NotEqual( Guid.Empty, programa.Id );
        Assert.Equal( "Ingeniería de Software", programa.Name );
        Assert.Equal( "ING-SW", programa.Code );
        Assert.True( programa.Enabled );
    }

    [Fact]
    public async Task Listar_programas_academicos_retorna_todos_ordenados_por_nombre() {
        HttpClient client = Client();
        await client.PostAsJsonAsync( "/api/v1/ProgramaAcademico", new CreateProgramaAcademicoCommand( "Sistemas", "SIS" ) );
        await client.PostAsJsonAsync( "/api/v1/ProgramaAcademico", new CreateProgramaAcademicoCommand( "Electrónica", "ELE" ) );

        GetAllProgramasAcademicosResponse? response = await client.GetFromJsonAsync<GetAllProgramasAcademicosResponse>(
            "/api/v1/ProgramaAcademico"
        );

        Assert.NotNull( response );
        Assert.Equal( 2, response.ProgramasAcademicos.Count );
        Assert.Equal( ["Electrónica", "Sistemas"], response.ProgramasAcademicos.Select( programa => programa.Name ) );
    }

    [Fact]
    public async Task Listar_programas_academicos_sin_registros_retorna_arreglo_vacio() {
        HttpClient client = Client();

        GetAllProgramasAcademicosResponse? response = await client.GetFromJsonAsync<GetAllProgramasAcademicosResponse>(
            "/api/v1/ProgramaAcademico"
        );

        Assert.NotNull( response );
        Assert.Empty( response.ProgramasAcademicos );
    }

    [Fact]
    public async Task Crear_codigo_duplicado_sin_importar_mayusculas_retorna_conflict() {
        HttpClient client = Client();
        await client.PostAsJsonAsync( "/api/v1/ProgramaAcademico", new CreateProgramaAcademicoCommand( "Sistemas", "SIS" ) );

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/ProgramaAcademico",
            new CreateProgramaAcademicoCommand( "Otro programa", "sis" )
        );

        Assert.Equal( HttpStatusCode.Conflict, response.StatusCode );
        CodeError? error = await response.Content.ReadFromJsonAsync<CodeError>();
        Assert.Equal( "Ya existe un programa académico con ese código", error?.Message );
    }

    [Theory]
    [InlineData( "", "SIS" )]
    [InlineData( "Sistemas", "" )]
    public async Task Crear_datos_obligatorios_vacios_retorna_badrequest(
        string name,
        string code
    ) {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/ProgramaAcademico",
            new CreateProgramaAcademicoCommand( name, code )
        );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }

    [Fact]
    public async Task Actualizar_programa_academico_retorna_datos_actualizados() {
        HttpClient client = Client();
        HttpResponseMessage create = await client.PostAsJsonAsync(
            "/api/v1/ProgramaAcademico",
            new CreateProgramaAcademicoCommand( "Sistemas", "SIS" )
        );
        ProgramaAcademicoResponse? created = await create.Content.ReadFromJsonAsync<ProgramaAcademicoResponse>();
        Assert.NotNull( created );

        HttpResponseMessage update = await client.PutAsJsonAsync(
            $"/api/v1/ProgramaAcademico/{created.Id}",
            new UpdateProgramaAcademicoCommand( created.Id, "  Ingeniería de Sistemas  ", "  ing-sis  " )
        );

        Assert.Equal( HttpStatusCode.OK, update.StatusCode );
        ProgramaAcademicoResponse? updated = await update.Content.ReadFromJsonAsync<ProgramaAcademicoResponse>();
        Assert.Equal( "Ingeniería de Sistemas", updated?.Name );
        Assert.Equal( "ING-SIS", updated?.Code );
    }

    [Fact]
    public async Task Actualizar_programa_academico_inexistente_retorna_notfound() {
        HttpClient client = Client();
        Guid programaId = Guid.NewGuid();

        HttpResponseMessage response = await client.PutAsJsonAsync(
            $"/api/v1/ProgramaAcademico/{programaId}",
            new UpdateProgramaAcademicoCommand( programaId, "Sistemas", "SIS" )
        );

        Assert.Equal( HttpStatusCode.NotFound, response.StatusCode );
    }

    [Fact]
    public async Task Eliminar_programa_academico_lo_retira_del_listado() {
        HttpClient client = Client();
        HttpResponseMessage create = await client.PostAsJsonAsync(
            "/api/v1/ProgramaAcademico",
            new CreateProgramaAcademicoCommand( "Sistemas", "SIS" )
        );
        ProgramaAcademicoResponse? created = await create.Content.ReadFromJsonAsync<ProgramaAcademicoResponse>();
        Assert.NotNull( created );

        HttpResponseMessage delete = await client.DeleteAsync( $"/api/v1/ProgramaAcademico/{created.Id}" );

        Assert.Equal( HttpStatusCode.NoContent, delete.StatusCode );
        GetAllProgramasAcademicosResponse? list = await client.GetFromJsonAsync<GetAllProgramasAcademicosResponse>(
            "/api/v1/ProgramaAcademico"
        );
        Assert.Empty( list!.ProgramasAcademicos );
    }

    [Fact]
    public async Task Eliminar_programa_academico_inexistente_retorna_notfound() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.DeleteAsync(
            $"/api/v1/ProgramaAcademico/{Guid.NewGuid()}"
        );

        Assert.Equal( HttpStatusCode.NotFound, response.StatusCode );
    }
}
