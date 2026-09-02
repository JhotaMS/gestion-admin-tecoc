using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Application.Features.Groups;
using gestionAdminTECOCApi.Application.Features.Groups.CreateGroup;
using gestionAdminTECOCApi.Application.Features.ProgramasAcademicos;
using gestionAdminTECOCApi.Application.Features.ProgramasAcademicos.CreateProgramaAcademico;
using gestionAdminTECOCApi.Application.Features.Users.CreateUser;
using gestionAdminTECOCApi.Application.Features.Users.GetAllUsers;
using gestionAdminTECOCApi.Application.Features.Users.GetPagedUsers;
using gestionAdminTECOCApi.Application.Features.Users.UpdateUser;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class UserGroupProgramaAcademicoTests : IClassFixture<WebApplicationFactory<Program>> {
    private readonly WebApplicationFactory<Program> _factory;

    public UserGroupProgramaAcademicoTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    service => service.ServiceType == typeof( DbContextOptions<ApplicationDbContext> )
                );

                if (descriptor is not null)
                    services.Remove( descriptor );

                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-user-group-programa-tests" )
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

    private static async Task<Guid> CreateGroupAsync( HttpClient client, string name, string code ) {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/Group", new CreateGroupCommand( name, code ) );
        GroupResponse? group = await response.Content.ReadFromJsonAsync<GroupResponse>();
        return group!.Id;
    }

    private static async Task<Guid> CreateProgramaAcademicoAsync( HttpClient client, string name, string code ) {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/ProgramaAcademico", new CreateProgramaAcademicoCommand( name, code ) );
        ProgramaAcademicoResponse? programa = await response.Content.ReadFromJsonAsync<ProgramaAcademicoResponse>();
        return programa!.Id;
    }

    [Fact]
    public async Task Crear_usuario_con_grupo_y_programa_academico_los_expone_en_el_listado() {
        HttpClient client = Client();
        Guid groupId = await CreateGroupAsync( client, "Grupo A", "GRP-A" );
        Guid programaId = await CreateProgramaAcademicoAsync( client, "Ingeniería de Software", "ING-SW" );

        HttpResponseMessage response = await client.PostAsJsonAsync( "/api/v1/User", new UserCommand(
            "Camila Restrepo", "CC", "1094567890", "crestrepo", "camila.restrepo@example.com", "Passw0rd!",
            groupId, programaId ) );

        Assert.Equal( HttpStatusCode.Created, response.StatusCode );

        GetAllUsersResponse? users = await client.GetFromJsonAsync<GetAllUsersResponse>( "/api/v1/User" );
        UserSummaryDto? created = users!.Users.SingleOrDefault( u => u.UserName == "crestrepo" );

        Assert.NotNull( created );
        Assert.NotNull( created!.Group );
        Assert.Equal( groupId, created.Group!.Id );
        Assert.NotNull( created.ProgramaAcademico );
        Assert.Equal( programaId, created.ProgramaAcademico!.Id );
    }

    [Fact]
    public async Task Crear_usuario_con_grupo_inexistente_retorna_badrequest() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PostAsJsonAsync( "/api/v1/User", new UserCommand(
            "Camila Restrepo", "CC", "1094567890", "crestrepo", "camila.restrepo@example.com", "Passw0rd!",
            Guid.NewGuid(), null ) );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }

    [Fact]
    public async Task Crear_usuario_con_programa_academico_inexistente_retorna_badrequest() {
        HttpClient client = Client();

        HttpResponseMessage response = await client.PostAsJsonAsync( "/api/v1/User", new UserCommand(
            "Camila Restrepo", "CC", "1094567890", "crestrepo", "camila.restrepo@example.com", "Passw0rd!",
            null, Guid.NewGuid() ) );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }

    [Fact]
    public async Task Actualizar_usuario_cambia_su_grupo_y_programa_academico() {
        HttpClient client = Client();
        Guid grupoInicial = await CreateGroupAsync( client, "Grupo A", "GRP-A" );
        Guid grupoNuevo = await CreateGroupAsync( client, "Grupo B", "GRP-B" );
        Guid programaNuevo = await CreateProgramaAcademicoAsync( client, "Ingeniería de Software", "ING-SW" );

        HttpResponseMessage create = await client.PostAsJsonAsync( "/api/v1/User", new UserCommand(
            "Camila Restrepo", "CC", "1094567890", "crestrepo", "camila.restrepo@example.com", "Passw0rd!",
            grupoInicial, null ) );
        UserCommandResponse? created = await create.Content.ReadFromJsonAsync<UserCommandResponse>();

        HttpResponseMessage update = await client.PutAsJsonAsync( $"/api/v1/User/{created!.Id}", new UpdateUserCommand(
            created.Id, "Camila Restrepo", "CC", "1094567890", "crestrepo", "camila.restrepo@example.com",
            grupoNuevo, programaNuevo ) );

        Assert.Equal( HttpStatusCode.OK, update.StatusCode );

        GetAllUsersResponse? users = await client.GetFromJsonAsync<GetAllUsersResponse>( "/api/v1/User" );
        UserSummaryDto? actualizado = users!.Users.Single( u => u.Id == created.Id );

        Assert.Equal( grupoNuevo, actualizado.Group!.Id );
        Assert.Equal( programaNuevo, actualizado.ProgramaAcademico!.Id );
    }

    [Fact]
    public async Task Actualizar_usuario_sin_enviar_grupo_lo_deja_sin_grupo() {
        HttpClient client = Client();
        Guid grupoInicial = await CreateGroupAsync( client, "Grupo A", "GRP-A" );

        HttpResponseMessage create = await client.PostAsJsonAsync( "/api/v1/User", new UserCommand(
            "Camila Restrepo", "CC", "1094567890", "crestrepo", "camila.restrepo@example.com", "Passw0rd!",
            grupoInicial, null ) );
        UserCommandResponse? created = await create.Content.ReadFromJsonAsync<UserCommandResponse>();

        await client.PutAsJsonAsync( $"/api/v1/User/{created!.Id}", new UpdateUserCommand(
            created.Id, "Camila Restrepo", "CC", "1094567890", "crestrepo", "camila.restrepo@example.com" ) );

        GetAllUsersResponse? users = await client.GetFromJsonAsync<GetAllUsersResponse>( "/api/v1/User" );
        UserSummaryDto? actualizado = users!.Users.Single( u => u.Id == created.Id );

        Assert.Null( actualizado.Group );
    }

    [Fact]
    public async Task Listado_paginado_expone_grupo_y_programa_academico() {
        HttpClient client = Client();
        Guid groupId = await CreateGroupAsync( client, "Grupo A", "GRP-A" );
        Guid programaId = await CreateProgramaAcademicoAsync( client, "Ingeniería de Software", "ING-SW" );

        await client.PostAsJsonAsync( "/api/v1/User", new UserCommand(
            "Camila Restrepo", "CC", "1094567890", "crestrepo", "camila.restrepo@example.com", "Passw0rd!",
            groupId, programaId ) );

        GetPagedUsersResponse? response = await client.GetFromJsonAsync<GetPagedUsersResponse>(
            "/api/v1/User/paged?pageNumber=1&pageSize=10" );

        PagedUserDto pagedUser = response!.Items.Single();
        Assert.Equal( groupId, pagedUser.Group!.Id );
        Assert.Equal( programaId, pagedUser.ProgramaAcademico!.Id );
    }
}
