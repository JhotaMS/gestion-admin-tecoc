using gestionAdminTECOCApi.Application.Messaging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace gestionAdminTECOCApi.Application.Features.Users.CreateUser;

public record UserCommand(
    [property: JsonPropertyName( "fullName" )]
    [Required] string FullName,
    [property: JsonPropertyName( "documentType" )]
    [Required] string DocumentType,
    [property: JsonPropertyName( "documentNumber" )]
    [Required] string DocumentNumber,
    [property: JsonPropertyName( "userName" )]
    [Required] string UserName,
    [property: JsonPropertyName( "email" )]
    [Required] string Email,
    [property: JsonPropertyName( "password" )]
    [Required] string Password
) : ICommand<UserCommandResponse>;
