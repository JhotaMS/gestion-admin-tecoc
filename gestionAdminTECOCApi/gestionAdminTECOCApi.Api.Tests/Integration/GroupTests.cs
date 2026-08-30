using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Api.Errors;
using gestionAdminTECOCApi.Application.Features.Groups;
using gestionAdminTECOCApi.Application.Features.Groups.CreateGroup;
using gestionAdminTECOCApi.Application.Features.Groups.GetAllGroups;
using gestionAdminTECOCApi.Application.Features.Groups.UpdateGroup;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class GroupTests : IClassFixture<WebApplicationFactory<Program>> {
    private readonly WebApplicationFactory<Program> _factory;

    public GroupTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    service => service.ServiceType == typeof( DbContextOptions<ApplicationDbContext> )
                );

                if (descriptor is not null)
                    services.Remove( descriptor );

                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-group-tests" )
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
    public async Task Crear_grupo_normaliza_datos_y_retorna_created() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/Group",
            new CreateGroupCommand( "  Grupo A  ", "  grp-a  " )
        );

        Assert.Equal( HttpStatusCode.Created, response.StatusCode );

        GroupResponse? group = await response.Content.ReadFromJsonAsync<GroupResponse>();
        Assert.NotNull( group );
        Assert.NotEqual( Guid.Empty, group.Id );
        Assert.Equal( "Grupo A", group.Name );
        Assert.Equal( "GRP-A", group.Code );
        Assert.True( group.Enabled );
    }

    [Fact]
    public async Task Listar_grupos_retorna_todos_ordenados_por_nombre() {
        HttpClient client = Client();
        await client.PostAsJsonAsync( "/api/v1/Group", new CreateGroupCommand( "Grupo B", "GRP-B" ) );
        await client.PostAsJsonAsync( "/api/v1/Group", new CreateGroupCommand( "Grupo A", "GRP-A" ) );

        GetAllGroupsResponse? response = await client.GetFromJsonAsync<GetAllGroupsResponse>(
            "/api/v1/Group"
        );

        Assert.NotNull( response );
        Assert.Equal( 2, response.Groups.Count );
        Assert.Equal( ["Grupo A", "Grupo B"], response.Groups.Select( group => group.Name ) );
    }

    [Fact]
    public async Task Listar_grupos_sin_registros_retorna_arreglo_vacio() {
        HttpClient client = Client();

        GetAllGroupsResponse? response = await client.GetFromJsonAsync<GetAllGroupsResponse>(
            "/api/v1/Group"
        );

        Assert.NotNull( response );
        Assert.Empty( response.Groups );
    }

    [Fact]
    public async Task Crear_codigo_duplicado_sin_importar_mayusculas_retorna_conflict() {
        HttpClient client = Client();
        await client.PostAsJsonAsync( "/api/v1/Group", new CreateGroupCommand( "Grupo A", "GRP-A" ) );

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/Group",
            new CreateGroupCommand( "Otro grupo", "grp-a" )
        );

        Assert.Equal( HttpStatusCode.Conflict, response.StatusCode );
        CodeError? error = await response.Content.ReadFromJsonAsync<CodeError>();
        Assert.Equal( "Ya existe un grupo con ese código", error?.Message );
    }

    [Theory]
    [InlineData( "", "GRP-A" )]
    [InlineData( "Grupo A", "" )]
    public async Task Crear_datos_obligatorios_vacios_retorna_badrequest(
        string name,
        string code
    ) {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/Group",
            new CreateGroupCommand( name, code )
        );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }

    [Fact]
    public async Task Actualizar_grupo_retorna_datos_actualizados() {
        HttpClient client = Client();
        HttpResponseMessage create = await client.PostAsJsonAsync(
            "/api/v1/Group",
            new CreateGroupCommand( "Grupo A", "GRP-A" )
        );
        GroupResponse? created = await create.Content.ReadFromJsonAsync<GroupResponse>();
        Assert.NotNull( created );

        HttpResponseMessage update = await client.PutAsJsonAsync(
            $"/api/v1/Group/{created.Id}",
            new UpdateGroupCommand( created.Id, "  Grupo Alfa  ", "  alfa  " )
        );

        Assert.Equal( HttpStatusCode.OK, update.StatusCode );
        GroupResponse? updated = await update.Content.ReadFromJsonAsync<GroupResponse>();
        Assert.Equal( "Grupo Alfa", updated?.Name );
        Assert.Equal( "ALFA", updated?.Code );
    }

    [Fact]
    public async Task Actualizar_con_id_distinto_retorna_badrequest() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PutAsJsonAsync(
            $"/api/v1/Group/{Guid.NewGuid()}",
            new UpdateGroupCommand( Guid.NewGuid(), "Grupo A", "GRP-A" )
        );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }

    [Fact]
    public async Task Actualizar_grupo_inexistente_retorna_notfound() {
        HttpClient client = Client();
        Guid groupId = Guid.NewGuid();

        HttpResponseMessage response = await client.PutAsJsonAsync(
            $"/api/v1/Group/{groupId}",
            new UpdateGroupCommand( groupId, "Grupo A", "GRP-A" )
        );

        Assert.Equal( HttpStatusCode.NotFound, response.StatusCode );
    }

    [Fact]
    public async Task Eliminar_grupo_lo_retira_del_listado() {
        HttpClient client = Client();
        HttpResponseMessage create = await client.PostAsJsonAsync(
            "/api/v1/Group",
            new CreateGroupCommand( "Grupo A", "GRP-A" )
        );
        GroupResponse? created = await create.Content.ReadFromJsonAsync<GroupResponse>();
        Assert.NotNull( created );

        HttpResponseMessage delete = await client.DeleteAsync( $"/api/v1/Group/{created.Id}" );

        Assert.Equal( HttpStatusCode.NoContent, delete.StatusCode );
        GetAllGroupsResponse? list = await client.GetFromJsonAsync<GetAllGroupsResponse>(
            "/api/v1/Group"
        );
        Assert.Empty( list!.Groups );
    }

    [Fact]
    public async Task Eliminar_grupo_inexistente_retorna_notfound() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.DeleteAsync(
            $"/api/v1/Group/{Guid.NewGuid()}"
        );

        Assert.Equal( HttpStatusCode.NotFound, response.StatusCode );
    }
}
