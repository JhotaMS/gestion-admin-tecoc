namespace gestionAdminTECOCApi.Infrastructure.Abstractions;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email, string name, string role);
}
