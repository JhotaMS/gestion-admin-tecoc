namespace gestionAdminTECOCApi.Application.Features.Auth.Login;

public record LoginResponse(
    string Token,
    int ExpiresInSeconds,
    UserDto User
);

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string Role
);
