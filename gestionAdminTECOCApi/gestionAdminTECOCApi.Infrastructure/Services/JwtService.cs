using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using gestionAdminTECOCApi.Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace gestionAdminTECOCApi.Infrastructure.Services;

public sealed class JwtService : IJwtService {
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _signingKey;
    private readonly string? _signingKeyNew;

    public JwtService( IConfiguration configuration ) {
        _issuer = configuration["Jwt:Issuer"] ?? configuration["JWT_ISSUER"] ?? "https://tecoc.edu.co";
        _audience = configuration["Jwt:Audience"] ?? configuration["JWT_AUDIENCE"] ?? "tecoc-spa";
        _signingKey = configuration["Jwt:SigningKey"] ?? configuration["JWT_SIGNING_KEY"] ?? throw new InvalidOperationException( "Jwt:SigningKey missing" );
        _signingKeyNew = configuration["Jwt:SigningKeyNew"] ?? configuration["JWT_SIGNING_KEY_NEW"];
    }

    public string GenerateAccessToken( Guid userId, string email, string[] roles ) {
        var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _signingKey ) );
        var creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );
        var claims = new List<Claim> {
            new( JwtRegisteredClaimNames.Sub, userId.ToString() ),
            new( JwtRegisteredClaimNames.Email, email ),
            new( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() ),
        };
        foreach (var role in roles) claims.Add( new Claim( ClaimTypes.Role, role ) );

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes( 15 ),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken( token );
    }

    public string GenerateRefreshToken() => Guid.NewGuid().ToString( "N" ) + Guid.NewGuid().ToString( "N" );

    public bool ValidateToken( string token, out Guid userId ) {
        userId = Guid.Empty;
        var keys = new List<string> { _signingKey };
        if (!string.IsNullOrWhiteSpace( _signingKeyNew )) keys.Add( _signingKeyNew );
        foreach (var k in keys) {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( k ) );
            try {
                var principal = handler.ValidateToken( token, new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds( 30 )
                }, out _ );
                var sub = principal.FindFirst( JwtRegisteredClaimNames.Sub )?.Value ?? principal.FindFirst( ClaimTypes.NameIdentifier )?.Value;
                if (Guid.TryParse( sub, out var gid )) { userId = gid; return true; }
            } catch { /* try next key */ }
        }
        return false;
    }
}
