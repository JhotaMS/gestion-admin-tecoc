using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Application.Features.Auth.Login;
using gestionAdminTECOCApi.Domain.Users;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class AuthLoginTests : IClassFixture<WebApplicationFactory<Program>> {
    private readonly WebApplicationFactory<Program> _factory;

    public AuthLoginTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof( DbContextOptions<ApplicationDbContext> ) );
                if (descriptor is not null) services.Remove( descriptor );
                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-login-tests" ) );
            } );
        } );

        SeedUser();
    }

    private void SeedUser() {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        if (!db.Set<User>().Any()) {
            var user = User.Create(
                "Docente Prueba", DocumentType.CedulaCiudadania, "1018329848", "docente1",
                "docente@tecoc.edu.co", PasswordHasher.Hash( "Test123*" )
            );
            db.Set<User>().Add( user );
            db.SaveChanges();
        }
    }

    private async Task<HttpResponseMessage> LoginAsync( string userName, string password ) {
        var client = _factory.CreateClient();
        return await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginCommand( userName, password ) );
    }

    [Fact]
    public async Task Login_con_usuario_y_contrasena_validos_retorna_200() {
        var response = await LoginAsync( "docente1", "Test123*" );

        Assert.Equal( HttpStatusCode.OK, response.StatusCode );
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull( body );
        Assert.Equal( "docente1", body.UserName );
        Assert.NotEqual( Guid.Empty, body.UserId );
    }

    [Fact]
    public async Task Login_con_contrasena_invalida_retorna_401() {
        var response = await LoginAsync( "docente1", "MalaClave1" );

        Assert.Equal( HttpStatusCode.Unauthorized, response.StatusCode );
    }

    [Fact]
    public async Task Login_con_usuario_inexistente_retorna_401() {
        var response = await LoginAsync( "nadie1", "Cualquiera1" );

        Assert.Equal( HttpStatusCode.Unauthorized, response.StatusCode );
    }
}
