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

    private async Task<HttpResponseMessage> LoginAsync( string email, string password ) {
        var client = _factory.CreateClient();
        return await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginCommand( email, password ) );
    }

    [Fact]
    public async Task Login_con_credenciales_validas_retorna_200() {
        var response = await LoginAsync( "docente@tecoc.edu.co", "Test123*" );

        Assert.Equal( HttpStatusCode.OK, response.StatusCode );
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull( body );
        Assert.Equal( "docente@tecoc.edu.co", body.User.Email );
        Assert.NotEqual( Guid.Empty, body.User.Id );
    }

    [Fact]
    public async Task Login_con_password_invalida_retorna_401() {
        var response = await LoginAsync( "docente@tecoc.edu.co", "MalaClave1" );

        Assert.Equal( HttpStatusCode.Unauthorized, response.StatusCode );
    }

    [Fact]
    public async Task Login_con_email_inexistente_retorna_401() {
        var response = await LoginAsync( "nadie@tecoc.edu.co", "Cualquiera1" );

        Assert.Equal( HttpStatusCode.Unauthorized, response.StatusCode );
    }
}
