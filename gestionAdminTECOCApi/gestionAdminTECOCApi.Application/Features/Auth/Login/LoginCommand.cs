using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.Auth.Login;

public record LoginCommand(
    string UserName,
    string Password,
    string? Email = null
) : ICommand<LoginResponse>;
