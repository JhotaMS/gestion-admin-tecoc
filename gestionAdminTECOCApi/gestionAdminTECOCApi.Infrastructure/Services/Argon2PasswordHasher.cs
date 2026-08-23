using gestionAdminTECOCApi.Domain.Ports;
using Isopoh.Cryptography.Argon2;

namespace gestionAdminTECOCApi.Infrastructure.Services;

public sealed class Argon2PasswordHasher : IPasswordHasher {
    public string Hash( string password ) {
        return Argon2.Hash( password );
    }

    public bool Verify( string hash, string password ) {
        return Argon2.Verify( hash, password );
    }
}
