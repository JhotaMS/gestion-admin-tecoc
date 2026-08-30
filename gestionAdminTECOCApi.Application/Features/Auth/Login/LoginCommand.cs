using gestionAdminTECOCApi.Application.Messaging;

namespace gestionAdminTECOCApi.Application.Features.Auth.Login;

public record LoginCommand(
    string Email,
    string Password
) : ICommand<LoginResponse>;
