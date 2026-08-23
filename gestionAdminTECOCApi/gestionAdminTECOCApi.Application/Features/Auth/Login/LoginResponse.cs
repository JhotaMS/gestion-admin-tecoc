namespace gestionAdminTECOCApi.Application.Features.Auth.Login;

public record LoginResponse(
    Guid UserId,
    string UserName,
    string Email
);
