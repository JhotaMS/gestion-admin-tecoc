using gestionAdminTECOCApi.Domain.Abstractions;

namespace gestionAdminTECOCApi.Domain.Users;

public class User : Entity<Guid> {
    private User(
        string fullName,
        DocumentType documentType,
        string documentNumber,
        string position
    ) : base( true ) {
        Id = Guid.NewGuid();
        FullName = fullName;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        Position = position;
    }

    public string FullName { get; private set; } = default!;
    public DocumentType DocumentType { get; private set; }
    public string DocumentNumber { get; private set; } = default!;
    public string Position { get; private set; } = default!;

    public static User Create(
        string fullName,
        DocumentType documentType,
        string documentNumber,
        string position
    ) => new(
        fullName,
        documentType,
        documentNumber,
        position
    );
}
