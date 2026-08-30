using System.Security.Cryptography;

namespace gestionAdminTECOCApi.Domain.Users;

public static class PasswordHasher {
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public static string Hash( string password ) {
        byte[] salt = RandomNumberGenerator.GetBytes( SaltSize );
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password
            , salt
            , Iterations
            , HashAlgorithmName.SHA256
            , HashSize
        );

        return $"{Convert.ToBase64String( salt )}.{Convert.ToBase64String( hash )}";
    }

    public static bool Verify( string hash, string password ) {
        string[] parts = hash.Split( '.', 2 );
        if (parts.Length != 2) return false;

        byte[] salt;
        byte[] expected;
        try {
            salt = Convert.FromBase64String( parts[0] );
            expected = Convert.FromBase64String( parts[1] );
        } catch (FormatException) {
            return false;
        }

        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(
            password
            , salt
            , Iterations
            , HashAlgorithmName.SHA256
            , HashSize
        );

        return CryptographicOperations.FixedTimeEquals( actual, expected );
    }
}
