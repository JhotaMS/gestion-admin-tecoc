using gestionAdminTECOCApi.Application.Features.Users.CreateUser;
using gestionAdminTECOCApi.Application.Features.Users.GetAllUsers;
using gestionAdminTECOCApi.Domain.Users;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class GetAllUsersTests : IClassFixture<WebApplicationFactory<Program>> {
    private readonly WebApplicationFactory<Program> _factory;

    public GetAllUsersTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof( DbContextOptions<ApplicationDbContext> ) );
                if (descriptor is not null)
                    services.Remove( descriptor );
                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-getallusers-tests" ) );
            } );
        } );

        Seed();
    }

    private void Seed() {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    private HttpClient Client() => _factory.CreateClient();

    [Fact]
    public async Task Listar_usuarios_retorna_los_usuarios_creados() {
        var client = Client();
        await client.PostAsJsonAsync( "/api/v1/User", new UserCommand(
            "Camila Restrepo", "CC", "1094567890", "crestrepo", "camila.restrepo@example.com", "Passw0rd!" ) );

        var response = await client.GetAsync( "/api/v1/User" );

        Assert.Equal( HttpStatusCode.OK, response.StatusCode );

        var body = await response.Content.ReadFromJsonAsync<GetAllUsersResponse>();
        Assert.NotNull( body );
        Assert.Contains( body!.Users, u =>
            u.FullName == "Camila Restrepo"
            && u.UserName == "crestrepo"
            && u.DocumentType == DocumentType.CedulaCiudadania
            && u.DocumentNumber == "1094567890"
            && u.Email == "camila.restrepo@example.com" );
    }

    [Fact]
    public async Task Listar_usuarios_sin_registros_retorna_lista_vacia() {
        var client = Client();

        var response = await client.GetAsync( "/api/v1/User" );

        Assert.Equal( HttpStatusCode.OK, response.StatusCode );

        var body = await response.Content.ReadFromJsonAsync<GetAllUsersResponse>();
        Assert.NotNull( body );
        Assert.Empty( body!.Users );
    }
}
