using System.Net;
using System.Net.Http.Json;
using gestionAdminTECOCApi.Application.Features.DocumentTypes.CreateDocumentType;
using gestionAdminTECOCApi.Application.Features.DocumentTypes.GetAllDocumentTypes;
using gestionAdminTECOCApi.Infrastructure.PostgreSql.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gestionAdminTECOCApi.Api.Tests.Integration;

public class DocumentTypeTests : IClassFixture<WebApplicationFactory<Program>> {
    private readonly WebApplicationFactory<Program> _factory;

    public DocumentTypeTests( WebApplicationFactory<Program> factory ) {
        _factory = factory.WithWebHostBuilder( builder => {
            builder.ConfigureServices( services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof( DbContextOptions<ApplicationDbContext> ) );
                if (descriptor is not null) services.Remove( descriptor );
                services.AddDbContext<ApplicationDbContext>( options =>
                    options.UseInMemoryDatabase( "tecoc-documenttype-tests" ) );
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
    public async Task Crear_tipo_documento_retorna_200_y_se_lista() {
        var client = Client();
        var response = await client.PostAsJsonAsync( "/api/v1/DocumentType", new CreateDocumentTypeCommand(
            "CC", "Cédula de ciudadanía" ) );

        Assert.Equal( HttpStatusCode.OK, response.StatusCode );

        var list = await client.GetFromJsonAsync<GetAllDocumentTypesResponse>( "/api/v1/DocumentType" );
        Assert.NotNull( list );
        Assert.Contains( list.DocumentTypes, d => d.Code == "CC" && d.Description == "Cédula de ciudadanía" );
    }

    [Fact]
    public async Task Crear_codigo_duplicado_retorna_badrequest() {
        var client = Client();
        var payload = new CreateDocumentTypeCommand( "CE", "Cédula de extranjería" );
        await client.PostAsJsonAsync( "/api/v1/DocumentType", payload );

        var second = await client.PostAsJsonAsync( "/api/v1/DocumentType", payload );

        Assert.Equal( HttpStatusCode.BadRequest, second.StatusCode );
    }

    [Fact]
    public async Task Eliminar_tipo_documento_lo_quita_de_la_lista() {
        var client = Client();
        var create = await client.PostAsJsonAsync( "/api/v1/DocumentType", new CreateDocumentTypeCommand(
            "TI", "Tarjeta de identidad" ) );
        var created = await create.Content.ReadFromJsonAsync<CreateDocumentTypeResponse>();
        Assert.NotNull( created );

        var delete = await client.DeleteAsync( $"/api/v1/DocumentType/{created.DocumentTypeId}" );
        Assert.Equal( HttpStatusCode.OK, delete.StatusCode );

        var list = await client.GetFromJsonAsync<GetAllDocumentTypesResponse>( "/api/v1/DocumentType" );
        Assert.DoesNotContain( list!.DocumentTypes, d => d.Code == "TI" );
    }

    [Fact]
    public async Task Actualizar_tipo_documento_cambia_descripcion() {
        var client = Client();
        var create = await client.PostAsJsonAsync( "/api/v1/DocumentType", new CreateDocumentTypeCommand(
            "NIT", "Número de identificación tributaria" ) );
        var created = await create.Content.ReadFromJsonAsync<CreateDocumentTypeResponse>();
        Assert.NotNull( created );

        var update = await client.PutAsJsonAsync( $"/api/v1/DocumentType/{created.DocumentTypeId}",
            new { documentTypeId = created.DocumentTypeId, code = "NIT", description = "NIT (actualizado)" } );
        Assert.Equal( HttpStatusCode.OK, update.StatusCode );

        var list = await client.GetFromJsonAsync<GetAllDocumentTypesResponse>( "/api/v1/DocumentType" );
        Assert.Contains( list!.DocumentTypes, d => d.Code == "NIT" && d.Description == "NIT (actualizado)" );
    }
}
