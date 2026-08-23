namespace gestionAdminTECOCApi.Domain.Ports;

public interface IJwtService {
    string GenerateAccessToken( Guid userId, string email, string[] roles );
    string GenerateRefreshToken();
    bool ValidateToken( string token, out Guid userId );
}
