namespace gestionAdminTECOCApi.Application.Features.Auth.Login;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    Guid UserId,
    string Email
);
