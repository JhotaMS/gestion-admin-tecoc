using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Application.Features.Users.CreateUser;
using gestionAdminTECOCApi.Application.Features.Users.GetPagedUsers;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class GetPagedUsersTests : IClassFixture<WebApplicationFactory<Program>> {
    private readonly WebApplicationFactory<Program> _factory;

    public GetPagedUsersTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof( DbContextOptions<ApplicationDbContext> ) );
                if (descriptor is not null)
                    services.Remove( descriptor );
                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-getpagedusers-tests" ) );
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

    private async Task CreateUsersAsync( HttpClient client, int count ) {
        for (int i = 0; i < count; i++) {
            await client.PostAsJsonAsync( "/api/v1/User", new UserCommand(
                $"Estudiante {i:00}", "CC", $"100000{i:00}", $"estudiante{i:00}", $"estudiante{i:00}@tecoc.edu.co", "Passw0rd!" ) );
        }
    }

    [Fact]
    public async Task Paginar_con_mas_de_8_usuarios_requiere_pagina_2() {
        var client = Client();
        await CreateUsersAsync( client, 10 );

        var page1 = await client.GetFromJsonAsync<GetPagedUsersResponse>( "/api/v1/User/paged?pageNumber=1&pageSize=8" );
        Assert.NotNull( page1 );
        Assert.Equal( 8, page1!.Items.Count );
        Assert.Equal( 10, page1.TotalCount );
        Assert.Equal( 2, page1.TotalPages );

        var page2 = await client.GetFromJsonAsync<GetPagedUsersResponse>( "/api/v1/User/paged?pageNumber=2&pageSize=8" );
        Assert.NotNull( page2 );
        Assert.Equal( 2, page2!.Items.Count );

        var page1Ids = page1.Items.Select( u => u.Id ).ToHashSet();
        Assert.DoesNotContain( page2.Items, u => page1Ids.Contains( u.Id ) );
    }

    [Fact]
    public async Task Con_8_o_menos_usuarios_solo_hay_una_pagina() {
        var client = Client();
        await CreateUsersAsync( client, 5 );

        var response = await client.GetAsync( "/api/v1/User/paged?pageNumber=1&pageSize=8" );
        Assert.Equal( HttpStatusCode.OK, response.StatusCode );

        var body = await response.Content.ReadFromJsonAsync<GetPagedUsersResponse>();
        Assert.NotNull( body );
        Assert.Equal( 5, body!.Items.Count );
        Assert.Equal( 1, body.TotalPages );
    }

    [Fact]
    public async Task Numero_de_pagina_invalido_retorna_badrequest() {
        var client = Client();

        var response = await client.GetAsync( "/api/v1/User/paged?pageNumber=0" );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }

    [Fact]
    public async Task Tamano_de_pagina_invalido_retorna_badrequest() {
        var client = Client();

        var response = await client.GetAsync( "/api/v1/User/paged?pageSize=0" );

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
    }
}
