using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Application.Features.Users.CreateUser;
using gestionAdminTECOCApi.Domain.Users;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class UserRegistrationTests : IClassFixture<WebApplicationFactory<Program>> {
    private readonly WebApplicationFactory<Program> _factory;

    public UserRegistrationTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof( DbContextOptions<ApplicationDbContext> ) );
                if (descriptor is not null) services.Remove( descriptor );
                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-user-registration-tests" ) );
            } );
        } );
    }

    [Fact]
    public async Task Register_con_datos_validos_retorna_201() {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/user",
            new UserCommand(
                "Ana Gómez Registro",
                "CC",
                "9988776654",
                "ana_gomez_registro",
                "ana.gomez.registro@tecoc.edu.co",
                "Test123*" ) );

        Assert.Equal( HttpStatusCode.Created, response.StatusCode );
        var body = await response.Content.ReadFromJsonAsync<UserCommandResponse>();
        Assert.NotNull( body );
        Assert.Equal( "Ana Gómez Registro", body.FullName );
    }

    [Fact]
    public async Task Register_sin_cuerpo_retorna_400() {
        var client = _factory.CreateClient();

        var response = await client.PostAsync(
            "/api/v1/user",
            new StringContent( string.Empty, System.Text.Encoding.UTF8, "application/json" ) );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }
}
