using gestionAdminTECOCApi.Application.Messaging;
using System.Text.Json.Serialization;

namespace gestionAdminTECOCApi.Application.Features.Auth.Login;

public record LoginCommand(
    [property: JsonPropertyName( "email" )] string Email,
    [property: JsonPropertyName( "password" )] string Password
) : ICommand<LoginResponse>;
