using System.Text.Json.Serialization;

namespace gestionAdminTECOCApi.Application.Features.Users.CreateUser;

public record UserCommandResponse(
    [property: JsonPropertyName( "id" )] Guid Id,
    [property: JsonPropertyName( "fullName" )] string FullName,
    [property: JsonPropertyName( "documentType" )] string DocumentType,
    [property: JsonPropertyName( "documentNumber" )] string DocumentNumber,
    [property: JsonPropertyName( "userName" )] string UserName,
    [property: JsonPropertyName( "email" )] string Email
);
