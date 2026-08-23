using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Users;

public class User : Entity<Guid> {
    private User(
        string fullName,
        DocumentType documentType,
        string documentNumber,
        string userName,
        string email,
        string passwordHash
    ) : base( true ) {
        Id = Guid.NewGuid();
        FullName = fullName;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
    }

    public string FullName { get; private set; } = default!;
    public DocumentType DocumentType { get; private set; }
    public string DocumentNumber { get; private set; } = default!;
    public string UserName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    public static User Create(
        string fullName,
        DocumentType documentType,
        string documentNumber,
        string userName,
        string email,
        string passwordHash
    ) => new(
        fullName,
        documentType,
        documentNumber,
        userName,
        email,
        passwordHash
    );

    public void RecordFailedLogin() {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5) {
            LockedUntil = DateTime.UtcNow.AddMinutes( 15 );
        }
    }

    public void ResetFailedLogins() {
        FailedLoginAttempts = 0;
        LockedUntil = null;
    }

    public void Unlock() {
        FailedLoginAttempts = 0;
        LockedUntil = null;
    }

    public bool IsLocked => LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;
}
