using gestionAdminTECOCApi.Application.Messaging;
using System.Text.Json.Serialization;

namespace gestionAdminTECOCApi.Application.Features.Auth.Login;

public record LoginCommand(
    string UserName,
    string Password,
    string? Email = null
) : ICommand<LoginResponse>;
